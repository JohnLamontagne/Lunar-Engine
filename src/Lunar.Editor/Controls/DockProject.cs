using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using DarkUI.Controls;
using DarkUI.Docking;

namespace Lunar.Editor.Controls
{
    public partial class DockProject : DarkToolWindow
    {
        private Project _project;

        #region Constructor Region

        public DockProject()
        {
            InitializeComponent();

            this.treeProject.MultiSelect = false;
            this.treeProject.MouseDoubleClick += TreeProject_MouseDoubleClick;
        }

        private void TreeProject_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (treeProject.SelectedNodes.Count > 0)
            {
                if (treeProject.SelectedNodes[0].Tag is FileInfo)
                {
                    this.File_Selected?.Invoke(this, new FileEventArgs((FileInfo)treeProject.SelectedNodes[0].Tag));   
                }
            }
        }

    

        #endregion

        public void InitalizeFromProject(Project project)
        {
            _project = project;

            treeProject.Nodes.Clear();

            var stack = new Stack<DarkTreeNode>();
            var node = new DarkTreeNode(_project.RootDirectory.Name)
            {
                Tag = _project.RootDirectory,
                Icon = Icons.folder_closed,
                ExpandedIcon = Icons.folder_open
            };
            stack.Push(node);

            while (stack.Count > 0)
            {
                var currentNode = stack.Pop();
                var directoryInfo = (DirectoryInfo)currentNode.Tag;
                foreach (var directory in directoryInfo.GetDirectories())
                {
                    var childDirectoryNode = new DarkTreeNode(directory.Name)
                    {
                        Tag = directory,
                        Icon = Icons.folder_closed,
                        ExpandedIcon = Icons.folder_open
                    };
                    currentNode.Nodes.Add(childDirectoryNode);
                    stack.Push(childDirectoryNode);
                }

                foreach (var file in _project.Files[directoryInfo.FullName])
                {
                    DarkTreeNode fileNode = new DarkTreeNode(file.Name)
                    {
                        Icon = Icons.document_16xLG,
                        Tag = file
                    };
                    
                    currentNode.Nodes.Add(fileNode);
                }
            }

            this.treeProject.Nodes.Add(node);
        }


        private void mapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.treeProject.SelectedNodes.Count > 0 && this.treeProject.SelectedNodes[0].Tag is DirectoryInfo)
            {
                using (SaveFileDialog dialog = new SaveFileDialog())
                {
                    dialog.RestoreDirectory = true;
                    dialog.InitialDirectory = ((DirectoryInfo)this.treeProject.SelectedNodes[0].Tag).FullName;
                    dialog.Filter = @"Lunar Engine Map Files (*.rmap)|*.rmap";
                    dialog.DefaultExt = ".rmap";
                    dialog.AddExtension = true;
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        string path = dialog.FileName;

                        var file = _project.AddMap(path);

                        this.TryAppendFileNode(file);
                    
                        this.File_Created?.Invoke(this, new FileEventArgs(file));
                    }
                }
            }

        }

        private void scriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.treeProject.SelectedNodes.Count > 0 && this.treeProject.SelectedNodes[0].Tag is DirectoryInfo)
            {
                using (SaveFileDialog dialog = new SaveFileDialog())
                {
                    dialog.RestoreDirectory = true;
                    dialog.InitialDirectory = ((DirectoryInfo) this.treeProject.SelectedNodes[0].Tag).FullName;
                    dialog.Filter = @"Lua Script Files (*.lua)|*.lua";
                    dialog.DefaultExt = ".lua";
                    dialog.AddExtension = true;
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        string path = dialog.FileName;

                        var file = _project.AddScript(path);

                        this.TryAppendFileNode(file);
                       
                        this.File_Created?.Invoke(this, new FileEventArgs(file));
                    }
                }
            }
        }

        private void TryAppendFileNode(FileInfo file)
        {
            // Make sure a node for this file does not already exist.
            foreach (var childNode in this.treeProject.SelectedNodes[0].Nodes)
            {
                if (childNode.Tag is FileInfo)
                {
                    if (((FileInfo) childNode.Tag).FullName == file.FullName)
                    {
                        return;
                    }
                }
            }

            DarkTreeNode fileNode = new DarkTreeNode(file.Name)
            {
                Icon = Icons.document_16xLG,
                Tag = file
            };

            this.treeProject.SelectedNodes[0].Nodes.Add(fileNode);
            this.treeProject.SelectNode(fileNode);

        }

        private void TryAppendDirectoryNode(DirectoryInfo directory)
        {
            // Make sure a node for this directory does not already exist.
            foreach (var childNode in this.treeProject.SelectedNodes[0].Nodes)
            {
                if (childNode.Tag is DirectoryInfo)
                {
                    if (((DirectoryInfo)childNode.Tag).FullName == directory.FullName)
                    {
                        return;
                    }
                }
            }

            var node = new DarkTreeNode(directory.Name)
            {
                Tag = _project.RootDirectory,
                Icon = Icons.folder_closed,
                ExpandedIcon = Icons.folder_open,
                Expanded = true
            };

            this.treeProject.SelectedNodes[0].Nodes.Add(node);
            this.treeProject.SelectNode(node);
        }

        private void TryRemoveDirectoryNode(DirectoryInfo directory)
        {
            if (this.treeProject.SelectedNodes[0].Tag is DirectoryInfo)
            {
                if (((DirectoryInfo)this.treeProject.SelectedNodes[0].Tag).FullName == directory.FullName)
                {
                    this.treeProject.SelectedNodes[0].ParentNode.Nodes.Remove(this.treeProject.SelectedNodes[0]);
                }
            }
        }

        private void TryRemoveFileNode(FileInfo file)
        {
            if (this.treeProject.SelectedNodes[0].Tag is FileInfo)
            {
                if (((FileInfo)this.treeProject.SelectedNodes[0].Tag).FullName == file.FullName)
                {
                    this.treeProject.SelectedNodes[0].ParentNode.Nodes.Remove(this.treeProject.SelectedNodes[0]);
                }
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.treeProject.SelectedNodes.Count > 0)
            {
                if (this.treeProject.SelectedNodes[0].Tag is FileInfo)
                {
                    _project.DeleteFile(((FileInfo) this.treeProject.SelectedNodes[0].Tag).FullName);
                    this.File_Removed?.Invoke(this,
                        new FileEventArgs((FileInfo) this.treeProject.SelectedNodes[0].Tag));
                    this.TryRemoveFileNode(((FileInfo) this.treeProject.SelectedNodes[0].Tag));
                }
                else if (this.treeProject.SelectedNodes[0].Tag is DirectoryInfo)
                {
                    _project.DeleteDirectory(((DirectoryInfo) this.treeProject.SelectedNodes[0].Tag).FullName);
                    this.TryRemoveDirectoryNode(((DirectoryInfo)this.treeProject.SelectedNodes[0].Tag));
                }
                
            }
        }

        private void folderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.treeProject.SelectedNodes.Count > 0 && this.treeProject.SelectedNodes[0].Tag is DirectoryInfo)
            {
                using (CreateDirectoryDialog dialog = new CreateDirectoryDialog())
                {
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        DirectoryInfo directory = _project.AddDirectory(((DirectoryInfo)this.treeProject.SelectedNodes[0].Tag).FullName
                                                                        + "/" + dialog.DirectoryPath);

                        this.TryAppendDirectoryNode(directory);
                    }
                }
            }
        }

        public event EventHandler<FileEventArgs> File_Selected;

        public event EventHandler<FileEventArgs> File_Created;

        public event EventHandler<FileEventArgs> File_Removed;

        public class FileEventArgs : EventArgs
        {
            private readonly FileInfo _file;

            public FileInfo File => _file;

            public FileEventArgs(FileInfo file)
            {
                _file = file;
            }
        }

     
    }
}
