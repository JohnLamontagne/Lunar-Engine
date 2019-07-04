using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
