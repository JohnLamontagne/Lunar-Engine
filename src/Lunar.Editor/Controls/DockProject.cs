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
                    this.FileSelected?.Invoke(this, new FileEventArgs(info));
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

            foreach (var animationFile in _project.AnimationFiles)
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

                            this.FileCreated?.Invoke(this, new FileEventArgs(file));
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

            foreach (var mapFile in _project.MapFiles)
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

                            this.FileCreated?.Invoke(this, new FileEventArgs(file));
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

            foreach (var itemFile in _project.ItemFiles)
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

                            this.FileCreated?.Invoke(this, new FileEventArgs(file));
                        }
                    }
                })
            };


            itemPathNode.Nodes.Add(addNode);

            return itemPathNode;
        }

        private DarkTreeNode BuildNPCTree()
        {
            var npcPathNode = new DarkTreeNode("Npcs")
            {
                Icon = Icons.folder_closed,
                ExpandedIcon = Icons.folder_open
            };

            foreach (var npcFile in _project.NPCFiles)
            {
                var fileNode = new DarkTreeNode(npcFile.Name)
                {
                    Tag = npcFile,
                    Icon = Icons.document_16xLG,
                };
                npcPathNode.Nodes.Add(fileNode);
            }

            var addNode = new DarkTreeNode("Add NPC")
            {
                Icon = Icons.Plus,
                Tag = (Action<DarkTreeNode>)((node) =>
                {
                    using (SaveFileDialog dialog = new SaveFileDialog())
                    {
                        dialog.InitialDirectory = _project.ServerRootDirectory.FullName + @"\Npcs";
                        dialog.RestoreDirectory = true;
                        dialog.Filter = $@"Lunar Engine NPC Files (*{EngineConstants.NPC_FILE_EXT})|*{EngineConstants.NPC_FILE_EXT}";
                        dialog.DefaultExt = EngineConstants.NPC_FILE_EXT;
                        dialog.AddExtension = true;
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            string path = dialog.FileName;

                            var file = _project.AddNPC(path);

                            this.FileCreated?.Invoke(this, new FileEventArgs(file));
                        }
                    }
                })
            };

            npcPathNode.Nodes.Add(addNode);

            return npcPathNode;
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
            projectTreeNode.Nodes.Add(this.BuildNPCTree());

            return projectTreeNode;
        }

        public void InitalizeFromProject(Project project)
        {
            _project = project;

            _project.ItemAdded += ProjectOnItemAdded;
            _project.ItemDeleted += ProjectOnItemDeleted;
            _project.ItemChanged += ProjectOnItemChanged;

            _project.NPCAdded += ProjectOnNpcAdded;
            _project.NPCDeleted += ProjectOnNpcDeleted;
            _project.NPCChanged += ProjectOnNpcChanged;

            _project.MapAdded += ProjectOnMapAdded;
            _project.MapDeleted += ProjectOnMapDeleted;
            _project.MapChanged += ProjectOnMapChanged;

            _project.AnimationAdded += ProjectOnAnimationAdded;
            _project.AnimationDeleted += ProjectOnAnimationDeleted;
            _project.AnimationChanged += ProjectOnAnimationChanged;


            treeProject.Nodes.Clear();

            var node = new DarkTreeNode(_project.GameName)
            {
                Icon = Icons.folder_closed,
                ExpandedIcon = Icons.folder_open
            };

            node.Nodes.Add(this.InitalizeProjectTree());

            treeProject.Nodes.Add(node);
        }

        private void ProjectOnItemChanged(object sender, GameFileChangedEventArgs args)
        {
            var nodeToChange = treeProject.FindNode($"Default\\Game Data\\Items\\{args.OldFile.Name}");
            nodeToChange.Tag = args.NewFile;
            this.UpdateNode(nodeToChange, args.NewFile.Name);

            this.FileChanged?.Invoke(this, args);
        }

        private void ProjectOnAnimationChanged(object sender, GameFileChangedEventArgs args)
        {
            var nodeToChange = treeProject.FindNode($"Default\\Game Data\\Animations\\{args.OldFile.Name}");
            nodeToChange.Tag = args.NewFile;
            this.UpdateNode(nodeToChange, args.NewFile.Name);

            this.FileChanged?.Invoke(this, args);
        }

        private void ProjectOnMapChanged(object sender, GameFileChangedEventArgs args)
        {
            var nodeToChange = treeProject.FindNode($"Default\\Game Data\\Maps\\{args.OldFile.Name}");
            nodeToChange.Tag = args.NewFile;
            this.UpdateNode(nodeToChange, args.NewFile.Name);

            this.FileChanged?.Invoke(this, args);
        }

        private void ProjectOnNpcChanged(object sender, GameFileChangedEventArgs args)
        {
            var nodeToChange = treeProject.FindNode($"Default\\Game Data\\Npcs\\{args.OldFile.Name}");
            nodeToChange.Tag = args.NewFile;
            this.UpdateNode(nodeToChange, args.NewFile.Name);

            this.FileChanged?.Invoke(this, args);
        }

        private void UpdateNode(DarkTreeNode nodeToChange, string newName)
        {
            nodeToChange.Text = newName;

            // ANOTHER FUCKING HACK, THANKS DarkUI
            var parentNode = nodeToChange.ParentNode;
            int oldIndex = parentNode.Nodes.IndexOf(nodeToChange);
            parentNode.Nodes.Remove(nodeToChange);
            parentNode.Nodes.Insert(oldIndex, nodeToChange);

            parentNode.Nodes.Add(new DarkTreeNode());
            parentNode.Nodes.RemoveAt(parentNode.Nodes.Count - 1);
        }

        private void ProjectOnAnimationDeleted(object sender, FileEventArgs args)
        {
            var nodeToDelete = treeProject.FindNode($"Default\\Game Data\\Animations\\{args.File.Name}");

            this.FileRemoved?.Invoke(this, new FileEventArgs(args.File));

            nodeToDelete?.ParentNode.Nodes.Remove(nodeToDelete);
        }

        private void ProjectOnAnimationAdded(object sender, FileEventArgs args)
        {
            DarkTreeNode fileNode = new DarkTreeNode(args.File.Name)
            {
                Icon = Icons.document_16xLG,
                Tag = args.File
            };

            var addNode = treeProject.FindNode($"Default\\Game Data\\Animations\\Add Animation");
            treeProject.Nodes.Remove(addNode);

            var animationsNode = treeProject.FindNode($"Default\\Game Data\\Animations");

            animationsNode.Nodes.Add(fileNode);
            animationsNode.Nodes.Add(addNode);
        }

        private void ProjectOnMapDeleted(object sender, FileEventArgs args)
        {
            var nodeToDelete = treeProject.FindNode($"Default\\Game Data\\Maps\\{args.File.Name}");

            this.FileRemoved?.Invoke(this, new FileEventArgs(args.File));

            nodeToDelete?.ParentNode.Nodes.Remove(nodeToDelete);
        }

        private void ProjectOnMapAdded(object sender, FileEventArgs args)
        {
            DarkTreeNode fileNode = new DarkTreeNode(args.File.Name)
            {
                Icon = Icons.document_16xLG,
                Tag = args.File
            };

            var mapsNode = treeProject.FindNode($"Default\\Game Data\\Maps");

            var addNode = treeProject.FindNode($"Default\\Game Data\\Maps\\Add Map");
            mapsNode.Nodes.Remove(addNode);

            mapsNode.Nodes.Add(fileNode);
            mapsNode.Nodes.Add(addNode);

        }

        private void ProjectOnNpcDeleted(object sender, FileEventArgs args)
        {
            var nodeToDelete = treeProject.FindNode($"Default\\Game Data\\Npcs\\{args.File.Name}");

            this.FileRemoved?.Invoke(this, new FileEventArgs(args.File));

            nodeToDelete?.ParentNode.Nodes.Remove(nodeToDelete);
        }

        private void ProjectOnNpcAdded(object sender, FileEventArgs args)
        {
            DarkTreeNode fileNode = new DarkTreeNode(args.File.Name)
            {
                Icon = Icons.document_16xLG,
                Tag = args.File
            };

            var npcsNode = treeProject.FindNode($"Default\\Game Data\\Npcs");

            var addNode = treeProject.FindNode($"Default\\Game Data\\Npcs\\Add NPC");
            npcsNode.Nodes.Remove(addNode);

            npcsNode.Nodes.Add(fileNode);
            npcsNode.Nodes.Add(addNode);
        }

        private void ProjectOnItemDeleted(object sender, FileEventArgs args)
        {
            var nodeToDelete = treeProject.FindNode($"Default\\Game Data\\Items\\{args.File.Name}");

            this.FileRemoved?.Invoke(this, new FileEventArgs(args.File));

            nodeToDelete?.ParentNode.Nodes.Remove(nodeToDelete);
        }

        private void ProjectOnItemAdded(object sender, FileEventArgs args)
        {
            DarkTreeNode fileNode = new DarkTreeNode(args.File.Name)
            {
                Icon = Icons.document_16xLG,
                Tag = args.File
            };

            var itemsNode = treeProject.FindNode($"Default\\Game Data\\Items");

            var addNode = treeProject.FindNode($"Default\\Game Data\\Items\\Add Item");
            itemsNode.Nodes.Remove(addNode);

            itemsNode.Nodes.Add(fileNode);
            itemsNode.Nodes.Add(addNode);
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

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.treeProject.SelectedNodes.Count > 0)
            {
                if (this.treeProject.SelectedNodes[0].Tag is FileInfo info)
                {
                    switch (info.Extension)
                    {
                        case EngineConstants.ITEM_FILE_EXT:
                            _project.RemoveItem(info.FullName);
                            break;

                        case EngineConstants.NPC_FILE_EXT:
                            _project.RemoveNPC(info.FullName);
                            break;

                        case EngineConstants.ANIM_FILE_EXT:
                            _project.RemoveAnimations(info.FullName);
                            break;

                        case EngineConstants.MAP_FILE_EXT:
                            _project.RemoveMap(info.FullName);
                            break;
                    }
                }
            }
        }

        public event EventHandler<FileEventArgs> FileSelected;

        public event EventHandler<FileEventArgs> FileCreated;

        public event EventHandler<GameFileChangedEventArgs> FileChanged;

        public event EventHandler<FileEventArgs> FileRemoved;
    }
}
