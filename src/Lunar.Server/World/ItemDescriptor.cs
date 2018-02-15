using Lunar.Server.Content.Graphics;
using Lunar.Server.Utilities.Scripting;
using Lunar.Server.World.BehaviorDefinition;
using System;
using Lunar.Core.World;

namespace Lunar.Server.World
{
    public class ItemDescriptor
    {
        private string _name;
        private Sprite _sprite;
        private bool _stackable;
        private EquipmentSlots _slotType;
        private int _strength;
        private int _intelligence;
        private int _dexterity;
        private int _defence;
        private int _health;
        private ItemTypes _itemType;
        private ItemBehaviorDefinition _behaviorDefinition;

        public string Name => _name;
        public Sprite Sprite => _sprite;
        public bool Stackable => _stackable;
        public ItemTypes ItemType => _itemType;
        public EquipmentSlots SlotType => _slotType;

        public int Strength => _strength;
        public int Intelligence => _intelligence;
        public int Dexterity => _dexterity;
        public int Defence => _defence;
        public int Health => _health;
        public ItemBehaviorDefinition BehaviorDefinition => _behaviorDefinition;

        public static ItemDescriptor Load(string fileName)
        {
            string filePath = Constants.FILEPATH_ITEMS + fileName;

            string name = "";
            Sprite sprite = null;
            bool stackable = false;
            ItemTypes itemType = ItemTypes.NA;
            EquipmentSlots slotType = EquipmentSlots.NE;
            int strength = 0;
            int intelligence = 0;
            int dexterity = 0;
            int defence = 0;
            int health = 0;
            ItemBehaviorDefinition behaviorDefinition;

            Script script = Server.ServiceLocator.GetService<ScriptManager>().GetScript(Constants.FILEPATH_ITEMS + fileName);
            var itemDef = script.GetTable("Item");

            name = itemDef["Name"].ToString();
            sprite = new Sprite(itemDef["Sprite"].ToString());
            stackable = (bool)itemDef["Stackable"];
            itemType = (ItemTypes)itemDef["ItemType"];
            slotType = (EquipmentSlots)itemDef["SlotType"];
            strength = Convert.ToInt32(itemDef["Strength"]);
            intelligence = Convert.ToInt32(itemDef["Intelligence"]);
            dexterity = Convert.ToInt32(itemDef["Dexterity"]);
            defence = Convert.ToInt32(itemDef["Defence"]);
            health = Convert.ToInt32(itemDef["Health"]);
            behaviorDefinition = (ItemBehaviorDefinition)itemDef["BehaviorDefinition"];

            var desc = new ItemDescriptor()
            {
                _name = name,
                _sprite = sprite,
                _stackable = stackable,
                _itemType = itemType,
                _slotType = slotType,
                _strength = strength,
                _intelligence = intelligence,
                _dexterity = dexterity,
                _defence = defence,
                _health = health,
                _behaviorDefinition = behaviorDefinition
            };

            script.ScriptChanged += desc.ScriptChanged;

            return desc;
        }

        private void ScriptChanged(object sender, EventArgs e)
        {
            (sender as Script).ReExecute();

            var itemDef = ((Script)sender).GetTable("Item");

            _behaviorDefinition = (ItemBehaviorDefinition)itemDef["BehaviorDefinition"];

            this.DefinitionChanged?.Invoke(this, new EventArgs());
        }

        public event EventHandler<EventArgs> DefinitionChanged;
    }
}
