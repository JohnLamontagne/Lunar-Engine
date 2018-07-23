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
using System.Windows.Forms;
using DarkUI.Controls;
using DarkUI.Docking;
using Lunar.Core;

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
                if (treeProject.SelectedNodes[0].Tag is FileInfo info)
                {
                    this.File_Selected?.Invoke(this, new FileEventArgs(info));
                }
                else
                {
                    (treeProject.SelectedNodes[0].Tag as Action<DarkTreeNode>)?.Invoke(treeProject.SelectedNodes[0].ParentNode);
                }
            }
        }



        #endregion

        private DarkTreeNode BuildAnimationTree()
        {
            var animationPathNode = new DarkTreeNode("Animations")
            {
                Icon = Icons.folder_closed,
                ExpandedIcon = Icons.folder_open
            };

            foreach (var animationFile in _project.NPCs)
            {
                var fileNode = new DarkTreeNode(animationFile.Name)
                {
                    Tag = animationFile,
                    Icon = Icons.document_16xLG,
                };
                animationPathNode.Nodes.Add(fileNode);
            }

            var addNode = new DarkTreeNode("Add Animation")
            {
                Icon = Icons.Plus,
                Tag = (Action<DarkTreeNode>)((node) =>
                {
                    using (SaveFileDialog dialog = new SaveFileDialog())
                    {
                        dialog.RestoreDirectory = true;
                        dialog.InitialDirectory = _project.ServerRootDirectory.FullName + @"\Animations";
                        dialog.Filter = $@"Lunar Engine Animation Files (*{EngineConstants.ANIM_FILE_EXT})|*{EngineConstants.ANIM_FILE_EXT}";
                        dialog.DefaultExt = EngineConstants.ANIM_FILE_EXT;
                        dialog.AddExtension = true;
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            string path = dialog.FileName;

                            var file = _project.AddAnimation(path);

                            DarkTreeNode fileNode = new DarkTreeNode(file.Name)
                            {
                                Icon = Icons.document_16xLG,
                                Tag = file
                            };

                            ((DarkTreeNode)node).Nodes.Insert(((DarkTreeNode)node).Nodes.Count - 1, fileNode);

                            // HACK BECAUSE ROBIN DIDN'T IMPLEMENT DarkTreeNode.Insert() properly!!!!!!!!!!!!!!!!!!!!
                            ((DarkTreeNode)node).Nodes.Add(new DarkTreeNode());
                            ((DarkTreeNode)node).Nodes.RemoveAt(((DarkTreeNode)node).Nodes.Count - 1);
                            // END HACK

                            this.File_Created?.Invoke(this, new FileEventArgs(file));
                        }
                    }
                })
            };

            animationPathNode.Nodes.Add(addNode);

            return animationPathNode;
        }

        private DarkTreeNode BuildMapTree()
        {
            var mapPathNode = new DarkTreeNode("Maps")
            {
                Icon = Icons.folder_closed,
                ExpandedIcon = Icons.folder_open
            };

            foreach (var mapFile in _project.Maps)
            {
                var fileNode = new DarkTreeNode(mapFile.Name)
                {
                    Tag = mapFile,
                    Icon = Icons.document_16xLG,
                };
                mapPathNode.Nodes.Add(fileNode);
            }

            var addNode = new DarkTreeNode("Add Map")
            {
                Icon = Icons.Plus,
                Tag = (Action<DarkTreeNode>) ((node) =>
                {
                    using (SaveFileDialog dialog = new SaveFileDialog())
                    {
                        dialog.RestoreDirectory = true;
                        dialog.InitialDirectory = _project.ServerRootDirectory.FullName + @"\Maps";
                        dialog.Filter = $@"Lunar Engine Item Files (*{EngineConstants.MAP_FILE_EXT})|*{EngineConstants.MAP_FILE_EXT}";
                        dialog.DefaultExt = EngineConstants.MAP_FILE_EXT;
                        dialog.AddExtension = true;
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            string path = dialog.FileName;

                            var file = _project.AddMap(path);

                            DarkTreeNode fileNode = new DarkTreeNode(file.Name)
                            {
                                Icon = Icons.document_16xLG,
                                Tag = file
                            };

                            ((DarkTreeNode)node).Nodes.Insert(((DarkTreeNode)node).Nodes.Count - 1, fileNode);

                            // HACK BECAUSE ROBIN DIDN'T IMPLEMENT DarkTreeNode.Insert() properly!!!!!!!!!!!!!!!!!!!!
                            ((DarkTreeNode)node).Nodes.Add(new DarkTreeNode());
                            ((DarkTreeNode)node).Nodes.RemoveAt(((DarkTreeNode)node).Nodes.Count - 1);
                            // END HACK

                            this.File_Created?.Invoke(this, new FileEventArgs(file));
                        }
                    }
                })
            };
            
            mapPathNode.Nodes.Add(addNode);

            return mapPathNode;
        }

        private DarkTreeNode BuildItemTree()
        {
            var itemPathNode = new DarkTreeNode("Items")
            {
                Icon = Icons.folder_closed,
                ExpandedIcon = Icons.folder_open
            };

            foreach (var itemFile in _project.Items)
            {
                var fileNode = new DarkTreeNode(itemFile.Name)
                {
                    Tag = itemFile,
                    Icon = Icons.document_16xLG,
                };
                itemPathNode.Nodes.Add(fileNode);
            }

            var addNode = new DarkTreeNode("Add Item")
            {
                Icon = Icons.Plus,
                Tag = (Action<DarkTreeNode>) ((node) =>
                {
                    using (SaveFileDialog dialog = new SaveFileDialog())
                    {
                        dialog.InitialDirectory = _project.ServerRootDirectory.FullName + @"\Items";
                        dialog.RestoreDirectory = true;
                        dialog.Filter = $@"Lunar Engine Item Files (*{EngineConstants.ITEM_FILE_EXT})|*{EngineConstants.ITEM_FILE_EXT}";
                        dialog.DefaultExt = EngineConstants.ITEM_FILE_EXT;
                        dialog.AddExtension = true;
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            string path = dialog.FileName;

                            var file = _project.AddItem(path);

                            DarkTreeNode fileNode = new DarkTreeNode(file.Name)
                            {
                                Icon = Icons.document_16xLG,
                                Tag = file
                            };

                            ((DarkTreeNode)node).Nodes.Insert(((DarkTreeNode)node).Nodes.Count - 1, fileNode);

                            // HACK BECAUSE ROBIN DIDN'T IMPLEMENT DarkTreeNode.Insert() properly!!!!!!!!!!!!!!!!!!!!
                            ((DarkTreeNode)node).Nodes.Add(new DarkTreeNode());
                            ((DarkTreeNode)node).Nodes.RemoveAt(((DarkTreeNode)node).Nodes.Count - 1);
                            // END HACK

                            this.File_Created?.Invoke(this, new FileEventArgs(file));
                        }
                    }
                })
            };


            itemPathNode.Nodes.Add(addNode);

            return itemPathNode;
        }

        private DarkTreeNode InitalizeProjectTree()
        {
            DarkTreeNode projectTreeNode = new DarkTreeNode("Game Data")
            {
                Icon = Icons.folder_closed,
                ExpandedIcon = Icons.folder_open
            };

            projectTreeNode.Nodes.Add(this.BuildMapTree());
            projectTreeNode.Nodes.Add(this.BuildItemTree());
            projectTreeNode.Nodes.Add(this.BuildAnimationTree());

            return projectTreeNode;
        }

        public void InitalizeFromProject(Project project)
        {
            _project = project;

            treeProject.Nodes.Clear();

            var node = new DarkTreeNode(_project.GameName)
            {
                Icon = Icons.folder_closed,
                ExpandedIcon = Icons.folder_open
            };

            node.Nodes.Add(this.InitalizeProjectTree());

            treeProject.Nodes.Add(node);
        }


        private void scriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.treeProject.SelectedNodes.Count > 0 && this.treeProject.SelectedNodes[0].Tag is DirectoryInfo)
            {
                using (SaveFileDialog dialog = new SaveFileDialog())
                {
                    dialog.RestoreDirectory = true;
                    dialog.InitialDirectory = ((DirectoryInfo) this.treeProject.SelectedNodes[0].Tag).FullName;
                    dialog.Filter = $@"Lua Script Files (*{EngineConstants.LUA_FILE_EXT})|*{EngineConstants.LUA_FILE_EXT}";
                    dialog.DefaultExt = EngineConstants.LUA_FILE_EXT;
                    dialog.AddExtension = true;
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        string path = dialog.FileName;

                        var file = _project.AddScript(path);

                        
                       
                        this.File_Created?.Invoke(this, new FileEventArgs(file));
                    }
                }
            }
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


        private void animationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.treeProject.SelectedNodes.Count > 0 && this.treeProject.SelectedNodes[0].Tag is DirectoryInfo)
            {
                using (SaveFileDialog dialog = new SaveFileDialog())
                {
                    dialog.RestoreDirectory = true;
                    dialog.InitialDirectory = ((DirectoryInfo)this.treeProject.SelectedNodes[0].Tag).FullName;
                    dialog.Filter = $@"Lunar Engine Animation Files (*{EngineConstants.ANIM_FILE_EXT})|*{EngineConstants.ANIM_FILE_EXT}";
                    dialog.DefaultExt = EngineConstants.ANIM_FILE_EXT;
                    dialog.AddExtension = true;
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        string path = dialog.FileName;

                        var file = _project.AddAnimation(path);

                        this.File_Created?.Invoke(this, new FileEventArgs(file));
                    }
                }
            }
        }
    }
}
