using DarkUI.Docking;

namespace Lunar.Editor.Controls
{
    public partial class SavableDocument : DarkDocument
    {
        public SavableDocument()
        {
            InitializeComponent();
        }

        public virtual void Save()
        {
        }
    }
}
