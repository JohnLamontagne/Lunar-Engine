/** Copyright 2018 John Lamontagne https://www.mmorpgcreation.com

	Licensed under the Apache License, Version 2.0 (the "License");
	you may not use this file except in compliance with the License.
	You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

	Unless required by applicable law or agreed to in writing, software
	distributed under the License is distributed on an "AS IS" BASIS,
	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	See the License for the specific language governing permissions and
	limitations under the License.
*/
using System;
using System.Windows.Forms;
using DarkUI.Forms;
using Lunar.Editor.World;

namespace Lunar.Editor.Controls
{
    public partial class WarpAttributeDialog : DarkDialog
    {
        private Map _mapSubject;
        private Form _parentForm;

        /// <summary>
        /// Map selected to place attribute on
        /// </summary>
        public Map MapSubject { get; private set; }

        public int WarpX
        {
            get => Convert.ToInt32(txtWarpX.Text);
            set => txtWarpX.Text = value.ToString();
        }

        public int WarpY
        {
            get => Convert.ToInt32(txtWarpY.Text);
            set => txtWarpY.Text = value.ToString();
        }

        public string WarpMapID
        {
            get => txtMapID.Text;
            set => txtMapID.Text = value;
        }

        public string WarpLayerName
        {
            get => this.txtWarpLayer.Text;
            set => this.txtWarpLayer.Text = value;
        }

        public WarpAttributeDialog(Form parentForm, Map mapSubject)
        {
            _parentForm = parentForm;

            this.btnOk.Click += BtnOk_Click;

            InitializeComponent();
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            this.Submitted?.Invoke(this, new EventArgs());
            this.Close();
        }

        private void btnSelectTile_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.SelectTile?.Invoke(this, new EventArgs());
        }

        public event EventHandler<EventArgs> Submitted;
        public event EventHandler<EventArgs> SelectTile;
    }
}
