using DarkUI.Forms;

namespace Lunar.Editor.Controls
{
    public partial class CreateDirectoryDialog : DarkDialog
    {
        public string DirectoryPath => this.txtDirectory.Text;

        public CreateDirectoryDialog()
        {
            InitializeComponent();
        }
    }
}
