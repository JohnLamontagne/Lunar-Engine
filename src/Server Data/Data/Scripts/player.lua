import('Lunar.Core')
import("System")
import("Lunar.Server")
import("Lunar.Server.World")
import("Lunar.Server.World.Actors")
import("Lunar.Server.World.BehaviorDefinition")
import("Lunar.Server.Utilities.Scripting")

function OnJoined(args)
  player = args.Invoker

  player:SendChatMessage(Settings.WelcomeMessage, ChatMessageType.Announcement)

   -- The below code works if you have an actual item implemented that is named Tome
   -- item = Item(Server.ServiceLocator:GetService(typeof("Lunar.Server.World.ItemManager")):GetItem("Tome"))
   -- player.Inventory:Add(item, 1)
end

function Attacked(args)
  player = args.Invoker
  player.Health = player.Health - args[1]
  player:SendPlayerStats()
end

function OnCreated(args)
  player = args.Invoker
  player:AddEventHandler("joinedGame", ScriptAction(OnJoined))
end

function OnDeath(args)
  player = args.Invoker

  player:SendChatMessage("You are dead!", ChatMessageType.Alert)
  player.Health = player.MaximumHealth
  player:JoinMap(player.Map)
end

BehaviorDefinition = ActorBehaviorDefinition()
BehaviorDefinition.OnCreated = ScriptAction(OnCreated)
BehaviorDefinition.Attacked = ScriptAction(Attacked)
BehaviorDefinition.OnDeath = ScriptAction(OnDeath)
