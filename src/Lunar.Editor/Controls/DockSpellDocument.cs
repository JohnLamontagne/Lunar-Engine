using System;
using DarkUI.Forms;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Lunar.Core;
using Lunar.Core.Utilities.Logic;
using Lunar.Core.World;
using ScintillaNET;
using Lunar.Graphics.Effects;

namespace Lunar.Editor.Controls
{
    public partial class DockSpellDocument : SavableDocument
    {
        private string _regularDockText;
        private string _unsavedDockText;
        private bool _unsaved;
        private string _activeScript;

        private Animation _casterAnimation;
        private Animation _targetAnimation;

        private Project _project;

        private SpellModel _spell;

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

            _spell = _project.LoadSpell(file.FullName);

            this.txtName.Text = _spell.Name;

            this.txtCastTime.Text = _spell.CastTime.ToString();
            this.txtCooldownTime.Text = _spell.CooldownTime.ToString();
            this.txtActiveTime.Text = _spell.ActiveTime.ToString();

            this.txtReqStr.Text = _spell.ReqStats.Strength.ToString();
            this.txtReqInt.Text = _spell.ReqStats.Intelligence.ToString();
            this.txtReqDef.Text = _spell.ReqStats.Defense.ToString();
            this.txtReqHealth.Text = _spell.ReqStats.Vitality.ToString();
            this.txtReqDex.Text = _spell.ReqStats.Dexterity.ToString();

            this.txtStrMod.Text = _spell.StatModifiers.Strength.ToString();
            this.txtDefMod.Text = _spell.StatModifiers.Defense.ToString();
            this.txtIntMod.Text = _spell.StatModifiers.Intelligence.ToString();
            this.txtDexMod.Text = _spell.StatModifiers.Dexterity.ToString();
            this.txtHealthMod.Text = _spell.StatModifiers.Vitality.ToString();

            this.txtHealthCost.Text = _spell.HealthCost.ToString();
            this.txtManaCost.Text = _spell.ManaCost.ToString();

            _regularDockText = _spell.Name + EngineConstants.SCRIPT_FILE_EXT;
            this.DockText = _regularDockText;
            _unsavedDockText = _regularDockText + "*";

            if (File.Exists(_project.ClientRootDirectory + "/" + _spell.DisplaySprite.TextureName))
                this.picTexture.Load(_project.ClientRootDirectory + "/" + _spell.DisplaySprite.TextureName);

            if (!string.IsNullOrEmpty(_spell.CasterAnimationPath))
            {
                _casterAnimation = _project.LoadAnimation(_project.ClientRootDirectory + "/" + _spell.CasterAnimationPath);
            }

            if (!string.IsNullOrEmpty(_spell.TargetAnimationPath))
            {
                _targetAnimation = _project.LoadAnimation(_project.ClientRootDirectory + "/" + _spell.TargetAnimationPath);
            }

            // Hook up UI display view handlers for animation rendering
            this.targetAnimView.OnDraw = OnTargetAnimationDraw;
            this.casterAnimView.OnDraw = OnCasterAnimationDraw;
            this.casterAnimView.OnUpdate = OnCasterAnimationUpdate;
            this.targetAnimView.OnUpdate = OnTargetAnimationUpdate;
        }

        private void OnTargetAnimationUpdate(View view)
        {
            _targetAnimation?.Update(view.GameTime);
        }

        private void OnCasterAnimationUpdate(View view)
        {
            _casterAnimation?.Update(view.GameTime);
        }

        private void OnTargetAnimationDraw(View view)
        {
            _targetAnimation?.DrawSubSurface(view.SpriteBatch);
            _targetAnimation?.DrawSurface(view.SpriteBatch);
        }

        private void OnCasterAnimationDraw(View view)
        {
            _casterAnimation?.DrawSubSurface(view.SpriteBatch);
            _casterAnimation?.DrawSurface(view.SpriteBatch);
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
            this.DockText = _spell.Name + EngineConstants.SPELL_FILE_EXT;
            _unsaved = false;
        }

        public override void Save()
        {
            _regularDockText = _spell.Name + EngineConstants.SPELL_FILE_EXT;

            this.DockText = _regularDockText;
            _unsaved = false;

            if (_spell.Name + EngineConstants.SPELL_FILE_EXT != this.ContentFile.Name)
            {
                File.Move(this.ContentFile.FullName, this.ContentFile.DirectoryName + "/" + _spell.Name + EngineConstants.SPELL_FILE_EXT);

                this.ContentFile = _project.ChangeSpell(this.ContentFile.FullName, this.ContentFile.DirectoryName + "\\" + _spell.Name + EngineConstants.SPELL_FILE_EXT);
            }

            _project.SaveSpell(this.ContentFile.FullName, _spell);
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

                    _spell.DisplaySprite.TextureName = Helpers.MakeRelative(path, _project.ClientRootDirectory.FullName + "/"); ;

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
            _spell.Name = txtName.Text;

            _regularDockText = _spell.Name + EngineConstants.SPELL_FILE_EXT;
            this.DockText = _regularDockText;
            _unsavedDockText = _regularDockText + "*";

            this.MarkUnsaved();
        }

        private void MarkUnsaved()
        {
            this.DockText = _unsavedDockText;
            _unsaved = true;
        }

        private void TxtReqStr_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(this.txtReqStr.Text, out int reqStr))
            {
                _spell.StatRequirements.Strength = reqStr;
            }
        }

        private void TxtReqInt_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(this.txtReqInt.Text, out int reqInt))
            {
                _spell.StatRequirements.Intelligence = reqInt;
            }
        }

        private void TxtReqDex_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(this.txtReqDex.Text, out int reqDex))
            {
                _spell.StatRequirements.Dexterity = reqDex;
            }
        }

        private void TxtReqDef_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(this.txtReqDef.Text, out int reqDef))
            {
                _spell.StatRequirements.Defense = reqDef;
            }
        }

        private void TxtReqHealth_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(this.txtReqHealth.Text, out int reqHealth))
            {
                _spell.StatRequirements.Vitality = reqHealth;
            }
        }

        private void TxtManaCost_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(this.txtManaCost.Text, out int manaCost))
            {
                _spell.ManaCost = manaCost;
            }
        }

        private void TxtHealthCost_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(this.txtHealthCost.Text, out int healthCost))
            {
                _spell.HealthCost = healthCost;
            }
        }

        private void TxtStrMod_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(this.txtStrMod.Text, out int strMod))
            {
                _spell.StatModifiers.Strength = strMod;
            }
        }

        private void TxtIntMod_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(this.txtIntMod.Text, out int intMod))
            {
                _spell.StatModifiers.Intelligence = intMod;
            }
        }

        private void TxtDexMod_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(this.txtDexMod.Text, out int dexMod))
            {
                _spell.StatModifiers.Dexterity = dexMod;
            }
        }

        private void TxtDefMod_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(this.txtDefMod.Text, out int defMod))
            {
                _spell.StatModifiers.Strength = defMod;
            }
        }

        private void TxtHealthMod_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(this.txtHealthMod.Text, out int healthMod))
            {
                _spell.StatModifiers.Vitality = healthMod;
            }
        }

        private void TxtCastTime_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(this.txtCastTime.Text, out int castTime))
            {
                _spell.CastTime = castTime;
            }
        }

        private void TxtActiveTime_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(this.txtActiveTime.Text, out int activeTime))
            {
                _spell.ActiveTime = activeTime;
            }
        }

        private void TxtCooldownTime_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(this.txtCooldownTime.Text, out int cooldownTime))
            {
                _spell.CooldownTime = cooldownTime;
            }
        }
    }
}