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
using System.Diagnostics;

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

        #endregion Field Region

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

            this.DockPanel.AddContent(_dockMapAttributes, _dockTilesetTools.DockGroup);

            this.DockPanel.AddContent(_dockProperties, _dockTilesetTools.DockGroup);

            _dockTilesetTools.DockGroup.SetVisibleContent(_dockTilesetTools);

            _dockTilesetTools.DockGroup.Hide();
            _dockTilesetTools.DockRegion.Hide();

            // Check window menu items which are contained in the dock panel
            BuildWindowMenu();
        }

        private void DockPanelOnContentRemoved(object sender, DockContentEventArgs e)
        {
            if (e.Content is DockMapDocument)
            {
                _dockTilesetTools.DockGroup.Hide();
                _dockTilesetTools.DockRegion.Hide();
            }
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

        public void OpenPythonDocument(FileInfo file)
        {
            var pyDoc = new DockScriptDocument(file.Name, Icons.document_16xLG, file)
            {
                Tag = file
            };

            // Make sure there isn't already an open document of this file
            var existingDoc = this.FindOpenDocument(file);
            if (existingDoc != null)
            {
                this.DockPanel.ActiveContent = existingDoc;
                return;
            }

            pyDoc.Parent = this.DockPanel;

            _editorDocuments.Add(pyDoc);
            this.DockPanel.AddContent(pyDoc);
        }

        public void OpenItemDocument(FileInfo file)
        {
            var itemDoc = new DockItemDocument(_project, file.Name, Icons.document_16xLG, file)
            {
                Tag = file
            };

            // Make sure there isn't already an open document of this file
            var existingDoc = this.FindOpenDocument(file);
            if (existingDoc != null)
            {
                this.DockPanel.ActiveContent = existingDoc;
                return;
            }

            itemDoc.Closed += ItemDoc_Closed;

            itemDoc.Parent = this.DockPanel;

            _editorDocuments.Add(itemDoc);
            this.DockPanel.AddContent(itemDoc);
        }

        private void ItemDoc_Closed(object sender, EventArgs e)
        {
            var file = (sender as DockDialogueDocument).ContentFile;
            _project.UnloadItem(file.FullName);

            _dockProject.RefreshItemScripts(file);
        }

        public void OpenAnimationDocument(FileInfo file)
        {
            var animDoc = new DockAnimationEditor(_project, file.Name, Icons.document_16xLG, file)
            {
                Tag = file
            };

            // Make sure there isn't already an open document of this file
            var existingDoc = this.FindOpenDocument(file);
            if (existingDoc != null)
            {
                this.DockPanel.ActiveContent = existingDoc;
                return;
            }

            animDoc.Parent = this.DockPanel;

            _editorDocuments.Add(animDoc);
            this.DockPanel.AddContent(animDoc);
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

        private SavableDocument FindOpenDocument(FileInfo file)
        {
            foreach (var doc in _editorDocuments)
            {
                if (doc.Tag == file)
                {
                    return doc;
                }
            }

            return default(SavableDocument);
        }

        private void OpenDialogueDocument(FileInfo file)
        {
            var dialogueDoc = new DockDialogueDocument(_project, file.Name, Icons.document_16xLG, file)
            {
                Tag = file
            };

            // Make sure there isn't already an open document of this file
            var existingDoc = this.FindOpenDocument(file);
            if (existingDoc != null)
            {
                this.DockPanel.ActiveContent = existingDoc;
                return;
            }

            dialogueDoc.Closed += DialogueDoc_Closed;

            _editorDocuments.Add(dialogueDoc);
            this.DockPanel.AddContent(dialogueDoc);
        }

        private void DialogueDoc_Closed(object sender, EventArgs e)
        {
            var file = (sender as DockDialogueDocument).ContentFile;
            _project.UnloadDialogue(file.FullName);

            _dockProject.RefreshDialogueScripts(file);
        }

        private void OpenSpellDocument(FileInfo file)
        {
            var spellDoc = new DockSpellDocument(_project, file.Name, Icons.document_16xLG, file)
            {
                Tag = file
            };

            // Make sure there isn't already an open document of this file
            var existingDoc = this.FindOpenDocument(file);
            if (existingDoc != null)
            {
                this.DockPanel.ActiveContent = existingDoc;
                return;
            }

            spellDoc.Closed += SpellDoc_Closed;

            _editorDocuments.Add(spellDoc);
            this.DockPanel.AddContent(spellDoc);
        }

        private void SpellDoc_Closed(object sender, EventArgs e)
        {
            var file = (sender as DockSpellDocument).ContentFile;
            _project.UnloadSpell(file.FullName);

            _dockProject.RefreshSpellScripts(file);
        }

        private void OpenMapDocument(FileInfo file)
        {
            var mapDoc = new DockMapDocument(file.Name, Icons.document_16xLG, file, _project, _dockTilesetTools, _dockLayers, _dockMapObject, _dockMapAttributes)
            {
                Tag = file
            };

            // Make sure there isn't already an open document of this file
            var existingDoc = this.FindOpenDocument(file);
            if (existingDoc != null)
            {
                this.DockPanel.ActiveContent = existingDoc;
                return;
            }

            mapDoc.Enter += MapDoc_Enter;
            mapDoc.VisibleChanged += MapDoc_VisibleChanged;
            mapDoc.Closed += MapDoc_Closed;
            mapDoc.Parent = this.DockPanel;

            _editorDocuments.Add(mapDoc);
            this.DockPanel.AddContent(mapDoc);
        }

        private void MapDoc_VisibleChanged(object sender, EventArgs e)
        {
            if ((sender as DockMapDocument).Visible)
            {
                _dockTilesetTools.DockGroup?.Show();
                _dockTilesetTools.DockRegion?.Show();
            }
            else
            {
                _dockTilesetTools.DockGroup?.Hide();
                _dockTilesetTools.DockRegion?.Hide();
            }
        }

        private void MapDoc_Closed(object sender, EventArgs e)
        {
            var mapFile = (sender as DockMapDocument).ContentFile;
            _project.UnloadMap(mapFile.FullName);

            _dockTilesetTools.DockGroup.Hide();
            _dockTilesetTools.DockRegion.Hide();

            _dockProject.RefreshMapScripts(mapFile);
        }

        public void OpenNPCDocument(FileInfo file)
        {
            var npcDoc = new DockNPCEditor(_project, file.Name, Icons.document_16xLG, file)
            {
                Tag = file
            };

            // Make sure there isn't already an open document of this file
            var existingDoc = this.FindOpenDocument(file);
            if (existingDoc != null)
            {
                this.DockPanel.ActiveContent = existingDoc;
                return;
            }

            npcDoc.Closed += NpcDoc_Closed;

            _editorDocuments.Add(npcDoc);
            npcDoc.Size = new Size(npcDoc.Width, npcDoc.Height);
            this.DockPanel.AddContent(npcDoc);

            npcDoc.Initalize();
        }

        private void NpcDoc_Closed(object sender, EventArgs e)
        {
            var npcFile = (sender as DockNPCEditor).ContentFile;
            _project.UnloadNPC(npcFile.FullName);

            _dockProject.RefreshNPCScripts(npcFile);
        }

        private void MapDoc_Enter(object sender, EventArgs e)
        {
            _dockTilesetTools.DockGroup?.Show();
            _dockTilesetTools.DockRegion.Show();

            _dockProperties.SetSubject(((DockMapDocument)sender).Map);
            _dockTilesetTools.SetMapSubject(((DockMapDocument)sender).Map);
            _dockLayers.SetMapSubject(((DockMapDocument)sender).Map);
            _dockMapAttributes.SetMapSubject(((DockMapDocument)sender).Map);
        }

        private void OpenFile(FileInfo file)
        {
            if (file.Extension == EngineConstants.SCRIPT_FILE_EXT)
            {
                this.OpenPythonDocument(file);
            }
            else if (file.Extension == EngineConstants.MAP_FILE_EXT)
            {
                this.OpenMapDocument(file);
            }
            else if (file.Extension == EngineConstants.ITEM_FILE_EXT)
            {
                this.OpenItemDocument(file);
            }
            else if (file.Extension == EngineConstants.ANIM_FILE_EXT)
            {
                this.OpenAnimationDocument(file);
            }
            else if (file.Extension == EngineConstants.NPC_FILE_EXT)
            {
                this.OpenNPCDocument(file);
            }
            else if (file.Extension == EngineConstants.DIALOGUE_FILE_EXT)
            {
                this.OpenDialogueDocument(file);
            }
            else if (file.Extension == EngineConstants.SPELL_FILE_EXT)
            {
                this.OpenSpellDocument(file);
            }
        }

        private void _dockProject_File_Created(object sender, FileEventArgs e)
        {
            this.OpenFile(e.File);
        }

        private void _dockProject_File_Selected(object sender, FileEventArgs e)
        {
            this.OpenFile(e.File);
        }

        #endregion Constructor Region

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

        #endregion Method Region

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
                _editorDocuments.Remove((SavableDocument)e.Content);
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

        #endregion Event Handler Region

        #region Serialization Region

        private void SerializeDockPanel(string path)
        {
            var state = DockPanel.GetDockPanelState();
            SerializerHelper.Serialize(state, path);
        }

        #endregion Serialization Region

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
                // Make sure they've actually entered something.
                if (string.IsNullOrEmpty(createProjectDialog.ClientDataPath) || string.IsNullOrEmpty(createProjectDialog.ServerDataPath))
                {
                    DarkMessageBox.ShowError("At least one project directory path is missing!", "Error Creating Project!", DarkDialogButton.Ok);
                    return;
                }

                string clientDataPath = Path.GetFullPath(createProjectDialog.ClientDataPath);
                string serverDataPath = Path.GetFullPath(createProjectDialog.ServerDataPath);

                // Make sure these directories actually exist.
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
                else
                {
                    DarkMessageBox.ShowError("Invalid project directories specified!", "Error Creating Project!", DarkDialogButton.Ok);
                    return;
                }
            }
        }

        private void SuiteForm_Paint(object sender, PaintEventArgs e)
        {
            this.lblMemUsage.Text = $"Memory: {((Process.GetCurrentProcess().PrivateMemorySize64 / 1024) / 1024)} MB";
        }
    }
}