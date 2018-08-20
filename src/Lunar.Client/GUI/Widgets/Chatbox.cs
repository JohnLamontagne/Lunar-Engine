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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lunar.Client.GUI.Widgets
{
    public class Chatbox : IWidget
    {
        private readonly WidgetContainer _container;
        private SpriteFont _font;

        public bool Visible { get { return _container.Visible; } set { _container.Visible = value; } }

        public bool Active { get; set; }

        public Vector2 Position { get { return _container.Position; } set { _container.Position = value; } }

        public Vector2 Origin { get; set; }

        public Vector2 ChatOffset { get; set; }

        public bool Selectable { get; set; }

        public string Tag { get; set; }

        public int ZOrder { get; set; }

        public event EventHandler<WidgetClickedEventArgs> Clicked;

        public event EventHandler Mouse_Hover;

        public SpriteFont Font
        {
            get => _font;
            set
            {
                _font = value;

                foreach (var widget in _container.GetWidgets<Label>())
                {
                    widget.Font = value;
                }

                foreach (var widget in _container.GetWidgets<Textbox>())
                {
                    widget.Font = value;
                }

                foreach (var widget in _container.GetWidgets<Button>())
                {
                    widget.Font = value;
                }
            }
        }

        public bool Draggable { get => _container.Draggable;
            set => _container.Draggable = value;
        }

        public int MaxEntries { get; set; }


        public Chatbox(Texture2D backSprite, SpriteFont font, int maxEntries)
        {
            _container = new WidgetContainer(backSprite);
            this.Font = font;
            this.MaxEntries = maxEntries;
            this.Selectable = true;
            this.Origin = Vector2.Zero;
        }

        public void Clear()
        {
            _container.RemoveWidgets<Label>();
        }

        public void AddEntry(string message, Color color)
        {
            var label = new Label(this.Font)
            {
                Text = message
            };
            label.WrapText(_container.Size.X - this.Font.MeasureString("X").X - this.ChatOffset.X);

            label.Color = color;
            label.Position = this.Position + this.ChatOffset;
            label.Visible = true;
            label.ZOrder = this.ZOrder + 1; // Ensure that it is displayed above the chatbox.

            _container.AddWidget(label, "entry" + label.GetHashCode() + Environment.TickCount);

            foreach (var widget in _container.GetWidgets<Label>())
            {
                widget.Position = new Vector2(widget.Position.X, widget.Position.Y - (this.Font.MeasureString(label.Text).Y));
            }

            var toRemove = new List<string>();
            foreach (var entry in _container.GetWidgetEntries())
            {
                if (entry.Value.Position.Y < this.Position.Y + (this.ChatOffset.Y + 1) - (this.MaxEntries * this.Font.MeasureString("X").Y))
                {
                    toRemove.Add(entry.Key);
                }
            }

            foreach (var key in toRemove)
            {
                _container.RemoveWidget(key);
            }
        }



        public void OnMouseHover(MouseState mouseState)
        {

        }

        public void OnLeftMouseDown(MouseState mouseState)
        {
        }

        public void Update(GameTime gameTime)
        {
            _container.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch, int widgetCount)
        {
            _container.Draw(spriteBatch, widgetCount);
        }

        public bool Contains(Point point)
        {
            return _container.Contains(point);
        }

        public void BindTo(IWidget widget)
        {
            throw new NotImplementedException();
        }

        public void OnRightMouseDown(MouseState mouseState)
        {

        }

    }
}