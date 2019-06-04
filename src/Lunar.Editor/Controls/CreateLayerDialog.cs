using DarkUI.Forms;

namespace Lunar.Editor.Controls
{
    public partial class CreateLayerDialog : DarkDialog
    {
        public string LayerName => this.txtLayer.Text;

        public CreateLayerDialog()
        {
            InitializeComponent();
        }
    }
}
