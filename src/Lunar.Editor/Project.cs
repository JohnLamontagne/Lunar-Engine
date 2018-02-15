using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Lunar.Editor.Utilities;
using Lunar.Editor.World;

namespace Lunar.Editor
{
    public class Project
    {
        private readonly Settings _settings;
        private readonly DirectoryInfo _directory;
        private readonly List<DirectoryInfo> _directories;
        private readonly Dictionary<string, List<FileInfo>> _files;

        public Settings Settings => _settings;

        public DirectoryInfo RootDirectory => _directory;

        public IEnumerable<DirectoryInfo> Directories => _directories;

        public IDictionary<string, List<FileInfo>> Files => _files;

        private Project(string projectPath)
        {
            _directory = new DirectoryInfo(projectPath);
            _settings = new Settings(projectPath);

            _directories = new List<DirectoryInfo>();
            _files = new Dictionary<string, List<FileInfo>>();

            this.LoadContents(this.RootDirectory);
        }

        public static Project Load(string projectPath)
        {
            var project = new Project(projectPath);

            return project;
        }

        private FileInfo AddFile(string filePath)
        {
            var f = File.Create(filePath);
            f.Close();
            
            var fileInfo = new FileInfo(filePath);
            _files[fileInfo.Directory.FullName].Add(fileInfo);

            return fileInfo;
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

        public void DeleteFile(string filePath)
        {
            _files.Remove(filePath);
            File.Delete(filePath);
        }

        public DirectoryInfo AddDirectory(string directoryPath)
        {
            DirectoryInfo directoryInfo = Directory.CreateDirectory(directoryPath);

            _directories.Add(directoryInfo);

            return directoryInfo;
        }

        public void DeleteDirectory(string directoryPath)
        {
            _files.Remove(directoryPath);

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
                    _files.Add(projectDirectory.FullName, new List<FileInfo>());
                }

                foreach (FileInfo f in projectDirectory.GetFiles())
                {
                    //Console.WriteLine("File {0}", f.FullName);
                   
                    _files[projectDirectory.FullName].Add(f);
                }
            }
            catch
            {
                Console.WriteLine("Directory {0}  \n could not be accessed!!!!", projectDirectory.FullName);
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

    }
}
