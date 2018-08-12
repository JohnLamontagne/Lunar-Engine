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
using System.Linq;
using Microsoft.Xna.Framework;
using Lunar.Editor.Utilities;
using Lunar.Editor.World;
using System.Xml.Linq;
using Lunar.Core;
using Lunar.Core.Content.Graphics;
using Lunar.Core.World;
using Lunar.Core.World.Actor.Descriptors;
using Lunar.Editor.Controls;

namespace Lunar.Editor
{
    public class Project
    {
        private readonly DirectoryInfo _serverDirectory;
        private readonly DirectoryInfo _clientDirectory;
        private readonly List<DirectoryInfo> _directories;
        private readonly string _projectPath;

        public DirectoryInfo ServerRootDirectory => _serverDirectory;
        public DirectoryInfo ClientRootDirectory => _clientDirectory;

        private readonly Dictionary<string, FileInfo> _mapFiles;
        private readonly Dictionary<string, FileInfo> _itemFiles;
        private readonly Dictionary<string, FileInfo> _npcFiles;
        private readonly Dictionary<string, FileInfo> _animationFiles;

        private readonly Dictionary<string, Map> _maps;
        private readonly Dictionary<string, ItemDescriptor> _items;
        private readonly Dictionary<string, NPCDescriptor> _npcs;
        private readonly Dictionary<string, AnimationDescription> _animations;

        public Dictionary<string, Map> Maps => _maps;
        public Dictionary<string, ItemDescriptor> Items => _items;
        public Dictionary<string, NPCDescriptor> NPCs => _npcs;
        public Dictionary<string, AnimationDescription> Animations => _animations;

        public IEnumerable<FileInfo> MapFiles => _mapFiles.Values;
        public IEnumerable<FileInfo> ItemFiles => _itemFiles.Values;
        public IEnumerable<FileInfo> AnimationFiles => _animationFiles.Values;
        public IEnumerable<FileInfo> NPCFiles => _npcFiles.Values;

        public IEnumerable<DirectoryInfo> Directories => _directories;

        public string GameName { get; set; }

        private Project(string projectPath, string serverDir, string clientDir)
        {
            _projectPath = projectPath;
            _serverDirectory = new DirectoryInfo(serverDir);
            _clientDirectory = new DirectoryInfo(clientDir);

            _mapFiles = new Dictionary<string, FileInfo>();
            _npcFiles = new Dictionary<string, FileInfo>();
            _itemFiles = new Dictionary<string, FileInfo>();
            _animationFiles = new Dictionary<string, FileInfo>();

            _maps = new Dictionary<string, Map>();
            _items = new Dictionary<string, ItemDescriptor>();
            _npcs= new Dictionary<string, NPCDescriptor>();
            _animations = new Dictionary<string, AnimationDescription>();

            _directories = new List<DirectoryInfo>();

            this.LoadContents(this.ServerRootDirectory);
        }

        public static Project Load(string projectPath)
        {
            var doc = XDocument.Load(projectPath);

            var generalSettings = doc.Elements("Config").Elements("General");
            string serverDataPath = generalSettings.Elements("Server_Data_Path").FirstOrDefault().Value;
            string clientDataPath = generalSettings.Elements("Client_Data_Path").FirstOrDefault().Value;

            var project = new Project(projectPath, serverDataPath, clientDataPath);
            project.GameName = "Default";
           
            return project;
        }

        public static Project Create(string projectPath, string serverDataDir, string clientDataDir)
        {
            var project = new Project(projectPath, serverDataDir.Replace(@"\", "/"), clientDataDir.Replace(@"\", "/"));
            project.GameName = "Default";
            project.Save();

            return project;
        }

        public FileInfo AddScript(string filePath)
        {
            return null;
        }

        public FileInfo AddMap(string filePath)
        {
            var map = new Map(new Vector2(Constants.NEW_MAP_X, Constants.NEW_MAP_Y), Path.GetFileNameWithoutExtension(filePath));
            map.Save(filePath);
            var mapFile = new FileInfo(filePath);
            _mapFiles.Add(filePath, mapFile);

            this.MapAdded?.Invoke(this, new FileEventArgs(mapFile));

            return mapFile;
        }

        public Map LoadMap(string filePath, TextureLoader textureLoader)
        {
            var map = Map.Load(filePath, this, textureLoader);

            if (!_maps.ContainsKey(filePath))
                _maps.Add(filePath, map);
            else
                _maps[filePath] = map;

            return map;
        }

        public FileInfo ChangeMap(string oldFilePath, string newFilePath)
        {
            var oldFile = _mapFiles[oldFilePath];
            _mapFiles.Remove(oldFilePath);
            var file = new FileInfo(newFilePath);
            _mapFiles.Add(newFilePath, new FileInfo(newFilePath));

            var map = _maps[oldFilePath];
            _maps.Remove(oldFilePath);
            _maps.Add(newFilePath, map);

            this.MapChanged?.Invoke(this, new GameFileChangedEventArgs(oldFile, file));

            return file;
        }

        public void RemoveMap(string filePath)
        {
            if (!_mapFiles.ContainsKey(filePath))
                return;

            if (_maps.ContainsKey(filePath))
                _maps.Remove(filePath);

            this.MapDeleted?.Invoke(this, new FileEventArgs(_mapFiles[filePath]));

            File.Delete(filePath);

            _mapFiles.Remove(filePath);
        }

        public FileInfo AddItem(string filePath)
        {
            var item = ItemDescriptor.Create();
            item.Name = Path.GetFileNameWithoutExtension(filePath);
            item.Save(filePath);
            var itemFile = new FileInfo(filePath);

            if (!_itemFiles.ContainsKey(filePath))
                _itemFiles.Add(filePath, itemFile);

            this.ItemAdded?.Invoke(this, new FileEventArgs(itemFile));

            return itemFile;
        }

        public ItemDescriptor LoadItem(string filePath)
        {
            if (_items.ContainsKey(filePath))
                return _items[filePath];

            var item = ItemDescriptor.Load(filePath);
            _items.Add(filePath, item);

            return item;
        }

        public void AddItem(FileInfo file)
        {
            if (!_itemFiles.ContainsKey(file.FullName))
                _itemFiles.Add(file.FullName, file);

            this.ItemAdded?.Invoke(this, new FileEventArgs(file));
        }

        public FileInfo ChangeItem(string oldFilePath, string newFilePath)
        {
            var oldFile = _itemFiles[oldFilePath];
            _itemFiles.Remove(oldFilePath);
            var file = new FileInfo(newFilePath);
            _itemFiles.Add(newFilePath, new FileInfo(newFilePath));

            var item = _items[oldFilePath];
            _items.Remove(oldFilePath);
            _items.Add(newFilePath, item);

            this.ItemChanged?.Invoke(this, new GameFileChangedEventArgs(oldFile, file));

            return file;
        }

        public void RemoveItem(string filePath)
        {
            if (!_itemFiles.ContainsKey(filePath))
                return;

            if (_items.ContainsKey(filePath))
                _items.Remove(filePath);

            this.ItemDeleted?.Invoke(this, new FileEventArgs(_itemFiles[filePath]));

            File.Delete(filePath);

            _itemFiles.Remove(filePath);
        }

        public FileInfo AddAnimation(string filePath)
        {
            var animation = AnimationDescription.Create();
            animation.Name = Path.GetFileNameWithoutExtension(filePath);
            animation.Save(filePath);
            var animationFile = new FileInfo(filePath);
            _animationFiles.Add(filePath, animationFile);

            this.AnimationAdded?.Invoke(this, new FileEventArgs(animationFile));

            return animationFile;
        }

        public AnimationDescription LoadAnimation(string filePath)
        {
            if (_animations.ContainsKey(filePath))
                return _animations[filePath];

            var animation = AnimationDescription.Load(filePath);
            _animations.Add(filePath, animation);

            return animation;
        }

        public FileInfo ChangeAnimation(string oldFilePath, string newFilePath)
        {
            var oldFile = _animationFiles[oldFilePath];
            _animationFiles.Remove(oldFilePath);
            var file = new FileInfo(newFilePath);
            _animationFiles.Add(newFilePath, new FileInfo(newFilePath));

            var animation = _animations[oldFilePath];
            _animations.Remove(oldFilePath);
            _animations.Add(newFilePath, animation);

            this.AnimationChanged?.Invoke(this, new GameFileChangedEventArgs(oldFile, file));

            return file;
        }

        public void RemoveAnimations(string filePath)
        {
            if (!_animationFiles.ContainsKey(filePath))
                return;

            if (_animations.ContainsKey(filePath))
                _animations.Remove(filePath);

            this.AnimationDeleted?.Invoke(this, new FileEventArgs(_animationFiles[filePath]));

            File.Delete(filePath);

            _animationFiles.Remove(filePath);
        }

        public FileInfo AddNPC(string filePath)
        {
            var npc = NPCDescriptor.Create();
            npc.Name = Path.GetFileNameWithoutExtension(filePath);
            npc.Save(filePath);
            var npcFile = new FileInfo(filePath);
            _npcFiles.Add(filePath, npcFile);

            this.NPCAdded?.Invoke(this, new FileEventArgs(npcFile));

            return npcFile;
        }

        public NPCDescriptor LoadNPC(string filePath)
        {
            if (_npcs.ContainsKey(filePath))
                return _npcs[filePath];

            var npc = NPCDescriptor.Load(filePath);
            _npcs.Add(filePath, npc);

            return npc;
        }

        public FileInfo ChangeNPC(string oldFilePath, string newFilePath)
        {
            var oldFile = _npcFiles[oldFilePath];
            _npcFiles.Remove(oldFilePath);
        
            var file = new FileInfo(newFilePath);
            _npcFiles.Add(newFilePath, new FileInfo(newFilePath));

            var npc = _npcs[oldFilePath];
            _npcs.Remove(oldFilePath);
            _npcs.Add(newFilePath, npc);

            this.NPCChanged?.Invoke(this, new GameFileChangedEventArgs(oldFile, file));

            return file;
        }

        public void RemoveNPC(string filePath)
        {
            if (!_npcFiles.ContainsKey(filePath))
                return;

            if (_npcs.ContainsKey(filePath))
                _npcs.Remove(filePath);

            this.NPCDeleted?.Invoke(this, new FileEventArgs(_npcFiles[filePath]));

            File.Delete(filePath);

            _npcFiles.Remove(filePath);
        }

        public DirectoryInfo AddDirectory(string directoryPath)
        {
            DirectoryInfo directoryInfo = Directory.CreateDirectory(directoryPath);

            _directories.Add(directoryInfo);

            return directoryInfo;
        }

        public void DeleteDirectory(string directoryPath)
        {
            DirectoryInfo directoryToRemove = null;
            foreach (var directory in _directories)
            {
                if (directory.FullName == directoryPath)
                {
                    directoryToRemove = directory;
                }
            }

            if (directoryToRemove != null)
                _directories.Remove(directoryToRemove);

            Directory.Delete(directoryPath);
        }

      
        private void LoadContents(DirectoryInfo projectDirectory)
        {

            var mapDirectory = new DirectoryInfo(projectDirectory.FullName + "/Maps/");

            if (!mapDirectory.Exists)
            {
                Directory.CreateDirectory(mapDirectory.FullName);
            }

            // Load all of the map files
            foreach (var file in mapDirectory.GetFiles("*" + EngineConstants.MAP_FILE_EXT, SearchOption.AllDirectories))
            {
                _mapFiles.Add(file.FullName, file);
            }

            var itemDirectory = new DirectoryInfo(projectDirectory.FullName + "/Items/");

            if (!itemDirectory.Exists)
            {
                Directory.CreateDirectory(itemDirectory.FullName);
            }

            // Load all of the item files
            foreach (var file in itemDirectory.GetFiles("*" + EngineConstants.ITEM_FILE_EXT, SearchOption.AllDirectories))
            {
                _itemFiles.Add(file.FullName, file);
                this.LoadItem(file.FullName);
            }

            var npcDirectory = new DirectoryInfo(projectDirectory.FullName + "/Npcs/");

            if (!npcDirectory.Exists)
            {
                Directory.CreateDirectory(npcDirectory.FullName);
            }

            // Load all of the item files
            foreach (var file in npcDirectory.GetFiles("*" + EngineConstants.NPC_FILE_EXT, SearchOption.AllDirectories))
            {
                _npcFiles.Add(file.FullName, file);
                this.LoadNPC(file.FullName);
            }

        }

        public void Save()
        {
            var xml = new XElement("Config",
                new XElement("General",
                    new XElement("Server_Data_Path", _serverDirectory.FullName),
                    new XElement("Client_Data_Path", _clientDirectory.FullName)
                )
            );
            xml.Save(_projectPath);
        }

        public event EventHandler<FileEventArgs> ItemDeleted;
        public event EventHandler<FileEventArgs> NPCDeleted;
        public event EventHandler<FileEventArgs> AnimationDeleted;
        public event EventHandler<FileEventArgs> MapDeleted;

        public event EventHandler<GameFileChangedEventArgs> NPCChanged;
        public event EventHandler<GameFileChangedEventArgs> AnimationChanged;
        public event EventHandler<GameFileChangedEventArgs> ItemChanged;
        public event EventHandler<GameFileChangedEventArgs> MapChanged;

        public event EventHandler<FileEventArgs> ItemAdded;
        public event EventHandler<FileEventArgs> NPCAdded;
        public event EventHandler<FileEventArgs> AnimationAdded;
        public event EventHandler<FileEventArgs> MapAdded;
    }
}
