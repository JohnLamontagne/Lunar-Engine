using Lunar.Server.Utilities.Scripting;

namespace Lunar.Server.World
{
    public class ItemArgs : ServerArgs
    {
        public Item Item { get; }

        public ItemArgs(Item item)
        {
            this.Item = item;
        }
    }
}