using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace Lunar.Graphics
{
    public static class ContentManagerExtensions
    {
        private static TextureHandler _textureHandler = new TextureHandler();

        public static T LoadAsset<T>(this ContentManager cM, string assetPath)
        {
            return cM.Load<T>(NormalizeAssetPath(cM, assetPath));
        }

        public static Texture2D LoadTexture2D(this ContentManager cM, string path)
        {
            return _textureHandler.LoadTexture2D(cM, path);
        }

        private static string NormalizeAssetPath(ContentManager contentManager, string assetPath)
        {
            if (string.IsNullOrWhiteSpace(assetPath))
                throw new ArgumentException("Asset path cannot be null or empty.", nameof(assetPath));

            var normalized = assetPath.Replace('\\', '/');

            if (Path.IsPathRooted(normalized))
            {
                normalized = ToRelativeContentPath(normalized);
            }

            normalized = normalized.TrimStart('/');

            var rootDirectory = (contentManager.RootDirectory ?? string.Empty).Replace('\\', '/').Trim('/');
            if (!string.IsNullOrEmpty(rootDirectory) && normalized.StartsWith(rootDirectory + "/", StringComparison.OrdinalIgnoreCase))
            {
                normalized = normalized[(rootDirectory.Length + 1)..];
            }

            var extension = Path.GetExtension(normalized);
            if (!string.IsNullOrEmpty(extension))
            {
                normalized = normalized[..^extension.Length];
            }

            return normalized;
        }

        private static string ToRelativeContentPath(string absolutePath)
        {
            var markerMappings = new[]
            {
                ("/Client Data/", string.Empty),
                ("/Data/", string.Empty),
                ("/gfx/", "gfx/"),
                ("/music/", "music/"),
                ("/sfx/", "sfx/")
            };

            foreach (var (marker, mappedPrefix) in markerMappings)
            {
                var markerIndex = absolutePath.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
                if (markerIndex < 0)
                    continue;

                var relativePart = absolutePath[(markerIndex + marker.Length)..].TrimStart('/');
                return mappedPrefix + relativePart;
            }

            return Path.GetFileNameWithoutExtension(absolutePath);
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
                            ((IGraphicsDeviceService)cM.ServiceProvider.GetService(typeof(IGraphicsDeviceService)))
                            .GraphicsDevice, fS));

                        fS.Close();
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