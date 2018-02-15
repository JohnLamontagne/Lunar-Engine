using System;

namespace Lunar.Client.GUI.Widgets
{
    public class WidgetClickedEventArgs : EventArgs
    {
        private readonly MouseButtons _mouseButton;

        public MouseButtons MouseButton { get { return _mouseButton; } }

        public WidgetClickedEventArgs(MouseButtons mouseButton)
        {
            _mouseButton = mouseButton;
        }
    }

    public enum MouseButtons
    {
        Left,
        Right
    }
}
