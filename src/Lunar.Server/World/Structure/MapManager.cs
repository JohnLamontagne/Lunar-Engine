using System;
using System.Collections.Generic;
using System.IO;
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
            FileInfo[] files = directoryInfo.GetFiles("*.rmap");

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
