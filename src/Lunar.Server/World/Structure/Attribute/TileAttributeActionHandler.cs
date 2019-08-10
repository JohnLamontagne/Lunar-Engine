using Lunar.Core.World.Structure.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunar.Server.World.Structure.Attribute
{
    public abstract class TileAttributeActionHandler : ITileAttributeActionHandler
    {
        public abstract void OnInitalize(ITileAttributeArgs args);

        public abstract void OnPlayerEntered(ITileAttributeArgs args);

        public abstract void OnPlayerLeft(ITileAttributeArgs args);

        public abstract void OnUpdate(ITileAttributeArgs args);
    }
}