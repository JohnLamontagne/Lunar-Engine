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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Lunar.Client.GUI.Widgets;
using Lunar.Client.Utilities.Services;

namespace Lunar.Client.GUI
{
    public class GUIManager
    {
        protected Dictionary<string, IWidget> _widgets;

        private IWidget _activeWidget;

        private RenderTarget2D _renderTarget;

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
            if (_activeWidget != null)
                _activeWidget.Draw(spriteBatch, _widgets.Count);
        }
    }
}