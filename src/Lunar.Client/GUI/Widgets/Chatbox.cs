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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Lunar.Client.GUI.Widgets
{
    public class Chatbox : WidgetContainer, ILexicalWidget
    {
        private SpriteFont _font;

        public Vector2 ChatOffset { get; set; }

        public SpriteFont Font
        {
            get => _font;
            set
            {
                _font = value;

                foreach (var widget in this.GetWidgets<ILexicalWidget>())
                {
                    widget.Font = value;
                }
            }
        }

        public int MaxEntries { get; set; }

        public Chatbox(Texture2D backSprite, SpriteFont font, int maxEntries)
            : base(backSprite)
        {
            this.Font = font;
            this.MaxEntries = maxEntries;
            this.Selectable = true;
            this.Origin = Vector2.Zero;
        }

        public void Clear()
        {
            this.RemoveWidgets<Label>();
        }

        public void AddEntry(string message, Color color)
        {
            var label = new Label(this.Font)
            {
                Text = message
            };
            label.WrapText(this.Size.X - this.Font.MeasureString("X").X - this.ChatOffset.X);

            label.Color = color;
            label.Position = this.Position + this.ChatOffset;
            label.Visible = true;
            label.ZOrder = this.ZOrder + 1; // Ensure that it is displayed above the chatbox.

            this.AddWidget(label, "entry" + label.GetHashCode() + Environment.TickCount);

            foreach (var widget in this.GetWidgets<Label>())
            {
                widget.Position = new Vector2(widget.Position.X, widget.Position.Y - (this.Font.MeasureString(label.Text).Y));
            }

            var toRemove = new List<string>();
            foreach (var entry in this.GetWidgetEntries())
            {
                if (entry.Value.Position.Y < this.Position.Y + (this.ChatOffset.Y + 1) - (this.MaxEntries * this.Font.MeasureString("X").Y))
                {
                    toRemove.Add(entry.Key);
                }
            }

            foreach (var key in toRemove)
            {
                this.RemoveWidget(key);
            }
        }
    }
}