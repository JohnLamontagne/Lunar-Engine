import("System")
import("Lunar.Server")
import("Lunar.Server.World")
import("Lunar.Server.World.BehaviorDefinition")
import("Lunar.Core")
import("Lunar.Core.World")

RubyRingBehaviorDefinition = ItemBehaviorDefinition()


Item = {
	Name = "Ruby ring",
	Sprite = "7",
	Stackable = false,
	ItemType = ItemTypes.Equipment,
	SlotType = EquipmentSlots.Ring,
	Strength = 2,
	Intelligence = 0,
	Dexterity = 0,
	Defence = 1,
	Health = 0,
	BehaviorDefinition = RubyRingBehaviorDefinition
}
