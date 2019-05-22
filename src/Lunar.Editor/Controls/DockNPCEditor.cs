/** Copyright 2018 John Lamontagne https://www.rpgorigin.com

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
using ScintillaNET;

namespace Lunar.Editor.Controls
{
    public partial class DockNPCEditor : SavableDocument
    {
        private FileInfo _file;
        private string _regularDockText;
        private string _unsavedDockText;
        private bool _unsaved;

        private Project _project;

        private NPCDescriptor _npc;

        private DockNPCEditor()
        {
            InitializeComponent();

            this.txtEditor.Lexer = Lexer.Python;

            this.txtEditor.StyleResetDefault();

            this.txtEditor.Styles[Style.Default].Font = "Consolas";
            this.txtEditor.Styles[Style.Default].Size = 12;

            this.txtEditor.Styles[Style.Default].BackColor = Color.FromArgb(29, 31, 33);
            this.txtEditor.Styles[Style.Default].ForeColor = Color.FromArgb(197, 200, 198);

            this.txtEditor.StyleClearAll();

            this.txtEditor.Styles[Style.Python.CommentBlock].ForeColor = Color.FromArgb(181, 189, 104);
            this.txtEditor.Styles[Style.Python.CommentLine].ForeColor = Color.FromArgb(181, 189, 104);
            this.txtEditor.Styles[Style.Python.CommentLine].Italic = true;

            this.txtEditor.Styles[Style.Python.String].ForeColor = Color.FromArgb(222, 147, 95);

            this.txtEditor.Styles[Style.Python.Operator].ForeColor = Color.FromArgb(240, 198, 116);

            this.txtEditor.Styles[Style.Python.Number].ForeColor = Color.FromArgb(138, 190, 183);

            this.txtEditor.Styles[Style.Python.Identifier].ForeColor = Color.FromArgb(178, 148, 187);

            this.txtEditor.Styles[Style.Python.Word].ForeColor = Color.FromArgb(130, 239, 104);

            this.txtEditor.CaretForeColor = Color.White;

            this.txtEditor.SetKeywords(0, "if not def");

            this.cmbVarType.Items.AddRange(new object[] { typeof(int), typeof(float), typeof(string) });
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

            this.txtStr.Text = _npc.Stats.Strength.ToString();
            this.txtInt.Text = _npc.Stats.Intelligence.ToString();
            this.txtDef.Text = _npc.Stats.Defense.ToString();
            this.txtHealth.Text = _npc.Stats.Health.ToString();
            this.txtDex.Text = _npc.Stats.Dexterity.ToString();
            this.txtFrameWidth.Text = _npc.FrameSize.X.ToString();
            this.txtFrameHeight.Text = _npc.FrameSize.Y.ToString();
            this.txtColLeft.Text = _npc.CollisionBounds.Left.ToString();
            this.txtColTop.Text = _npc.CollisionBounds.Top.ToString();
            this.txtColWidth.Text = _npc.CollisionBounds.Width.ToString();
            this.txtColHeight.Text = _npc.CollisionBounds.Height.ToString();
            this.txtMaxRoam.Text = _npc.MaxRoam.X.ToString();
            this.txtSpeed.Text = _npc.Speed.ToString();
            this.txtAttackRange.Text = _npc.AttackRange.ToString();

            this.UpdateScriptEditor();
                

            this.cmbEquipSlot.DataSource = Enum.GetValues(typeof(EquipmentSlots));
            this.cmbEquipSlot.SelectedItem = EquipmentSlots.MainArm;

            this.UpdateCustomVariablesView();
            

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

        private void UpdateCustomVariablesView()
        {
            int prevIndex = 0;
            if (this.lstVariables.SelectedIndices.Count > 0)
                prevIndex = this.lstVariables.SelectedIndices[0];


            this.lstVariables.Items.Clear();
            foreach (var val in _npc.CustomVariables.Keys)
            {
                this.lstVariables.Items.Add(new DarkListItem(val.ToString()));
            }
            if (prevIndex < 0)
                prevIndex = 0;

            if (this.lstVariables.Items.Count > 0)
                this.lstVariables.SelectItem(prevIndex);
        }

        private void UpdateScriptEditor()
        {
            if (File.Exists(_project.ServerRootDirectory.FullName + "/Scripts/" + _npc.BehaviorScriptPath))
            {
                this.txtEditor.Text = File.ReadAllText(_project.ServerRootDirectory.FullName + "/Scripts/" + _npc.BehaviorScriptPath);
                this.scriptSectorPanel.SectionHeader = "Script Editor: " + Path.GetFileName(_npc.BehaviorScriptPath);
                this.txtEditor.Enabled = true;
            }
            else
            {
                this.scriptSectorPanel.SectionHeader = "Script Editor";
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

            _npc.Stats.Strength = newStr;
        }

        private void txtInt_TextChanged(object sender, EventArgs e)
        {
            this.MarkUnsaved();

            int.TryParse(this.txtInt.Text, out int newInt);

            _npc.Stats.Intelligence = newInt;
        }

        private void txtDex_TextChanged(object sender, EventArgs e)
        {
            this.MarkUnsaved();

            int.TryParse(this.txtDex.Text, out int newDex);

            _npc.Stats.Dexterity = newDex;
        }

        private void txtDef_TextChanged(object sender, EventArgs e)
        {
            this.MarkUnsaved();

            int.TryParse(this.txtDef.Text, out int newDef);

            _npc.Stats.Defense = newDef;
        }

        private void txtHealth_TextChanged(object sender, EventArgs e)
        {
            this.MarkUnsaved();

            int.TryParse(this.txtHealth.Text, out int newHealth);

            _npc.Stats.Health = newHealth;
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
            if (string.IsNullOrEmpty(this.txtFrameWidth.Text))
                return;

            this.MarkUnsaved();

            int.TryParse(this.txtFrameWidth.Text, out int newWidth);

            if (newWidth <= 0)
                return;

            _npc.FrameSize = new Vector(newWidth, _npc.FrameSize.Y);

            this.picSpriteSheet.Invalidate();
        }

        private void txtFrameHeight_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtFrameHeight.Text))
                return;

            this.MarkUnsaved();

            int.TryParse(this.txtFrameHeight.Text, out int newHeight);

            if (newHeight <= 0)
                return;

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

        private void TxtSpeed_TextChanged(object sender, EventArgs e)
        {
            this.MarkUnsaved();

            float.TryParse(this.txtSpeed.Text, out float newSpeed);

            _npc.Speed = newSpeed;
        }

        private void TxtAttackRange_TextChanged(object sender, EventArgs e)
        {
            this.MarkUnsaved();

            int.TryParse(this.txtAttackRange.Text, out int newAttackRange);

            _npc.AttackRange = newAttackRange;
        }

        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.RestoreDirectory = true;
                dialog.InitialDirectory = _project.ServerRootDirectory.FullName + "/Scripts/";
                dialog.Filter = @"Python script Files (*.py)|*.py";
                dialog.DefaultExt = ".py";
                dialog.AddExtension = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string path = dialog.FileName;
                    _npc.BehaviorScriptPath = HelperFunctions.MakeRelative(path, _project.ServerRootDirectory.FullName + "/Scripts/");

                    File.CreateText(path).Write(Constants.DEFAULT_PY_ACTOR_BEHAVIOR);

                    this.txtEditor.Text = Constants.DEFAULT_PY_ACTOR_BEHAVIOR;
                    this.scriptSectorPanel.SectionHeader = "Script Editor: " + Path.GetFileName(_npc.BehaviorScriptPath);
                    this.txtEditor.Enabled = true;

                    this.MarkUnsaved();
                }
            }
        }

        private void LoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.RestoreDirectory = true;
                dialog.InitialDirectory = _project.ServerRootDirectory.FullName + "/Scripts/";
                dialog.Filter = @"Python script Files (*.py)|*.py";
                dialog.DefaultExt = ".py";
                dialog.AddExtension = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string path = dialog.FileName;

                    _npc.BehaviorScriptPath = HelperFunctions.MakeRelative(path, _project.ServerRootDirectory.FullName + "/Scripts/");

                    this.UpdateScriptEditor();

                    this.MarkUnsaved();
                }
            }
        }

        private void FillCustomVariableFields(string varName)
        {
            if (_npc.CustomVariables.ContainsKey(varName))
            {
                this.txtVarName.Text = varName;
                this.cmbVarType.SelectedItem = _npc.CustomVariables[varName].GetType();
                this.txtVarVal.Text = _npc.CustomVariables[varName].ToString();
            }
        }

        private void BtnClearSpritesheet_Click(object sender, EventArgs e)
        {
            _npc.TexturePath = string.Empty;
            this.picCollisionPreview.Image = null;
        }

        private void ButtonAddVariable_Click(object sender, EventArgs e)
        {
            int i = 1;
            string newName = "var";

            foreach (var name in _npc.CustomVariables.Keys)
            {
                if (newName + i == name)
                {
                    i += 1;
                }
            }

            newName += i;

            _npc.CustomVariables.Add(newName, 0);

            this.UpdateCustomVariablesView();
        }

        private void ButtonRemoveVariable_Click(object sender, EventArgs e)
        {
            
        }

        private void LstVariables_SelectedIndicesChanged(object sender, EventArgs e)
        {
            if (this.lstVariables.SelectedIndices.Count > 0 && this.lstVariables.SelectedIndices[0] >= 0)
            {
                string varName = this.lstVariables.Items[this.lstVariables.SelectedIndices[0]].Text;

                this.FillCustomVariableFields(varName);
            }
        }

        private void TxtVarName_TextChanged(object sender, EventArgs e)
        {
            if (this.lstVariables.SelectedIndices.Count > 0 && this.lstVariables.SelectedIndices[0] >= 0)
            {
                string varName = this.lstVariables.Items[this.lstVariables.SelectedIndices[0]].Text;

                if (varName == this.txtVarName.Text)
                    return;

                if (_npc.CustomVariables.ContainsKey(varName))
                {
                    var oldVariableVal = _npc.CustomVariables[varName];
                    _npc.CustomVariables.Remove(varName);

                    if (!_npc.CustomVariables.ContainsKey(this.txtVarName.Text))
                    {
                        _npc.CustomVariables.Add(this.txtVarName.Text, oldVariableVal);

                        this.lstVariables.Items[this.lstVariables.SelectedIndices[0]].Text = this.txtVarName.Text;
                    }
                    else
                    {
                        this.txtVarName.Text = varName;
                    }
                }
                
            }
        }
    }
}
