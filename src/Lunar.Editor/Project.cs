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
using Lunar.Core.Utilities.Data;
using Lunar.Core.Utilities.Data.FileSystem;
using Lunar.Core.Utilities.Data.Management;
using Lunar.Core.World;
using Lunar.Core.World.Actor.Descriptors;
using Lunar.Core.World.Structure;
using Lunar.Core.Utilities.Logic;

namespace Lunar.Editor
{
    public class Project
    {
        private IDataManager<MapDescriptor> _mapDataManager;

        private readonly DirectoryInfo _serverRootDirectory;
        private readonly DirectoryInfo _serverWorldDirectory;
        private readonly DirectoryInfo _clientDirectory;
        private readonly List<DirectoryInfo> _directories;
        private readonly string _projectPath;

        public DirectoryInfo ServerWorldDirectory => _serverWorldDirectory;

        public DirectoryInfo ServerRootDirectory => _serverRootDirectory;

        public DirectoryInfo ClientRootDirectory => _clientDirectory;

        private readonly Dictionary<string, FileInfo> _mapFiles;
        private readonly Dictionary<string, FileInfo> _itemFiles;
        private readonly Dictionary<string, FileInfo> _npcFiles;
        private readonly Dictionary<string, FileInfo> _animationFiles;
        private readonly Dictionary<string, FileInfo> _scriptFiles;

        private readonly Dictionary<string, Map> _maps;
        private readonly Dictionary<string, ItemDescriptor> _items;
        private readonly Dictionary<string, NPCDescriptor> _npcs;
        private readonly Dictionary<string, AnimationDescriptor> _animations;

        public Dictionary<string, Map> Maps => _maps;
        public Dictionary<string, ItemDescriptor> Items => _items;
        public Dictionary<string, NPCDescriptor> NPCs => _npcs;
        public Dictionary<string, AnimationDescriptor> Animations => _animations;

        public IEnumerable<FileInfo> MapFiles => _mapFiles.Values;
        public IEnumerable<FileInfo> ItemFiles => _itemFiles.Values;
        public IEnumerable<FileInfo> AnimationFiles => _animationFiles.Values;

        public IEnumerable<FileInfo> ScriptFiles => _scriptFiles.Values;
        public IEnumerable<FileInfo> NPCFiles => _npcFiles.Values;

        public IEnumerable<DirectoryInfo> Directories => _directories;

        public string GameName { get; set; }

        private Project(string projectPath, string serverDir, string clientDir)
        {
            _projectPath = projectPath;
            _serverRootDirectory = new DirectoryInfo(serverDir);
            _serverWorldDirectory = new DirectoryInfo(serverDir + "/World/");
            _clientDirectory = new DirectoryInfo(clientDir);

            _mapFiles = new Dictionary<string, FileInfo>();
            _npcFiles = new Dictionary<string, FileInfo>();
            _itemFiles = new Dictionary<string, FileInfo>();
            _animationFiles = new Dictionary<string, FileInfo>();
            _scriptFiles = new Dictionary<string, FileInfo>();

            _maps = new Dictionary<string, Map>();
            _items = new Dictionary<string, ItemDescriptor>();
            _npcs= new Dictionary<string, NPCDescriptor>();
            _animations = new Dictionary<string, AnimationDescriptor>();

            _directories = new List<DirectoryInfo>();

            _mapDataManager = new FSDataFactory().Create<MapFSDataManager>(new FSDataFactoryArguments(_serverWorldDirectory + "/Maps/"));

            this.LoadContents(this.ServerWorldDirectory);
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
            File.CreateText(filePath).Close();
            var scriptFile = new FileInfo(filePath);
            _scriptFiles.Add(Helpers.NormalizePath(filePath), scriptFile);

            this.ScriptAdded?.Invoke(this, new FileEventArgs(scriptFile));

            return scriptFile;
        }

        public void RemoveScript(string filePath)
        {
            File.Delete(filePath);
            this.ScriptDeleted?.Invoke(this, new FileEventArgs(_scriptFiles[Helpers.NormalizePath(filePath)]));
            _scriptFiles.Remove(Helpers.NormalizePath(filePath));
        }

        public void SaveScript(string filePath, string contents)
        {
            File.WriteAllText(filePath, contents);
        }

        public FileInfo LoadScript(string filePath)
        {
            return new FileInfo(filePath);
            
        } 

        public FileInfo AddMap(string filePath)
        {
            var map = new Map(new Vector2(Constants.NEW_MAP_X, Constants.NEW_MAP_Y), Path.GetFileNameWithoutExtension(filePath));
            this.SaveMap(filePath, map);
            var mapFile = new FileInfo(filePath);
            _mapFiles.Add(Helpers.NormalizePath(filePath), mapFile);

            this.MapAdded?.Invoke(this, new FileEventArgs(mapFile));

            return mapFile;
        }

        public Map LoadMap(string filePath, TextureLoader textureLoader)
        {
            var mapDescriptor = _mapDataManager.Load(new MapFSDataManagerArguments(Path.GetFileNameWithoutExtension(filePath)));
            
            var map = new Map(mapDescriptor, textureLoader);
            map.Initalize(this, textureLoader);

            if (!_maps.ContainsKey(Helpers.NormalizePath(filePath)))
                _maps.Add(Helpers.NormalizePath(filePath), map);
            else
                _maps[Helpers.NormalizePath(filePath)] = map;

            return map;
        }

        public void SaveMap(string filePath, Map map)
        {
            _mapDataManager.Save(map.Descriptor, new MapFSDataManagerArguments(Path.GetFileNameWithoutExtension(filePath)));
        }

        public FileInfo ChangeMap(string oldFilePath, string newFilePath)
        {
            var oldFile = _mapFiles[Helpers.NormalizePath(oldFilePath)];
            _mapFiles.Remove(Helpers.NormalizePath(oldFilePath));
            var file = new FileInfo(newFilePath);
            _mapFiles.Add(newFilePath, new FileInfo(Helpers.NormalizePath(newFilePath)));

            var map = _maps[Helpers.NormalizePath(oldFilePath)];
            _maps.Remove(Helpers.NormalizePath(oldFilePath));
            _maps.Add(Helpers.NormalizePath(newFilePath), map);

            this.MapChanged?.Invoke(this, new GameFileChangedEventArgs(oldFile, file));

            return file;
        }

        public void RemoveMap(string filePath)
        {
            if (!_mapFiles.ContainsKey(Helpers.NormalizePath(filePath)))
                return;

            _maps.Remove(Helpers.NormalizePath(filePath));

            this.MapDeleted?.Invoke(this, new FileEventArgs(_mapFiles[filePath]));

            File.Delete(filePath);

            _mapFiles.Remove(Helpers.NormalizePath(filePath));
        }

        public FileInfo AddItem(string filePath)
        {
            var item = ItemDescriptor.Create();
            item.Name = Path.GetFileNameWithoutExtension(filePath);
            item.Save(filePath);
            var itemFile = new FileInfo(filePath);

            if (!_itemFiles.ContainsKey(Helpers.NormalizePath(filePath)))
                _itemFiles.Add(Helpers.NormalizePath(filePath), itemFile);

            this.ItemAdded?.Invoke(this, new FileEventArgs(itemFile));

            return itemFile;
        }

        public ItemDescriptor LoadItem(string filePath)
        {
            if (_items.ContainsKey(Helpers.NormalizePath(filePath)))
                return _items[Helpers.NormalizePath(filePath)];

            var item = ItemDescriptor.Load(filePath);
            _items.Add(Helpers.NormalizePath(filePath), item);

            return item;
        }

        public void AddItem(FileInfo file)
        {
            if (!_itemFiles.ContainsKey(Helpers.NormalizePath(file.FullName)))
                _itemFiles.Add(Helpers.NormalizePath(file.FullName), file);

            this.ItemAdded?.Invoke(this, new FileEventArgs(file));
        }

        public FileInfo ChangeItem(string oldFilePath, string newFilePath)
        {
            var oldFile = _itemFiles[Helpers.NormalizePath(oldFilePath)];
            _itemFiles.Remove(Helpers.NormalizePath(oldFilePath));
            var file = new FileInfo(newFilePath);
            _itemFiles.Add(Helpers.NormalizePath(newFilePath), new FileInfo(newFilePath));

            var item = _items[Helpers.NormalizePath(oldFilePath)];
            _items.Remove(Helpers.NormalizePath(oldFilePath));
            _items.Add(Helpers.NormalizePath(newFilePath), item);

            this.ItemChanged?.Invoke(this, new GameFileChangedEventArgs(oldFile, file));

            return file;
        }

        public void RemoveItem(string filePath)
        {
            if (!_itemFiles.ContainsKey(Helpers.NormalizePath(filePath)))
                return;

            if (_items.ContainsKey(Helpers.NormalizePath(filePath)))
                _items.Remove(Helpers.NormalizePath(filePath));

            this.ItemDeleted?.Invoke(this, new FileEventArgs(_itemFiles[Helpers.NormalizePath(filePath)]));

            File.Delete(filePath);

            _itemFiles.Remove(Helpers.NormalizePath(filePath));
        }

        public FileInfo AddAnimation(string filePath)
        {
            var animation = AnimationDescriptor.Create();
            animation.Name = Path.GetFileNameWithoutExtension(filePath);
            animation.Save(filePath);
            var animationFile = new FileInfo(filePath);
            _animationFiles.Add(Helpers.NormalizePath(filePath), animationFile);

            this.AnimationAdded?.Invoke(this, new FileEventArgs(animationFile));

            return animationFile;
        }

        public AnimationDescriptor LoadAnimation(string filePath)
        {
            if (_animations.ContainsKey(Helpers.NormalizePath(filePath)))
                return _animations[Helpers.NormalizePath(filePath)];

            var animation = AnimationDescriptor.Load(filePath);
            _animations.Add(Helpers.NormalizePath(filePath), animation);

            return animation;
        }

        public FileInfo ChangeAnimation(string oldFilePath, string newFilePath)
        {
            var oldFile = _animationFiles[Helpers.NormalizePath(oldFilePath)];
            _animationFiles.Remove(Helpers.NormalizePath(oldFilePath));
            var file = new FileInfo(newFilePath);
            _animationFiles.Add(newFilePath, new FileInfo(newFilePath));

            var animation = _animations[Helpers.NormalizePath(oldFilePath)];
            _animations.Remove(Helpers.NormalizePath(oldFilePath));
            _animations.Add(Helpers.NormalizePath(newFilePath), animation);

            this.AnimationChanged?.Invoke(this, new GameFileChangedEventArgs(oldFile, file));

            return file;
        }

        public void RemoveAnimations(string filePath)
        {
            if (_animations.ContainsKey(Helpers.NormalizePath(filePath)))
                _animations.Remove(Helpers.NormalizePath(filePath));
            else
                return;

            this.AnimationDeleted?.Invoke(this, new FileEventArgs(_animationFiles[Helpers.NormalizePath(filePath)]));

            File.Delete(filePath);

            _animationFiles.Remove(Helpers.NormalizePath(filePath));
        }

        public FileInfo AddNPC(string filePath)
        {
            string uniqueID = Path.GetFileNameWithoutExtension(filePath);
            var npc = NPCDescriptor.Create(uniqueID);
            npc.Name = Path.GetFileNameWithoutExtension(filePath);
            npc.Save(filePath);
            var npcFile = new FileInfo(filePath);
            _npcFiles.Add(Helpers.NormalizePath(filePath), npcFile);

            this.NPCAdded?.Invoke(this, new FileEventArgs(npcFile));

            return npcFile;
        }

        public NPCDescriptor LoadNPC(string filePath)
        {
            if (_npcs.ContainsKey(Helpers.NormalizePath(filePath)))
                return _npcs[Helpers.NormalizePath(filePath)];

            var npc = NPCDescriptor.Load(filePath);
            _npcs.Add(Helpers.NormalizePath(filePath), npc);

            return npc;
        }

        public void UnloadNPC(string filePath)
        {
            if (_npcs.ContainsKey(Helpers.NormalizePath(filePath)))
            {
                _npcs.Remove(Helpers.NormalizePath(filePath));
            }
        }

        public FileInfo ChangeNPC(string oldFilePath, string newFilePath)
        {
            var oldFile = _npcFiles[Helpers.NormalizePath(oldFilePath)];
            _npcFiles.Remove(Helpers.NormalizePath(oldFilePath));
        
            var file = new FileInfo(newFilePath);
            _npcFiles.Add(Helpers.NormalizePath(newFilePath), new FileInfo(newFilePath));

            var npc = _npcs[Helpers.NormalizePath(oldFilePath)];
            _npcs.Remove(Helpers.NormalizePath(oldFilePath));
            _npcs.Add(Helpers.NormalizePath(newFilePath), npc);

            this.NPCChanged?.Invoke(this, new GameFileChangedEventArgs(oldFile, file));

            return file;
        }

        public void RemoveNPC(string filePath)
        {
            string norm = Helpers.NormalizePath(filePath);
            if (!_npcFiles.ContainsKey(Helpers.NormalizePath(filePath)))
                return;

            if (_npcs.ContainsKey(Helpers.NormalizePath(filePath)))
                _npcs.Remove(Helpers.NormalizePath(filePath));

            this.NPCDeleted?.Invoke(this, new FileEventArgs(_npcFiles[Helpers.NormalizePath(filePath)]));

            File.Delete(filePath);

            _npcFiles.Remove(Helpers.NormalizePath(filePath));
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
                _mapFiles.Add(Helpers.NormalizePath(file.FullName), file);
            }

            var itemDirectory = new DirectoryInfo(projectDirectory.FullName + "/Items/");

            if (!itemDirectory.Exists)
            {
                Directory.CreateDirectory(itemDirectory.FullName);
            }

            // Load all of the item files
            foreach (var file in itemDirectory.GetFiles("*" + EngineConstants.ITEM_FILE_EXT, SearchOption.AllDirectories))
            {
                _itemFiles.Add(Helpers.NormalizePath(file.FullName), file);
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
                _npcFiles.Add(Helpers.NormalizePath(file.FullName), file);
                this.LoadNPC(file.FullName);
            }


            var scriptDirectory = new DirectoryInfo(projectDirectory.FullName + "/Scripts/");

            if (!scriptDirectory.Exists)
            {
                Directory.CreateDirectory(scriptDirectory.FullName);
            }

            // Load all of the item files
            foreach (var file in scriptDirectory.GetFiles("*" + EngineConstants.SCRIPT_FILE_EXT, SearchOption.AllDirectories))
            {
                _scriptFiles.Add(Helpers.NormalizePath(file.FullName), file);
                this.LoadScript(file.FullName);
            }
        }

        public void Save()
        {
            var xml = new XElement("Config",
                new XElement("General",
                    new XElement("Server_Data_Path", _serverRootDirectory.FullName),
                    new XElement("Client_Data_Path", _clientDirectory.FullName)
                )
            );
            xml.Save(_projectPath);
        }

        public event EventHandler<FileEventArgs> ItemDeleted;
        public event EventHandler<FileEventArgs> NPCDeleted;
        public event EventHandler<FileEventArgs> AnimationDeleted;
        public event EventHandler<FileEventArgs> MapDeleted;
        public event EventHandler<FileEventArgs> ScriptDeleted;

        public event EventHandler<GameFileChangedEventArgs> NPCChanged;
        public event EventHandler<GameFileChangedEventArgs> AnimationChanged;
        public event EventHandler<GameFileChangedEventArgs> ItemChanged;
        public event EventHandler<GameFileChangedEventArgs> MapChanged;
        public event EventHandler<GameFileChangedEventArgs> ScriptChanged;

        public event EventHandler<FileEventArgs> ItemAdded;
        public event EventHandler<FileEventArgs> NPCAdded;
        public event EventHandler<FileEventArgs> AnimationAdded;
        public event EventHandler<FileEventArgs> MapAdded;
        public event EventHandler<FileEventArgs> ScriptAdded;
    }
}
