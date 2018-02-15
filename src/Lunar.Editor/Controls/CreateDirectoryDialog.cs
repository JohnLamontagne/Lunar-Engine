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
    public partial class CreateDirectoryDialog : DarkDialog
    {
        public string DirectoryPath => this.txtDirectory.Text;

        public CreateDirectoryDialog()
        {
            InitializeComponent();
        }
    }
}
