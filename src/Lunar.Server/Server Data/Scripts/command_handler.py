import clr
clr.AddReference('System')
clr.AddReference('Lunar.Server')
clr.AddReference('Lunar.Core')
from Lunar.Core import *
from Lunar.Core.Utilities.Data import *
from Lunar.Server import *
from Lunar.Server.Utilities.Commands import *

def warpToCommand(args):
  player = args[0]
  x = int(args[1][0])
  y = int(args[1][1])

  if not player.Descriptor.Role.Supercedes(Settings.Roles["Admin"]):
    player.NetworkComponent.SendChatMessage("You do not have the correct permissions to use this command!", ChatMessageType.Alert)
    return
    
  player.WarpTo(Vector(x, y))
  player.NetworkComponent.SendChatMessage("Warped to " + str(x) + ":" + str(y), ChatMessageType.Announcement)

def warpToPlayerCommand(args):
  player = args[0]
  playerName = args[1][0]
  
  if not player.Descriptor.Role.Supercedes(Settings.Roles["Admin"]):
    player.NetworkComponent.SendChatMessage("You do not have the correct permissions to use this command!", ChatMessageType.Alert)
    return

  targetPlayer = Server.ServiceLocator.GetService("PlayerManager").GetPlayer(playerName)

  if not targetPlayer.Map.ActorInMap(player):
    player.JoinMap(targetPlayer.Map)

  player.WarpTo(targetPlayer.Position)
  player.NetworkComponent.SendChatMessage("Warped to " + playerName, ChatMessageType.Announcement)

def setSpeedCommand(args):
  player = args[0]
  speed = float(args[1][0])

  if not player.Descriptor.Role.Supercedes(Settings.Roles["Admin"]):
    player.NetworkComponent.SendChatMessage("You do not have the correct permissions to use this command!", ChatMessageType.Alert)
    return

  player.NetworkComponent.SendChatMessage("Set speed to to " + str(speed), ChatMessageType.Announcement)

  player.Speed = speed

def spawnItemCommand(args):
  player = args[0]
  itemName = args[1][0]
  amount = int(args[1][1])

  if not player.Descriptor.Role.Supercedes(Settings.Roles["Admin"]):
    player.NetworkComponent.SendChatMessage("You do not have the correct permissions to use this command!", ChatMessageType.Alert)
    return

  itemDesc = Server.ServiceLocator.GetService("ItemManager").GetItem(itemName)
  item = Item(itemDesc)
  player.AddToInventory(item, amount)

def spawnNPCCommand(args):
  player = args[0]
  npcName = args[1][0]

  if not player.Descriptor.Role.Supercedes(Settings.Roles["Admin"]):
    player.NetworkComponent.SendChatMessage("You do not have the correct permissions to use this command!", ChatMessageType.Alert)
    return

  npcDesc = Server.ServiceLocator.GetService("NPCManager").GetNPC(npcName)
  npc = NPC(npcDesc, player.Map)
  npc.WarpTo(player.Position)
  
def setCollisionBoundsCommand(args):
  player = args[0]
  left = int(args[1][0])
  top = int(args[1][1])
  width = int(args[1][2])
  height = int(args[1][3])

  if not player.Role.Supercedes(Settings.Roles["Admin"]):
    player.NetworkComponent.SendChatMessage("You do not have the correct permissions to use this command!", ChatMessageType.Alert)

  player.CollisionBounds = Rect(left, top, width, height)
  player.NetworkComponent.SendChatMessage("Set collision bounds to " + player.CollisionBounds.ToString(), ChatMessageType.Announcement)

Server.ServiceLocator.Get[CommandHandler]().AddHandler("warpTo", warpToCommand)
Server.ServiceLocator.Get[CommandHandler]().AddHandler("warpToPlayer", warpToPlayerCommand)
Server.ServiceLocator.Get[CommandHandler]().AddHandler("setSpeed", setSpeedCommand)
Server.ServiceLocator.Get[CommandHandler]().AddHandler("spawnItem", spawnItemCommand)
Server.ServiceLocator.Get[CommandHandler]().AddHandler("spawnNPC", spawnNPCCommand)
Server.ServiceLocator.Get[CommandHandler]().AddHandler("setCollision", setCollisionBoundsCommand)
