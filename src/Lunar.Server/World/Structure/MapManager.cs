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
using Lunar.Core;
using Lunar.Core.Utilities;

namespace Lunar.Server.World.Structure
{
    public class MapManager : IService
    {
        private Dictionary<string, Map> _maps;

        public MapManager()
        {
            _maps = new Dictionary<string, Map>();

            this.LoadMaps();
        }

        private void LoadMaps()
        {
            Console.WriteLine("Loading Maps...");

            DirectoryInfo directoryInfo = new DirectoryInfo(Constants.FILEPATH_MAPS);
            FileInfo[] files = directoryInfo.GetFiles($"*{EngineConstants.MAP_FILE_EXT}");

            foreach (var file in files)
            {
                Map map = Map.Load(file.FullName);
                map.ConstructPathfinder();
                _maps.Add(map.Name, map);
            }

            Console.WriteLine($"Loaded {files.Length} maps.");
        }

        public bool MapExists(string mapName)
        {
            return _maps.ContainsKey(mapName);
        }

        public Map GetMap(string mapName)
        {
            return _maps[mapName];
        }

      

        public void Initalize()
        {
            throw new NotImplementedException();
        }
    }
}
