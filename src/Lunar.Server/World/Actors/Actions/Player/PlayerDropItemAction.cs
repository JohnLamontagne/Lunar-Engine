using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunar.Server.World.Actors.Actions.Player
{
    class PlayerDropItemAction : IAction<Actors.Player>
    {
        private readonly int _slotNum;

        public PlayerDropItemAction(int slotNum)
        {
            _slotNum = slotNum;
        }

        public void Execute(Actors.Player player)
        {
            if (player.Inventory.GetSlot(_slotNum) != null)
            {
                player.Map.SpawnItem(player.Inventory.GetSlot(_slotNum).Item, player.Descriptor.Position, player.Layer);
                player.Inventory.RemoveItem(_slotNum, 1);
            }
        }
    }
}
