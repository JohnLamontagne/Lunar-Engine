using Lunar.Core.Utilities;
using Lunar.Core.Utilities.Data;
using Lunar.Core.World.Structure.Attribute;
using Lunar.Server.Utilities;
using Lunar.Server.World;
using System;
using System.Linq;

namespace Lunar.Server.World.Structure.Attribute
{
    public class WarpTileAttributeActionHandler : TileAttributeActionHandler
    {
        private readonly WorldManager _worldManager;
        private readonly Logger _logger;

        public WarpTileAttributeActionHandler(WorldManager worldManager, Logger logger)
        {
            _worldManager = worldManager;
            _logger = logger;
        }

        public override void OnInitalize(ITileAttributeArgs args)
        {
        }

        public override void OnUpdate(ITileAttributeArgs args)
        {
        }

        public override void OnPlayerEntered(ITileAttributeArgs args)
        {
            var attribute = args.Attribute as WarpTileAttribute;
            var player = (args as TileAttributePlayerArgs).Player;
            var tile = (args as TileAttributePlayerArgs).Tile;

            if (player.MapID != attribute.WarpMap)
            {
                var map = _worldManager.GetMap(attribute.WarpMap);

                if (map != null)
                {
                    player.JoinMap(map);

                    var newLayer = map.Layers.FirstOrDefault(l => l.Name == attribute.LayerName);

                    if (newLayer != null)
                    {
                        player.Layer = newLayer;
                    }
                }
                else
                {
                    _logger.LogEvent($"Player {player.Descriptor.Name} stepped on warp tile where destination does not exist!", LogTypes.ERROR,
                        new Exception($"Player {player.Descriptor.Name} stepped on warp tile where destination does not exist!"));

                    return;
                }
            }
            player.WarpTo(new Vector(attribute.X, attribute.Y));
        }

        public override void OnPlayerLeft(ITileAttributeArgs args)
        {
        }
    }
}