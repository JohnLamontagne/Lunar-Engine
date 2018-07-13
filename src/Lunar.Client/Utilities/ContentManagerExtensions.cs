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
