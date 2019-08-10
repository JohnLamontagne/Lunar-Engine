using Lunar.Editor.Utilities;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Lunar.Core.Utilities.Logic;
using Lunar.Editor.Controls;
using Lunar.Graphics;

namespace Lunar.Editor.World
{
    public class MapObjectPropertiesHelper
    {
        private MapObject _mapObject;
        private TextureLoader _textureLoader;
        private Project _project;

        // Used by editor to keep reference to map object sprite file
        private string _spriteFilePath;

        [Category("Texture Source")]
        [DisplayName("Texture File")]
        [Description("Source image for map object")]
        [EditorAttribute(typeof(SpriteFileNameEditor),
            typeof(System.Drawing.Design.UITypeEditor))]
        public string FileName
        {
            get => _spriteFilePath;
            set
            {
                _mapObject.Sprite = new Sprite(_textureLoader.LoadFromFile(value))
                {
                    Texture = { Tag = Helpers.MakeRelative(value, _project.ClientRootDirectory.FullName + "/") }
                };

                _spriteFilePath = _mapObject.Sprite.Texture.Tag.ToString();
            }
        }

        [Category("Texture Source")]
        public int Left
        {
            get => _mapObject.Sprite.Transform.Rect.X;
            set => _mapObject.Sprite.Transform.Rect = new Rectangle(value, _mapObject.Sprite.Transform.Rect.Y,
                _mapObject.Sprite.Transform.Rect.Width, _mapObject.Sprite.Transform.Rect.Height);
        }

        [Category("Texture Source")]
        public int Top
        {
            get => _mapObject.Sprite.Transform.Rect.Y;
            set => _mapObject.Sprite.Transform.Rect = new Rectangle(_mapObject.Sprite.Transform.Rect.X, value,
                _mapObject.Sprite.Transform.Rect.Width, _mapObject.Sprite.Transform.Rect.Height);
        }

        [Category("Texture Source")]
        public int Width
        {
            get => _mapObject.Sprite.Transform.Rect.Width;
            set => _mapObject.Sprite.Transform.Rect = new Rectangle(_mapObject.Sprite.Transform.Rect.X, _mapObject.Sprite.Transform.Rect.Y,
                value, _mapObject.Sprite.Transform.Rect.Height);
        }

        [Category("Texture Source")]
        public int Height
        {
            get => _mapObject.Sprite.Transform.Rect.Height;
            set => _mapObject.Sprite.Transform.Rect = new Rectangle(_mapObject.Sprite.Transform.Rect.X, _mapObject.Sprite.Transform.Rect.Y,
                _mapObject.Sprite.Transform.Rect.Width, value);
        }

        [Category("General")]
        public Vector2 Position
        {
            get => _mapObject.Position;
            set => _mapObject.Position = value;
        }

        [Category("General")]
        [DisplayName("Script File")]
        [EditorAttribute(typeof(ScriptFileNameEditor),
            typeof(System.Drawing.Design.UITypeEditor))]
        public string LuaScript
        {
            get => _mapObject.LuaScriptPath;
            set => _mapObject.LuaScriptPath = Helpers.MakeRelative(value, _project.ServerWorldDirectory.FullName + "/");
        }

        [Category("Animation")]
        [DisplayName("Animated")]
        public bool Animated
        {
            get => _mapObject.Animated;
            set
            {
                _mapObject.Animated = value;
                Rectangle srcRectangle = _mapObject.Sprite.Transform.Rect;
                _mapObject.Sprite = new AnimatedSprite(_mapObject.Sprite.Texture);
                _mapObject.Sprite.Transform.Rect = srcRectangle;
            }
        }

        [Category("Animation")]
        [DisplayName("Frame Time")]
        public int FrameTime
        {
            get => _mapObject.FrameTime;
            set => _mapObject.FrameTime = value;
        }

        [Category("Lighting")]
        [DisplayName("Emmitts Light")]
        public bool LightSource
        {
            get => _mapObject.LightSource;
            set => _mapObject.LightSource = value;
        }

        [Category("Lighting")]
        [DisplayName("Radius")]
        public float LightRadius
        {
            get => _mapObject.LightRadius;
            set => _mapObject.LightRadius = value;
        }

        [Category("Lighting")]
        [Browsable(false)]
        public Color LightColor
        {
            get => _mapObject.LightColor;
            set => _mapObject.LightColor = value;
        }

        [Category("Lighting")]
        [DisplayName("R")]
        public byte R
        {
            get => _mapObject.LightColor.R;
            set => _mapObject.LightColor = new Color(value, this.G, this.B, this.A);
        }

        [Category("Lighting")]
        [DisplayName("G")]
        public byte G
        {
            get => _mapObject.LightColor.G;
            set => _mapObject.LightColor = new Color(this.R, value, this.B, this.A);
        }

        [Category("Lighting")]
        [DisplayName("B")]
        public byte B
        {
            get => _mapObject.LightColor.B;
            set => _mapObject.LightColor = new Color(this.R, this.G, value, this.A);
        }

        [Category("Lighting")]
        [DisplayName("A")]
        public byte A
        {
            get => _mapObject.LightColor.A;
            set => _mapObject.LightColor = new Color(this.R, this.G, this.B, value);
        }

        public MapObjectPropertiesHelper(MapObject mapObject, TextureLoader textureLoader, Project project)
        {
            _mapObject = mapObject;
            _textureLoader = textureLoader;
            _project = project;
        }
    }
}