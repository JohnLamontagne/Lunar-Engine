/** Copyright 2018 John Lamontagne https://www.rpgorigin.com

	Licensed under the Apache License, Version 2.0 (the "License");
	you may not use this file except in compliance with the License.
	You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

	Unless required by applicable law or agreed to in writing, software
	distributed under the License is distributed on an "AS IS" BASIS,
	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	See the License for the specific language governing permissions and
	limitations under the License.
*/

using Lunar.Client.GUI.Widgets;
using Lunar.Core;
using Lunar.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;

namespace Lunar.Client.GUI
{
    public class GUIManager : WidgetCollection
    {
        public void LoadFromFile(string filePath, ContentManager content)
        {
            var doc = XDocument.Load(filePath);
            var root = doc.Root;

            var fonts = new Dictionary<string, SpriteFont>();

            foreach (var fontEntry in root.Elements("fonts").Elements("font"))
            {
                string fontName = fontEntry.Attribute("name")?.Value;
                string fontPath = fontEntry.Attribute("path")?.Value;
                fonts.Add(fontName, content.LoadAsset<SpriteFont>(Constants.FILEPATH_DATA + fontPath));
            }

            this.LoadWidgets(root, fonts, content, this);
        }

        private void LoadWidgetsFromFileImport(string filePath, Dictionary<string, SpriteFont> fonts,
            ContentManager content, WidgetCollection parent)
        {
            var doc = XDocument.Load(filePath);
            this.LoadWidgets(doc.Root, fonts, content, parent);
        }

        private void LoadWidgets(XElement parent, Dictionary<string, SpriteFont> fonts,
            ContentManager content, WidgetCollection collection)
        {
            foreach (var element in parent.Elements())
            {
                switch (element.Name.LocalName.ToLower())
                {
                    case "fonts":
                        break; // handled by LoadFromFile
                    case "import":
                        string file = element.Attribute("file")?.Value;
                        this.LoadWidgetsFromFileImport(Constants.FILEPATH_DATA + file, fonts, content, collection);
                        break;
                    case "button":
                        this.LoadButton(element, fonts, content, collection);
                        break;
                    case "label":
                        this.LoadLabel(element, fonts, collection);
                        break;
                    case "statusbar":
                        this.LoadStatusBar(element, fonts, content, collection);
                        break;
                    case "checkbox":
                        this.LoadCheckbox(element, content, collection);
                        break;
                    case "picture":
                        this.LoadPicture(element, content, collection);
                        break;
                    case "container":
                        this.LoadContainer(element, fonts, content, collection);
                        break;
                    case "textbox":
                        this.LoadTextbox(element, fonts, content, collection);
                        break;
                    case "chatbox":
                        this.LoadChatbox(element, fonts, content, collection);
                        break;
                    case "slider":
                        this.LoadSlider(element, content, collection);
                        break;
                }
            }
        }

        // --- helpers ---

        private string Attr(XElement e, string name, string fallback = null) =>
            e.Attribute(name)?.Value ?? fallback;

        private int AttrInt(XElement e, string name, int fallback = 0)
        {
            int.TryParse(Attr(e, name), out int v);
            return v != 0 ? v : fallback;
        }

        private float AttrFloat(XElement e, string name, float fallback = 0f)
        {
            float.TryParse(Attr(e, name), out float v);
            return v != 0f ? v : fallback;
        }

        private bool AttrBool(XElement e, string name, bool fallback = true)
        {
            string raw = Attr(e, name);
            return raw != null ? bool.Parse(raw) : fallback;
        }

        private Color ParseColor(string colorName)
        {
            if (string.IsNullOrEmpty(colorName))
                return Color.White;

            var prop = typeof(Color).GetProperty(colorName, BindingFlags.Static | BindingFlags.Public);
            return prop != null ? (Color)prop.GetValue(null) : Color.White;
        }

        protected virtual Vector2 ParseSize(string sizeX, string sizeY, Texture2D texture)
        {
            float x = ParseDimension(sizeX, texture.Width);
            float y = ParseDimension(sizeY, texture.Height);
            return new Vector2(x, y);
        }

        private float ParseDimension(string value, float textureSize)
        {
            if (string.IsNullOrEmpty(value)) return textureSize;
            if (value.Contains("%"))
            {
                float.TryParse(value.Replace("%", ""), out float pct);
                return textureSize * (pct / 100f);
            }
            float.TryParse(value, out float result);
            return result;
        }

        // --- loaders ---

        private void LoadButton(XElement e, Dictionary<string, SpriteFont> fonts,
            ContentManager content, WidgetCollection parent)
        {
            var texture = content.LoadTexture2D(Constants.FILEPATH_DATA + Attr(e, "texture"));
            var button = new Button(texture, Attr(e, "text", ""), fonts[Attr(e, "font")], 0)
            {
                Position = parent.ParsePosition(Attr(e, "x"), Attr(e, "y")),
                ZOrder   = AttrInt(e, "zorder"),
                Visible  = AttrBool(e, "visible")
            };
            parent.AddWidget(button, Attr(e, "name"));
        }

        private void LoadLabel(XElement e, Dictionary<string, SpriteFont> fonts, WidgetCollection parent)
        {
            var label = new Label(fonts[Attr(e, "font")])
            {
                Text     = Attr(e, "text", ""),
                Position = parent.ParsePosition(Attr(e, "x"), Attr(e, "y")),
                Color    = ParseColor(Attr(e, "color")),
                ZOrder   = AttrInt(e, "zorder"),
                Visible  = AttrBool(e, "visible")
            };
            parent.AddWidget(label, Attr(e, "name"));
        }

        private void LoadStatusBar(XElement e, Dictionary<string, SpriteFont> fonts,
            ContentManager content, WidgetCollection parent)
        {
            var backSprite = content.LoadTexture2D(Constants.FILEPATH_DATA + Attr(e, "back-sprite"));
            var fillSprite = content.LoadTexture2D(Constants.FILEPATH_DATA + Attr(e, "fill-sprite"));

            var fillX = AttrInt(e, "fill-x");
            var fillY = AttrInt(e, "fill-y");
            var fillRect = new Rectangle(fillX, fillY, fillSprite.Width, fillSprite.Height);

            var sb = new StatusBar(backSprite, fillSprite, fillRect, fonts[Attr(e, "font")])
            {
                Text       = Attr(e, "text", ""),
                Position   = parent.ParsePosition(Attr(e, "x"), Attr(e, "y")),
                ForeColor  = ParseColor(Attr(e, "color")),
                TextOffset = new Vector2(AttrInt(e, "pad-x"), AttrInt(e, "pad-y")),
                ZOrder     = AttrInt(e, "zorder"),
                Visible    = AttrBool(e, "visible")
            };
            parent.AddWidget(sb, Attr(e, "name"));
        }

        private void LoadTextbox(XElement e, Dictionary<string, SpriteFont> fonts,
            ContentManager content, WidgetCollection parent)
        {
            var texture = content.LoadTexture2D(Constants.FILEPATH_DATA + Attr(e, "texture"));
            float scaleX = AttrFloat(e, "scale-x"); if (scaleX <= 0) scaleX = 1f;
            float scaleY = AttrFloat(e, "scale-y"); if (scaleY <= 0) scaleY = 1f;

            var textBox = new Textbox(texture, fonts[Attr(e, "font")],
                new Vector2(AttrInt(e, "pad-x"), AttrInt(e, "pad-y")), 0)
            {
                Text     = Attr(e, "text", ""),
                Position = parent.ParsePosition(Attr(e, "x"), Attr(e, "y")),
                ForeColor = ParseColor(Attr(e, "color")),
                Origin   = new Vector2(AttrFloat(e, "origin-x"), AttrFloat(e, "origin-y")),
                Mask     = Attr(e, "mask"),
                Scale    = new Vector2(scaleX, scaleY),
                ZOrder   = AttrInt(e, "zorder"),
                Visible  = AttrBool(e, "visible")
            };
            parent.AddWidget(textBox, Attr(e, "name"));
        }

        private void LoadChatbox(XElement e, Dictionary<string, SpriteFont> fonts,
            ContentManager content, WidgetCollection parent)
        {
            var texture = content.LoadTexture2D(Constants.FILEPATH_DATA + Attr(e, "texture"));
            int.TryParse(Attr(e, "maxlines"), out int maxLines);

            var chatBox = new Chatbox(texture, fonts[Attr(e, "font")], maxLines)
            {
                Position    = parent.ParsePosition(Attr(e, "x"), Attr(e, "y")),
                ChatOffset  = new Vector2(AttrInt(e, "pad-x"), AttrInt(e, "pad-y")),
                ZOrder      = AttrInt(e, "zorder"),
                Visible     = AttrBool(e, "visible"),
                Draggable   = AttrBool(e, "draggable", false)
            };

            this.LoadWidgets(e, fonts, content, chatBox);
            parent.AddWidget(chatBox, Attr(e, "name"));
        }

        private void LoadPicture(XElement e, ContentManager content, WidgetCollection parent)
        {
            var texturePath = Attr(e, "texture");
            if (string.IsNullOrWhiteSpace(texturePath))
                return;

            Texture2D texture = content.LoadTexture2D(Constants.FILEPATH_DATA + texturePath);

            Enum.TryParse(Attr(e, "display", "Normal"), out DisplayWidgetMode displayMode);

            Vector2 scale = Vector2.One;
            if (displayMode == DisplayWidgetMode.Stretch && texture != null)
            {
                scale = parent is WidgetContainer c
                    ? c.Size / new Vector2(texture.Width, texture.Height)
                    : new Vector2(Settings.ResolutionX, Settings.ResolutionY) / new Vector2(texture.Width, texture.Height);
            }

            var pic = new Picture(texture)
            {
                Position    = parent.ParsePosition(Attr(e, "x"), Attr(e, "y")),
                Origin      = new Vector2(AttrFloat(e, "origin-x"), AttrFloat(e, "origin-y")),
                DisplayMode = displayMode,
                Scale       = scale,
                ZOrder      = AttrInt(e, "zorder"),
                Visible     = AttrBool(e, "visible")
            };
            parent.AddWidget(pic, Attr(e, "name"));
        }

        private void LoadContainer(XElement e, Dictionary<string, SpriteFont> fonts,
            ContentManager content, WidgetCollection parent)
        {
            var texturePath = Attr(e, "texture");
            var containerName = Attr(e, "name");
            var parsedPosition = parent.ParsePosition(Attr(e, "x"), Attr(e, "y"));
            var parsedOrigin = new Vector2(AttrFloat(e, "origin-x"), AttrFloat(e, "origin-y"));
            Texture2D texture = texturePath != null
                ? content.LoadTexture2D(Constants.FILEPATH_DATA + texturePath)
                : null;

            Vector2 size = texture != null
                ? this.ParseSize(Attr(e, "width"), Attr(e, "height"), texture)
                : Vector2.One;

            var container = texture != null
                ? new WidgetContainer(texture)
                : new WidgetContainer(size);

            container.Position = parsedPosition;
            container.Origin = parsedOrigin;
            container.Size = size;
            container.ZOrder = AttrInt(e, "zorder");
            container.Draggable = AttrBool(e, "draggable", false);
            container.Visible = AttrBool(e, "visible");

            this.LoadWidgets(e, fonts, content, container);
            parent.AddWidget(container, containerName);
        }

        private void LoadCheckbox(XElement e, ContentManager content, WidgetCollection parent)
        {
            string checkedPath   = Attr(e, "checked-texture") ?? Attr(e, "texture");
            string uncheckedPath = Attr(e, "unchecked-texture") ?? Attr(e, "texture");

            var chkBox = new Checkbox(
                content.LoadTexture2D(Constants.FILEPATH_DATA + checkedPath),
                content.LoadTexture2D(Constants.FILEPATH_DATA + uncheckedPath))
            {
                Position = parent.ParsePosition(Attr(e, "x"), Attr(e, "y")),
                ZOrder   = AttrInt(e, "zorder"),
                Visible  = AttrBool(e, "visible")
            };
            parent.AddWidget(chkBox, Attr(e, "name"));
        }

        private void LoadSlider(XElement e, ContentManager content, WidgetCollection parent)
        {
            Enum.TryParse(Attr(e, "orientation", "Vertical"), out Orientation orientation);

            var slider = new Slider(
                content.LoadTexture2D(Constants.FILEPATH_DATA + Attr(e, "texture")),
                content.LoadTexture2D(Constants.FILEPATH_DATA + Attr(e, "control-texture")),
                orientation)
            {
                Position     = parent.ParsePosition(Attr(e, "x"), Attr(e, "y")),
                Padding      = new Vector2(AttrInt(e, "pad-x"), AttrInt(e, "pad-y")),
                MaximumValue = AttrInt(e, "max-value"),
                ZOrder       = AttrInt(e, "zorder"),
                Visible      = AttrBool(e, "visible")
            };
            parent.AddWidget(slider, Attr(e, "name"));
        }
    }
}
