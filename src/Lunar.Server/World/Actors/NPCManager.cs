/** Copyright 2018 John Lamontagne https://www.mmorpgcreation.com

	Licensed under the Apache License, Version 2.0 (the "License");
	you may not use this file except in compliance with the License.
	You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

	Unless required by applicable law or agreed to in writing, software
	distributed under the License is distributed on an "AS IS" BASIS,
	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	See the License for the specific language governing permissions and
	limitations under the License.
*/
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
