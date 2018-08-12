namespace Lunar.Editor.Controls
{
    partial class NPCSpawnDialog
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelWarpData = new System.Windows.Forms.Panel();
            this.txtMaxSpawns = new DarkUI.Controls.DarkTextBox();
            this.darkLabel2 = new DarkUI.Controls.DarkLabel();
            this.txtRespawnTime = new DarkUI.Controls.DarkTextBox();
            this.darkLabel1 = new DarkUI.Controls.DarkLabel();
            this.cmbNPC = new System.Windows.Forms.ComboBox();
            this.lblWarpLayer = new DarkUI.Controls.DarkLabel();
            this.darkLabel3 = new DarkUI.Controls.DarkLabel();
            this.panelWarpData.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelWarpData
            // 
            this.panelWarpData.Controls.Add(this.darkLabel3);
            this.panelWarpData.Controls.Add(this.txtMaxSpawns);
            this.panelWarpData.Controls.Add(this.darkLabel2);
            this.panelWarpData.Controls.Add(this.txtRespawnTime);
            this.panelWarpData.Controls.Add(this.darkLabel1);
            this.panelWarpData.Controls.Add(this.cmbNPC);
            this.panelWarpData.Controls.Add(this.lblWarpLayer);
            this.panelWarpData.Location = new System.Drawing.Point(3, 12);
            this.panelWarpData.Name = "panelWarpData";
            this.panelWarpData.Size = new System.Drawing.Size(294, 150);
            this.panelWarpData.TabIndex = 0;
            // 
            // txtMaxSpawns
            // 
            this.txtMaxSpawns.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtMaxSpawns.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMaxSpawns.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtMaxSpawns.Location = new System.Drawing.Point(104, 85);
            this.txtMaxSpawns.Name = "txtMaxSpawns";
            this.txtMaxSpawns.Size = new System.Drawing.Size(94, 20);
            this.txtMaxSpawns.TabIndex = 16;
            this.txtMaxSpawns.TextChanged += new System.EventHandler(this.txtMaxSpawns_TextChanged);
            this.txtMaxSpawns.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMaxSpawns_KeyPress);
            // 
            // darkLabel2
            // 
            this.darkLabel2.AutoSize = true;
            this.darkLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel2.Location = new System.Drawing.Point(3, 87);
            this.darkLabel2.Name = "darkLabel2";
            this.darkLabel2.Size = new System.Drawing.Size(95, 13);
            this.darkLabel2.TabIndex = 15;
            this.darkLabel2.Text = "Maximum Spawns:";
            // 
            // txtRespawnTime
            // 
            this.txtRespawnTime.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtRespawnTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtRespawnTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtRespawnTime.Location = new System.Drawing.Point(104, 57);
            this.txtRespawnTime.Name = "txtRespawnTime";
            this.txtRespawnTime.Size = new System.Drawing.Size(94, 20);
            this.txtRespawnTime.TabIndex = 14;
            this.txtRespawnTime.TextChanged += new System.EventHandler(this.txtRespawnTime_TextChanged);
            this.txtRespawnTime.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtRespawnTime_KeyPress);
            // 
            // darkLabel1
            // 
            this.darkLabel1.AutoSize = true;
            this.darkLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel1.Location = new System.Drawing.Point(9, 59);
            this.darkLabel1.Name = "darkLabel1";
            this.darkLabel1.Size = new System.Drawing.Size(81, 13);
            this.darkLabel1.TabIndex = 13;
            this.darkLabel1.Text = "Respawn Time:";
            // 
            // cmbNPC
            // 
            this.cmbNPC.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbNPC.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbNPC.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbNPC.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbNPC.FormattingEnabled = true;
            this.cmbNPC.Location = new System.Drawing.Point(104, 27);
            this.cmbNPC.Name = "cmbNPC";
            this.cmbNPC.Size = new System.Drawing.Size(168, 21);
            this.cmbNPC.TabIndex = 12;
            this.cmbNPC.SelectedIndexChanged += new System.EventHandler(this.cmbNPC_SelectedIndexChanged);
            // 
            // lblWarpLayer
            // 
            this.lblWarpLayer.AutoSize = true;
            this.lblWarpLayer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.lblWarpLayer.Location = new System.Drawing.Point(66, 30);
            this.lblWarpLayer.Name = "lblWarpLayer";
            this.lblWarpLayer.Size = new System.Drawing.Size(32, 13);
            this.lblWarpLayer.TabIndex = 8;
            this.lblWarpLayer.Text = "NPC:";
            // 
            // darkLabel3
            // 
            this.darkLabel3.AutoSize = true;
            this.darkLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel3.Location = new System.Drawing.Point(204, 64);
            this.darkLabel3.Name = "darkLabel3";
            this.darkLabel3.Size = new System.Drawing.Size(47, 13);
            this.darkLabel3.TabIndex = 17;
            this.darkLabel3.Text = "seconds";
            // 
            // NPCSpawnDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(307, 213);
            this.Controls.Add(this.panelWarpData);
            this.Name = "NPCSpawnDialog";
            this.Text = "Attribute Data";
            this.Controls.SetChildIndex(this.panelWarpData, 0);
            this.panelWarpData.ResumeLayout(false);
            this.panelWarpData.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelWarpData;
        private DarkUI.Controls.DarkLabel lblWarpLayer;
        private DarkUI.Controls.DarkTextBox txtMaxSpawns;
        private DarkUI.Controls.DarkLabel darkLabel2;
        private DarkUI.Controls.DarkTextBox txtRespawnTime;
        private DarkUI.Controls.DarkLabel darkLabel1;
        private System.Windows.Forms.ComboBox cmbNPC;
        private DarkUI.Controls.DarkLabel darkLabel3;
    }
}
