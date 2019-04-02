namespace Lunar.Editor.Controls.AttributeDialogs
{
    partial class ItemSpawnDialog
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
            this.darkLabel3 = new DarkUI.Controls.DarkLabel();
            this.txtRespawnTime = new DarkUI.Controls.DarkTextBox();
            this.darkLabel1 = new DarkUI.Controls.DarkLabel();
            this.cmbItem = new System.Windows.Forms.ComboBox();
            this.lblWarpLayer = new DarkUI.Controls.DarkLabel();
            this.panelWarpData.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelWarpData
            // 
            this.panelWarpData.Controls.Add(this.darkLabel3);
            this.panelWarpData.Controls.Add(this.txtRespawnTime);
            this.panelWarpData.Controls.Add(this.darkLabel1);
            this.panelWarpData.Controls.Add(this.cmbItem);
            this.panelWarpData.Controls.Add(this.lblWarpLayer);
            this.panelWarpData.Location = new System.Drawing.Point(3, 12);
            this.panelWarpData.Name = "panelWarpData";
            this.panelWarpData.Size = new System.Drawing.Size(294, 150);
            this.panelWarpData.TabIndex = 0;
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
            // cmbItem
            // 
            this.cmbItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbItem.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbItem.FormattingEnabled = true;
            this.cmbItem.Location = new System.Drawing.Point(104, 27);
            this.cmbItem.Name = "cmbItem";
            this.cmbItem.Size = new System.Drawing.Size(168, 21);
            this.cmbItem.TabIndex = 12;
            this.cmbItem.SelectedIndexChanged += new System.EventHandler(this.cmbNPC_SelectedIndexChanged);
            // 
            // lblWarpLayer
            // 
            this.lblWarpLayer.AutoSize = true;
            this.lblWarpLayer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.lblWarpLayer.Location = new System.Drawing.Point(66, 30);
            this.lblWarpLayer.Name = "lblWarpLayer";
            this.lblWarpLayer.Size = new System.Drawing.Size(30, 13);
            this.lblWarpLayer.TabIndex = 8;
            this.lblWarpLayer.Text = "Item:";
            // 
            // ItemSpawnDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(307, 213);
            this.Controls.Add(this.panelWarpData);
            this.Name = "ItemSpawnDialog";
            this.Text = "Attribute Data";
            this.Controls.SetChildIndex(this.panelWarpData, 0);
            this.panelWarpData.ResumeLayout(false);
            this.panelWarpData.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelWarpData;
        private DarkUI.Controls.DarkLabel lblWarpLayer;
        private DarkUI.Controls.DarkTextBox txtRespawnTime;
        private DarkUI.Controls.DarkLabel darkLabel1;
        private System.Windows.Forms.ComboBox cmbItem;
        private DarkUI.Controls.DarkLabel darkLabel3;
    }
}
