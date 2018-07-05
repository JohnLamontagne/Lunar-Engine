﻿using DarkUI.Docking;
using DarkUI.Forms;
using DarkUI.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
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

            _dockProject.File_Created += _dockProject_File_Created;
            _dockProject.File_Selected += _dockProject_File_Selected;
            _dockProject.File_Removed += _dockProject_File_Removed;

         
            this.DockPanel.AddContent(_dockProject);
            this.DockPanel.AddContent(_dockTilesetTools);

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

        private void _dockProject_File_Removed(object sender, DockProject.FileEventArgs e)
        {
            if (e.File.Extension == ".lua")
            {
                this.CloseLuaDocument(e.File);
            }
            else if (e.File.Extension == ".litm")
            {
                this.CloseItemDocument(e.File);
            }
        }

        private void CloseLuaDocument(FileInfo file)
        {
            // Close the appropiate document
            foreach (var lDoc in _editorDocuments)
            {
                if (lDoc.Tag == file)
                {
                    _editorDocuments.Remove(lDoc);
                    lDoc.Close();
                    DockPanel.RemoveContent(lDoc);
                    return;
                }
            }
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
            var itemDoc = new DockItemEditor(_project, file.Name, Icons.document_16xLG, file)
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

        private void CloseItemDocument(FileInfo file)
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

        private void MapDoc_Enter(object sender, EventArgs e)
        {
            _dockTilesetTools.DockGroup?.Show();

            _dockProperties.SetSubject(((DockMapDocument)sender).Map);
            _dockTilesetTools.SetMapSubject(((DockMapDocument)sender).Map);
            _dockLayers.SetMapSubject(((DockMapDocument)sender).Map);
            _dockMapAttributes.SetMapSubject(((DockMapDocument)sender).Map);
        }

        private void _dockProject_File_Created(object sender, DockProject.FileEventArgs e)
        {
            if (e.File.Extension == ".lua")
            {
                this.OpenLuaDocument(e.File);
            }
            else if (e.File.Extension == ".rmap")
            {
                this.OpenMapDocument(e.File);
            }
            else if (e.File.Extension == ".litm")
            {
                this.OpenItemDocument(e.File);    
            }
        }

        private void _dockProject_File_Selected(object sender, DockProject.FileEventArgs e)
        {
            if (e.File.Extension == ".lua")
            {
                this.OpenLuaDocument(e.File);
            }
            else if (e.File.Extension == ".rmap")
            {
                this.OpenMapDocument(e.File);
            }
            else if (e.File.Extension == ".litm")
            {
                this.OpenItemDocument(e.File);
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