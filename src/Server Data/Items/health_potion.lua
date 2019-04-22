import("Lunar.Server")
import("System")
import("Lunar.Server.World")
import("Lunar.Server.World.BehaviorDefinition")
import("Lunar.Server.Utilities.Scripting")
import("Lunar.Core")
import("Lunar.Core.World")

function OnUse(args)
  if (args[0]:GetType() == typeof("Lunar.Server.World.Actors.Player")) then -- Only players can consume this item
    player = args[0]
    item = args.Invoker

    if (player.Health + item.Health <= player.MaximumHealth) then
      player.Health = player.Health + item.Health
    else
      player.Health = player.MaximumHealth
    end

    player:SendChatMessage("You feel a warm energy flow through your body, restoring some life points.", ChatMessageType.Regular)
    player:SendPlayerData()
    player:RemoveItem(item, 1)
  end
end

HealthPotionBehaviorDefinition = ItemBehaviorDefinition()
HealthPotionBehaviorDefinition.OnUse = ScriptAction(OnUse)

Item = {
  Name = "Health potion",
  Sprite = "health_potion",
  Stackable = false,
  ItemType = ItemTypes.Usable,
  SlotType = EquipmentSlots.NE,
  Strength = 0,
  Intelligence = 0,
  Dexterity = 0,
  Defence = 0,
  Health = 10,
  BehaviorDefinition = HealthPotionBehaviorDefinition
}
