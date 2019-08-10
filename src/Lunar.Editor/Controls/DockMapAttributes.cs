using System;
using DarkUI.Controls;
using DarkUI.Docking;
using Lunar.Core.World.Structure.Attribute;
using Lunar.Editor.World;

namespace Lunar.Editor.Controls
{
    public partial class DockMapAttributes : DarkToolWindow
    {
        public TileAttribute Attribute { get; private set; }

        private WarpAttributeDialog _tileAttributeDialog;
        private NPCSpawnDialog _npcSpawnAttributeDialog;
        private StartDialogueDialog _startDialogueAttributeDialog;

        public Map MapSubject { get; private set; }

        public Project Project { get; private set; }

        public DockMapAttributes()
        {
            InitializeComponent();

            this.Attribute = null;
        }

        public void SetProject(Project project)
        {
            this.Project = project;
        }

        public void SetMapSubject(Map map)
        {
            this.MapSubject = map;
        }

        private void btnNone_CheckedChanged(object sender, EventArgs e)
        {
            if (!((DarkRadioButton)sender).Checked)
                return;

            this.Attribute = null;
        }

        private void btnBlocked_CheckedChanged(object sender, System.EventArgs e)
        {
            if (!((DarkRadioButton)sender).Checked)
                return;

            this.Attribute = new BlockedTileAttribute();
        }

        private void btnPlayerSpawn_CheckedChanged(object sender, EventArgs e)
        {
            if (!((DarkRadioButton)sender).Checked)
                return;

            this.Attribute = new PlayerSpawnTileAttribute();
        }

        private void btnWarp_CheckedChanged(object sender, EventArgs e)
        {
            if (!((DarkRadioButton)sender).Checked)
                return;

            _tileAttributeDialog = new WarpAttributeDialog(this.ParentForm, this.MapSubject);
            _tileAttributeDialog.SelectTile += (o, args) => this.SelectingTile?.Invoke(o, args);
            _tileAttributeDialog.Submitted += WarpDialog_Submitted;
            _tileAttributeDialog.Show(this.ParentForm);
        }

        private void btnNPCSpawn_CheckedChanged(object sender, EventArgs e)
        {
            if (!((DarkRadioButton)sender).Checked)
                return;

            _npcSpawnAttributeDialog = new NPCSpawnDialog(this.ParentForm, this.Project);
            _npcSpawnAttributeDialog.SelectTile += (o, args) => this.SelectingTile?.Invoke(o, args);
            _npcSpawnAttributeDialog.Submitted += NpcSpawnAttributeDialogOnSubmitted;
            _npcSpawnAttributeDialog.Show(this.ParentForm);
        }

        private void RadioDialogueInit_CheckedChanged(object sender, EventArgs e)
        {
            if (!((DarkRadioButton)sender).Checked)
                return;

            _startDialogueAttributeDialog = new StartDialogueDialog(this.ParentForm, this.Project);
            _startDialogueAttributeDialog.Submitted += _startDialogueAttributeDialog_Submitted;
            _startDialogueAttributeDialog.Show(this.ParentForm);
        }

        private void _startDialogueAttributeDialog_Submitted(object sender, EventArgs e)
        {
            this.Attribute = new StartDialogueTileAttribute(_startDialogueAttributeDialog.Dialogue, _startDialogueAttributeDialog.Branch);
        }

        private void NpcSpawnAttributeDialogOnSubmitted(object sender, EventArgs e)
        {
            this.Attribute = new NPCSpawnTileAttribute(_npcSpawnAttributeDialog.NPC, _npcSpawnAttributeDialog.RespawnTime, _npcSpawnAttributeDialog.MaxSpawns);
        }

        private void WarpDialog_Submitted(object sender, EventArgs e)
        {
            this.Attribute = new WarpTileAttribute(_tileAttributeDialog.WarpX, _tileAttributeDialog.WarpY, _tileAttributeDialog.WarpMapID, _tileAttributeDialog.WarpLayerName);
        }

        public event EventHandler<EventArgs> SelectingTile;

        private void NPCSpawnDialog_Submitted(object sender, EventArgs e)
        {
            this.Attribute = new WarpTileAttribute(_tileAttributeDialog.WarpX, _tileAttributeDialog.WarpY, _tileAttributeDialog.WarpMapID, _tileAttributeDialog.WarpLayerName);
        }
    }
}