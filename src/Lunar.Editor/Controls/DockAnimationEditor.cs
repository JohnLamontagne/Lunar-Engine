using System;
using DarkUI.Forms;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Lunar.Core;
using Lunar.Core.Utilities.Logic;
using Lunar.Editor.Utilities;
using Lunar.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Lunar.Graphics.Effects;

namespace Lunar.Editor.Controls
{
    public partial class DockAnimationEditor : SavableDocument
    {
        private string _regularDockText;
        private string _unsavedDockText;
        private bool _unsaved;
        private string _activeScript;
        private TextureLoader _surfaceAnimationTextureLoader;
        private TextureLoader _subSurfaceAnimationTextureLoader;

        private Project _project;

        private Animation _animation;

        public DockAnimationEditor(Project project, string text, Image icon, FileInfo file)
            : base(file)
        {
            InitializeComponent();

            _activeScript = "";

            _project = project;

            _regularDockText = text;
            _unsavedDockText = text + "*";

            DockText = text;
            Icon = icon;

            this.txtSurfaceTexPath.Text = _animation.SurfaceAnimation.TexturePath;
            this.txtSurfaceFrameTime.Text = _animation.SurfaceAnimation.FrameTime.ToString();
            this.txtSurfaceFrameWidth.Text = _animation.SurfaceAnimation.FrameWidth.ToString();
            this.txtSurfaceFrameHeight.Text = _animation.SurfaceAnimation.FrameHeight.ToString();
            this.txtSurfaceLoopCount.Text = _animation.SurfaceAnimation.LoopCount.ToString();

            this.txtSubSurfaceTexPath.Text = _animation.SubSurfaceAnimation.TexturePath;
            this.txtSubSurfaceFrameTime.Text = _animation.SubSurfaceAnimation.FrameTime.ToString();
            this.txtSubSurfaceFrameWidth.Text = _animation.SubSurfaceAnimation.FrameWidth.ToString();
            this.txtSubSurfaceFrameHeight.Text = _animation.SubSurfaceAnimation.FrameHeight.ToString();
            this.txtSubSurfaceLoopCount.Text = _animation.SubSurfaceAnimation.LoopCount.ToString();

            _animation.Play();

            // Hook up UI display view handlers
            this.subSurfaceAnimView.OnDraw = OnSubAnimDraw;
            this.surfaceAnimView.OnDraw = OnSurfAnimDraw;
            this.surfaceAnimView.OnUpdate = OnSurfAnimUpdate;
        }

        private void OnSurfAnimUpdate(View view)
        {
            _animation.Update(view.GameTime);
        }

        private void OnSurfAnimDraw(View view)
        {
            _animation.DrawSurface(view.SpriteBatch);
        }

        private void OnSubAnimDraw(View view)
        {
            _animation.DrawSubSurface(view.SpriteBatch);
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
            _regularDockText = _animation.Name + EngineConstants.ANIM_FILE_EXT;

            this.DockText = _regularDockText;
            _unsaved = false;

            if (_animation.Name + EngineConstants.ITEM_FILE_EXT != this.ContentFile.Name)
            {
                File.Move(this.ContentFile.FullName, this.ContentFile.DirectoryName + "/" + _animation.Name + EngineConstants.ITEM_FILE_EXT);

                this.ContentFile = _project.ChangeItem(this.ContentFile.FullName, this.ContentFile.DirectoryName + "\\" + _animation.Name + EngineConstants.ITEM_FILE_EXT);
            }

            _animation.Save(this.ContentFile.FullName);
        }

        private void buttonSave_Click(object sender, System.EventArgs e)
        {
            this.Save();
        }

        private void btnSelectSurfaceTex_Click(object sender, EventArgs e)
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
                    string path = dialog.FileName;

                    _animation.SurfaceAnimation.TexturePath = Helpers.MakeRelative(path, _project.ClientRootDirectory.FullName + "/");

                    Texture2D animTexture = _surfaceAnimationTextureLoader.LoadFromFile(path);

                    _animation.SurfaceAnimation.Sprite = new Sprite(animTexture);

                    this.txtSurfaceFrameTime.Text = "1";
                    this.txtSurfaceFrameWidth.Text = animTexture.Width.ToString();
                    this.txtSurfaceFrameHeight.Text = animTexture.Height.ToString();
                    this.txtSurfaceTexPath.Text = _animation.SurfaceAnimation.TexturePath;

                    this.MarkUnsaved();
                }
            }
        }

        private void surfaceAnimView_Load(object sender, EventArgs e)
        {
            this.MarkUnsaved();

            _surfaceAnimationTextureLoader = new TextureLoader(this.surfaceAnimView.GraphicsDevice);

            if (File.Exists(_project.ClientRootDirectory + "/" + _animation.SurfaceAnimation.TexturePath))
                _animation.SurfaceAnimation.Sprite = new Sprite(_surfaceAnimationTextureLoader.LoadFromFile(_project.ClientRootDirectory + "/" + _animation.SurfaceAnimation.TexturePath));
        }

        private void subSurfaceAnimView_Load(object sender, EventArgs e)
        {
            this.MarkUnsaved();

            _subSurfaceAnimationTextureLoader = new TextureLoader(this.subSurfaceAnimView.GraphicsDevice);

            if (File.Exists(_project.ClientRootDirectory + "/" + _animation.SubSurfaceAnimation.TexturePath))
                _animation.SubSurfaceAnimation.Sprite = new Sprite(_subSurfaceAnimationTextureLoader.LoadFromFile(_project.ClientRootDirectory + "/" + _animation.SubSurfaceAnimation.TexturePath));
        }

        private void txtSurfaceFrameTime_TextChanged(object sender, EventArgs e)
        {
            this.MarkUnsaved();

            int.TryParse(txtSurfaceFrameTime.Text, out int frameTime);

            _animation.SurfaceAnimation.FrameTime = frameTime;
        }

        private void txtSurfaceFrameTime_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtSurfaceFrameWidth_TextChanged(object sender, EventArgs e)
        {
            this.MarkUnsaved();

            int.TryParse(txtSurfaceFrameWidth.Text, out int frameWidth);

            _animation.SurfaceAnimation.FrameWidth = frameWidth;
        }

        private void txtSurfaceFrameWidth_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtSurfaceFrameHeight_TextChanged(object sender, EventArgs e)
        {
            this.MarkUnsaved();

            int.TryParse(txtSurfaceFrameHeight.Text, out int frameHeight);

            _animation.SurfaceAnimation.FrameHeight = frameHeight;
        }

        private void txtSurfaceFrameHeight_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void btnSelectSubSurfaceTex_Click(object sender, EventArgs e)
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
                    string path = dialog.FileName;

                    _animation.SubSurfaceAnimation.TexturePath = Helpers.MakeRelative(path, _project.ClientRootDirectory.FullName + "/");

                    Texture2D animTexture = _subSurfaceAnimationTextureLoader.LoadFromFile(path);

                    _animation.SubSurfaceAnimation.Sprite = new Sprite(animTexture);

                    this.txtSubSurfaceFrameTime.Text = "1";
                    this.txtSubSurfaceFrameWidth.Text = animTexture.Width.ToString();
                    this.txtSubSurfaceFrameHeight.Text = animTexture.Height.ToString();
                    this.txtSubSurfaceTexPath.Text = _animation.SubSurfaceAnimation.TexturePath;

                    this.MarkUnsaved();
                }
            }
        }

        private void txtSubSurfaceFrameTime_TextChanged(object sender, EventArgs e)
        {
            this.MarkUnsaved();

            int.TryParse(txtSubSurfaceFrameTime.Text, out int frameTime);

            _animation.SubSurfaceAnimation.FrameTime = frameTime;
        }

        private void txtSubSurfaceFrameTime_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtSubSurfaceFrameWidth_TextChanged(object sender, EventArgs e)
        {
            this.MarkUnsaved();

            int.TryParse(txtSubSurfaceFrameWidth.Text, out int frameWidth);

            _animation.SubSurfaceAnimation.FrameWidth = frameWidth;
        }

        private void txtSubSurfaceFrameWidth_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtSubSurfaceFrameHeight_TextChanged(object sender, EventArgs e)
        {
            this.MarkUnsaved();

            int.TryParse(txtSubSurfaceFrameHeight.Text, out int frameHeight);

            _animation.SubSurfaceAnimation.FrameHeight = frameHeight;
        }

        private void txtSubSurfaceFrameHeight_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtSurfaceLoopCount_TextChanged(object sender, EventArgs e)
        {
            this.MarkUnsaved();

            int.TryParse(txtSubSurfaceFrameHeight.Text, out int loopCount);

            _animation.SubSurfaceAnimation.LoopCount = loopCount;
        }

        private void txtSurfaceLoopCount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtSubSurfaceLoopCount_TextChanged(object sender, EventArgs e)
        {
            this.MarkUnsaved();

            int.TryParse(txtSubSurfaceFrameHeight.Text, out int frameHeight);

            _animation.SubSurfaceAnimation.FrameHeight = frameHeight;
        }

        private void txtSubSurfaceLoopCount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void MarkUnsaved()
        {
            this.DockText = _unsavedDockText;
            _unsaved = true;
        }
    }
}