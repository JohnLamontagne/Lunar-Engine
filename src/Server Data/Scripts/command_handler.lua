import("System")
import("System.Text")
import("Lunar.Server")
import("Lunar.Server.Utilities.Scripting")
import('Lunar.Core')
import('Lunar.Core.Utilities.Data')
import('Lunar.Server.World')
import('Lunar.Server.World.Actors')

function warpToCommand(args)
  player = args[0]
  x = Convert.ToInt32(args[1][0])
  y = Convert.ToInt32(args[1][1])

  if (not player.Role:Supercedes("Admin")) then
    player:SendChatMessage("You do not have the correct permissions to use this command!", ChatMessageType.Alert)
    return;
  end

  player:SendChatMessage("Warped to " .. x .. ":" .. y, ChatMessageType.Announcement)

  player:WarpTo(Vector(x, y))
end

function warpToPlayerCommand(args)
  player = args[0]
  playerName = args[1][0]

  if (not player.Role:Supercedes("Admin")) then
    player:SendChatMessage("You do not have the correct permissions to use this command!", ChatMessageType.Alert)
    return;
  end

  targetPlayer = Server.ServiceLocator:GetService("PlayerManager"):GetPlayer(playerName)

  if (not targetPlayer.Map:ActorInMap(player)) then
    player:JoinMap(targetPlayer.Map)
  end

  player:WarpTo(targetPlayer.Position)
  player:SendChatMessage("Warped to " .. playerName, ChatMessageType.Announcement)
end

function setSpeedCommand(args)
  player = args[0]
  speed = Convert.ToSingle(args[1][0])

  if (not player.Role:Supercedes("Admin")) then
    player:SendChatMessage("You do not have the correct permissions to use this command!", ChatMessageType.Alert)
    return;
  end

  player:SendChatMessage("Set speed to to " .. speed, ChatMessageType.Announcement)

  player.Speed = speed
end

function spawnItemCommand(args)
  player = args[0]
  itemName = args[1][0]
  amount = Convert.ToInt32(args[1][1])

  if (not player.Role:Supercedes("Admin")) then
    player:SendChatMessage("You do not have the correct permissions to use this command!", ChatMessageType.Alert)
    return;
  end

  itemDesc = Server.ServiceLocator:GetService("ItemManager"):GetItem(itemName)
  item = Item(itemDesc)
  player:AddToInventory(item, amount)
end

function spawnNPCCommand(args)
  player = args[0]
  npcName = args[1][0]

  if (not player.Role:Supercedes("Admin")) then
    player:SendChatMessage("You do not have the correct permissions to use this command!", ChatMessageType.Alert)
    return;
  end

  npcDesc = Server.ServiceLocator:GetService("NPCManager"):GetNPC(npcName)
  npc = NPC(npcDesc, player.Map)
  npc:WarpTo(player.Position)
end

function setCollisionBoundsCommand(args)
  player = args[0]
  left = Convert.ToInt32(args[1][0])
  top = Convert.ToInt32(args[1][1])
  width = Convert.ToInt32(args[1][2])
  height = Convert.ToInt32(args[1][3])

  if (not player.Role:Supercedes("Admin")) then
    player:SendChatMessage("You do not have the correct permissions to use this command!", ChatMessageType.Alert)
    return;
  end

  player.CollisionBounds = Rect(left, top, width, height)
  player:SendChatMessage("Set collision bounds to " .. player.CollisionBounds:ToString(), ChatMessageType.Announcement)
end

Server.ServiceLocator:GetService("CommandHandler"):AddHandler("warpTo", ScriptAction(warpToCommand))
Server.ServiceLocator:GetService("CommandHandler"):AddHandler("warpToPlayer", ScriptAction(warpToPlayerCommand))
Server.ServiceLocator:GetService("CommandHandler"):AddHandler("setSpeed", ScriptAction(setSpeedCommand))
Server.ServiceLocator:GetService("CommandHandler"):AddHandler("spawnItem", ScriptAction(spawnItemCommand))
Server.ServiceLocator:GetService("CommandHandler"):AddHandler("spawnNPC", ScriptAction(spawnNPCCommand))
Server.ServiceLocator:GetService("CommandHandler"):AddHandler("setCollision", ScriptAction(setCollisionBoundsCommand))
