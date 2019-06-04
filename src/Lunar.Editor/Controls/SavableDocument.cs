using System.Windows.Forms;
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

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Control | Keys.S:
                    this.Save();
                    break;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
