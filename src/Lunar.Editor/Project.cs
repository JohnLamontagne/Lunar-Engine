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

        public IEnumerable<FileInfo> Maps => _mapFiles.Values;
        public IEnumerable<FileInfo> Items => _itemFiles.Values;
        public IEnumerable<FileInfo> Animations => _animationFiles.Values;
        public IEnumerable<FileInfo> NPCs => _npcFiles.Values;

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
            return mapFile;
        }

        public FileInfo AddItem(string filePath)
        {
            var item = ItemDescriptor.Create();
            item.Name = Path.GetFileNameWithoutExtension(filePath);
            item.Save(filePath);
            var itemFile = new FileInfo(filePath);
            _itemFiles.Add(filePath, itemFile);
            return itemFile;
        }

        public FileInfo AddAnimation(string filePath)
        {
            var animation = AnimationDescription.Create();
            animation.Name = Path.GetFileNameWithoutExtension(filePath);
            animation.Save(filePath);
            var animationFile = new FileInfo(filePath);
            _animationFiles.Add(filePath, animationFile);
            return animationFile;
        }

        public void DeleteFile(string filePath)
        {
            // TODO: implement
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

            // Load all of the map files
            foreach (var file in mapDirectory.GetFiles("*" + EngineConstants.MAP_FILE_EXT, SearchOption.AllDirectories))
            {
                _mapFiles.Add(file.FullName, file);
            }

            var itemDirectory = new DirectoryInfo(projectDirectory.FullName + "/Items/");

            // Load all of the map files
            foreach (var file in itemDirectory.GetFiles("*" + EngineConstants.ITEM_FILE_EXT, SearchOption.AllDirectories))
            {
                _itemFiles.Add(file.FullName, file);
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

    }
}
