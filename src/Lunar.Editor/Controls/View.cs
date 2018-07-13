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
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Lunar.Editor.Controls
{
    public partial class View : UserControl
    {
        private SwapChainRenderTarget _chain;
        private PresentationParameters _presentationParams;
        private GraphicsDevice _graphicsDevice;
        private SpriteBatch _spriteBatch;
        private Thread _gameLoopThread;

        private ContentManager _contentManager;


        private GameTime _gameTime;
        private Stopwatch _timer;
        private TimeSpan _elapsed;
        private bool _active;

        public GraphicsDevice GraphicsDevice => _graphicsDevice;
        public SpriteBatch SpriteBatch => _spriteBatch;
        public bool SuspendOnFormInactive { get; set; }
        public GameTime GameTime => _gameTime;

        public View()
        {
            InitializeComponent();

            _timer = Stopwatch.StartNew();
            _elapsed = _timer.Elapsed;
            _active = true;

            this.Resize += View_Resize;

           Application.Idle += delegate
           {
               this.GameLoop();

               this.ParentForm?.Invalidate();
           };

            this.Load += View_Load;
        }

        private void View_Resize(object sender, EventArgs e)
        {
            if (_presentationParams == null)
                return;

            _presentationParams.IsFullScreen = false;
            _presentationParams.BackBufferWidth = this.Width; //
            _presentationParams.BackBufferHeight = this.Height; //
            _presentationParams.RenderTargetUsage = RenderTargetUsage.DiscardContents;
            _presentationParams.PresentationInterval = PresentInterval.Immediate;
            this.CreateSwapChain();
        }

        private void View_Load(object sender, EventArgs e)
        {
            this.InitMonogame();
            this.CreateSwapChain();
        }

        private void InitMonogame()
        {
            try
            {
                //Create the Graphics Device
                _presentationParams = new PresentationParameters();
                _presentationParams.IsFullScreen = false;
                _presentationParams.BackBufferWidth = this.Width; //
                _presentationParams.BackBufferHeight = this.Height; //
                _presentationParams.RenderTargetUsage = RenderTargetUsage.DiscardContents;
                _presentationParams.PresentationInterval = PresentInterval.Immediate;

                // Create device
                _graphicsDevice = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, GraphicsProfile.HiDef,
                    _presentationParams);

                //Define our spritebatch :D
                _spriteBatch = new SpriteBatch(_graphicsDevice);

                this.OnInitalize?.Invoke(this);
            }
            catch (Exception ex)
            {
                // ignored
                MessageBox.Show("Failed to initialize MonoGame. Exception Info: " + ex + "\nClosing Now");
                Application.Exit();
            }
        }

        private void CreateSwapChain()
        {
            if (_chain != null)
            {
                _chain.Dispose();
            }
            if (_graphicsDevice != null)
            {
                if (this.Width > 0 && this.Height > 0)
                {
                    _chain = new SwapChainRenderTarget(_graphicsDevice,
                        this.Handle,
                        this.Width, this.Height, false, SurfaceFormat.Color,
                            DepthFormat.Depth24, 0, RenderTargetUsage.DiscardContents, PresentInterval.Immediate);
                }
            }
        }

        private void GameLoop()
        {
            if (SuspendOnFormInactive && !_active)
            {
                return;
            }

            _gameTime = new GameTime(_timer.Elapsed, _timer.Elapsed - _elapsed);
            _elapsed = _timer.Elapsed;

            this.OnUpdate?.Invoke(this);

            if (this.Visible)
            {
                this.Draw();
            }
        }

        private void Draw()
        {
            if (_graphicsDevice != null && _chain != null && this.ParentForm?.WindowState != FormWindowState.Minimized)
            {
                _graphicsDevice.SetRenderTarget(_chain);
                _graphicsDevice.Clear(Color.Black);

                _spriteBatch.Begin();

                this.OnDraw?.Invoke(this);

                _spriteBatch.End();

                _chain.Present();
                _graphicsDevice.SetRenderTarget(null);
            }
        }

        public Texture2D LoadTexture2D(string filePath)
        {
            Texture2D file;
            RenderTarget2D result;

            using (var titleStream = new FileStream(filePath, FileMode.Open))
            {
                file = Texture2D.FromStream(this.GraphicsDevice, titleStream);
            }

            //Setup a render target to hold our final texture which will have premulitplied alpha values
            result = new RenderTarget2D(this.GraphicsDevice, file.Width, file.Height);

            this.GraphicsDevice.SetRenderTarget(result);
            this.GraphicsDevice.Clear(Color.Black);

            //Multiply each color by the source alpha, and write in just the color values into the final texture
            var blendColor = new BlendState
            {
                ColorWriteChannels = ColorWriteChannels.Red | ColorWriteChannels.Green | ColorWriteChannels.Blue,
                AlphaDestinationBlend = Blend.Zero,
                ColorDestinationBlend = Blend.Zero,
                AlphaSourceBlend = Blend.SourceAlpha,
                ColorSourceBlend = Blend.SourceAlpha
            };

            var spriteBatch = new SpriteBatch(this.GraphicsDevice);
            spriteBatch.Begin(SpriteSortMode.Immediate, blendColor);
            spriteBatch.Draw(file, file.Bounds, Color.White);
            spriteBatch.End();

            //Now copy over the alpha values from the PNG source texture to the final one, without multiplying them
            var blendAlpha = new BlendState
            {
                ColorWriteChannels = ColorWriteChannels.Alpha,
                AlphaDestinationBlend = Blend.Zero,
                ColorDestinationBlend = Blend.Zero,
                AlphaSourceBlend = Blend.One,
                ColorSourceBlend = Blend.One
            };

            spriteBatch.Begin(SpriteSortMode.Immediate, blendAlpha);
            spriteBatch.Draw(file, file.Bounds, Color.White);
            spriteBatch.End();

            //Release the GPU back to drawing to the screen
            this.GraphicsDevice.SetRenderTarget(null);

            return result;
        }


        public Action<View> OnDraw { get; set; }
        public Action<View> OnUpdate { get; set; }
        public Action<View> OnInitalize { get; set; }
    }
}
