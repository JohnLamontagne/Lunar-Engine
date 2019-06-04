using DarkUI.Forms;

namespace Lunar.Editor.Controls
{
    public partial class CreateScriptDialog : DarkDialog
    {
        public string ScriptName => this.txtScriptName.Text;

        public CreateScriptDialog()
        {
            InitializeComponent();
        }
    }
}
