using System;
using System.IO;
using System.Windows.Forms;
using DarkUI.Forms;
using Lunar.Core.World.Actor.Descriptors;

namespace Lunar.Editor.Controls
{
    public partial class NPCSpawnDialog : DarkDialog
    {
        private Form _parentForm;

        public string NPC { get; private set; }

        public int RespawnTime { get; private set; }

        public int MaxSpawns { get; private set; }

       
        public NPCSpawnDialog(Form parentForm, Project project)
        {
            _parentForm = parentForm;

            this.btnOk.Click += BtnOk_Click;

            InitializeComponent();

            this.cmbNPC.Items.Clear();
            foreach (var npcFile in project.NPCFiles)
            {
                this.cmbNPC.Items.Add(Path.GetFileNameWithoutExtension(npcFile.Name));
            }
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            this.Submitted?.Invoke(this, new EventArgs());
            this.Close();
        }

        public event EventHandler<EventArgs> Submitted;
        public event EventHandler<EventArgs> SelectTile;

        private void txtMaxSpawns_TextChanged(object sender, EventArgs e)
        {
            int.TryParse(this.txtMaxSpawns.Text, out int maxSpawns);

            this.MaxSpawns = maxSpawns;
        }

        private void cmbNPC_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.NPC = this.cmbNPC.SelectedItem.ToString();
        }

        private void txtRespawnTime_TextChanged(object sender, EventArgs e)
        {
            int.TryParse(txtRespawnTime.Text, out int respawnTime);

            this.RespawnTime = respawnTime;
        }

        private void txtRespawnTime_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtMaxSpawns_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
