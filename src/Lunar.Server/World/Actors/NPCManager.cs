using System;
using System.Collections.Generic;
using System.IO;
using Lunar.Core.Utilities;

namespace Lunar.Server.World.Actors
{
    public class NPCManager : IService
    {
        private Dictionary<string, NPCDescriptor> _npcs;

        public NPCManager()
        {
            _npcs = new Dictionary<string, NPCDescriptor>();

            this.LoadNPCS();
        }

        private void LoadNPCS()
        {
            Console.WriteLine("Loading NPCs...");

            var directoryInfo = new DirectoryInfo(Constants.FILEPATH_NPCS);
            FileInfo[] files = directoryInfo.GetFiles("*.lua");

            foreach (var file in files)
            {
                NPCDescriptor npcDesc = NPCDescriptor.Load(file.Name);
                _npcs.Add(npcDesc.Name, npcDesc);
            }

            Console.WriteLine($"Loaded {files.Length} NPCs.");
        }

        public NPCDescriptor GetNPC(string npcName)
        {
            return _npcs[npcName];
        }

        public void Initalize()
        {
            throw new NotImplementedException();
        }
    }
}
