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
using Lunar.Core.World;
using Lunar.Server.Utilities;

namespace Lunar.Server.World
{
    public class ItemManager
    {
        private readonly Dictionary<string, ItemModel> _items;
        private readonly IDataManager<ItemModel> _dataManager;
        private readonly Logger _logger;

        public ItemManager(IDataManagerFactory dataManagerFactory, Logger logger)
        {
            _items = new Dictionary<string, ItemModel>();
            _dataManager = dataManagerFactory.Create<ItemModel>(new FSDataFactoryArguments(Constants.FILEPATH_NPCS));
            _logger = logger;
        }

        private void LoadItems()
        {
            Console.WriteLine("Loading Items...");

            var directoryInfo = new DirectoryInfo(Constants.FILEPATH_ITEMS);
            FileInfo[] files = directoryInfo.GetFiles($"*.{EngineConstants.ITEM_FILE_EXT}");

            foreach (var file in files)
            {
                var itemDesc = _dataManager.Load(new ContentFileDataLoaderArguments(Path.GetFileNameWithoutExtension(file.Name)));

                if (itemDesc != null)
                    _items.Add(itemDesc.Name, itemDesc);
            }

            Console.WriteLine($"Loaded {files.Length} items.");
        }

        public ItemModel Get(string itemName)
        {
            if (!_items.ContainsKey(itemName))
            {
                _logger.LogEvent($"Item {itemName} does not exist", LogTypes.ERROR, new Exception($"Item {itemName} does not exist"));
                return null;
            }

            return _items[itemName];
        }

        public void Initalize()
        {
            this.LoadItems();
        }
    }
}