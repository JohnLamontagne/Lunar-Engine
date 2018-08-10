using System;
using System.Collections.Generic;
using System.IO;

namespace Lunar.Core.World
{
    public class ItemDescriptor
    {
        private string _name;
        private string _texturePath;
        private bool _stackable;
        private EquipmentSlots _slotType;
        private int _strength;
        private int _intelligence;
        private int _dexterity;
        private int _defence;
        private int _health;
        private ItemTypes _itemType;
        private Dictionary<string, string> _scripts;

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public string TexturePath
        {
            get => _texturePath;
            set => _texturePath = value;
        }

        public bool Stackable
        {
            get => _stackable;
            set => _stackable = value;
        }

        public ItemTypes ItemType
        {
            get => _itemType;
            set => _itemType = value;
        }

        public EquipmentSlots SlotType
        {
            get => _slotType;
            set => _slotType = value;
        }

        public Dictionary<string, string> Scripts => _scripts;

        public int Strength
        {
            get => _strength;
            set => _strength = value;
        }

        public int Intelligence
        {
            get => _intelligence;
            set => _intelligence = value;
        }

        public int Dexterity
        {
            get => _dexterity;
            set => _dexterity = value;
        }

        public int Defence
        {
            get => _defence;
            set => _defence = value;
        }

        public int Health
        {
            get => _health;
            set => _health = value;
        }

        public ItemDescriptor()
        {
            _scripts = new Dictionary<string, string>();
        }

        public void Save(string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                using (var binaryWriter = new BinaryWriter(fileStream))
                {
                    binaryWriter.Write(this.Name);
                    binaryWriter.Write(this.TexturePath);
                    binaryWriter.Write(this.Stackable);
                    binaryWriter.Write(this.ItemType.ToString());
                    binaryWriter.Write(this.SlotType.ToString());
                    binaryWriter.Write(this.Strength);
                    binaryWriter.Write(this.Intelligence);
                    binaryWriter.Write(this.Dexterity);
                    binaryWriter.Write(this.Defence);
                    binaryWriter.Write(this.Health);
                    binaryWriter.Write(this.Scripts.Count);
                    foreach (var script in this.Scripts)
                    {
                        binaryWriter.Write(script.Key);
                        binaryWriter.Write(script.Value);
                    }

                }
            }
        }

        public static ItemDescriptor Create()
        {
            var desc = new ItemDescriptor()
            {
                _name = "Blank",
                _texturePath = "",
                _stackable = false,
                _itemType = ItemTypes.NA,
                _slotType = EquipmentSlots.NE,
                _strength = 0,
                _intelligence = 0,
                _dexterity = 0,
                _defence = 0,
                _health = 0,
            };

            return desc;
        }

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
            Dictionary<string, string> scripts = new Dictionary<string, string>();

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
                    for (int i = 0; i < scriptCount; i++)
                    {
                        scripts.Add(binaryReader.ReadString(), binaryReader.ReadString());
                    }
                }
            }

            var desc = new ItemDescriptor()
            {
                _name = name,
                _texturePath = texturePath,
                _scripts = scripts,
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

        public event EventHandler<EventArgs> DefinitionChanged;
    }
}
