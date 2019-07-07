using Lunar.Server.World.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunar.Server.World.Dialogue
{
    public class DialogueResponseArgs : EventArgs
    {
        public Player Player { get; }

        public DialogueResponseArgs(Player player)
        {
            this.Player = player;
        }
    }
}