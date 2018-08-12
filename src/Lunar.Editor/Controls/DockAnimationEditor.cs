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
using Lunar.Core.Content.Graphics;
using Lunar.Core.Utilities.Logic;
using Lunar.Editor.Content.Graphics;
using Lunar.Editor.Utilities;
using Lunar.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace Lunar.Editor.Controls
{
    public partial class DockAnimationEditor : SavableDocument
    {
        private  FileInfo _file;
        private string _regularDockText;
        private string _unsavedDockText;
        private bool _unsaved;
        private string _activeScript;
        private TextureLoader _surfaceAnimationTextureLoader;
        private TextureLoader _subSurfaceAnimationTextureLoader;

        private Project _project;

        private AnimationDescription _animationDescription;
        private Animation _subSurfaceAnimation;
        private Animation _surfaceAnimation;

        private DockAnimationEditor()
        {
            InitializeComponent();

            _activeScript = "";

        }

        public DockAnimationEditor(Project project, string text, Image icon, FileInfo file)
            : this()
        {
            _project = project;

            _regularDockText = text;
            _unsavedDockText = text + "*";


            DockText = text;
            Icon = icon;

            _file = file;

            _animationDescription = _project.LoadAnimation(file.FullName);

            this.txtSurfaceTexPath.Text = _animationDescription.SurfaceAnimation.TexturePath;
            this.txtSurfaceFrameTime.Text = _animationDescription.SurfaceAnimation.FrameTime.ToString();
            this.txtSurfaceFrameWidth.Text = _animationDescription.SurfaceAnimation.FrameWidth.ToString();
            this.txtSurfaceFrameHeight.Text = _animationDescription.SurfaceAnimation.FrameHeight.ToString();
            this.txtSurfaceLoopCount.Text = _animationDescription.SurfaceAnimation.LoopCount.ToString();

            this.txtSubSurfaceTexPath.Text = _animationDescription.SubSurfaceAnimation.TexturePath;
            this.txtSubSurfaceFrameTime.Text = _animationDescription.SubSurfaceAnimation.FrameTime.ToString();
            this.txtSubSurfaceFrameWidth.Text = _animationDescription.SubSurfaceAnimation.FrameWidth.ToString();
            this.txtSubSurfaceFrameHeight.Text = _animationDescription.SubSurfaceAnimation.FrameHeight.ToString();
            this.txtSubSurfaceLoopCount.Text = _animationDescription.SubSurfaceAnimation.LoopCount.ToString();

            _subSurfaceAnimation = new Animation(_animationDescription);
            _surfaceAnimation = new Animation(_animationDescription);

         

            _subSurfaceAnimation.Play();
            _surfaceAnimation.Play();

            this.subSurfaceAnimView.OnDraw = OnSubAnimDraw;
            this.surfaceAnimView.OnDraw = OnSurfAnimDraw;

            this.subSurfaceAnimView.OnUpdate = OnSubAnimUpdate;
            this.surfaceAnimView.OnUpdate = OnSurfAnimUpdate;
        }

        private void OnSurfAnimUpdate(View view)
        {
            _surfaceAnimation.Update(view.GameTime);
        }

        private void OnSubAnimUpdate(View view)
        {
            _subSurfaceAnimation.Update(view.GameTime);
        }

        private void OnSurfAnimDraw(View view)
        {
            _surfaceAnimation.Draw(view.SpriteBatch);
        }

        private void OnSubAnimDraw(View view)
        {
            _subSurfaceAnimation.Draw(view.SpriteBatch);
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
            _regularDockText = _animationDescription.Name + EngineConstants.ANIM_FILE_EXT;

            this.DockText = _regularDockText;
            _unsaved = false;

            if (_animationDescription.Name + EngineConstants.ITEM_FILE_EXT != _file.Name)
            {
                File.Move(_file.FullName, _file.DirectoryName + "/" + _animationDescription.Name + EngineConstants.ITEM_FILE_EXT);

                _file = _project.ChangeItem(_file.FullName, _file.DirectoryName + "\\" + _animationDescription.Name + EngineConstants.ITEM_FILE_EXT);
            }

            _animationDescription.Save(_file.FullName);
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

                    _animationDescription.SurfaceAnimation.TexturePath = HelperFunctions.MakeRelative(path, _project.ClientRootDirectory.FullName + "/");

                    Texture2D animTexture = _surfaceAnimationTextureLoader.LoadFromFile(path);

                    _surfaceAnimation.SurfaceSprite = new Sprite(animTexture);

                    this.txtSurfaceFrameTime.Text = "1";
                    this.txtSurfaceFrameWidth.Text = animTexture.Width.ToString();
                    this.txtSurfaceFrameHeight.Text = animTexture.Height.ToString();
                    this.txtSurfaceTexPath.Text = _animationDescription.SurfaceAnimation.TexturePath;

                    this.MarkUnsaved();
                }
            }
        }

        private void surfaceAnimView_Load(object sender, EventArgs e)
        {
            this.MarkUnsaved();

            _surfaceAnimationTextureLoader = new TextureLoader(this.surfaceAnimView.GraphicsDevice);

            if (File.Exists(_project.ClientRootDirectory + "/" + _animationDescription.SubSurfaceAnimation.TexturePath))
                _surfaceAnimation.SurfaceSprite = new Sprite(_surfaceAnimationTextureLoader.LoadFromFile(_project.ClientRootDirectory + "/" + _animationDescription.SurfaceAnimation.TexturePath));
        }

        private void subSurfaceAnimView_Load(object sender, EventArgs e)
        {
            this.MarkUnsaved();

            _subSurfaceAnimationTextureLoader = new TextureLoader(this.subSurfaceAnimView.GraphicsDevice);

            if (File.Exists(_project.ClientRootDirectory + "/" + _animationDescription.SubSurfaceAnimation.TexturePath))
                _subSurfaceAnimation.SubSurfaceSprite = new Sprite(_subSurfaceAnimationTextureLoader.LoadFromFile(_project.ClientRootDirectory + "/" +_animationDescription.SubSurfaceAnimation.TexturePath));
        }


        private void txtSurfaceFrameTime_TextChanged(object sender, EventArgs e)
        {
            this.MarkUnsaved();

            int.TryParse(txtSurfaceFrameTime.Text, out int frameTime);

            _animationDescription.SurfaceAnimation.FrameTime = frameTime;
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

            _animationDescription.SurfaceAnimation.FrameWidth = frameWidth;
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

            _animationDescription.SurfaceAnimation.FrameHeight = frameHeight;
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

                    _animationDescription.SubSurfaceAnimation.TexturePath = HelperFunctions.MakeRelative(path, _project.ClientRootDirectory.FullName + "/");

                    Texture2D animTexture = _subSurfaceAnimationTextureLoader.LoadFromFile(path);

                    _subSurfaceAnimation.SubSurfaceSprite = new Sprite(animTexture);

                    this.txtSubSurfaceFrameTime.Text = "1";
                    this.txtSubSurfaceFrameWidth.Text = animTexture.Width.ToString();
                    this.txtSubSurfaceFrameHeight.Text = animTexture.Height.ToString();
                    this.txtSubSurfaceTexPath.Text = _animationDescription.SubSurfaceAnimation.TexturePath;

                    this.MarkUnsaved();
                }
            }
        }

        private void txtSubSurfaceFrameTime_TextChanged(object sender, EventArgs e)
        {
            this.MarkUnsaved();

            int.TryParse(txtSubSurfaceFrameTime.Text, out int frameTime);

            _animationDescription.SubSurfaceAnimation.FrameTime = frameTime;
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

            _animationDescription.SubSurfaceAnimation.FrameWidth = frameWidth;
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

            _animationDescription.SubSurfaceAnimation.FrameHeight = frameHeight;
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

            _animationDescription.SubSurfaceAnimation.LoopCount = loopCount;
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

            _animationDescription.SubSurfaceAnimation.FrameHeight = frameHeight;
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
