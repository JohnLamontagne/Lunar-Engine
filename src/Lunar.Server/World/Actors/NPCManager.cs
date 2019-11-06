/** Copyright 2018 John Lamontagne https://www.rpgorigin.com

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
using Lunar.Core;
using Lunar.Core.Utilities;
using Lunar.Core.Utilities.Data.FileSystem;
using Lunar.Core.Utilities.Data.Management;
using Lunar.Core.World.Actor.Descriptors;

namespace Lunar.Server.World.Actors
{
    public class NPCManager : IService
    {
        private Dictionary<string, NPCModel> _npcs;

        private IDataManager<NPCModel> _npcDataManager;

        public NPCManager()
        {
            _npcs = new Dictionary<string, NPCModel>();

            _npcDataManager = Engine.Services.Get<IDataManagerFactory>().Create<NPCModel>(new FSDataFactoryArguments(Constants.FILEPATH_NPCS));
        }

        private void LoadNPCS()
        {
            Console.WriteLine("Loading NPCs...");

            var directoryInfo = new DirectoryInfo(Constants.FILEPATH_NPCS);
            FileInfo[] files = directoryInfo.GetFiles("*" + EngineConstants.NPC_FILE_EXT);

            foreach (var file in files)
            {
                NPCModel npcDesc = _npcDataManager.Load(new ContentFileDataLoaderArguments(Path.GetFileNameWithoutExtension(file.Name)));

                if (npcDesc != null)
                    _npcs.Add(npcDesc.UniqueID, npcDesc);
            }

            Console.WriteLine($"Loaded {_npcs.Count} NPCs.");
        }

        public NPCModel Get(string npcName)
        {
            return !_npcs.ContainsKey(npcName) ? null : _npcs[npcName];
        }

        public void Initalize()
        {
            this.LoadNPCS();
        }
    }
}