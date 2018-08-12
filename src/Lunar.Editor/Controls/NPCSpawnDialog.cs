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
using Lunar.Core.World.Actor.Descriptors;
using Lunar.Editor.World;

namespace Lunar.Editor.Controls
{
    public partial class NPCSpawnDialog : DarkDialog
    {
        private Form _parentForm;

        public NPCDescriptor NPC { get; private set; }

        public int RespawnTime { get; private set; }

        public int MaxSpawns { get; private set; }

       
        public NPCSpawnDialog(Form parentForm, Project project)
        {
            _parentForm = parentForm;

            this.btnOk.Click += BtnOk_Click;

            InitializeComponent();

            this.cmbNPC.Items.Clear();
            foreach (var npc in project.NPCs.Values)
            {
                this.cmbNPC.Items.Add(npc);
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
            int.TryParse(txtRespawnTime.Text, out int maxSpawns);

            this.MaxSpawns = maxSpawns;
        }

        private void cmbNPC_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.NPC = (NPCDescriptor)this.cmbNPC.SelectedItem;
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
