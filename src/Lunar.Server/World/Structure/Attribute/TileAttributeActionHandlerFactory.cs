using Lunar.Core.World.Structure.Attribute;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Lunar.Server.World.Structure.Attribute
{
    public class TileAttributeActionHandlerFactory
    {
        private static readonly Dictionary<Type, Type> _lookupTable = new Dictionary<Type, Type>
        {
            { typeof(WarpTileAttribute), typeof(WarpTileAttributeActionHandler) },
            { typeof(NPCSpawnTileAttribute), typeof(NPCSpawnAttributeActionHandler) },
            { typeof(StartDialogueTileAttribute), typeof(DialogueTileAttributeActionHandler) },
        };

        private readonly IServiceProvider _services;

        public TileAttributeActionHandlerFactory(IServiceProvider services)
        {
            _services = services;
        }

        public ITileAttributeActionHandler Create(TileAttribute attribute)
        {
            if (_lookupTable.TryGetValue(attribute.GetType(), out var handlerType))
            {
                return (ITileAttributeActionHandler)ActivatorUtilities.CreateInstance(_services, handlerType);
            }

            return null;
        }
    }
}