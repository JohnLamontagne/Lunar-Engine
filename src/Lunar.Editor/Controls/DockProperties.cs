using DarkUI.Docking;

namespace Lunar.Editor.Controls
{
    public partial class DockProperties : DarkToolWindow
    {
        #region Constructor Region

        public DockProperties()
        {
            InitializeComponent();
        }

        #endregion

        public void SetSubject(object obj)
        {
            propertyGrid.SelectedObject = obj;
        }
}
}
