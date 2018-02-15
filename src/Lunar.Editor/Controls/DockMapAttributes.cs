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
