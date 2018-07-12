using Lunar.Server.Content.Graphics;
using Lunar.Server.Utilities.Scripting;
using Lunar.Server.World.BehaviorDefinition;
using System;
using System.IO;
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

        public static ItemDescriptor Load(string filePath)
        {
            string name = "";
            string texturePath = null;
            bool stackable = false;
            ItemTypes itemType = ItemTypes.NA;
            EquipmentSlots slotType = EquipmentSlots.NE;
            int strength = 0;
            int intelligence = 0;
            int dexterity = 0;
            int defence = 0;
            int health = 0;
            ItemBehaviorDefinition behaviorDefinition = null;

            using (var fileStream = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                using (var binaryReader = new BinaryReader(fileStream))
                {
                    name = binaryReader.ReadString();
                    texturePath = binaryReader.ReadString();
                    stackable = binaryReader.ReadBoolean();
                    itemType = (ItemTypes)Enum.Parse(typeof(ItemTypes), binaryReader.ReadString());
                    slotType = (EquipmentSlots)Enum.Parse(typeof(EquipmentSlots), binaryReader.ReadString());
                    strength = binaryReader.ReadInt32();
                    intelligence = binaryReader.ReadInt32();
                    dexterity = binaryReader.ReadInt32();
                    defence = binaryReader.ReadInt32();
                    health = binaryReader.ReadInt32();

                    int scriptCount = binaryReader.ReadInt32();
                    behaviorDefinition = new ItemBehaviorDefinition();
                    for (int i = 0; i < scriptCount; i++)
                    {
                        // script type
                        // script content

                        string scriptActionHook = binaryReader.ReadString();
                        string scriptContent = binaryReader.ReadString();

                        Script script = new Script(scriptContent, false);

                        switch (scriptActionHook)
                        {
                            case "OnAcquired":
                                behaviorDefinition.OnAcquired = new ScriptAction(new Action<ScriptActionArgs>(
                                    (args =>
                                        {
                                            script.GetFunction("OnAcquired").Call(args);
                                        }
                                )));
                                break;

                            case "OnCreated":
                                behaviorDefinition.OnCreated = new ScriptAction(new Action<ScriptActionArgs>(
                                    (args =>
                                        {
                                            script.GetFunction("OnCreated").Call(args);
                                        }
                                    )));
                                break;

                            case "OnDropped":
                                behaviorDefinition.OnDropped = new ScriptAction(new Action<ScriptActionArgs>(
                                    (args =>
                                        {
                                            script.GetFunction("OnDropped").Call(args);
                                        }
                                    )));
                                break;

                            case "OnEquip":
                                behaviorDefinition.OnEquip = new ScriptAction(new Action<ScriptActionArgs>(
                                    (args =>
                                        {
                                            script.GetFunction("OnEquip").Call(args);
                                        }
                                    )));
                                break;

                            case "OnUse":
                                behaviorDefinition.OnUse = new ScriptAction(new Action<ScriptActionArgs>(
                                    (args =>
                                        {
                                            script.GetFunction("OnUse").Call(args);
                                        }
                                    )));
                                break;
                        }
                    }
                }
            }

            var desc = new ItemDescriptor()
            {
                _name = name,
                _sprite = new Sprite(texturePath),
                _behaviorDefinition = behaviorDefinition,
                _stackable = stackable,
                _itemType = itemType,
                _slotType = slotType,
                _strength = strength,
                _intelligence = intelligence,
                _dexterity = dexterity,
                _defence = defence,
                _health = health,
            };

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
