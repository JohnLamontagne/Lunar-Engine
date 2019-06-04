using System;
using System.Windows.Forms;
using DarkUI.Forms;

namespace Lunar.Editor.Controls
{
    public partial class CreateProjectDialog : DarkDialog
    {
        public string ServerDataPath => this.txtServerDataDir.Text;
        public string ClientDataPath => this.txtClientDataDir.Text;

        public CreateProjectDialog()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            var folderBrowser = new FolderBrowserDialog();

            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                this.txtServerDataDir.Text = folderBrowser.SelectedPath;
            }
        }

        private void btnBrowseClient_Click(object sender, EventArgs e)
        {
            var folderBrowser = new FolderBrowserDialog();

            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                this.txtClientDataDir.Text = folderBrowser.SelectedPath;
            }
        }
    }
}
