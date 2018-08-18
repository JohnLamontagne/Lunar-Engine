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
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Lunar.Client.GUI.Widgets;
using Lunar.Client.Utilities.Services;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Content;
using Lunar.Client.Utilities;
using DisplayMode = Microsoft.Xna.Framework.Graphics.DisplayMode;

namespace Lunar.Client.GUI
{
    public class GUIManager
    {
        protected Dictionary<string, IWidget> _widgets;

        private IWidget _activeWidget;

        private readonly RenderTarget2D _renderTarget;

        public GUIManager()
        {
            _widgets = new Dictionary<string, IWidget>();

            var graphicsDevice = Client.ServiceLocator.GetService<GraphicsDeviceService>().GraphicsDevice;

            var pp = graphicsDevice.PresentationParameters;
            _renderTarget = new RenderTarget2D(graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight,
                false, pp.BackBufferFormat, pp.DepthStencilFormat, 0, RenderTargetUsage.PreserveContents);
        }

        public virtual void AddWidget(IWidget widget, string name)
        {
            _widgets.Add(name, widget);
        }

        public virtual void RemoveWidgets<T>() where T : IWidget
        {
            // Create a new dictionary without the specified elements.
            _widgets = (from pair in _widgets
                        where !(pair.Value is T)
                        select pair).ToDictionary(pair => pair.Key,
                                               pair => pair.Value);
        }

        public T GetWidget<T>(string id) where T : IWidget
        {
            IWidget value = _widgets[id];

            if (value.GetType() == typeof(T))
            {
                return (T)value;
            }

            return default(T);
        }

        public bool WidgetExists(string id)
        {
            return _widgets.TryGetValue(id, out IWidget entry);
        }

        public IEnumerable<T> GetWidgets<T>() where T : IWidget
        {
            return from widget in _widgets.Values
                   where widget is T
                   select (T)widget;
        }

        public Dictionary<string, IWidget> GetWidgetEntries()
        {
            return _widgets;
        }

        public void RemoveWidget(string id)
        {
            _widgets.Remove(id);
        }

        public void ClearWidgets()
        {
            _activeWidget = null;
            _widgets.Clear();
        }

        public virtual void Update(GameTime gameTime)
        {
           
            var mouseState = Mouse.GetState();
            IWidget oldActive = _activeWidget;

            // Process input for the active widget first.
            if (_activeWidget != null)
            {
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    if (_activeWidget.Contains(mouseState.Position))
                    {
                        _activeWidget.OnLeftMouseDown(mouseState);
                    }
                    else
                    {
                        Console.WriteLine("{0} no longer active!", _activeWidget.GetType().ToString());

                        oldActive = _activeWidget;
                        _activeWidget.Active = false;
                        _activeWidget = null;
                    }
                }
                else if (mouseState.RightButton == ButtonState.Pressed)
                {
                    if (_activeWidget.Contains(mouseState.Position))
                    {
                        _activeWidget.OnRightMouseDown(mouseState);
                    }
                }
                else
                {
                    if (_activeWidget.Contains(mouseState.Position))
                    {
                        _activeWidget.OnMouseHover(mouseState);
                    }
                }
            }

            foreach (var widget in _widgets.Values)
            {
                if (widget == _activeWidget)
                    continue;

                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    if (widget.Contains(mouseState.Position) && (_activeWidget == null || !_activeWidget.Contains(mouseState.Position)))
                    {
                        widget.OnLeftMouseDown(mouseState);

                        if (widget.Selectable)
                        {
                            widget.Active = true;


                            if (_activeWidget != null)
                            {
                                _activeWidget.Active = false;
                            }

                            _activeWidget = widget;
                                

                            Console.WriteLine("{0} now active!", widget.GetType().ToString());

                            break;
                        }
                    }
                    else
                    {
                        if (_activeWidget == widget)
                        {
                            widget.Active = false;
                            oldActive = _activeWidget;
                            _activeWidget = null;

                            Console.WriteLine("{0} no longer active!", widget.GetType().ToString());
                        }
                    }
                }
                else if (mouseState.RightButton== ButtonState.Pressed)
                {
                    if (widget.Contains(mouseState.Position) && (_activeWidget == null || !_activeWidget.Contains(mouseState.Position)))
                    {
                        widget.OnRightMouseDown(mouseState);
                    }
                }
                else
                {
                    if (widget.Contains(mouseState.Position) && (_activeWidget == null || !_activeWidget.Contains(mouseState.Position)))
                    {
                        widget.OnMouseHover(mouseState);
                    }
                }
            }
            

            if (_activeWidget == null && oldActive != null)
            {
                _activeWidget = oldActive;
                _activeWidget.Active = true;
            }

            foreach (var widget in _widgets.Values)
            {
                widget.Update(gameTime);
            }
        }

        public virtual void Begin(SpriteBatch spriteBatch)
        {
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
        }

        public virtual void End(SpriteBatch spriteBatch)
        {
            spriteBatch.End();

            Client.ServiceLocator.GetService<GraphicsDeviceService>().GraphicsDevice.SetRenderTarget(null);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            spriteBatch.Draw(_renderTarget, Vector2.Zero, Color.White);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
           
            foreach (var widget in _widgets.Values)
            {
                if (widget != _activeWidget)
                    widget.Draw(spriteBatch, _widgets.Count);
            }

            // Active widget is always on top.
            _activeWidget?.Draw(spriteBatch, _widgets.Count);
        }

        public void LoadFromFile(string filePath, ContentManager content)
        {
            var doc = XDocument.Load(filePath);

            var fontEntries = doc.Elements("GUI").Elements("Fonts").Elements("font");

            var fonts = new Dictionary<string, SpriteFont>();

            foreach (var fontEntry in fontEntries)
            {
                var font = content.Load<SpriteFont>(Constants.FILEPATH_DATA + fontEntry.Value.ToString());
                fonts.Add(fontEntry.Attribute("name")?.Value.ToString(), font);
            }

            var widgetEntries = doc.Element("GUI")?.Element("Widgets");

            this.LoadWidgets(widgetEntries, fonts, content, this);
        }

        private void LoadWidgetsFromFileImport(string filePath, Dictionary<string, SpriteFont> fonts,
            ContentManager content, GUIManager parent)
        {
            var doc = XDocument.Load(filePath);

            var widgetEntries = doc.Element("Widgets");

            this.LoadWidgets(widgetEntries, fonts, content, parent);
        }

        private void LoadWidgets(XElement widgetEntries, Dictionary<string, SpriteFont> fonts, ContentManager content, GUIManager parent)
        {
            if (widgetEntries == null)
                return;

            foreach (var importElement in widgetEntries.Elements("import"))
            {
                this.LoadWidgetsFromFileImport(Constants.FILEPATH_DATA + importElement.Attribute("file")?.Value.ToString(), fonts, content, parent);
            }

            foreach (var buttonElement in widgetEntries.Elements("button"))
            {
                this.LoadButtonFromXML(buttonElement, fonts, content, parent);
            }

            foreach (var labelElement in widgetEntries.Elements("label"))
            {
                this.LoadLabelFromXML(labelElement, fonts, parent);
            }

            foreach (var sbElement in widgetEntries.Elements("statusbar"))
            {
                this.LoadStatusBarFromXML(sbElement, fonts, content, parent);
            }

            foreach (var checkboxElement in widgetEntries.Elements("checkbox"))
            {
                this.LoadCheckboxFromXML(checkboxElement, fonts, content, parent);
            }

            foreach (var picElement in widgetEntries.Elements("picture"))
            {
                this.LoadPictureFromXML(picElement, content, parent);
            }

            foreach (var containerElement in widgetEntries.Elements("container"))
            {
                this.LoadWidgetContainerFromXML(containerElement, fonts, content, parent);
            }

            foreach (var textboxElement in widgetEntries.Elements("textbox"))
            {
                this.LoadTextboxFromXML(textboxElement, fonts, content, parent);
            }

            foreach (var chatboxElement in widgetEntries.Elements("chatbox"))
            {
                this.LoadChatboxFromXML(chatboxElement, fonts, content, parent);
            }
        }

        private void LoadChatboxFromXML(XElement chatboxElement, Dictionary<string, SpriteFont> fonts, ContentManager content, GUIManager parent)
        {
            string chatboxName = chatboxElement.Attribute("name")?.Value.ToString();

            string texturePath = chatboxElement.Element("texture")?.Value.ToString();
            string fontName = chatboxElement.Element("font")?.Value.ToString();

            int.TryParse(chatboxElement.Element("padding")?.Element("x")?.Value.ToString(), out int offX);
            int.TryParse(chatboxElement.Element("padding")?.Element("y")?.Value.ToString(), out int offY);

            int.TryParse(chatboxElement.Element("maxlines")?.Value.ToString(), out int maxLines);

            int.TryParse(chatboxElement.Element("zorder")?.Value.ToString(), out int zOrder);

            var texture = content.LoadTexture2D(Constants.FILEPATH_DATA + texturePath);

            var position = parent.ParsePosition(chatboxElement.Element("position")?.Element("x")?.Value.ToString(),
                chatboxElement.Element("position")?.Element("y")?.Value.ToString());

            if (!bool.TryParse(chatboxElement.Element("visible")?.Value, out bool visible))
            {
                visible = true;
            }

            SpriteFont font = fonts[fontName];
            var chatBox = new Chatbox(texture, font, maxLines)
            {
                Position = position,
                ChatOffset = new Vector2(offX, offY),
                ZOrder = zOrder,
                Visible = visible
            };

            parent.AddWidget(chatBox, chatboxName);
        }

        private void LoadStatusBarFromXML(XElement sbElement, Dictionary<string, SpriteFont> fonts, ContentManager content, GUIManager parent)
        {
            string sbName = sbElement.Attribute("name")?.Value.ToString();

            string text = sbElement.Element("text")?.Value.ToString() ?? "";
            string fontName = sbElement.Element("font")?.Value.ToString();
            uint.TryParse(sbElement.Element("fontsize")?.Value.ToString(), out uint charSize);

            string texturePath = sbElement.Element("backSprite").Value.ToString();
            string texturePath2 = sbElement.Element("fillSprite").Value.ToString();

            var color = this.ParseColor(sbElement.Element("color"));

            var position = parent.ParsePosition(sbElement.Element("position")?.Element("x")?.Value.ToString(),
                                              sbElement.Element("position")?.Element("y")?.Value.ToString());

            var fillPosition = parent.ParsePosition(sbElement.Element("fillPosition")?.Element("x")?.Value.ToString(),
                                              sbElement.Element("fillPosition")?.Element("y")?.Value.ToString());

            int.TryParse(sbElement.Element("padding")?.Element("x")?.Value.ToString(), out int offX);
            int.TryParse(sbElement.Element("padding")?.Element("y")?.Value.ToString(), out int offY);

            int.TryParse(sbElement.Element("zorder")?.Value.ToString(), out int zOrder);

            Texture2D backSprite = content.LoadTexture2D(Constants.FILEPATH_DATA + texturePath);
            Texture2D fillSprite = content.LoadTexture2D(Constants.FILEPATH_DATA + texturePath2);

            if (!bool.TryParse(sbElement.Element("visible")?.Value, out bool visible))
            {
                visible = true;
            }

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

        private void LoadTextboxFromXML(XElement textboxElement, Dictionary<string, SpriteFont> fonts,
            ContentManager content, GUIManager parent)
        {
            string textboxName = textboxElement.Attribute("name")?.Value.ToString();

            string text = textboxElement.Element("text")?.Value.ToString() ?? "";
            string texturePath = textboxElement.Element("texture")?.Value.ToString();
            string fontName = textboxElement.Element("font")?.Value.ToString();

            uint.TryParse(textboxElement.Element("fontsize")?.Value.ToString(), out uint charSize);

            int.TryParse(textboxElement.Element("padding")?.Element("x")?.Value.ToString(), out int offX);
            int.TryParse(textboxElement.Element("padding")?.Element("y")?.Value.ToString(), out int offY);
            Vector2 textOffset = new Vector2(offX, offY);

            float.TryParse(textboxElement.Element("scale")?.Element("x")?.Value.ToString(), out float scaleX);
            float.TryParse(textboxElement.Element("scale")?.Element("y")?.Value.ToString(), out float scaleY);

            if (scaleX <= 0)
                scaleX = 1;
            if (scaleY <= 0)
                scaleY = 1;

            Vector2 scale = new Vector2(scaleX, scaleY);

            var color = this.ParseColor(textboxElement.Element("color"));

            float.TryParse(textboxElement.Element("origin")?.Element("x")?.Value.ToString(), out float originX);
            float.TryParse(textboxElement.Element("origin")?.Element("y")?.Value.ToString(), out float originY);
            Vector2 origin = new Vector2(originX, originY);

            string mask = textboxElement.Element("mask")?.Value.ToString() ?? null;

            var texture = content.LoadTexture2D(Constants.FILEPATH_DATA + texturePath);
           
            
            var position = parent.ParsePosition(textboxElement.Element("position")?.Element("x")?.Value.ToString(),
                textboxElement.Element("position")?.Element("y")?.Value.ToString());

            int.TryParse(textboxElement.Element("zorder")?.Value.ToString(), out int zOrder);

            if (!bool.TryParse(textboxElement.Element("visible")?.Value, out bool visible))
            {
                visible = true;
            }

            SpriteFont font = fonts[fontName];
            var textBox = new Textbox(texture, font, textOffset, charSize)
            {
                Text = text,
                Position = position,
                ForeColor = color,
                Origin = origin,
                Mask = mask,
                ZOrder = zOrder,
                Visible = visible,
                Scale = scale,
            };

            parent.AddWidget(textBox, textboxName);
        }

        private void LoadWidgetContainerFromXML(XElement containerElement, Dictionary<string, SpriteFont> fonts, ContentManager content, GUIManager parent)
        {
            string containerName = containerElement.Attribute("name")?.Value.ToString();

            string texturePath = containerElement.Element("texture")?.Value.ToString();

            var position = parent.ParsePosition(containerElement.Element("position")?.Element("x")?.Value.ToString(),
                containerElement.Element("position")?.Element("y")?.Value.ToString());

            float.TryParse(containerElement.Element("origin")?.Element("x")?.Value.ToString(), out float originX);
            float.TryParse(containerElement.Element("origin")?.Element("y")?.Value.ToString(), out float originY);
            Vector2 origin = new Vector2(originX, originY);

            Texture2D texture = content.LoadTexture2D(Constants.FILEPATH_DATA + texturePath);

            int.TryParse(containerElement.Element("zorder")?.Value.ToString(), out int zOrder);

            bool.TryParse(containerElement.Element("draggable")?.Value, out bool draggable);

            if (!bool.TryParse(containerElement.Element("visible")?.Value, out bool visible))
            {
                visible = true;
            }

            var container = new WidgetContainer(texture)
            {
                Position = position,
                Origin = origin,
                ZOrder = zOrder,
                Draggable = draggable,
                Visible = visible
            };

            // load its children if it has them
            this.LoadWidgets(containerElement.Element("Widgets"), fonts, content, container);

            parent.AddWidget(container, containerName);
        }

        private void LoadPictureFromXML(XElement picElement, ContentManager content, GUIManager parent)
        {
            string picName = picElement.Attribute("name")?.Value.ToString();

            string texturePath = picElement.Element("texture")?.Value.ToString();

            var position = parent.ParsePosition(picElement.Element("position")?.Element("x")?.Value.ToString(),
                picElement.Element("position")?.Element("y")?.Value.ToString());

            float.TryParse(picElement.Element("origin")?.Element("x")?.Value.ToString(), out float originX);
            float.TryParse(picElement.Element("origin")?.Element("y")?.Value.ToString(), out float originY);
            Vector2 origin = new Vector2(originX, originY);

            Texture2D texture = content.LoadTexture2D(Constants.FILEPATH_DATA + texturePath);

            int.TryParse(picElement.Element("zorder")?.Value.ToString(), out int zOrder);

            Enum.TryParse(picElement.Element("display")?.Value.ToString(), out DisplayWidgetMode displayMode);

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

            if (!bool.TryParse(picElement.Element("visible")?.Value, out bool visible))
            {
                visible = true;
            }

            var pic = new Picture(texture)
            {
                Position = position,
                Origin = origin,
                ZOrder = zOrder,
                Visible = visible,
                DisplayMode = displayMode,
                Scale = scale
            };

            parent.AddWidget(pic, picName);
        }

        private void LoadCheckboxFromXML(XElement chkElement, Dictionary<string, SpriteFont> fonts, ContentManager content, GUIManager parent)
        {
            string chkBoxName = chkElement.Attribute("name")?.Value.ToString();

            string checkedTexturePath = chkElement.Element("texture")?.Value.ToString();
            string uncheckedTexturePath = chkElement.Element("texture")?.Value.ToString();
            string fontName = chkElement.Element("font")?.Value.ToString();

            var position = parent.ParsePosition(chkElement.Element("position")?.Element("x")?.Value.ToString(),
                chkElement.Element("position")?.Element("y")?.Value.ToString());

            Texture2D checkedTexture = content.LoadTexture2D(Constants.FILEPATH_DATA + checkedTexturePath);
            Texture2D uncheckedTexture = content.LoadTexture2D(Constants.FILEPATH_DATA + uncheckedTexturePath);
            SpriteFont font = fonts[fontName];

            int.TryParse(chkElement.Element("zorder")?.Value.ToString(), out int zOrder);

            if (!bool.TryParse(chkElement.Element("visible")?.Value, out bool visible))
            {
                visible = true;
            }

            var chkBox = new Checkbox(checkedTexture, uncheckedTexture)
            {
                Position = position,
                ZOrder = zOrder,
                Visible = visible
            };

            parent.AddWidget(chkBox, chkBoxName);

        }

        private void LoadLabelFromXML(XElement lblElement, Dictionary<string, SpriteFont> fonts, GUIManager parent)
        {
            string lblName = lblElement.Attribute("name")?.Value.ToString();

            string text = lblElement.Element("text")?.Value.ToString() ?? "";
            string fontName = lblElement.Element("font")?.Value.ToString();
            uint.TryParse(lblElement.Element("fontsize")?.Value.ToString(), out uint charSize);

            var color = this.ParseColor(lblElement.Element("color"));

            var position = parent.ParsePosition(lblElement.Element("position")?.Element("x")?.Value.ToString(),
                lblElement.Element("position")?.Element("y")?.Value.ToString());

            int.TryParse(lblElement.Element("zorder")?.Value.ToString(), out int zOrder);

            if (!bool.TryParse(lblElement.Element("visible")?.Value, out bool visible))
            {
                visible = true;
            }

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

        private void LoadButtonFromXML(XElement buttonElement, Dictionary<string, SpriteFont> fonts, ContentManager content, GUIManager parent)
        {
            string btnName = buttonElement.Attribute("name")?.Value.ToString();

            string text = buttonElement.Element("text")?.Value.ToString() ?? "";

            string texturePath = buttonElement.Element("texture")?.Value.ToString();
            string fontName = buttonElement.Element("font")?.Value.ToString();
            uint.TryParse(buttonElement.Element("fontsize")?.Value.ToString(), out uint charSize);

            var position = parent.ParsePosition(buttonElement.Element("position")?.Element("x")?.Value.ToString(),
                buttonElement.Element("position")?.Element("y")?.Value.ToString());

            Texture2D texture = content.LoadTexture2D(Constants.FILEPATH_DATA + texturePath);
            
            SpriteFont font = fonts[fontName];

            int.TryParse(buttonElement.Element("zorder")?.Value.ToString(), out int zOrder);

            if (!bool.TryParse(buttonElement.Element("visible")?.Value, out bool visible))
            {
                visible = true;
            }

            var button = new Button(texture, text, font, charSize)
            {
                Position = position,
                ZOrder = zOrder,
                Visible = visible
            };

            parent.AddWidget(button, btnName);
        }

        private Color ParseColor(XElement colorElement)
        {
            if (colorElement?.Value == null)
                return Color.White;

            // Try to get the color if the user has specified a valid one
            var colorType = typeof(Color).GetProperty(colorElement.Value.ToString(),
                BindingFlags.Static | BindingFlags.Public);

            if (colorType != null)
            {
                Color color = new Color();
                return (Color)colorType.GetValue(color, null);
            }
            else
            {
                float.TryParse(colorElement.Element("r")?.Value.ToString(), out float r);
                float.TryParse(colorElement.Element("g")?.Value.ToString(), out float g);
                float.TryParse(colorElement.Element("b")?.Value.ToString(), out float b);
                float.TryParse(colorElement.Element("a")?.Value.ToString(), out float a);
                return new Color(new Vector4(r, g, b, a));
            }
        }

        protected virtual Vector2 ParsePosition(string posX, string posY)
        {
            float x = 0;
            float y = 0;

            if (posX == null)
            {
                x = 0;
            }
            else if (posX.Contains("%"))
            {
                float.TryParse(posX.Replace("%", ""), out float pX);
                x = Settings.ResolutionX * (pX / 100f);
            }
            else
            {
                float.TryParse(posX, out x);
            }

            if (posY == null)
            {
                y = 0;
            }
            else if (posY.Contains("%"))
            {
                float.TryParse(posY.Replace("%", ""), out float pY);
                y = Settings.ResolutionY * (pY / 100f);
            }
            else
            {
                float.TryParse(posY, out y);
            }

            return new Vector2(x, y);
        }
    }
}