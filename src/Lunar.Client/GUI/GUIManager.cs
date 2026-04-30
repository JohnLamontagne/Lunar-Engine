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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace Lunar.Client.GUI
{
    public class GUIManager : WidgetCollection
    {
        public void LoadFromFile(string filePath, ContentManager content)
        {
            var jsonString = File.ReadAllText(filePath);
            using var doc = JsonDocument.Parse(jsonString);
            var root = doc.RootElement;

            var fonts = new Dictionary<string, SpriteFont>();

            if (root.TryGetProperty("fonts", out var fontsArray))
            {
                foreach (var fontEntry in fontsArray.EnumerateArray())
                {
                    var fontName = fontEntry.GetProperty("name").GetString();
                    var fontPath = fontEntry.GetProperty("path").GetString();
                    var font = content.Load<SpriteFont>(Constants.FILEPATH_DATA + fontPath);
                    fonts.Add(fontName, font);
                }
            }

            if (root.TryGetProperty("widgets", out var widgetsElement))
            {
                this.LoadWidgets(widgetsElement, fonts, content, this);
            }
        }

        private void LoadWidgetsFromFileImport(string filePath, Dictionary<string, SpriteFont> fonts,
            ContentManager content, WidgetCollection parent)
        {
            var jsonString = File.ReadAllText(filePath);
            using var doc = JsonDocument.Parse(jsonString);
            var widgetEntries = doc.RootElement;

            this.LoadWidgets(widgetEntries, fonts, content, parent);
        }

        private void LoadWidgets(JsonElement widgetEntries, Dictionary<string, SpriteFont> fonts, ContentManager content, WidgetCollection parent)
        {
            foreach (var widget in widgetEntries.EnumerateArray())
            {
                var widgetType = widget.GetProperty("type").GetString().ToLower();

                switch (widgetType)
                {
                    case "import":
                        string importFile = widget.GetProperty("file").GetString();
                        this.LoadWidgetsFromFileImport(Constants.FILEPATH_DATA + importFile, fonts, content, parent);
                        break;
                    case "button":
                        this.LoadButtonFromJSON(widget, fonts, content, parent);
                        break;
                    case "label":
                        this.LoadLabelFromJSON(widget, fonts, parent);
                        break;
                    case "statusbar":
                        this.LoadStatusBarFromJSON(widget, fonts, content, parent);
                        break;
                    case "checkbox":
                        this.LoadCheckboxFromJSON(widget, fonts, content, parent);
                        break;
                    case "picture":
                        this.LoadPictureFromJSON(widget, content, parent);
                        break;
                    case "container":
                        this.LoadWidgetContainerFromJSON(widget, fonts, content, parent);
                        break;
                    case "textbox":
                        this.LoadTextboxFromJSON(widget, fonts, content, parent);
                        break;
                    case "chatbox":
                        this.LoadChatboxFromJSON(widget, fonts, content, parent);
                        break;
                    case "slider":
                        this.LoadSliderFromJSON(widget, content, parent);
                        break;
                }
            }
        }

        private void LoadSliderFromJSON(JsonElement sliderElement, ContentManager content, WidgetCollection parent)
        {
            string sliderName = sliderElement.GetProperty("name").GetString();
            string texturePath = sliderElement.GetProperty("texture").GetString();
            string controlTexturePath = sliderElement.GetProperty("control_texture").GetString();

            int paddingX = 0;
            int paddingY = 0;
            if (sliderElement.TryGetProperty("padding", out var paddingJson))
            {
                if (paddingJson.TryGetProperty("x", out var padX))
                    int.TryParse(padX.GetRawText(), out paddingX);
                if (paddingJson.TryGetProperty("y", out var padY))
                    int.TryParse(padY.GetRawText(), out paddingY);
            }

            Enum.TryParse(sliderElement.TryGetProperty("orientation", out var orient) ? orient.GetString() : "Vertical", out Orientation orientation);

            int zOrder = 0;
            if (sliderElement.TryGetProperty("zorder", out var zo))
                int.TryParse(zo.GetRawText(), out zOrder);

            var position = parent.ParsePosition(
                sliderElement.TryGetProperty("position", out var pos) && pos.TryGetProperty("x", out var px) ? px.GetRawText() : null,
                sliderElement.TryGetProperty("position", out var pos2) && pos2.TryGetProperty("y", out var py) ? py.GetRawText() : null);

            int maxValue = 0;
            if (sliderElement.TryGetProperty("maximum_value", out var mv))
                int.TryParse(mv.GetRawText(), out maxValue);

            bool visible = true;
            if (sliderElement.TryGetProperty("visible", out var vis))
                bool.TryParse(vis.GetRawText(), out visible);

            Texture2D containerTexture = content.LoadTexture2D(Constants.FILEPATH_DATA + texturePath);
            Texture2D controlTexture = content.LoadTexture2D(Constants.FILEPATH_DATA + controlTexturePath);

            var slider = new Slider(containerTexture, controlTexture, orientation)
            {
                Position = position,
                ZOrder = zOrder,
                Padding = new Vector2(paddingX, paddingY),
                Visible = visible,
                MaximumValue = maxValue
            };

            parent.AddWidget(slider, sliderName);
        }

        private void LoadChatboxFromJSON(JsonElement chatboxElement, Dictionary<string, SpriteFont> fonts, ContentManager content, WidgetCollection parent)
        {
            string chatboxName = chatboxElement.GetProperty("name").GetString();
            string texturePath = chatboxElement.GetProperty("texture").GetString();
            string fontName = chatboxElement.GetProperty("font").GetString();

            int offX = 0, offY = 0;
            if (chatboxElement.TryGetProperty("padding", out var padding))
            {
                if (padding.TryGetProperty("x", out var px)) int.TryParse(px.GetRawText(), out offX);
                if (padding.TryGetProperty("y", out var py)) int.TryParse(py.GetRawText(), out offY);
            }

            int maxLines = 0;
            if (chatboxElement.TryGetProperty("maxlines", out var ml)) int.TryParse(ml.GetRawText(), out maxLines);

            int zOrder = 0;
            if (chatboxElement.TryGetProperty("zorder", out var zo)) int.TryParse(zo.GetRawText(), out zOrder);

            var texture = content.LoadTexture2D(Constants.FILEPATH_DATA + texturePath);

            string posX = null, posY = null;
            if (chatboxElement.TryGetProperty("position", out var pos))
            {
                if (pos.TryGetProperty("x", out var px2)) posX = px2.GetRawText();
                if (pos.TryGetProperty("y", out var py2)) posY = py2.GetRawText();
            }
            var position = parent.ParsePosition(posX, posY);

            bool draggable = false;
            if (chatboxElement.TryGetProperty("draggable", out var drag)) bool.TryParse(drag.GetRawText(), out draggable);

            bool visible = true;
            if (chatboxElement.TryGetProperty("visible", out var vis)) bool.TryParse(vis.GetRawText(), out visible);

            SpriteFont font = fonts[fontName];
            var chatBox = new Chatbox(texture, font, maxLines)
            {
                Position = position,
                ChatOffset = new Vector2(offX, offY),
                ZOrder = zOrder,
                Visible = visible,
                Draggable = draggable
            };

            if (chatboxElement.TryGetProperty("widgets", out var widgets))
                this.LoadWidgets(widgets, fonts, content, chatBox);

            parent.AddWidget(chatBox, chatboxName);
        }

        private void LoadStatusBarFromJSON(JsonElement sbElement, Dictionary<string, SpriteFont> fonts, ContentManager content, WidgetCollection parent)
        {
            string sbName = sbElement.GetProperty("name").GetString();
            string text = sbElement.TryGetProperty("text", out var t) ? t.GetString() : "";
            string fontName = sbElement.GetProperty("font").GetString();
            uint charSize = 0;
            if (sbElement.TryGetProperty("fontsize", out var fs)) uint.TryParse(fs.GetRawText(), out charSize);

            string texturePath = sbElement.GetProperty("backSprite").GetString();
            string texturePath2 = sbElement.GetProperty("fillSprite").GetString();

            var color = sbElement.TryGetProperty("color", out var colorEl) ? this.ParseColor(colorEl) : Color.White;

            string posX = null, posY = null;
            if (sbElement.TryGetProperty("position", out var pos))
            {
                if (pos.TryGetProperty("x", out var px)) posX = px.GetRawText();
                if (pos.TryGetProperty("y", out var py)) posY = py.GetRawText();
            }
            var position = parent.ParsePosition(posX, posY);

            string fillPosX = null, fillPosY = null;
            if (sbElement.TryGetProperty("fillPosition", out var fpos))
            {
                if (fpos.TryGetProperty("x", out var px)) fillPosX = px.GetRawText();
                if (fpos.TryGetProperty("y", out var py)) fillPosY = py.GetRawText();
            }
            var fillPosition = parent.ParsePosition(fillPosX, fillPosY);

            int offX = 0, offY = 0;
            if (sbElement.TryGetProperty("padding", out var padding))
            {
                if (padding.TryGetProperty("x", out var px)) int.TryParse(px.GetRawText(), out offX);
                if (padding.TryGetProperty("y", out var py)) int.TryParse(py.GetRawText(), out offY);
            }

            int zOrder = 0;
            if (sbElement.TryGetProperty("zorder", out var zo)) int.TryParse(zo.GetRawText(), out zOrder);

            Texture2D backSprite = content.LoadTexture2D(Constants.FILEPATH_DATA + texturePath);
            Texture2D fillSprite = content.LoadTexture2D(Constants.FILEPATH_DATA + texturePath2);

            bool visible = true;
            if (sbElement.TryGetProperty("visible", out var vis)) bool.TryParse(vis.GetRawText(), out visible);

            SpriteFont font = fonts[fontName];
            var _statusbar = new StatusBar(backSprite, fillSprite, new Rectangle(Convert.ToInt32(fillPosition.X), Convert.ToInt32(fillPosition.Y), fillSprite.Width, fillSprite.Height), font)
            {
                Text = text,
                Position = position,
                ForeColor = color,
                Visible = visible,
                TextOffset = new Vector2(offX, offY),
                ZOrder = zOrder
            };

            parent.AddWidget(_statusbar, sbName);
        }

        private void LoadTextboxFromJSON(JsonElement textboxElement, Dictionary<string, SpriteFont> fonts,
            ContentManager content, WidgetCollection parent)
        {
            string textboxName = textboxElement.GetProperty("name").GetString();
            string text = textboxElement.TryGetProperty("text", out var t) ? t.GetString() : "";
            string texturePath = textboxElement.GetProperty("texture").GetString();
            string fontName = textboxElement.GetProperty("font").GetString();

            uint charSize = 0;
            if (textboxElement.TryGetProperty("fontsize", out var fs)) uint.TryParse(fs.GetRawText(), out charSize);

            int offX = 0, offY = 0;
            if (textboxElement.TryGetProperty("padding", out var padding))
            {
                if (padding.TryGetProperty("x", out var px)) int.TryParse(px.GetRawText(), out offX);
                if (padding.TryGetProperty("y", out var py)) int.TryParse(py.GetRawText(), out offY);
            }
            Vector2 textOffset = new Vector2(offX, offY);

            float scaleX = 1f, scaleY = 1f;
            if (textboxElement.TryGetProperty("scale", out var scale))
            {
                if (scale.TryGetProperty("x", out var sx)) float.TryParse(sx.GetRawText(), out scaleX);
                if (scale.TryGetProperty("y", out var sy)) float.TryParse(sy.GetRawText(), out scaleY);
            }
            if (scaleX <= 0) scaleX = 1;
            if (scaleY <= 0) scaleY = 1;
            Vector2 scaleVec = new Vector2(scaleX, scaleY);

            var color = textboxElement.TryGetProperty("color", out var colorEl) ? this.ParseColor(colorEl) : Color.White;

            float originX = 0, originY = 0;
            if (textboxElement.TryGetProperty("origin", out var origin))
            {
                if (origin.TryGetProperty("x", out var ox)) float.TryParse(ox.GetRawText(), out originX);
                if (origin.TryGetProperty("y", out var oy)) float.TryParse(oy.GetRawText(), out originY);
            }
            Vector2 originVec = new Vector2(originX, originY);

            string mask = textboxElement.TryGetProperty("mask", out var m) ? m.GetString() : null;

            var texture = content.LoadTexture2D(Constants.FILEPATH_DATA + texturePath);

            string posX = null, posY = null;
            if (textboxElement.TryGetProperty("position", out var pos))
            {
                if (pos.TryGetProperty("x", out var px)) posX = px.GetRawText();
                if (pos.TryGetProperty("y", out var py)) posY = py.GetRawText();
            }
            var position = parent.ParsePosition(posX, posY);

            int zOrder = 0;
            if (textboxElement.TryGetProperty("zorder", out var zo)) int.TryParse(zo.GetRawText(), out zOrder);

            bool visible = true;
            if (textboxElement.TryGetProperty("visible", out var vis)) bool.TryParse(vis.GetRawText(), out visible);

            SpriteFont font = fonts[fontName];
            var textBox = new Textbox(texture, font, textOffset, charSize)
            {
                Text = text,
                Position = position,
                ForeColor = color,
                Origin = originVec,
                Mask = mask,
                ZOrder = zOrder,
                Visible = visible,
                Scale = scaleVec,
            };

            parent.AddWidget(textBox, textboxName);
        }

        private void LoadWidgetContainerFromJSON(JsonElement containerElement, Dictionary<string, SpriteFont> fonts, ContentManager content, WidgetCollection parent)
        {
            string containerName = containerElement.GetProperty("name").GetString();
            string texturePath = containerElement.TryGetProperty("texture", out var tp) ? tp.GetString() : null;

            string posX = null, posY = null;
            if (containerElement.TryGetProperty("position", out var pos))
            {
                if (pos.TryGetProperty("x", out var px)) posX = px.GetRawText();
                if (pos.TryGetProperty("y", out var py)) posY = py.GetRawText();
            }
            var position = parent.ParsePosition(posX, posY);

            float originX = 0, originY = 0;
            if (containerElement.TryGetProperty("origin", out var origin))
            {
                if (origin.TryGetProperty("x", out var ox)) float.TryParse(ox.GetRawText(), out originX);
                if (origin.TryGetProperty("y", out var oy)) float.TryParse(oy.GetRawText(), out originY);
            }
            Vector2 originVec = new Vector2(originX, originY);

            Texture2D texture = texturePath != null ? content.LoadTexture2D(Constants.FILEPATH_DATA + texturePath) : null;

            string sizeX = null, sizeY = null;
            if (containerElement.TryGetProperty("size", out var sz))
            {
                if (sz.TryGetProperty("x", out var sx)) sizeX = sx.GetRawText();
                if (sz.TryGetProperty("y", out var sy)) sizeY = sy.GetRawText();
            }
            Vector2 size = texture != null ? this.ParseSize(sizeX, sizeY, texture) : Vector2.One;

            int zOrder = 0;
            if (containerElement.TryGetProperty("zorder", out var zo)) int.TryParse(zo.GetRawText(), out zOrder);

            bool draggable = false;
            if (containerElement.TryGetProperty("draggable", out var drag)) bool.TryParse(drag.GetRawText(), out draggable);

            bool visible = true;
            if (containerElement.TryGetProperty("visible", out var vis)) bool.TryParse(vis.GetRawText(), out visible);

            var container = new WidgetContainer(texture)
            {
                Position = position,
                Origin = originVec,
                ZOrder = zOrder,
                Draggable = draggable,
                Visible = visible,
                Size = size
            };

            if (containerElement.TryGetProperty("widgets", out var widgets))
                this.LoadWidgets(widgets, fonts, content, container);

            parent.AddWidget(container, containerName);
        }

        private void LoadPictureFromJSON(JsonElement picElement, ContentManager content, WidgetCollection parent)
        {
            string picName = picElement.GetProperty("name").GetString();
            string texturePath = picElement.GetProperty("texture").GetString();

            string posX = null, posY = null;
            if (picElement.TryGetProperty("position", out var pos))
            {
                if (pos.TryGetProperty("x", out var px)) posX = px.GetRawText();
                if (pos.TryGetProperty("y", out var py)) posY = py.GetRawText();
            }
            var position = parent.ParsePosition(posX, posY);

            float originX = 0, originY = 0;
            if (picElement.TryGetProperty("origin", out var origin))
            {
                if (origin.TryGetProperty("x", out var ox)) float.TryParse(ox.GetRawText(), out originX);
                if (origin.TryGetProperty("y", out var oy)) float.TryParse(oy.GetRawText(), out originY);
            }
            Vector2 originVec = new Vector2(originX, originY);

            Texture2D texture = content.LoadTexture2D(Constants.FILEPATH_DATA + texturePath);

            int zOrder = 0;
            if (picElement.TryGetProperty("zorder", out var zo)) int.TryParse(zo.GetRawText(), out zOrder);

            DisplayWidgetMode displayMode = DisplayWidgetMode.Normal;
            if (picElement.TryGetProperty("display", out var disp))
                Enum.TryParse(disp.GetString(), out displayMode);

            Vector2 scale = Vector2.One;
            if (displayMode == DisplayWidgetMode.Stretch)
            {
                if (parent is WidgetContainer container)
                {
                    scale = container.Size / new Vector2(texture.Width, texture.Height);
                }
                else
                {
                    scale = new Vector2(Settings.ResolutionX, Settings.ResolutionY) / new Vector2(texture.Width, texture.Height);
                }
            }

            bool visible = true;
            if (picElement.TryGetProperty("visible", out var vis)) bool.TryParse(vis.GetRawText(), out visible);

            var pic = new Picture(texture)
            {
                Position = position,
                Origin = originVec,
                ZOrder = zOrder,
                Visible = visible,
                DisplayMode = displayMode,
                Scale = scale
            };

            parent.AddWidget(pic, picName);
        }

        private void LoadCheckboxFromJSON(JsonElement chkElement, Dictionary<string, SpriteFont> fonts, ContentManager content, WidgetCollection parent)
        {
            string chkBoxName = chkElement.GetProperty("name").GetString();
            string checkedTexturePath = chkElement.TryGetProperty("checkedTexture", out var ct) ? ct.GetString() : chkElement.GetProperty("texture").GetString();
            string uncheckedTexturePath = chkElement.TryGetProperty("uncheckedTexture", out var ut) ? ut.GetString() : chkElement.GetProperty("texture").GetString();
            string fontName = chkElement.TryGetProperty("font", out var fn) ? fn.GetString() : null;

            string posX = null, posY = null;
            if (chkElement.TryGetProperty("position", out var pos))
            {
                if (pos.TryGetProperty("x", out var px)) posX = px.GetRawText();
                if (pos.TryGetProperty("y", out var py)) posY = py.GetRawText();
            }
            var position = parent.ParsePosition(posX, posY);

            Texture2D checkedTexture = content.LoadTexture2D(Constants.FILEPATH_DATA + checkedTexturePath);
            Texture2D uncheckedTexture = content.LoadTexture2D(Constants.FILEPATH_DATA + uncheckedTexturePath);

            int zOrder = 0;
            if (chkElement.TryGetProperty("zorder", out var zo)) int.TryParse(zo.GetRawText(), out zOrder);

            bool visible = true;
            if (chkElement.TryGetProperty("visible", out var vis)) bool.TryParse(vis.GetRawText(), out visible);

            var chkBox = new Checkbox(checkedTexture, uncheckedTexture)
            {
                Position = position,
                ZOrder = zOrder,
                Visible = visible
            };

            parent.AddWidget(chkBox, chkBoxName);
        }

        private void LoadLabelFromJSON(JsonElement lblElement, Dictionary<string, SpriteFont> fonts, WidgetCollection parent)
        {
            string lblName = lblElement.GetProperty("name").GetString();
            string text = lblElement.TryGetProperty("text", out var t) ? t.GetString() : "";
            string fontName = lblElement.GetProperty("font").GetString();
            uint charSize = 0;
            if (lblElement.TryGetProperty("fontsize", out var fs)) uint.TryParse(fs.GetRawText(), out charSize);

            var color = lblElement.TryGetProperty("color", out var colorEl) ? this.ParseColor(colorEl) : Color.White;

            string posX = null, posY = null;
            if (lblElement.TryGetProperty("position", out var pos))
            {
                if (pos.TryGetProperty("x", out var px)) posX = px.GetRawText();
                if (pos.TryGetProperty("y", out var py)) posY = py.GetRawText();
            }
            var position = parent.ParsePosition(posX, posY);

            int zOrder = 0;
            if (lblElement.TryGetProperty("zorder", out var zo)) int.TryParse(zo.GetRawText(), out zOrder);

            bool visible = true;
            if (lblElement.TryGetProperty("visible", out var vis)) bool.TryParse(vis.GetRawText(), out visible);

            SpriteFont font = fonts[fontName];
            var label = new Label(font)
            {
                Text = text,
                Position = position,
                Color = color,
                ZOrder = zOrder,
                Visible = visible
            };

            parent.AddWidget(label, lblName);
        }

        private void LoadButtonFromJSON(JsonElement buttonElement, Dictionary<string, SpriteFont> fonts, ContentManager content, WidgetCollection parent)
        {
            string btnName = buttonElement.GetProperty("name").GetString();
            string text = buttonElement.TryGetProperty("text", out var t) ? t.GetString() : "";
            string texturePath = buttonElement.GetProperty("texture").GetString();
            string fontName = buttonElement.GetProperty("font").GetString();
            uint charSize = 0;
            if (buttonElement.TryGetProperty("fontsize", out var fs)) uint.TryParse(fs.GetRawText(), out charSize);

            string posX = null, posY = null;
            if (buttonElement.TryGetProperty("position", out var pos))
            {
                if (pos.TryGetProperty("x", out var px)) posX = px.GetRawText();
                if (pos.TryGetProperty("y", out var py)) posY = py.GetRawText();
            }
            var position = parent.ParsePosition(posX, posY);

            Texture2D texture = content.LoadTexture2D(Constants.FILEPATH_DATA + texturePath);
            SpriteFont font = fonts[fontName];

            int zOrder = 0;
            if (buttonElement.TryGetProperty("zorder", out var zo)) int.TryParse(zo.GetRawText(), out zOrder);

            bool visible = true;
            if (buttonElement.TryGetProperty("visible", out var vis)) bool.TryParse(vis.GetRawText(), out visible);

            var button = new Button(texture, text, font, charSize)
            {
                Position = position,
                ZOrder = zOrder,
                Visible = visible
            };

            parent.AddWidget(button, btnName);
        }

        private string GetJsonString(JsonElement element, string property, string defaultValue = null)
        {
            return element.TryGetProperty(property, out var prop) ? prop.GetString() : defaultValue;
        }

        private int GetJsonInt(JsonElement element, string property, int defaultValue = 0)
        {
            if (element.TryGetProperty(property, out var prop))
                int.TryParse(prop.GetRawText(), out int val);
            return defaultValue;
        }

        private bool GetJsonBool(JsonElement element, string property, bool defaultValue = true)
        {
            if (element.TryGetProperty(property, out var prop))
                bool.TryParse(prop.GetRawText(), out bool val);
            return defaultValue;
        }

        private Color ParseColor(JsonElement colorElement)
        {
            if (colorElement.ValueKind == JsonValueKind.Null || colorElement.ValueKind == JsonValueKind.Undefined)
                return Color.White;

            if (colorElement.ValueKind == JsonValueKind.String)
            {
                string colorName = colorElement.GetString();
                var colorType = typeof(Color).GetProperty(colorName,
                    BindingFlags.Static | BindingFlags.Public);

                if (colorType != null)
                {
                    Color color = new Color();
                    return (Color)colorType.GetValue(color, null);
                }
            }
            else if (colorElement.ValueKind == JsonValueKind.Object)
            {
                float.TryParse(colorElement.TryGetProperty("r", out var r) ? r.GetRawText() : "1", out float rVal);
                float.TryParse(colorElement.TryGetProperty("g", out var g) ? g.GetRawText() : "1", out float gVal);
                float.TryParse(colorElement.TryGetProperty("b", out var b) ? b.GetRawText() : "1", out float bVal);
                float.TryParse(colorElement.TryGetProperty("a", out var a) ? a.GetRawText() : "1", out float aVal);
                return new Color(new Vector4(rVal, gVal, bVal, aVal));
            }

            return Color.White;
        }

        protected virtual Vector2 ParseSize(string sizeX, string sizeY, Texture2D texture)
        {
            float x = 0;
            float y = 0;

            if (string.IsNullOrEmpty(sizeX))
            {
                x = texture.Width;
            }
            else if (sizeX.Contains("%"))
            {
                float.TryParse(sizeX.Replace("%", ""), out float pX);
                x = texture.Width * (pX / 100f);
            }
            else
            {
                float.TryParse(sizeX, out x);
            }

            if (string.IsNullOrEmpty(sizeY))
            {
                y = texture.Height;
            }
            else if (sizeY.Contains("%"))
            {
                float.TryParse(sizeY.Replace("%", ""), out float pY);
                y = texture.Height * (pY / 100f);
            }
            else
            {
                float.TryParse(sizeY, out y);
            }

            return new Vector2(x, y);
        }

    }
}