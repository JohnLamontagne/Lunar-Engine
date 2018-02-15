using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
