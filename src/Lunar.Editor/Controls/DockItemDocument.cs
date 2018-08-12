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
using DarkUI.Forms;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Lunar.Core;
using Lunar.Core.Utilities.Logic;
using Lunar.Core.World;
using Lunar.Editor.World;
using Microsoft.Xna.Framework.Graphics;
using ScintillaNET;

namespace Lunar.Editor.Controls
{
    public partial class DockItemDocument : SavableDocument
    {
        private FileInfo _file;
        private string _regularDockText;
        private string _unsavedDockText;
        private bool _unsaved;
        private string _activeScript;

        private Project _project;

        private ItemDescriptor _item;

        private DockItemDocument()
        {
            InitializeComponent();

            _activeScript = "";

            this.txtEditor.Lexer = Lexer.Lua;

            this.txtEditor.StyleResetDefault();

            this.txtEditor.Styles[Style.Default].Font = "Consolas";
            this.txtEditor.Styles[Style.Default].Size = 12;

            this.txtEditor.Styles[Style.Default].BackColor = Color.FromArgb(1, 36, 76);
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

        public DockItemDocument(Project project, string text, Image icon, FileInfo file)
            : this()
        {
            _project = project;

        
            DockText = text;
            Icon = icon;

            _file = file;

            _item = _project.LoadItem(file.FullName);

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

            _regularDockText = _item.Name + EngineConstants.ITEM_FILE_EXT;
            this.DockText = _regularDockText;
            _unsavedDockText = _regularDockText + "*";

            if (File.Exists(_project.ClientRootDirectory + "/" + _item.TexturePath))
                this.picTexture.Load(_project.ClientRootDirectory + "/" + _item.TexturePath);

            onUseToolStripMenuItem.Checked = true;
            this.txtEditor.Text = _item.Scripts.ContainsKey("OnUse") ? _item.Scripts["OnUse"] : "";

            if (_item.ItemType != ItemTypes.Equipment)
                this.panelEquipment.Enabled = false;
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

            if (_item.Name + EngineConstants.ITEM_FILE_EXT != _file.Name)
            {
                File.Move(_file.FullName, _file.DirectoryName + "/" + _item.Name + EngineConstants.ITEM_FILE_EXT);

                _file = _project.ChangeItem(_file.FullName, _file.DirectoryName + "\\" + _item.Name + EngineConstants.ITEM_FILE_EXT);
            }

            _item.Save(_file.FullName);
        }

        private void txtEditor_TextChanged(object sender, System.EventArgs e)
        {
            this.MarkUnsaved();

            if (_item.Scripts.ContainsKey(_activeScript))
            {
                _item.Scripts[_activeScript] = txtEditor.Text;
            }
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

                    _item.TexturePath = HelperFunctions.MakeRelative(path, _project.ClientRootDirectory.FullName + "/"); ;

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
            this.MarkUnsaved();

            int.TryParse(txtStr.Text, out int newStr);

            _item.Strength = newStr;
        }

        private void txtInt_TextChanged(object sender, EventArgs e)
        {
            this.MarkUnsaved();

            int.TryParse(txtInt.Text, out int newInt);

            _item.Intelligence = newInt;
        }

        private void txtDex_TextChanged(object sender, EventArgs e)
        {
            this.MarkUnsaved();

            int.TryParse(txtDex.Text, out int newDex);

            _item.Dexterity = newDex;
        }

        private void txtDef_TextChanged(object sender, EventArgs e)
        {
            this.MarkUnsaved();

            int.TryParse(txtDef.Text, out int newDef);

            _item.Defence = newDef;
        }

        private void txtHealth_TextChanged(object sender, EventArgs e)
        {
            this.MarkUnsaved();

            int.TryParse(txtHealth.Text, out int newHealth);

            _item.Health = newHealth;
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            _item.Name = txtName.Text;

            _regularDockText = _item.Name + EngineConstants.ITEM_FILE_EXT;
            this.DockText = _regularDockText;
            _unsavedDockText = _regularDockText + "*";

            this.MarkUnsaved();
        }

        private void radioStackable_CheckedChanged(object sender, EventArgs e)
        {
            this.MarkUnsaved();

            _item.Stackable = radioStackable.Checked;
        }

        private void radioNotStackable_CheckedChanged(object sender, EventArgs e)
        {
            this.MarkUnsaved();

            _item.Stackable = !radioNotStackable.Checked;
        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.MarkUnsaved();

            _item.ItemType = (ItemTypes)this.cmbType.SelectedItem;

            if (_item.ItemType != ItemTypes.Equipment)
                this.panelEquipment.Enabled = false;
            else
                this.panelEquipment.Enabled = true;
        }


        private void cmbEquipmentSlot_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.MarkUnsaved();

            _item.SlotType = (EquipmentSlots) cmbEquipmentSlot.SelectedItem;
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
                _item.Scripts.Add("OnUse", this.txtEditor.Text);
            }

            _activeScript = "OnUse";
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
                _item.Scripts.Add("OnEqip", this.txtEditor.Text);
            }

            _activeScript = "OnEquip";
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
                _item.Scripts.Add("OnAcquired", this.txtEditor.Text);
            }

            _activeScript = "OnAcquired";
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
                _item.Scripts.Add("OnDropped", this.txtEditor.Text);
            }

            _activeScript = "OnDropped";
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
                _item.Scripts.Add("OnCreated", this.txtEditor.Text);
            }

            _activeScript = "OnCreated";
        }

        private void MarkUnsaved()
        {
            this.DockText = _unsavedDockText;
            _unsaved = true;
        }

        private void panelEquipment_EnabledChanged(object sender, EventArgs e)
        {
            foreach (Control child in panelEquipment.Controls)
                child.Enabled = panelEquipment.Enabled;
        }

        private void txtStr_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtInt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtDex_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtDef_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtHealth_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
