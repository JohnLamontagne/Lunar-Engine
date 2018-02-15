using DarkUI.Docking;
using Lunar.Editor.World;

namespace Lunar.Editor.Controls
{
    public partial class DockMapObjectProperties : DarkToolWindow
    {
        public DockMapObjectProperties()
        {
            InitializeComponent();
        }

        public void SetSubject(MapObjectPropertiesHelper mapObject)
        {
            this.mapObjectProperties.SelectedObject = mapObject;
        }
    }
}
