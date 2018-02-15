namespace Lunar.Server.World
{
    public class InventorySlot
    {
        private Item _item;
        private int _amount;

        public Item Item { get { return _item; } }

        public int Amount { get { return _amount; } set { _amount = value; } }

        public InventorySlot(Item item, int amount)
        {
            _item = item;
            this.Amount = amount;
        }
    }
}
