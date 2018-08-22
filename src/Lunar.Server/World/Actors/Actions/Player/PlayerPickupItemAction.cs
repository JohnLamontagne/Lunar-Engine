using System.Linq;
using Lunar.Server.World.Structure;

namespace Lunar.Server.World.Actors.Actions.Player
{
    class PlayerPickupItemAction : IAction<Actors.Player>
    {
        public void Execute(Actors.Player player)
        {
            MapItem mapItem = player.Map.GetMapItems().FirstOrDefault(mItem => mItem.WithinReachOf(player));

            if (mapItem != null)
            {
                player.Inventory.Add(mapItem.Item, mapItem.Amount);
                player.Map.RemoveItem(mapItem.Item);
            }
        }
    }
}
