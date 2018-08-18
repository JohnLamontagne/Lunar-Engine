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
using DarkUI.Docking;
using DarkUI.Forms;
using DarkUI.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Lunar.Core;
using Lunar.Editor.Controls;
using Lunar.Editor.Utilities;

namespace Lunar.Editor
{
    public partial class SuiteForm : DarkForm
    {
        #region Field Region

        private List<SavableDocument> _editorDocuments;

        private DockProject _dockProject;
        private DockTilesetTools _dockTilesetTools;
        private DockLayers _dockLayers;
        private DockProperties _dockProperties;
        private DockMapObjectProperties _dockMapObject;
        private DockMapAttributes _dockMapAttributes;
        

        private Project _project;

        public Project Project => _project;

        #endregion

        #region Constructor Region

        public SuiteForm()
        {
            InitializeComponent();

            // Add the control scroll message filter to re-route all mousewheel events
            // to the control the user is currently hovering over with their cursor.
            Application.AddMessageFilter(new ControlScrollFilter());

            // Add the dock content drag message filter to handle moving dock content around.
            Application.AddMessageFilter(DockPanel.DockContentDragFilter);

            // Add the dock panel message filter to filter through for dock panel splitter
            // input before letting events pass through to the rest of the application.
            Application.AddMessageFilter(DockPanel.DockResizeFilter);

            // Hook in all the UI events manually for clarity.
            HookEvents();

            // Build the tool windows and add them to the dock panel
            _dockProject = new DockProject();
            _dockTilesetTools = new DockTilesetTools();
            _dockLayers = new DockLayers();
            _dockProperties = new DockProperties();
            _dockMapObject = new DockMapObjectProperties();
            _dockMapAttributes = new DockMapAttributes();

            this.DockPanel.AllowDrop = false;
            _dockTilesetTools.AllowDrop = false;
            _dockProject.AllowDrop = false;
            _dockLayers.AllowDrop = false;
            _dockProperties.AllowDrop = false;
            _dockMapObject.AllowDrop = false;
            _dockMapAttributes.AllowDrop = false;

            _editorDocuments = new List<SavableDocument>();

            _dockProject.FileCreated += _dockProject_File_Created;
            _dockProject.FileSelected += _dockProject_File_Selected;
            _dockProject.FileRemoved += _dockProject_File_Removed;
            _dockProject.FileChanged += DockProjectOnFileChanged;

         
            this.DockPanel.AddContent(_dockProject);
            this.DockPanel.AddContent(_dockTilesetTools);

            this.DockPanel.ContentRemoved += DockPanelOnContentRemoved;

            _dockTilesetTools.DockRegion.Size = new Size(_dockTilesetTools.Width, _dockTilesetTools.DockRegion.Height);

            this.DockPanel.AddContent(_dockLayers, _dockTilesetTools.DockGroup);

            this.DockPanel.AddContent(_dockMapObject, _dockTilesetTools.DockGroup);

            this.DockPanel.AddContent(_dockMapAttributes,_dockTilesetTools.DockGroup);

            this.DockPanel.AddContent(_dockProperties, _dockTilesetTools.DockGroup);

            _dockTilesetTools.DockGroup.SetVisibleContent(_dockTilesetTools);

            _dockTilesetTools.DockGroup.Hide();

            // Check window menu items which are contained in the dock panel
            BuildWindowMenu();
        }

        private void DockPanelOnContentRemoved(object sender, DockContentEventArgs e)
        {
            if (e.Content is DockMapDocument)
                _dockTilesetTools.DockGroup.Hide();
        }

        private void DockProjectOnFileChanged(object sender, GameFileChangedEventArgs e)
        {
            foreach (var editor in _editorDocuments)
            {
                if (((FileInfo)editor.Tag).Name == e.OldFile.Name)
                {
                    editor.Tag = e.NewFile;
                }
            }
        }

        private void _dockProject_File_Removed(object sender, FileEventArgs e)
        {
            this.CloseDocument(e.File);
        }

        private void OpenLuaDocument(FileInfo file)
        {
            var luaDoc = new DockLUADocument(file.Name, Icons.document_16xLG, file)
            {
                Tag = file
            };

            // Make sure there isn't already an open document of this file
            // and if there are, just activate it.
            foreach (var lDoc in _editorDocuments)
            {
                if (lDoc.Tag == file)
                {
                    this.DockPanel.ActiveContent = lDoc;
                    return;
                }
            }

            luaDoc.Enter += LuaDoc_Enter;

            _editorDocuments.Add(luaDoc);
            DockPanel.AddContent(luaDoc);
        }

        private void OpenItemDocument(FileInfo file)
        {
            var itemDoc = new DockItemDocument(_project, file.Name, Icons.document_16xLG, file)
            {
                Tag = file
            };

            // and if there are, just activate it.
            foreach (var iDoc in _editorDocuments)
            {
                if (iDoc.Tag == file)
                {
                    this.DockPanel.ActiveContent = iDoc;
                    return;
                }
            }

            itemDoc.Enter += ItemDoc_Enter;

            _editorDocuments.Add(itemDoc);
            DockPanel.AddContent(itemDoc);
        }

        private void OpenAnimationDocument(FileInfo file)
        {
            var animDoc = new DockAnimationEditor(_project, file.Name, Icons.document_16xLG, file)
            {
                Tag = file
            };

            // and if there are, just activate it.
            foreach (var aDoc in _editorDocuments)
            {
                if (aDoc.Tag == file)
                {
                    this.DockPanel.ActiveContent = aDoc;
                    return;
                }
            }

            animDoc.Enter += AnimationDoc_Enter;

            _editorDocuments.Add(animDoc);
            DockPanel.AddContent(animDoc);
        }

        private void CloseDocument(FileInfo file)
        {
            // Close the appropiate document
            foreach (var iDoc in _editorDocuments)
            {
                if (iDoc.Tag == file)
                {
                    _editorDocuments.Remove(iDoc);
                    iDoc.Close();
                    DockPanel.RemoveContent(iDoc);
                    return;
                }
            }
        }


        private void ItemDoc_Enter(object sender, EventArgs e)
        {
            _dockTilesetTools.DockGroup.Hide();
        }

        private void AnimationDoc_Enter(object sender, EventArgs e)
        {
            _dockTilesetTools.DockGroup.Hide();
        }

        private void LuaDoc_Enter(object sender, EventArgs e)
        {
            _dockTilesetTools.DockGroup.Hide();
        }

        private void OpenMapDocument(FileInfo file)
        {
            var mapDoc = new DockMapDocument(file.Name, Icons.document_16xLG, file, _project, _dockTilesetTools, _dockLayers, _dockMapObject, _dockMapAttributes)
            {
                Tag = file
            };

            // Make sure there isn't already an open document of this file
            foreach (var mDoc in _editorDocuments)
            {
                if (mDoc.Tag == file)
                {
                    this.DockPanel.ActiveContent = mDoc;
                    return;
                }
            }

            mapDoc.Enter += MapDoc_Enter;

            _editorDocuments.Add(mapDoc);
            DockPanel.AddContent(mapDoc);
        }

        private void OpenNPCDocument(FileInfo file)
        {
            var npcDoc = new DockNPCEditor(_project, file.Name, Icons.document_16xLG, file)
            {
                Tag = file
            };


            // Make sure there isn't already an open document of this file
            foreach (var nDoc in _editorDocuments)
            {
                if (((FileInfo)nDoc.Tag).Name == file.Name)
                {
                    this.DockPanel.ActiveContent = nDoc;
                    return;
                }
            }

            npcDoc.Enter += NPCDoc_Enter;

            _editorDocuments.Add(npcDoc);
            DockPanel.AddContent(npcDoc);
        }

        private void NPCDoc_Enter(object sender, EventArgs e)
        {
            _dockTilesetTools.DockGroup.Hide();
        }

        private void MapDoc_Enter(object sender, EventArgs e)
        {
            _dockTilesetTools.DockGroup?.Show();

            _dockProperties.SetSubject(((DockMapDocument)sender).Map);
            _dockTilesetTools.SetMapSubject(((DockMapDocument)sender).Map);
            _dockLayers.SetMapSubject(((DockMapDocument)sender).Map);
            _dockMapAttributes.SetMapSubject(((DockMapDocument)sender).Map);
        }

        private void _dockProject_File_Created(object sender, FileEventArgs e)
        {
            if (e.File.Extension == EngineConstants.LUA_FILE_EXT)
            {
                this.OpenLuaDocument(e.File);
            }
            else if (e.File.Extension == EngineConstants.MAP_FILE_EXT)
            {
                this.OpenMapDocument(e.File);
            }
            else if (e.File.Extension == EngineConstants.ITEM_FILE_EXT)
            {
                this.OpenItemDocument(e.File);    
            }
            else if (e.File.Extension == EngineConstants.ANIM_FILE_EXT)
            {
                this.OpenAnimationDocument(e.File);
            }
            else if (e.File.Extension == EngineConstants.NPC_FILE_EXT)
            {
                this.OpenNPCDocument(e.File);
            }
        }

        private void _dockProject_File_Selected(object sender, FileEventArgs e)
        {
            if (e.File.Extension == EngineConstants.LUA_FILE_EXT)
            {
                this.OpenLuaDocument(e.File);
            }
            else if (e.File.Extension == EngineConstants.MAP_FILE_EXT)
            {
                this.OpenMapDocument(e.File);
            }
            else if (e.File.Extension == EngineConstants.ITEM_FILE_EXT)
            {
                this.OpenItemDocument(e.File);
            }
            else if (e.File.Extension == EngineConstants.ANIM_FILE_EXT)
            {
                this.OpenAnimationDocument(e.File);
            }
            else if (e.File.Extension == EngineConstants.NPC_FILE_EXT)
            {
                this.OpenNPCDocument(e.File);
            }
        }

        #endregion

        #region Method Region

        private void HookEvents()
        {
            FormClosing += MainForm_FormClosing;

            DockPanel.ContentAdded += DockPanel_ContentAdded;
            DockPanel.ContentRemoved += DockPanel_ContentRemoved;
            
            mnuClose.Click += Close_Click;

            mnuProject.Click += Project_Click;

        }

        private void ToggleToolWindow(DarkToolWindow toolWindow)
        {
            if (toolWindow.DockPanel == null)
                DockPanel.AddContent(toolWindow);
            else
                DockPanel.RemoveContent(toolWindow);
        }

        private void BuildWindowMenu()
        {
            mnuProject.Checked = DockPanel.ContainsContent(_dockProject);
        }

        #endregion

        #region Event Handler Region

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SerializeDockPanel("dockpanel.config");
        }

        private void DockPanel_ContentAdded(object sender, DockContentEventArgs e)
        {
        }

        private void DockPanel_ContentRemoved(object sender, DockContentEventArgs e)
        {
            if (e.Content is SavableDocument)
            {
                _editorDocuments.Remove((SavableDocument) e.Content);
            }
        }


        private void Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Project_Click(object sender, EventArgs e)
        {
            ToggleToolWindow(_dockProject);
        }

        #endregion

        #region Serialization Region

        private void SerializeDockPanel(string path)
        {
            var state = DockPanel.GetDockPanelState();
            SerializerHelper.Serialize(state, path);
        }
     

        #endregion

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var fileBrowserDialog = new OpenFileDialog();
            fileBrowserDialog.Filter = @"Project Files (*.lproj)|*.lproj";
            fileBrowserDialog.DefaultExt = ".lproj";
            fileBrowserDialog.AddExtension = true;

            if (fileBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                _project = Project.Load(fileBrowserDialog.FileName);

                Properties.Settings.Default["LastProjectPath"] = fileBrowserDialog.FileName;
                Properties.Settings.Default.Save(); // Saves settings in application configuration file

                _dockTilesetTools.SetProject(_project);

                this.PopulateProjectTree();
            }
        }

        private void PopulateProjectTree()
        {
            _dockProject.InitalizeFromProject(_project);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var doc in this.DockPanel.Controls)
            {
                if (doc is SavableDocument)
                    ((SavableDocument)doc).Save();
            }
        }

        private void mostRecentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = Properties.Settings.Default["LastProjectPath"].ToString();
            if (File.Exists(path))
            {
                _project = Project.Load(path);

                _dockTilesetTools.SetProject(_project);

                this.PopulateProjectTree();
            }
        }

        private void mnuNewFile_Click(object sender, EventArgs e)
        {
            var createProjectDialog = new CreateProjectDialog();

            if (createProjectDialog.ShowDialog() == DialogResult.OK)
            {
                string clientDataPath = Path.GetFullPath(createProjectDialog.ClientDataPath);
                string serverDataPath = Path.GetFullPath(createProjectDialog.ServerDataPath);

                if (Directory.Exists(clientDataPath) && Directory.Exists(serverDataPath))
                {
                    var fileBrowserDialog = new SaveFileDialog();
                    fileBrowserDialog.Filter = @"Project Files (*.lproj)|*.lproj";
                    fileBrowserDialog.DefaultExt = ".lproj";
                    fileBrowserDialog.AddExtension = true;

                    if (fileBrowserDialog.ShowDialog() == DialogResult.OK)
                    {
                        _project = Project.Create(fileBrowserDialog.FileName, serverDataPath, clientDataPath);

                        _dockTilesetTools.SetProject(_project);

                        this.PopulateProjectTree();
                    }
                }
            }
        }
    }
}
