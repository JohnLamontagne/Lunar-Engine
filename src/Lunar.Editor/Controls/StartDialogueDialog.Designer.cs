namespace Lunar.Editor.Controls
{
    partial class StartDialogueDialog
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
            this.cmbBranch = new System.Windows.Forms.ComboBox();
            this.darkLabel1 = new DarkUI.Controls.DarkLabel();
            this.cmbDialogue = new System.Windows.Forms.ComboBox();
            this.lblWarpLayer = new DarkUI.Controls.DarkLabel();
            this.panelWarpData.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelWarpData
            // 
            this.panelWarpData.Controls.Add(this.cmbBranch);
            this.panelWarpData.Controls.Add(this.darkLabel1);
            this.panelWarpData.Controls.Add(this.cmbDialogue);
            this.panelWarpData.Controls.Add(this.lblWarpLayer);
            this.panelWarpData.Location = new System.Drawing.Point(6, 22);
            this.panelWarpData.Margin = new System.Windows.Forms.Padding(6);
            this.panelWarpData.Name = "panelWarpData";
            this.panelWarpData.Size = new System.Drawing.Size(539, 277);
            this.panelWarpData.TabIndex = 0;
            // 
            // cmbBranch
            // 
            this.cmbBranch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBranch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbBranch.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbBranch.FormattingEnabled = true;
            this.cmbBranch.Location = new System.Drawing.Point(191, 106);
            this.cmbBranch.Margin = new System.Windows.Forms.Padding(6);
            this.cmbBranch.Name = "cmbBranch";
            this.cmbBranch.Size = new System.Drawing.Size(305, 32);
            this.cmbBranch.TabIndex = 14;
            this.cmbBranch.SelectedIndexChanged += new System.EventHandler(this.CmbBranch_SelectedIndexChanged);
            // 
            // darkLabel1
            // 
            this.darkLabel1.AutoSize = true;
            this.darkLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel1.Location = new System.Drawing.Point(101, 106);
            this.darkLabel1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.darkLabel1.Name = "darkLabel1";
            this.darkLabel1.Size = new System.Drawing.Size(80, 25);
            this.darkLabel1.TabIndex = 13;
            this.darkLabel1.Text = "Branch:";
            // 
            // cmbDialogue
            // 
            this.cmbDialogue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbDialogue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDialogue.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbDialogue.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbDialogue.FormattingEnabled = true;
            this.cmbDialogue.Location = new System.Drawing.Point(191, 50);
            this.cmbDialogue.Margin = new System.Windows.Forms.Padding(6);
            this.cmbDialogue.Name = "cmbDialogue";
            this.cmbDialogue.Size = new System.Drawing.Size(305, 32);
            this.cmbDialogue.TabIndex = 12;
            this.cmbDialogue.SelectedIndexChanged += new System.EventHandler(this.cmbDialogue_SelectedIndexChanged);
            // 
            // lblWarpLayer
            // 
            this.lblWarpLayer.AutoSize = true;
            this.lblWarpLayer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.lblWarpLayer.Location = new System.Drawing.Point(86, 53);
            this.lblWarpLayer.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblWarpLayer.Name = "lblWarpLayer";
            this.lblWarpLayer.Size = new System.Drawing.Size(95, 25);
            this.lblWarpLayer.TabIndex = 8;
            this.lblWarpLayer.Text = "Dialogue:";
            // 
            // StartDialogueDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(563, 393);
            this.Controls.Add(this.panelWarpData);
            this.Margin = new System.Windows.Forms.Padding(11);
            this.Name = "StartDialogueDialog";
            this.Text = "Attribute Data";
            this.Controls.SetChildIndex(this.panelWarpData, 0);
            this.panelWarpData.ResumeLayout(false);
            this.panelWarpData.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelWarpData;
        private DarkUI.Controls.DarkLabel lblWarpLayer;
        private DarkUI.Controls.DarkLabel darkLabel1;
        private System.Windows.Forms.ComboBox cmbDialogue;
        private System.Windows.Forms.ComboBox cmbBranch;
    }
}
