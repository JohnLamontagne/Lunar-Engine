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
                    Texture = {Tag = HelperFunctions.MakeRelative(value, _project.ClientRootDirectory.FullName + "/")}
                };

                _spriteFilePath = _mapObject.Sprite.Texture.Tag.ToString();
            }
        }

        [Category("Texture Source")]
        public int Left
        {
            get => _mapObject.Sprite.SourceRectangle.Left;
            set => _mapObject.Sprite.SourceRectangle = new Rectangle(value, _mapObject.Sprite.SourceRectangle.Top,
                _mapObject.Sprite.SourceRectangle.Width, _mapObject.Sprite.SourceRectangle.Height);
        }

        [Category("Texture Source")]
        public int Top
        {
            get => _mapObject.Sprite.SourceRectangle.Top;
            set => _mapObject.Sprite.SourceRectangle = new Rectangle(_mapObject.Sprite.SourceRectangle.Left, value,
                _mapObject.Sprite.SourceRectangle.Width, _mapObject.Sprite.SourceRectangle.Height);
        }

        [Category("Texture Source")]
        public int Width
        {
            get => _mapObject.Sprite.SourceRectangle.Width;
            set => _mapObject.Sprite.SourceRectangle = new Rectangle(_mapObject.Sprite.SourceRectangle.Left, _mapObject.Sprite.SourceRectangle.Top,
                value, _mapObject.Sprite.SourceRectangle.Height);
        }

        [Category("Texture Source")]
        public int Height
        {
            get => _mapObject.Sprite.SourceRectangle.Height;
            set => _mapObject.Sprite.SourceRectangle = new Rectangle(_mapObject.Sprite.SourceRectangle.Left, _mapObject.Sprite.SourceRectangle.Top,
                _mapObject.Sprite.SourceRectangle.Width, value);
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
            set => _mapObject.LuaScriptPath = HelperFunctions.MakeRelative(value, _project.ServerRootDirectory.FullName + "/");
        }


        [Category("Animation")]
        [DisplayName("Animated")]
        public bool Animated
        {
            get => _mapObject.Animated;
            set
            {
                _mapObject.Animated = value;
                Rectangle srcRectangle = _mapObject.Sprite.SourceRectangle;
                _mapObject.Sprite = new AnimatedSprite(_mapObject.Sprite.Texture)
                {
                    SourceRectangle = srcRectangle
                };
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
