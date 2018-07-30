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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lunar.Client.GUI.Widgets
{
    public interface IWidget
    {
        int ZOrder { get; set; }

        bool Visible { get; set; }

        bool Active { get; set; }

        bool Selectable { get; set; }

        Vector2 Position { get; set; }

        Vector2 Origin { get; set; }

        string Tag { get; set; }

        void Update(GameTime gameTime);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="widgetCount">Number of widgets in the passing GUIManager instance</param>
        void Draw(SpriteBatch spriteBatch, int widgetCount);

        bool Contains(Point point);

        /// <summary>
        /// Binds this widget's position relative to that of the specified one.
        /// </summary>
        /// <param name="widget"></param>
        void BindTo(IWidget widget);

        void OnMouseHover(MouseState mouseState);

        void OnLeftMouseDown(MouseState mouseState);

        void OnRightMouseDown(MouseState mouseState);

        event EventHandler<WidgetClickedEventArgs> Clicked;

        event EventHandler Mouse_Hover;
    }
}