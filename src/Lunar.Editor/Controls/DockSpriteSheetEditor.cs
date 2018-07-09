using System;
using DarkUI.Forms;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Lunar.Core.World;
using Lunar.Editor.World;
using ScintillaNET;

namespace Lunar.Editor.Controls
{
    public partial class DockSpriteSheetEditor : SavableDocument
    {
        private FileInfo _file;
        private string _regularDockText;
        private string _unsavedDockText;
        private bool _unsaved;

        private Project _project;

        private ItemDescriptor _item;

        private DockSpriteSheetEditor()
        {
            InitializeComponent();

            this.txtEditor.Lexer = Lexer.Lua;

            this.txtEditor.StyleResetDefault();

            this.txtEditor.Styles[Style.Default].Font = "Consolas";
            this.txtEditor.Styles[Style.Default].Size = 12;

            this.txtEditor.Styles[Style.Default].BackColor = Color.FromArgb(29, 31, 33);
            this.txtEditor.Styles[Style.Default].ForeColor = Color.FromArgb(197, 200, 198);

            this.txtEditor.StyleClearAll();

            this.txtEditor.Styles[Style.Lua.Comment].ForeColor = Color.FromArgb(181, 189, 104);
            this.txtEditor.Styles[Style.Lua.CommentLine].ForeColor = Color.FromArgb(181, 189, 104);
            this.txtEditor.Styles[Style.Lua.CommentLine].Italic = true;

            this.txtEditor.Styles[Style.Lua.String].ForeColor = Color.FromArgb(222, 147, 95);

            this.txtEditor.Styles[Style.Lua.Operator].ForeColor = Color.FromArgb(240, 198, 116);

            this.txtEditor.Styles[Style.Lua.Number].ForeColor = Color.FromArgb(138, 190, 183);

            this.txtEditor.Styles[Style.Lua.Preprocessor].ForeColor = Color.FromArgb(129, 162, 190);

            this.txtEditor.Styles[Style.Lua.Identifier].ForeColor = Color.FromArgb(178, 148, 187);

            this.txtEditor.Styles[Style.Lua.Word].ForeColor = Color.FromArgb(130, 239, 104);

            this.txtEditor.SetKeywords(0, "if then end not function");
        }

        public DockSpriteSheetEditor(Project project, string text, Image icon, FileInfo file)
            : this()
        {
            _project = project;

            _regularDockText = text;
            _unsavedDockText = text + "*";


            DockText = text;
            Icon = icon;

            _file = file;

            _item = ItemDescriptor.Load(file.FullName);

            this.txtName.Text = _item.Name;
            this.radioStackable.Checked = _item.Stackable;
            this.radioNotStackable.Checked = !_item.Stackable;
            this.cmbType.DataSource = Enum.GetValues(typeof(ItemTypes));
            this.cmbType.SelectedItem = _item.ItemType;
            this.txtStr.Text = _item.Strength.ToString();
            this.txtInt.Text = _item.Intelligence.ToString();
            this.txtDef.Text = _item.Defence.ToString();
            this.txtHealth.Text = _item.Health.ToString();
            this.txtDex.Text = _item.Dexterity.ToString();
            this.cmbEquipmentSlot.DataSource = Enum.GetValues(typeof(EquipmentSlots));
            this.cmbEquipmentSlot.SelectedItem = _item.SlotType;
            this.picTexture.Load(_item.TexturePath);

            onUseToolStripMenuItem.Checked = true;
            if (_item.Scripts.ContainsKey("OnUse"))
            {
                this.txtEditor.Text = _item.Scripts["OnUse"];
            }
            else
            {
                this.txtEditor.Text = "function OnUse(args) \n end";
            }
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
            this.DockText = _regularDockText;
            _unsaved = false;
        }

        public void Save()
        {

            _item.Strength = int.Parse(txtStr.Text);
            _item.Intelligence = int.Parse(txtInt.Text);
            _item.Dexterity = int.Parse(txtDex.Text);
            _item.Defence = int.Parse(txtDef.Text);
            _item.Health = int.Parse(txtHealth.Text);
            _item.Name = txtName.Text;
            _item.Stackable = radioStackable.Checked;
            _item.ItemType = (ItemTypes)cmbType.SelectedItem;
            _item.SlotType = (EquipmentSlots)cmbEquipmentSlot.SelectedItem;

            this.DockText = _regularDockText;
            _unsaved = false;
            _item.Save(_file.FullName);
        }

        private void txtEditor_TextChanged(object sender, System.EventArgs e)
        {
            this.DockText = _unsavedDockText;
            _unsaved = true;
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

                    _item.TexturePath = path;

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

        private void txtStr_TextChanged(object sender, EventArgs e)
        {
            this.DockText = _unsavedDockText;
            _unsaved = true;

        }

        private void txtInt_TextChanged(object sender, EventArgs e)
        {
            this.DockText = _unsavedDockText;
            _unsaved = true;

        }

        private void txtDex_TextChanged(object sender, EventArgs e)
        {
            this.DockText = _unsavedDockText;
            _unsaved = true;

        }

        private void txtDef_TextChanged(object sender, EventArgs e)
        {
            this.DockText = _unsavedDockText;
            _unsaved = true;

        }

        private void txtHealth_TextChanged(object sender, EventArgs e)
        {
            this.DockText = _unsavedDockText;
            _unsaved = true;

            
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            this.DockText = _unsavedDockText;
            _unsaved = true;

            
        }

        private void radioStackable_CheckedChanged(object sender, EventArgs e)
        {
            this.DockText = _unsavedDockText;
            _unsaved = true;

        }

        private void radioNotStackable_CheckedChanged(object sender, EventArgs e)
        {
            this.DockText = _unsavedDockText;
            _unsaved = true;

            
        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.DockText = _unsavedDockText;
            _unsaved = true;

        }


        private void cmbEquipmentSlot_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.DockText = _unsavedDockText;
            _unsaved = true;

            
        }

        private void onUseToolStripMenuItem_Click(object sender, EventArgs e)
        {

            onUseToolStripMenuItem.Checked = true;
            onEquipToolStripMenuItem.Checked = false;
            onAcquiredToolStripMenuItem.Checked = false;
            onDroppedToolStripMenuItem.Checked = false;
            onCreatedToolStripMenuItem.Checked = false;

            if (_item.Scripts.ContainsKey("OnUse"))
            {
                this.txtEditor.Text = _item.Scripts["OnUse"];
            }
            else
            {
                this.txtEditor.Text = "function OnUse(args) \nend";
            }
        }

        private void onEquipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            onEquipToolStripMenuItem.Checked = true;
            onUseToolStripMenuItem.Checked = false;
            onAcquiredToolStripMenuItem.Checked = false;
            onDroppedToolStripMenuItem.Checked = false;
            onCreatedToolStripMenuItem.Checked = false;

            if (_item.Scripts.ContainsKey("OnEquip"))
            {
                this.txtEditor.Text = _item.Scripts["OnEqip"];
            }
            else
            {
                this.txtEditor.Text = "function OnEquip(args) \nend";
            }
        }

        private void onAcquiredToolStripMenuItem_Click(object sender, EventArgs e)
        {
            onAcquiredToolStripMenuItem.Checked = true;
            onEquipToolStripMenuItem.Checked = false;
            onUseToolStripMenuItem.Checked = false;
            onDroppedToolStripMenuItem.Checked = false;
            onCreatedToolStripMenuItem.Checked = false;

            if (_item.Scripts.ContainsKey("OnAcquired"))
            {
                this.txtEditor.Text = _item.Scripts["OnAcquired"];
            }
            else
            {
                this.txtEditor.Text = "function OnAcquired(args) \nend";
            }
        }

        private void onDroppedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            onDroppedToolStripMenuItem.Checked = true;
            onAcquiredToolStripMenuItem.Checked = false;
            onEquipToolStripMenuItem.Checked = false;
            onUseToolStripMenuItem.Checked = false;
            onCreatedToolStripMenuItem.Checked = false;

            if (_item.Scripts.ContainsKey("OnDropped"))
            {
                this.txtEditor.Text = _item.Scripts["OnDropped"];
            }
            else
            {
                this.txtEditor.Text = "function OnDropped(args) \nend";
            }
        }

        private void onCreatedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            onCreatedToolStripMenuItem.Checked = true;
            onDroppedToolStripMenuItem.Checked = false;
            onAcquiredToolStripMenuItem.Checked = false;
            onEquipToolStripMenuItem.Checked = false;
            onUseToolStripMenuItem.Checked = false;

            if (_item.Scripts.ContainsKey("OnCreated"))
            {
                this.txtEditor.Text = _item.Scripts["OnCreated"];
            }
            else
            {
                this.txtEditor.Text = "function OnCreated(args) \nend";
            }
        }

    }
}
