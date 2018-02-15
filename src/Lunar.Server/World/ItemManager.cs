using System;
using System.Collections.Generic;
using System.IO;
using Lunar.Core.Utilities;
using Lunar.Server.Utilities;

namespace Lunar.Server.World
{
    public class ItemManager : IService
    {
        private Dictionary<string, ItemDescriptor> _items;

        public ItemManager()
        {
            _items = new Dictionary<string, ItemDescriptor>();

            this.LoadItems();
        }

        private void LoadItems()
        {
            Console.WriteLine("Loading Items...");

            var directoryInfo = new DirectoryInfo(Constants.FILEPATH_ITEMS);
            FileInfo[] files = directoryInfo.GetFiles("*.lua");

            foreach (var file in files)
            {
                ItemDescriptor item = ItemDescriptor.Load(file.Name);
                _items.Add(item.Name, item);
            }

            Console.WriteLine($"Loaded {files.Length} items.");
        }

        public ItemDescriptor GetItem(string itemName)
        {
            if (!_items.ContainsKey(itemName))
            {
                Logger.LogEvent($"Item {itemName} does not exist", LogTypes.ERROR);
                return null;
            }

            return _items[itemName];
        }

        public void Initalize()
        {
            throw new NotImplementedException();
        }
    }
}
