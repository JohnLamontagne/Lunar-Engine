using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Lunar.Client.Utilities
{
    public static class ContentManagerExtensions
    {
        private static TextureHandler _textureHandler = new TextureHandler();

        public static Texture2D LoadTexture2D(this ContentManager cM, string path)
        {
            return _textureHandler.LoadTexture2D(cM, path);
        }

        private class TextureHandler
        {
            private Dictionary<string, Texture2D> _textures;

            public TextureHandler()
            {
                _textures = new Dictionary<string, Texture2D>();
            }

            public Texture2D LoadTexture2D(ContentManager cM, string path)
            {
                if (!_textures.ContainsKey(path))
                {
                    if (File.Exists(path))
                    {
                        FileStream fS = File.Open(path, FileMode.Open);

                        _textures.Add(path, Texture2D.FromStream(
                            ((IGraphicsDeviceService) cM.ServiceProvider.GetService(typeof(IGraphicsDeviceService)))
                            .GraphicsDevice, fS));
                    }
                    else
                    {
                        Console.WriteLine("Could not load texture {0}: does not exist.", path); 

                        return new Texture2D(((IGraphicsDeviceService)cM.ServiceProvider.GetService(typeof(IGraphicsDeviceService)))
                            .GraphicsDevice, 1, 1);
                    }
                }

                return _textures[path];
            }
        }
    }
}
