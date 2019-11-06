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
using Lunar.Server.World.Conversation;
using Newtonsoft.Json.Linq;
using System.Text;
using Newtonsoft.Json;
using DarkUI.Controls;
using Lunar.Graphics.Effects;

namespace Lunar.Editor
{
    public class Project
    {
        private DialogueFactory _dialogueFactory;

        private IDataManager<MapModel<LayerModel<TileModel<SpriteInfo>>>> _mapDataManager;
        private IDataManager<BaseAnimation<IAnimationLayer<SpriteInfo>>> _animationDataManager;
        private IDataManager<NPCModel> _npcDataManager;
        private IDataManager<ItemModel> _itemDataManager;
        private IDataManager<SpellModel> _spellDataManager;

        private JObject _scriptMap;

        private readonly DirectoryInfo _serverRootDirectory;
        private readonly DirectoryInfo _serverWorldDirectory;
        private readonly DirectoryInfo _clientDirectory;
        private readonly List<DirectoryInfo> _directories;
        private readonly string _projectPath;

        public DirectoryInfo ServerWorldDirectory => _serverWorldDirectory;

        public DirectoryInfo ServerRootDirectory => _serverRootDirectory;

        public DirectoryInfo ClientRootDirectory => _clientDirectory;

        private Dictionary<string, FileInfo> _mapFiles;
        private Dictionary<string, FileInfo> _itemFiles;
        private Dictionary<string, FileInfo> _npcFiles;
        private Dictionary<string, FileInfo> _animationFiles;
        private Dictionary<string, FileInfo> _scriptFiles;
        private Dictionary<string, FileInfo> _dialogueFiles;
        private Dictionary<string, FileInfo> _spellFiles;

        private readonly Dictionary<string, Map> _maps;
        private readonly Dictionary<string, ItemModel> _items;
        private readonly Dictionary<string, NPCModel> _npcs;
        private readonly Dictionary<string, Animation> _animations;
        private readonly Dictionary<string, Dialogue> _dialogues;
        private readonly Dictionary<string, SpellModel> _spells;

        public JObject ScriptMap { get => _scriptMap; }

        public Dictionary<string, Map> Maps => _maps;
        public Dictionary<string, ItemModel> Items => _items;
        public Dictionary<string, NPCModel> NPCs => _npcs;
        public Dictionary<string, Animation> Animations => _animations;

        public Dictionary<string, SpellModel> Spells => _spells;

        public Dictionary<string, Dialogue> Dialogues => _dialogues;

        public IEnumerable<FileInfo> MapFiles => _mapFiles.Values;
        public IEnumerable<FileInfo> ItemFiles => _itemFiles.Values;
        public IEnumerable<FileInfo> AnimationFiles => _animationFiles.Values;

        public IEnumerable<FileInfo> ScriptFiles => _scriptFiles.Values;
        public IEnumerable<FileInfo> NPCFiles => _npcFiles.Values;

        public IEnumerable<FileInfo> DialogueFiles => _dialogueFiles.Values;

        public IEnumerable<FileInfo> SpellFiles => _spellFiles.Values;

        public IEnumerable<DirectoryInfo> Directories => _directories;

        public string GameName { get; set; }

        private Project(string projectPath, string serverDir, string clientDir)
        {
            _projectPath = projectPath;
            _serverRootDirectory = new DirectoryInfo(serverDir);
            _serverWorldDirectory = new DirectoryInfo(serverDir + "/World/");
            _clientDirectory = new DirectoryInfo(clientDir);

            _dialogueFactory = new DialogueFactory();

            _mapFiles = new Dictionary<string, FileInfo>();
            _npcFiles = new Dictionary<string, FileInfo>();
            _itemFiles = new Dictionary<string, FileInfo>();
            _animationFiles = new Dictionary<string, FileInfo>();
            _scriptFiles = new Dictionary<string, FileInfo>();
            _dialogueFiles = new Dictionary<string, FileInfo>();
            _spellFiles = new Dictionary<string, FileInfo>();

            _maps = new Dictionary<string, Map>();
            _items = new Dictionary<string, ItemModel>();
            _npcs = new Dictionary<string, NPCModel>();
            _animations = new Dictionary<string, Animation>();
            _dialogues = new Dictionary<string, Dialogue>();
            _spells = new Dictionary<string, SpellModel>();

            _directories = new List<DirectoryInfo>();

            IDataManagerFactory dataManagerFactory = new FSDataFactory();
            dataManagerFactory.Initalize();

            _mapDataManager = dataManagerFactory.Create<MapModel<LayerModel<TileModel<SpriteInfo>>>>(new FSDataFactoryArguments(_serverWorldDirectory + "/Maps/"));
            _animationDataManager = dataManagerFactory.Create<BaseAnimation<IAnimationLayer<SpriteInfo>>>(new FSDataFactoryArguments(_serverWorldDirectory + "/Animations/"));
            _npcDataManager = dataManagerFactory.Create<NPCModel>(new FSDataFactoryArguments(_serverWorldDirectory + "/NPCs/"));
            _itemDataManager = dataManagerFactory.Create<ItemModel>(new FSDataFactoryArguments(_serverWorldDirectory + "/Items/"));
            _spellDataManager = dataManagerFactory.Create<SpellModel>(new FSDataFactoryArguments(_serverWorldDirectory + "/Spells/"));

            if (File.Exists(this.ServerRootDirectory + "/internal/.scriptmap"))
            {
                string text = File.ReadAllText(this.ServerRootDirectory + "/internal/.scriptmap");

                if (!string.IsNullOrEmpty(text))
                    _scriptMap = JObject.Parse(text);
                else
                    _scriptMap = new JObject();
            }
            else
            {
                _scriptMap = new JObject();
            }

            this.LoadContents(this.ServerWorldDirectory);
        }

        public void SaveScriptMap()
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                this.ScriptMap.WriteTo(writer);
            }
            sw.Close();
            File.WriteAllText(this.ServerRootDirectory + "/internal/" + ".scriptmap", sb.ToString());
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

        public FileInfo AddScriptToGameContent(FileInfo contentFile, string scriptName)
        {
            string directory = contentFile.DirectoryName + "/.scripts/";

            // Make sure the .scripts directory exists.
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            string filePath = directory + scriptName + ".py";

            if (!this.ScriptMap.ContainsKey(contentFile.Name))
            {
                _scriptMap.Add(contentFile.Name, JToken.FromObject(new List<string>()));
            }

            _scriptMap.Value<JArray>(contentFile.Name).Add(Helpers.MakeRelative(filePath, this.ServerRootDirectory.FullName + "/"));

            this.SaveScriptMap();

            FileInfo scriptFile = this.AddScript(Helpers.GetNextAvailableFilename(filePath));

            this.ScriptAdded?.Invoke(this, new FileEventArgs(scriptFile));

            return scriptFile;
        }

        public void RemoveScript(string filePath)
        {
            File.Delete(filePath);
            this.ScriptDeleted?.Invoke(this, new FileEventArgs(_scriptFiles[Helpers.NormalizePath(filePath)]));
            _scriptFiles.Remove(Helpers.NormalizePath(filePath));

            List<JToken> refsToRemove = new List<JToken>();
            foreach (var content in this.ScriptMap.Values())
                foreach (var script in content.Values())
                    if (script.ToString() == Helpers.MakeRelative(filePath, this.ServerRootDirectory.FullName + "/"))
                        refsToRemove.Add(script);

            foreach (var refToRemove in refsToRemove)
                refToRemove.Remove();

            this.SaveScriptMap();
        }

        public void SaveScript(string filePath, string contents)
        {
            File.WriteAllText(filePath, contents);
        }

        public FileInfo LoadScript(string filePath)
        {
            FileInfo file = new FileInfo(Helpers.NormalizePath(filePath));
            _scriptFiles.Add(Helpers.NormalizePath(filePath), file);
            return file;
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
            var mapDescriptor = _mapDataManager.Load(new ContentFileDataLoaderArguments(Path.GetFileNameWithoutExtension(filePath)));

            var map = new Map(mapDescriptor, textureLoader, this);

            if (!_maps.ContainsKey(Helpers.NormalizePath(filePath)))
                _maps.Add(Helpers.NormalizePath(filePath), map);
            else
                _maps[Helpers.NormalizePath(filePath)] = map;

            return map;
        }

        public void SaveMap(string filePath, Map map)
        {
            _mapDataManager.Save(map, new ContentFileDataLoaderArguments(Path.GetFileNameWithoutExtension(filePath)));
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

        public void UnloadMap(string filePath)
        {
            if (_maps.ContainsKey(Helpers.NormalizePath(filePath)))
            {
                _maps.Remove(Helpers.NormalizePath(filePath));
            }
        }

        public void RemoveMap(string filePath)
        {
            if (!_mapFiles.ContainsKey(Helpers.NormalizePath(filePath)))
                return;

            var mapFile = _mapFiles[Helpers.NormalizePath(filePath)];

            _maps.Remove(Helpers.NormalizePath(filePath));

            this.MapDeleted?.Invoke(this, new FileEventArgs(mapFile));

            File.Delete(filePath);

            _mapFiles.Remove(Helpers.NormalizePath(filePath));
        }

        public FileInfo AddDialogue(string filePath)
        {
            var dialogue = _dialogueFactory.Create(filePath);
            var dialogueFile = new FileInfo(filePath);

            if (!_dialogueFiles.ContainsKey(Helpers.NormalizePath(filePath)))
                _dialogueFiles.Add(Helpers.NormalizePath(filePath), dialogueFile);

            this.DialogueAdded?.Invoke(this, new FileEventArgs(dialogueFile));

            return dialogueFile;
        }

        public void SaveDialogue(string filePath, Dialogue dialogue)
        {
            _dialogueFactory.Save(dialogue, filePath);
        }

        public Dialogue LoadDialogue(string filePath)
        {
            if (_dialogues.ContainsKey(Helpers.NormalizePath(filePath)))
                return _dialogues[Helpers.NormalizePath(filePath)];

            var dialogue = _dialogueFactory.LoadDialogue(filePath);
            _dialogues.Add(Helpers.NormalizePath(filePath), dialogue);

            return dialogue;
        }

        public void UnloadDialogue(string filePath)
        {
            if (_dialogues.ContainsKey(Helpers.NormalizePath(filePath)))
            {
                _dialogues.Remove(Helpers.NormalizePath(filePath));
            }
        }

        public FileInfo ChangeDialogue(string oldFilePath, string newFilePath)
        {
            var oldFile = _dialogueFiles[Helpers.NormalizePath(oldFilePath)];
            _dialogueFiles.Remove(Helpers.NormalizePath(oldFilePath));
            var file = new FileInfo(newFilePath);
            _dialogueFiles.Add(Helpers.NormalizePath(newFilePath), new FileInfo(newFilePath));

            var dialogue = _dialogues[Helpers.NormalizePath(oldFilePath)];
            _dialogues.Remove(Helpers.NormalizePath(oldFilePath));
            _dialogues.Add(Helpers.NormalizePath(newFilePath), dialogue);

            this.DialogueChanged?.Invoke(this, new GameFileChangedEventArgs(oldFile, file));

            return file;
        }

        public void RemoveDialogue(string filePath)
        {
            if (!_dialogueFiles.ContainsKey(Helpers.NormalizePath(filePath)))
                return;

            if (_dialogues.ContainsKey(Helpers.NormalizePath(filePath)))
                _dialogues.Remove(Helpers.NormalizePath(filePath));

            this.DialogueDeleted?.Invoke(this, new FileEventArgs(_dialogueFiles[Helpers.NormalizePath(filePath)]));

            File.Delete(filePath);

            _dialogueFiles.Remove(Helpers.NormalizePath(filePath));
        }

        public FileInfo AddItem(string filePath)
        {
            var item = ItemModel.Create();
            item.Name = Path.GetFileNameWithoutExtension(filePath);
            _itemDataManager.Save(item, new ContentFileDataLoaderArguments(Path.GetFileNameWithoutExtension(filePath)));
            var itemFile = new FileInfo(filePath);

            if (!_itemFiles.ContainsKey(Helpers.NormalizePath(filePath)))
                _itemFiles.Add(Helpers.NormalizePath(filePath), itemFile);

            this.ItemAdded?.Invoke(this, new FileEventArgs(itemFile));

            return itemFile;
        }

        public void UnloadItem(string filePath)
        {
            if (_items.ContainsKey(Helpers.NormalizePath(filePath)))
            {
                _items.Remove(Helpers.NormalizePath(filePath));
            }
        }

        public ItemModel LoadItem(string filePath)
        {
            if (_items.ContainsKey(Helpers.NormalizePath(filePath)))
                return _items[Helpers.NormalizePath(filePath)];

            var item = _itemDataManager.Load(new ContentFileDataLoaderArguments(Path.GetFileNameWithoutExtension(filePath)));
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

        public void SaveItem(string filePath, ItemModel item)
        {
            _itemDataManager.Save(item, new ContentFileDataLoaderArguments(Path.GetFileNameWithoutExtension(filePath)));
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

        //
        public FileInfo AddSpell(string filePath)
        {
            var spell = new SpellModel();
            spell.Name = Path.GetFileNameWithoutExtension(filePath);
            _spellDataManager.Save(spell, new ContentFileDataLoaderArguments(Path.GetFileNameWithoutExtension(filePath)));
            var spellFile = new FileInfo(filePath);

            if (!_spellFiles.ContainsKey(Helpers.NormalizePath(filePath)))
                _spellFiles.Add(Helpers.NormalizePath(filePath), spellFile);

            this.SpellAdded?.Invoke(this, new FileEventArgs(spellFile));

            return spellFile;
        }

        public void UnloadSpell(string filePath)
        {
            if (_spells.ContainsKey(Helpers.NormalizePath(filePath)))
            {
                _spells.Remove(Helpers.NormalizePath(filePath));
            }
        }

        public SpellModel LoadSpell(string filePath)
        {
            if (_spells.ContainsKey(Helpers.NormalizePath(filePath)))
                return _spells[Helpers.NormalizePath(filePath)];

            var spell = _spellDataManager.Load(new ContentFileDataLoaderArguments(Path.GetFileNameWithoutExtension(filePath)));
            _spells.Add(Helpers.NormalizePath(filePath), spell);

            return spell;
        }

        public void AddSpell(FileInfo file)
        {
            if (!_spellFiles.ContainsKey(Helpers.NormalizePath(file.FullName)))
                _spellFiles.Add(Helpers.NormalizePath(file.FullName), file);

            this.SpellAdded?.Invoke(this, new FileEventArgs(file));
        }

        public FileInfo ChangeSpell(string oldFilePath, string newFilePath)
        {
            var oldFile = _spellFiles[Helpers.NormalizePath(oldFilePath)];
            _spellFiles.Remove(Helpers.NormalizePath(oldFilePath));
            var file = new FileInfo(newFilePath);
            _spellFiles.Add(Helpers.NormalizePath(newFilePath), new FileInfo(newFilePath));

            var spell = _spells[Helpers.NormalizePath(oldFilePath)];
            _spells.Remove(Helpers.NormalizePath(oldFilePath));
            _spells.Add(Helpers.NormalizePath(newFilePath), spell);

            this.SpellChanged?.Invoke(this, new GameFileChangedEventArgs(oldFile, file));

            return file;
        }

        public void SaveSpell(string filePath, SpellModel spell)
        {
            _spellDataManager.Save(spell, new ContentFileDataLoaderArguments(Path.GetFileNameWithoutExtension(filePath)));
        }

        public void RemoveSpell(string filePath)
        {
            if (!_spellFiles.ContainsKey(Helpers.NormalizePath(filePath)))
                return;

            if (_spells.ContainsKey(Helpers.NormalizePath(filePath)))
                _spells.Remove(Helpers.NormalizePath(filePath));

            this.SpellDeleted?.Invoke(this, new FileEventArgs(_spellFiles[Helpers.NormalizePath(filePath)]));

            File.Delete(filePath);

            _spellFiles.Remove(Helpers.NormalizePath(filePath));
        }

        public FileInfo AddAnimation(string filePath)
        {
            var animation = Animation.Create();
            animation.Name = Path.GetFileNameWithoutExtension(filePath);
            animation.Save(filePath);
            var animationFile = new FileInfo(filePath);
            _animationFiles.Add(Helpers.NormalizePath(filePath), animationFile);

            this.AnimationAdded?.Invoke(this, new FileEventArgs(animationFile));

            return animationFile;
        }

        public Animation LoadAnimation(string filePath)
        {
            if (_animations.ContainsKey(Helpers.NormalizePath(filePath)))
                return _animations[Helpers.NormalizePath(filePath)];

            var animationDesc = _animationDataManager.Load(new ContentFileDataLoaderArguments(Path.GetFileNameWithoutExtension(filePath)));

            var animation = new Animation(animationDesc);

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
            var npc = NPCModel.Create(uniqueID);
            npc.Name = Path.GetFileNameWithoutExtension(filePath);
            _npcDataManager.Save(npc, new ContentFileDataLoaderArguments(Path.GetFileNameWithoutExtension(filePath)));
            var npcFile = new FileInfo(filePath);
            _npcFiles.Add(Helpers.NormalizePath(filePath), npcFile);

            this.NPCAdded?.Invoke(this, new FileEventArgs(npcFile));

            return npcFile;
        }

        public void SaveNPC(string filePath, NPCModel npc)
        {
            _npcDataManager.Save(npc, new ContentFileDataLoaderArguments(Path.GetFileNameWithoutExtension(filePath)));
        }

        public NPCModel LoadNPC(string filePath)
        {
            if (_npcs.ContainsKey(Helpers.NormalizePath(filePath)))
                return _npcs[Helpers.NormalizePath(filePath)];

            var npc = _npcDataManager.Load(new ContentFileDataLoaderArguments(Path.GetFileNameWithoutExtension(filePath)));
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

        private Dictionary<string, FileInfo> LoadContentFiles(DirectoryInfo directory, string extension)
        {
            Dictionary<string, FileInfo> files = new Dictionary<string, FileInfo>();

            if (!directory.Exists)
            {
                Directory.CreateDirectory(directory.FullName);
            }

            foreach (var file in directory.GetFiles("*" + extension, SearchOption.AllDirectories))
            {
                files.Add(Helpers.NormalizePath(file.FullName), file);
            }

            return files;
        }

        private void LoadContents(DirectoryInfo projectDirectory)
        {
            _mapFiles = this.LoadContentFiles(new DirectoryInfo(projectDirectory.FullName + "/Maps/"), EngineConstants.MAP_FILE_EXT);
            _itemFiles = this.LoadContentFiles(new DirectoryInfo(projectDirectory.FullName + "/Items/"), EngineConstants.ITEM_FILE_EXT);
            _npcFiles = this.LoadContentFiles(new DirectoryInfo(projectDirectory.FullName + "/Npcs/"), EngineConstants.NPC_FILE_EXT);
            _scriptFiles = this.LoadContentFiles(new DirectoryInfo(projectDirectory.FullName + "/Scripts/"), EngineConstants.SCRIPT_FILE_EXT);
            _animationFiles = this.LoadContentFiles(new DirectoryInfo(projectDirectory.FullName + "/Animations/"), EngineConstants.ANIM_FILE_EXT);
            _dialogueFiles = this.LoadContentFiles(new DirectoryInfo(projectDirectory.FullName + "/Dialogues/"), EngineConstants.DIALOGUE_FILE_EXT);

            // Load content scripts
            foreach (var entry in _scriptMap)
            {
                string contentPath = entry.Key;

                foreach (var scriptPath in entry.Value.ToObject<string[]>())
                {
                    if (File.Exists(scriptPath))
                    {
                        var file = new FileInfo(scriptPath);
                        _scriptFiles.Add(Helpers.NormalizePath(file.FullName), file);
                    }
                }
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

        public event EventHandler<FileEventArgs> SpellDeleted;

        public event EventHandler<FileEventArgs> DialogueDeleted;

        public event EventHandler<GameFileChangedEventArgs> NPCChanged;

        public event EventHandler<GameFileChangedEventArgs> AnimationChanged;

        public event EventHandler<GameFileChangedEventArgs> ItemChanged;

        public event EventHandler<GameFileChangedEventArgs> MapChanged;

        public event EventHandler<GameFileChangedEventArgs> ScriptChanged;

        public event EventHandler<GameFileChangedEventArgs> DialogueChanged;

        public event EventHandler<GameFileChangedEventArgs> SpellChanged;

        public event EventHandler<FileEventArgs> ItemAdded;

        public event EventHandler<FileEventArgs> NPCAdded;

        public event EventHandler<FileEventArgs> AnimationAdded;

        public event EventHandler<FileEventArgs> MapAdded;

        public event EventHandler<FileEventArgs> ScriptAdded;

        public event EventHandler<FileEventArgs> DialogueAdded;

        public event EventHandler<FileEventArgs> SpellAdded;
    }
}