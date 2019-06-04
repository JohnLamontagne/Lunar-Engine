using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lunar.Editor.Utilities
{
    public class TextureLoader
    {
        private GraphicsDevice _graphicsDevice;

        public TextureLoader(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        public Texture2D LoadFromFileStream(Stream stream)
        {
            Texture2D file;
            RenderTarget2D result;

            file = Texture2D.FromStream(_graphicsDevice, stream);

            //Setup a render target to hold our final texture which will have premulitplied alpha values
            result = new RenderTarget2D(_graphicsDevice, file.Width, file.Height);
            _graphicsDevice.SetRenderTarget(result);
            _graphicsDevice.Clear(Color.Black);

            //Multiply each color by the source alpha, and write in just the color values into the final texture
            var blendColor = new BlendState
            {
                ColorWriteChannels = ColorWriteChannels.Red | ColorWriteChannels.Green | ColorWriteChannels.Blue,
                AlphaDestinationBlend = Blend.Zero,
                ColorDestinationBlend = Blend.Zero,
                AlphaSourceBlend = Blend.SourceAlpha,
                ColorSourceBlend = Blend.SourceAlpha
            };

            var spriteBatch = new SpriteBatch(_graphicsDevice);
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
            _graphicsDevice.SetRenderTarget(null);

            return file;
        }

        public Texture2D LoadFromFile(string fileName)
        {
            Texture2D texture2D = null;
            using (var fileStream = new FileStream(fileName, FileMode.Open))
            {
                texture2D = this.LoadFromFileStream(fileStream);
            }

            return texture2D;
        }

        public void CreateBorder(Texture2D texture, int borderWidth, Color borderColor)
        {
            Color[] colors = new Color[texture.Width * texture.Height];

            for (int x = 0; x < texture.Width; x++)
            {
                for (int y = 0; y < texture.Height; y++)
                {
                    bool colored = false;
                    for (int i = 0; i <= borderWidth; i++)
                    {
                        if (x == i || y == i || x == texture.Width - 1 - i || y == texture.Height - 1 - i)
                        {
                            colors[x + y * texture.Width] = borderColor;
                            colored = true;
                            break;
                        }
                    }

                    if (colored == false)
                        colors[x + y * texture.Width] = Color.Transparent;
                }
            }

            texture.SetData(colors);
        }
    }

}
