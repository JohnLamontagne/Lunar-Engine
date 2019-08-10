using Lunar.Core.Content.Graphics;
using Lunar.Core.World.Structure;
using Lunar.Core.World.Structure.Attribute;
using System;
using System.Collections.Generic;

namespace Lunar.Server.World.Structure.Attribute
{
    public static class TileAttributeActionHandlerFactory
    {
        /// <summary>
        /// Dictionary values should be Types of TileAttributeActionHandler<BaseTileAttributeArgs>
        /// </summary>
        private static Dictionary<Type, Type> _lookupTable;

        static TileAttributeActionHandlerFactory()
        {
            _lookupTable = new Dictionary<Type, Type>();

            _lookupTable.Add(typeof(WarpTileAttribute), typeof(WarpTileAttributeActionHandler));
            _lookupTable.Add(typeof(NPCSpawnTileAttribute), typeof(NPCSpawnAttributeActionHandler));
            _lookupTable.Add(typeof(StartDialogueTileAttribute), typeof(DialogueAttributeActionHandler));
        }

        public static ITileAttributeActionHandler Create(TileAttribute attribute)
        {
            if (_lookupTable.ContainsKey(attribute.GetType()))
            {
                var f = Activator.CreateInstance(_lookupTable[attribute.GetType()]);
                return f as ITileAttributeActionHandler;
            }
            else
                return default(ITileAttributeActionHandler);
        }
    }
}