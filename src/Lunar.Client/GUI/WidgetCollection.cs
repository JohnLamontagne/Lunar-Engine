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
using Lunar.Core.Utilities.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lunar.Client.GUI
{
    /// <summary>
    /// Holds a named collection of widgets and dispatches input/draw calls to them.
    /// Both <see cref="GUIManager"/> (the scene-level GUI root) and
    /// <see cref="WidgetContainer"/> (a grouping widget) extend this; neither needs
    /// graphics-device access just to manage children.
    /// </summary>
    public abstract class WidgetCollection
    {
        protected Dictionary<string, IWidget> _widgets;
        private readonly FlexibleStack<IWidget> _orderedWidgets;

        protected WidgetCollection()
        {
            _widgets = new Dictionary<string, IWidget>();
            _orderedWidgets = new FlexibleStack<IWidget>();
        }

        public IWidget ActiveWidget
        {
            get
            {
                var topWidget = _orderedWidgets.Peek();
                return topWidget != null && topWidget.Active ? topWidget : null;
            }
        }

        public virtual void AddWidget(IWidget widget, string name)
        {
            widget.NameChanged += (w, a) =>
            {
                var changedWidget = w as IWidget;
                _widgets.Remove(a.OldName);
                _widgets.Add(changedWidget.Name, changedWidget);
            };

            widget.Activated += (w, a) =>
            {
                if (this.ActiveWidget != null && widget != this.ActiveWidget)
                {
                    Console.WriteLine("Widget {0} no longer active!", this.ActiveWidget.Name);
                    this.ActiveWidget.Active = false;
                }

                _orderedWidgets.Remove(widget);
                _orderedWidgets.Push(widget);

                Console.WriteLine("Widget {0} now active!", widget.Name);
            };

            widget.Name = name;
            _widgets.Add(name, widget);
            _orderedWidgets.Push(widget);
        }

        public virtual void RemoveWidgets<T>() where T : IWidget
        {
            _widgets = (from pair in _widgets
                        where !(pair.Value is T)
                        select pair).ToDictionary(pair => pair.Key, pair => pair.Value);
            _orderedWidgets.Clear();
            _orderedWidgets.Add(_widgets.Values);
        }

        public T GetWidget<T>(string id) where T : IWidget
        {
            _widgets.TryGetValue(id, out IWidget value);
            if (value != null && value.GetType() == typeof(T))
                return (T)value;
            return default(T);
        }

        public bool WidgetExists(string id) => _widgets.ContainsKey(id);

        public IEnumerable<T> GetWidgets<T>() where T : IWidget =>
            _widgets.Values.OfType<T>();

        public Dictionary<string, IWidget> GetWidgetEntries() => _widgets;

        public void RemoveWidget(string id)
        {
            _orderedWidgets.Remove(_widgets[id]);
            _widgets.Remove(id);
        }

        public void RemoveWidget(IWidget widget)
        {
            string key = _widgets.FirstOrDefault(e => e.Value == widget).Key;
            _orderedWidgets.Remove(_widgets[key]);
            _widgets.Remove(key);
        }

        public void ClearWidgets()
        {
            _widgets.Clear();
            _orderedWidgets.Clear();
        }

        public virtual void Update(GameTime gameTime)
        {
            var mouseState = Lunar.Client.Utilities.Input.Input.Mouse;

            for (int i = 0; i < _orderedWidgets.Count; i++)
            {
                var widget = _orderedWidgets[i];

                if (!widget.Visible)
                    continue;

                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    if (widget.Contains(mouseState.Position))
                    {
                        widget.OnLeftMouseDown(mouseState);

                        if (widget == this.ActiveWidget)
                            break;

                        if (widget.Selectable)
                        {
                            widget.Active = true;
                            break;
                        }
                    }
                    else if (this.ActiveWidget == widget)
                    {
                        widget.Active = false;
                        Console.WriteLine("Widget {0} no longer active!", widget.Name);
                    }
                }
                else if (mouseState.RightButton == ButtonState.Pressed)
                {
                    if (widget.Contains(mouseState.Position) && (this.ActiveWidget == null || !this.ActiveWidget.Contains(mouseState.Position)))
                        widget.OnRightMouseDown(mouseState);
                }

                if (widget.Contains(mouseState.Position) && (this.ActiveWidget == null || !this.ActiveWidget.Contains(mouseState.Position)))
                    widget.OnMouseHover(mouseState);
            }

            for (int i = 0; i < _orderedWidgets.Count; i++)
                _orderedWidgets[i].Update(gameTime);
        }

        public virtual void Begin(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
        }

        public virtual void End(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            foreach (var widget in _orderedWidgets.Values.Reverse())
                widget.Draw(spriteBatch, _widgets.Count);
        }

        /// <summary>
        /// Resolves a position string from XML — accepts plain pixels or "n%".
        /// Default base resolves percentages against the screen resolution;
        /// <see cref="WidgetContainer"/> overrides to resolve against its own size.
        /// Public so XML loaders can call it on a <c>WidgetCollection parent</c> reference.
        /// </summary>
        public virtual Vector2 ParsePosition(string posX, string posY)
        {
            float x = 0;
            float y = 0;

            if (string.IsNullOrEmpty(posX))
                x = 0;
            else if (posX.Contains("%"))
            {
                float.TryParse(posX.Replace("%", ""), out float pX);
                x = Settings.ResolutionX * (pX / 100f);
            }
            else
                float.TryParse(posX, out x);

            if (string.IsNullOrEmpty(posY))
                y = 0;
            else if (posY.Contains("%"))
            {
                float.TryParse(posY.Replace("%", ""), out float pY);
                y = Settings.ResolutionY * (pY / 100f);
            }
            else
                float.TryParse(posY, out y);

            return new Vector2(x, y);
        }
    }
}
