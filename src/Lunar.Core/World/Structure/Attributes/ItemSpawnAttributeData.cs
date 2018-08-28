using System;

namespace Lunar.Core.World.Structure.Attributes
{
    [Serializable]
    public class ItemSpawnAttributeData : AttributeData
    {
        public string ItemName { get; set; }

        public int RespawnTime { get; set; }

        public ItemSpawnAttributeData(ItemDescriptor item, int respawnTime)
        {
            this.ItemName = item.Name;
            this.RespawnTime = respawnTime;
        }
    }
}
