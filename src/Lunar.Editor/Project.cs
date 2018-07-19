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
using Lunar.Core.Content.Graphics;
using Lunar.Core.World;

namespace Lunar.Editor
{
    public class Project
    {
        private readonly DirectoryInfo _serverDirectory;
        private readonly DirectoryInfo _clientDirectory;
        private readonly List<DirectoryInfo> _directories;
        private readonly Dictionary<string, List<FileInfo>> _files;
        private string _projectPath;

        public DirectoryInfo ServerRootDirectory => _serverDirectory;
        public DirectoryInfo ClientRootDirectory => _clientDirectory;

        public IEnumerable<DirectoryInfo> Directories => _directories;

        public string GameName { get; set; }


        private Project(string projectPath, string serverDir, string clientDir)
        {
            _projectPath = projectPath;
            _serverDirectory = new DirectoryInfo(serverDir);
            _clientDirectory = new DirectoryInfo(clientDir);

            _directories = new List<DirectoryInfo>();
            _files = new Dictionary<string, List<FileInfo>>();

            this.LoadContents(this.ServerRootDirectory);
            this.LoadContents(this.ClientRootDirectory);
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

        private FileInfo AddFile(string filePath)
        {
            var f = File.Create(filePath);
            f.Close();
            
            var fileInfo = new FileInfo(filePath);
            this.GetFiles(Path.GetFullPath(fileInfo.Directory.FullName)).Add(fileInfo);

            return fileInfo;
        }

        public List<FileInfo> GetFiles(string path)
        {
            string corrected = path.Replace(@"\", "/") + "/";
            return _files[corrected];
        }

        public FileInfo AddScript(string filePath)
        {
            return this.AddFile(filePath);
        }

        public FileInfo AddMap(string filePath)
        {
            var map = new Map(new Vector2(Constants.NEW_MAP_X, Constants.NEW_MAP_Y), "blank");
            map.Save(filePath);
            return new FileInfo(filePath);
        }

        public FileInfo AddItem(string filePath)
        {
            var item = ItemDescriptor.Create();
            item.Save(filePath);
            return new FileInfo(filePath);
        }

        public FileInfo AddAnimation(string filePath)
        {
            var animation = AnimationDescription.Create();
            animation.Save(filePath);
            return new FileInfo(filePath);
        }

        public void DeleteFile(string filePath)
        {
            _files.Remove(Path.GetFullPath(filePath));
            File.Delete(filePath);
        }

        public DirectoryInfo AddDirectory(string directoryPath)
        {

            DirectoryInfo directoryInfo = Directory.CreateDirectory(directoryPath);

            _directories.Add(directoryInfo);

            string correctedPath = Path.GetFullPath(directoryPath).Replace(@"\", "/");

            if (!correctedPath.EndsWith("/"))
                correctedPath += "/";
            
            if (!_files.ContainsKey(correctedPath))
                _files.Add(correctedPath, new List<FileInfo>());

            return directoryInfo;
        }

        public void DeleteDirectory(string directoryPath)
        {
            _files.Remove(Path.GetFullPath(directoryPath).Replace(@"\", "/"));

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
            try
            {
                if (!_files.ContainsKey(projectDirectory.FullName))
                {
                    this.AddDirectory(projectDirectory.FullName);
                }

                foreach (FileInfo f in projectDirectory.GetFiles())
                {
                    //Console.WriteLine("File {0}", f.FullName);

                    this.GetFiles(Path.GetFullPath(projectDirectory.FullName)).Add(f);
                }
            }
            catch
            {
                Console.WriteLine(@"Directory {0} could not be accessed!!!!", projectDirectory.FullName);
                return;  // We alredy got an error trying to access dir so dont try to access it again
            }

            // process each directory
            // If I have been able to see the files in the directory I should also be able 
            // to look at its directories so I dont think I should place this in a try catch block
            foreach (DirectoryInfo d in projectDirectory.GetDirectories())
            {
                _directories.Add(d);
                this.LoadContents(d);
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
