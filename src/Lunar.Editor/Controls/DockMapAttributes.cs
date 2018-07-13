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
using DarkUI.Controls;
using DarkUI.Docking;
using Lunar.Core.World.Structure;
using Lunar.Editor.World;

namespace Lunar.Editor.Controls
{
    public partial class DockMapAttributes : DarkToolWindow
    {
        public TileAttributes Attribute { get; private set; }
        public AttributeData AttributeData { get; private set; }
        private TileAttributeDialog _tileAttributeDialog;

        public Map MapSubject { get; private set; }

        public DockMapAttributes()
        {
            InitializeComponent();
        }

        public void SetMapSubject(Map map)
        {
            this.MapSubject = map;
        }

        private void btnNone_CheckedChanged(object sender, EventArgs e)
        {
            if (!((DarkRadioButton)sender).Checked)
                return;

            this.Attribute = TileAttributes.None;
            this.AttributeData = new AttributeData(); 
        }

        private void btnBlocked_CheckedChanged(object sender, System.EventArgs e)
        {
            if (!((DarkRadioButton)sender).Checked)
                return;

            this.Attribute = TileAttributes.Blocked;
            this.AttributeData = new AttributeData();
        }

        private void btnPlayerSpawn_CheckedChanged(object sender, System.EventArgs e)
        {
            if (!((DarkRadioButton)sender).Checked)
                return;

            this.Attribute = TileAttributes.PlayerSpawn;
            this.AttributeData = new AttributeData();
        }

        private void btnWarp_CheckedChanged(object sender, System.EventArgs e)
        {
            if (!((DarkRadioButton)sender).Checked)
                return;

            this.Attribute = TileAttributes.Warp;

            _tileAttributeDialog = new TileAttributeDialog(this.ParentForm, this.MapSubject);
            _tileAttributeDialog.SelectTile += (o, args) => this.SelectingTile?.Invoke(o, args);
            _tileAttributeDialog.Show(this.ParentForm);
            _tileAttributeDialog.Submitted += Dialog_Submitted;

        }

        private void Dialog_Submitted(object sender, EventArgs e)
        {
            this.AttributeData = new WarpAttributeData(_tileAttributeDialog.WarpX, _tileAttributeDialog.WarpY, _tileAttributeDialog.WarpMapID, _tileAttributeDialog.WarpLayerName);
        }

        public event EventHandler<EventArgs> SelectingTile;
    }
}
