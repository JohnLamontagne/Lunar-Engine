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
using DarkUI.Controls;
using Lunar.Core;
using Lunar.Core.Utilities.Data;
using Lunar.Core.Utilities.Logic;
using Lunar.Core.World;
using Lunar.Core.World.Actor.Descriptors;
using Lunar.Editor.World;
using Microsoft.Xna.Framework.Graphics;
using ScintillaNET;

namespace Lunar.Editor.Controls
{
    public partial class DockNPCEditor : SavableDocument
    {
        private FileInfo _file;
        private string _regularDockText;
        private string _unsavedDockText;
        private bool _unsaved;
        private string _activeScript;

        private Project _project;

        private NPCDescriptor _npc;

        private DockNPCEditor()
        {
            InitializeComponent();

            _activeScript = "";

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

        public DockNPCEditor(Project project, string text, Image icon, FileInfo file)
            : this()
        {
            _project = project;

            _regularDockText = text;
            _unsavedDockText = text + "*";


            DockText = text;
            Icon = icon;

            _file = file;

            _npc = _project.LoadNPC(file.FullName);

            this.txtName.Text = _npc.Name;

            this.radAggressive.Checked = _npc.Aggressive;
            this.radUnaggressive.Checked = !_npc.Aggressive;

            this.txtStr.Text = _npc.Strength.ToString();
            this.txtInt.Text = _npc.Intelligence.ToString();
            this.txtDef.Text = _npc.Defence.ToString();
            this.txtHealth.Text = _npc.Health.ToString();
            this.txtDex.Text = _npc.Dexterity.ToString();
            this.txtFrameWidth.Text = _npc.FrameSize.X.ToString();
            this.txtFrameHeight.Text = _npc.FrameSize.Y.ToString();
            this.txtColLeft.Text = _npc.CollisionBounds.Left.ToString();
            this.txtColTop.Text = _npc.CollisionBounds.Top.ToString();
            this.txtColWidth.Text = _npc.CollisionBounds.Width.ToString();
            this.txtColHeight.Text = _npc.CollisionBounds.Height.ToString();
            this.txtMaxRoam.Text = _npc.MaxRoam.X.ToString();

            this.cmbEquipSlot.DataSource = Enum.GetValues(typeof(EquipmentSlots));
            this.cmbEquipSlot.SelectedItem = EquipmentSlots.MainArm;

            if (File.Exists(_project.ClientRootDirectory + "/" + _npc.TexturePath))
            {
                this.picSpriteSheet.Load(_project.ClientRootDirectory + "/" + _npc.TexturePath);
                this.picCollisionPreview.Load(_project.ClientRootDirectory + "/" + _npc.TexturePath);

                if (_npc.FrameSize == Vector.Zero)
                    _npc.FrameSize = new Vector(this.picSpriteSheet.Image.Width, this.picSpriteSheet.Image.Height);

                this.txtFrameWidth.Text = _npc.FrameSize.X.ToString();
                this.txtFrameHeight.Text = _npc.FrameSize.Y.ToString();
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
            this.MarkUnsaved();
        }

        public override void Save()
        {
            _regularDockText = _npc.Name + EngineConstants.NPC_FILE_EXT;

            this.DockText = _regularDockText;
            _unsaved = false;

            if (_npc.Name + EngineConstants.NPC_FILE_EXT != _file.Name)
            {
                File.Move(_file.FullName, _file.DirectoryName + "/" + _npc.Name + EngineConstants.NPC_FILE_EXT);

                _file = _project.ChangeNPC(_file.FullName, _file.DirectoryName + "\\" + _npc.Name + EngineConstants.NPC_FILE_EXT);
            }

            _npc.Save(_file.FullName);
        }

        private void txtEditor_TextChanged(object sender, System.EventArgs e)
        {
            this.MarkUnsaved();

            if (_npc.Scripts.ContainsKey(_activeScript))
            {
                _npc.Scripts[_activeScript] = txtEditor.Text;
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
                    this.MarkUnsaved();

                    string path = dialog.FileName; 

                    _npc.TexturePath = path;
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
            _npc.Name = txtName.Text;

            this.DockText = _npc.Name + EngineConstants.NPC_FILE_EXT;
            _unsavedDockText = _npc.Name + EngineConstants.NPC_FILE_EXT + "*";

            this.MarkUnsaved();
        }

       

        private void cmbEquipmentSlot_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.lstItems.Items.Clear();

            this.lstItems.Items.Add(new DarkListItem("Nothing"));

            // Update the item list with all items that are equippable to that slot
            foreach (var item in _project.Items.Values)
            {
                if (item.SlotType == (EquipmentSlots) this.cmbEquipSlot.SelectedItem)
                {
                    var itemEntry = new DarkListItem(item.Name)
                    {
                        Tag = item
                    };

                    this.lstItems.Items.Add(itemEntry);
                }
            }

            this.MarkUnsaved();
        }

        private void onUseToolStripMenuItem_Click(object sender, EventArgs e)
        {

            onUseToolStripMenuItem.Checked = true;
            onEquipToolStripMenuItem.Checked = false;
            onAcquiredToolStripMenuItem.Checked = false;
            onDroppedToolStripMenuItem.Checked = false;
            onCreatedToolStripMenuItem.Checked = false;

            if (_npc.Scripts.ContainsKey("OnUse"))
            {
                this.txtEditor.Text = _npc.Scripts["OnUse"];
            }
            else
            {
                _npc.Scripts.Add("OnUse", this.txtEditor.Text);
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

            if (_npc.Scripts.ContainsKey("OnEquip"))
            {
                this.txtEditor.Text = _npc.Scripts["OnEqip"];
            }
            else
            {
                _npc.Scripts.Add("OnEqip", this.txtEditor.Text);
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

            if (_npc.Scripts.ContainsKey("OnAcquired"))
            {
                this.txtEditor.Text = _npc.Scripts["OnAcquired"];
            }
            else
            {
                _npc.Scripts.Add("OnAcquired", this.txtEditor.Text);
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

            if (_npc.Scripts.ContainsKey("OnDropped"))
            {
                this.txtEditor.Text = _npc.Scripts["OnDropped"];
            }
            else
            {
                _npc.Scripts.Add("OnDropped", this.txtEditor.Text);
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

            if (_npc.Scripts.ContainsKey("OnCreated"))
            {
                this.txtEditor.Text = _npc.Scripts["OnCreated"];
            }
            else
            {
                _npc.Scripts.Add("OnCreated", this.txtEditor.Text);
            }

            _activeScript = "OnCreated";
        }

        private void txtColTop_TextChanged(object sender, EventArgs e)
        {
            this.MarkUnsaved();

            int.TryParse(this.txtColTop.Text, out int newTop);

            _npc.CollisionBounds = new Rect(_npc.CollisionBounds.Left, newTop, _npc.CollisionBounds.Width, _npc.CollisionBounds.Height);

            this.picCollisionPreview.Invalidate();
        }

        private void txtColHeight_TextChanged(object sender, EventArgs e)
        {
            this.MarkUnsaved();

            int.TryParse(this.txtColHeight.Text, out int newHeight);

            _npc.CollisionBounds = new Rect(_npc.CollisionBounds.Left, _npc.CollisionBounds.Top, _npc.CollisionBounds.Width, newHeight);

            this.picCollisionPreview.Invalidate();
        }

        private void txtColLeft_TextChanged(object sender, EventArgs e)
        {
            this.MarkUnsaved();

            int.TryParse(this.txtColLeft.Text, out int newLeft);

            _npc.CollisionBounds = new Rect(newLeft, _npc.CollisionBounds.Top, _npc.CollisionBounds.Width, _npc.CollisionBounds.Height);

            this.picCollisionPreview.Invalidate();
        }

        private void txtColWidth_TextChanged(object sender, EventArgs e)
        {
            this.MarkUnsaved();

            int.TryParse(this.txtColWidth.Text, out int newWidth);

            _npc.CollisionBounds = new Rect(_npc.CollisionBounds.Left, _npc.CollisionBounds.Top, newWidth, _npc.CollisionBounds.Height);

            this.picCollisionPreview.Invalidate();
        }

        private void txtStr_TextChanged(object sender, EventArgs e)
        {
            this.MarkUnsaved();

            int.TryParse(this.txtStr.Text, out int newStr);

            _npc.Strength = newStr;
        }

        private void txtInt_TextChanged(object sender, EventArgs e)
        {
            this.MarkUnsaved();

            int.TryParse(this.txtInt.Text, out int newInt);

            _npc.Intelligence = newInt;
        }

        private void txtDex_TextChanged(object sender, EventArgs e)
        {
            this.MarkUnsaved();

            int.TryParse(this.txtDex.Text, out int newDex);

            _npc.Dexterity = newDex;
        }

        private void txtDef_TextChanged(object sender, EventArgs e)
        {
            this.MarkUnsaved();

            int.TryParse(this.txtDef.Text, out int newDef);

            _npc.Defence = newDef;
        }

        private void txtHealth_TextChanged(object sender, EventArgs e)
        {
            this.MarkUnsaved();

            int.TryParse(this.txtHealth.Text, out int newHealth);

            _npc.Health = newHealth;
        }

        private void picSpriteSheet_Paint(object sender, PaintEventArgs e)
        {
            const string notLoadedMessage = "Load Spritesheet Image";

            if (string.IsNullOrEmpty(_npc.TexturePath))
            {
                e.Graphics.DrawString(notLoadedMessage, DefaultFont, Brushes.White, new Point((int)(this.picSpriteSheet.Width / 2f - e.Graphics.MeasureString(notLoadedMessage, DefaultFont).Width / 2f),
                    (int)(this.picSpriteSheet.Width / 2f - e.Graphics.MeasureString(notLoadedMessage, DefaultFont).Height / 2f)));
            }
            else
            {
                float factor_x = this.picSpriteSheet.Image.Width / (float)this.picSpriteSheet.Width;
                float factor_y = this.picSpriteSheet.Image.Height / (float)this.picSpriteSheet.Height;

                for (int x = (int)_npc.FrameSize.X; x < this.picSpriteSheet.Image.Width; x += (int)_npc.FrameSize.X)
                {
                    e.Graphics.DrawLine(new Pen(Color.Red, 3), new Point((int)(x / factor_x), 0), new Point((int)(x / factor_x), this.picSpriteSheet.Height));
                }

                for (int y = (int)_npc.FrameSize.Y; y < this.picSpriteSheet.Image.Height; y += (int)_npc.FrameSize.Y)
                {
                    e.Graphics.DrawLine(new Pen(Color.Red, 3), new Point(0, (int)(y / factor_y)), new Point(this.picSpriteSheet.Width, (int)(y / factor_y)));
                }
            }
        }

        private void picCollisionPreview_Paint(object sender, PaintEventArgs e)
        {
            if (string.IsNullOrEmpty(_npc.TexturePath))
                return;

            float factor_x = this.picSpriteSheet.Image.Width / (float)this.picSpriteSheet.Width;
            float factor_y = this.picSpriteSheet.Image.Height / (float)this.picSpriteSheet.Height;


            for (int x = 0; x < this.picSpriteSheet.Image.Width; x += (int) _npc.FrameSize.X)
            {
                for (int y = 0; y < this.picSpriteSheet.Image.Height; y += (int) _npc.FrameSize.Y)
                {
                    Color color = Color.FromArgb(122, Color.Red);
                    e.Graphics.FillRectangle(new SolidBrush(color), new Rectangle((int)((x + _npc.CollisionBounds.Left) / factor_x), 
                        (int)((y + _npc.CollisionBounds.Top) / factor_y), (int)(_npc.CollisionBounds.Width / factor_x), (int)(_npc.CollisionBounds.Height / factor_y)));
                }
            }

           
        }

        private void picSpriteSheet_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_npc.TexturePath))
                return;

            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.RestoreDirectory = true;
                dialog.InitialDirectory = _project.ClientRootDirectory.FullName;
                dialog.Filter = @"Tileset Files (*.png)|*.png";
                dialog.DefaultExt = ".png";
                dialog.AddExtension = true;
                dialog.Multiselect = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string path = dialog.FileName;

                    _npc.TexturePath = HelperFunctions.MakeRelative(path, _project.ClientRootDirectory.FullName + "/");
                    
                    this.picSpriteSheet.Load(path);
                    _npc.FrameSize = new Vector(this.picSpriteSheet.Image.Width, this.picSpriteSheet.Image.Height);
                    this.picSpriteSheet.Refresh();

                    this.picCollisionPreview.Load(path);

                    this.MarkUnsaved();
                }
            }
        }

        private void txtFrameWidth_TextChanged(object sender, EventArgs e)
        {
            this.MarkUnsaved();

            int.TryParse(this.txtFrameWidth.Text, out int newWidth);

            _npc.FrameSize = new Vector(newWidth, _npc.FrameSize.Y);

            this.picSpriteSheet.Invalidate();
        }

        private void txtFrameHeight_TextChanged(object sender, EventArgs e)
        {
            this.MarkUnsaved();

            int.TryParse(this.txtFrameHeight.Text, out int newHeight);

            _npc.FrameSize = new Vector(_npc.FrameSize.X, newHeight);

            this.picSpriteSheet.Invalidate();
        }

        private void txtMaxRoam_TextChanged(object sender, EventArgs e)
        {
            this.MarkUnsaved();

            int.TryParse(this.txtMaxRoam.Text, out int newMaxRoam);

            _npc.MaxRoam = new Vector(newMaxRoam, newMaxRoam);
        }

        private void MarkUnsaved()
        {
            this.DockText = _unsavedDockText;
            _unsaved = true;

            
        }

        private void lstItems_SelectedIndicesChanged(object sender, EventArgs e)
        {
        }

        private void radAggressive_CheckedChanged(object sender, EventArgs e)
        {
            _npc.Aggressive = this.radAggressive.Checked;
           this.MarkUnsaved();
        }

        private void radUnaggressive_CheckedChanged(object sender, EventArgs e)
        {
            _npc.Aggressive = !this.radUnaggressive.Checked;
            this.MarkUnsaved();
        }
    }
}
