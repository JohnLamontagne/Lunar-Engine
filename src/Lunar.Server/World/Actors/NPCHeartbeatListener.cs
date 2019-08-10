using Lunar.Server.World.Actors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunar.Server.World.Actors
{
    /// <summary>
    /// Keeps track of the npcs which have been spawned as a result of this tile.
    /// </summary>
    public class NPCHeartbeatListener
    {
        public ObservableCollection<NPC> NPCs { get; }

        public NPCHeartbeatListener()
        {
            this.NPCs = new ObservableCollection<NPC>();

            this.NPCs.CollectionChanged += (sender, args) =>
            {
                foreach (NPC npc in args.NewItems)
                {
                    npc.Died += (o, eventArgs) =>
                    {
                        this.NPCs.Remove(npc);
                    };
                }
            };
        }
    }
}