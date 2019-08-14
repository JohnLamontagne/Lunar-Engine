using System;
using DarkUI.Forms;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Lunar.Core;
using Lunar.Core.Utilities.Logic;
using Lunar.Core.World;
using ScintillaNET;

namespace Lunar.Editor.Controls
{
    public partial class DockSpellDocument : SavableDocument
    {
        private string _regularDockText;
        private string _unsavedDockText;
        private bool _unsaved;
        private string _activeScript;

        private Project _project;

        private ItemDescriptor _item;

        public DockSpellDocument()
        {
        }

        private DockSpellDocument(FileInfo file)
            : base(file)
        {
            InitializeComponent();

            _activeScript = "";
        }

        public DockSpellDocument(Project project, string text, Image icon, FileInfo file)
            : this(file)
        {
            _project = project;

            DockText = text;
            Icon = icon;

            _item = _project.LoadItem(file.FullName);

            this.txtName.Text = _item.Name;

            this.txtStr.Text = _item.Strength.ToString();
            this.txtInt.Text = _item.Intelligence.ToString();
            this.txtDef.Text = _item.Defence.ToString();
            this.txtHealth.Text = _item.Health.ToString();
            this.txtDex.Text = _item.Dexterity.ToString();

            _regularDockText = _item.Name + EngineConstants.ITEM_FILE_EXT;
            this.DockText = _regularDockText;
            _unsavedDockText = _regularDockText + "*";

            if (File.Exists(_project.ClientRootDirectory + "/" + _item.SpriteInfo.TextureName))
                this.picTexture.Load(_project.ClientRootDirectory + "/" + _item.SpriteInfo.TextureName);
        }

        public override void Close()
        {
            if (_unsaved)
            {
                var result = DarkMessageBox.ShowWarning(@"You will lose any unsaved changes. Continue?", @"Close document", DarkDialogButton.YesNo);
                if (result == DialogResult.No)
                    return;
            }

            base.Close();
        }

        private void DockItemEditor_Load(object sender, System.EventArgs e)
        {
            this.DockText = _item.Name + EngineConstants.ITEM_FILE_EXT;
            _unsaved = false;
        }

        public override void Save()
        {
            _regularDockText = _item.Name + EngineConstants.ITEM_FILE_EXT;

            this.DockText = _regularDockText;
            _unsaved = false;

            if (_item.Name + EngineConstants.ITEM_FILE_EXT != this.ContentFile.Name)
            {
                File.Move(this.ContentFile.FullName, this.ContentFile.DirectoryName + "/" + _item.Name + EngineConstants.ITEM_FILE_EXT);

                this.ContentFile = _project.ChangeItem(this.ContentFile.FullName, this.ContentFile.DirectoryName + "\\" + _item.Name + EngineConstants.ITEM_FILE_EXT);
            }

            _item.Save(this.ContentFile.FullName);
        }

        private void buttonSave_Click(object sender, System.EventArgs e)
        {
            this.Save();
        }

        private void picTexture_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.RestoreDirectory = true;
                dialog.InitialDirectory = _project.ClientRootDirectory.FullName;
                dialog.Filter = @"Image Files (*.png)|*.png";
                dialog.DefaultExt = ".png";
                dialog.AddExtension = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    this.DockText = _unsavedDockText;
                    _unsaved = true;

                    string path = dialog.FileName;

                    _item.SpriteInfo.TextureName = Helpers.MakeRelative(path, _project.ClientRootDirectory.FullName + "/"); ;

                    this.picTexture.Load(path);
                }
            }
        }

        private void txtEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control && e.KeyCode == Keys.S)
            {
                this.Save();
                e.SuppressKeyPress = true;
            }
        }

        private void DockItemEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control && e.KeyCode == Keys.S)
            {
                this.Save();
                e.SuppressKeyPress = true;
            }
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            _item.Name = txtName.Text;

            _regularDockText = _item.Name + EngineConstants.ITEM_FILE_EXT;
            this.DockText = _regularDockText;
            _unsavedDockText = _regularDockText + "*";

            this.MarkUnsaved();
        }

        private void MarkUnsaved()
        {
            this.DockText = _unsavedDockText;
            _unsaved = true;
        }
    }
}