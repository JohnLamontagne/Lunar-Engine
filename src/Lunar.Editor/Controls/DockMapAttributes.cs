using System;
using DarkUI.Controls;
using DarkUI.Docking;
using Lunar.Core.World.Structure;
using Lunar.Core.World.Structure.TileAttribute;
using Lunar.Editor.World;

namespace Lunar.Editor.Controls
{
    public partial class DockMapAttributes : DarkToolWindow
    {
        public TileAttributes Attribute { get; private set; }
        public AttributeData AttributeData { get; private set; }
        private WarpAttributeDialog _tileAttributeDialog;
        private NPCSpawnDialog _npcSpawnAttributeDialog;
        private StartDialogueDialog _startDialogueAttributeDialog;

        public Map MapSubject { get; private set; }

        public Project Project { get; private set; }

        public DockMapAttributes()
        {
            InitializeComponent();

            this.AttributeData = new AttributeData();
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

        private void btnPlayerSpawn_CheckedChanged(object sender, EventArgs e)
        {
            if (!((DarkRadioButton)sender).Checked)
                return;

            this.Attribute = TileAttributes.PlayerSpawn;
            this.AttributeData = new AttributeData();
        }

        private void btnWarp_CheckedChanged(object sender, EventArgs e)
        {
            if (!((DarkRadioButton)sender).Checked)
                return;

            this.Attribute = TileAttributes.Warp;

            _tileAttributeDialog = new WarpAttributeDialog(this.ParentForm, this.MapSubject);
            _tileAttributeDialog.SelectTile += (o, args) => this.SelectingTile?.Invoke(o, args);
            _tileAttributeDialog.Submitted += WarpDialog_Submitted;
            _tileAttributeDialog.Show(this.ParentForm);
        }

        private void btnNPCSpawn_CheckedChanged(object sender, EventArgs e)
        {
            if (!((DarkRadioButton)sender).Checked)
                return;

            this.Attribute = TileAttributes.NPCSpawn;

            _npcSpawnAttributeDialog = new NPCSpawnDialog(this.ParentForm, this.Project);
            _npcSpawnAttributeDialog.SelectTile += (o, args) => this.SelectingTile?.Invoke(o, args);
            _npcSpawnAttributeDialog.Submitted += NpcSpawnAttributeDialogOnSubmitted;
            _npcSpawnAttributeDialog.Show(this.ParentForm);
        }

        private void RadioDialogueInit_CheckedChanged(object sender, EventArgs e)
        {
            if (!((DarkRadioButton)sender).Checked)
                return;

            this.Attribute = TileAttributes.StartDialogue;

            _startDialogueAttributeDialog = new StartDialogueDialog(this.ParentForm, this.Project);
            _startDialogueAttributeDialog.Submitted += _startDialogueAttributeDialog_Submitted; ;
            _startDialogueAttributeDialog.Show(this.ParentForm);
        }

        private void _startDialogueAttributeDialog_Submitted(object sender, EventArgs e)
        {
            this.AttributeData = new StartDialogueAttributeData(_startDialogueAttributeDialog.Dialogue, _startDialogueAttributeDialog.Branch);
        }

        private void NpcSpawnAttributeDialogOnSubmitted(object sender, EventArgs e)
        {
            this.AttributeData = new NPCSpawnAttributeData(_npcSpawnAttributeDialog.NPC, _npcSpawnAttributeDialog.RespawnTime, _npcSpawnAttributeDialog.MaxSpawns);
        }

        private void WarpDialog_Submitted(object sender, EventArgs e)
        {
            this.AttributeData = new WarpAttributeData(_tileAttributeDialog.WarpX, _tileAttributeDialog.WarpY, _tileAttributeDialog.WarpMapID, _tileAttributeDialog.WarpLayerName);
        }

        public event EventHandler<EventArgs> SelectingTile;

        private void NPCSpawnDialog_Submitted(object sender, EventArgs e)
        {
            this.AttributeData = new WarpAttributeData(_tileAttributeDialog.WarpX, _tileAttributeDialog.WarpY, _tileAttributeDialog.WarpMapID, _tileAttributeDialog.WarpLayerName);
        }
    }
}