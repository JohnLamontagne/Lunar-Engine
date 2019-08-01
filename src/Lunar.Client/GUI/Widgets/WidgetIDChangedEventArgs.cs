using System;

namespace Lunar.Client.GUI.Widgets
{
    public class WidgetNameChangedEventArgs : EventArgs
    {
        public string OldName { get; }

        public WidgetNameChangedEventArgs(string oldName)
        {
            this.OldName = oldName;
        }
    }
}