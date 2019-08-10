using Lunar.Core.Content.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunar.Core.World.Structure.Attribute
{
    /// <summary>
    /// Contains a number of actions that are invoked when simiarly named subroutines are engaged in the Tile owning
    /// this handler's attribute.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITileAttributeActionHandler
    {
        void OnInitalize(ITileAttributeArgs args);

        void OnUpdate(ITileAttributeArgs args);

        void OnPlayerEntered(ITileAttributeArgs args);

        void OnPlayerLeft(ITileAttributeArgs args);
    }
}