namespace Lunar.Editor.Controls
{
    partial class DockMapAttributes
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
            this.btnBlocked = new DarkUI.Controls.DarkRadioButton();
            this.btnPlayerSpawn = new DarkUI.Controls.DarkRadioButton();
            this.btnNone = new DarkUI.Controls.DarkRadioButton();
            this.btnWarp = new DarkUI.Controls.DarkRadioButton();
            this.btnNPCSpawn = new DarkUI.Controls.DarkRadioButton();
            this.SuspendLayout();
            // 
            // btnBlocked
            // 
            this.btnBlocked.AutoSize = true;
            this.btnBlocked.Location = new System.Drawing.Point(59, 69);
            this.btnBlocked.Name = "btnBlocked";
            this.btnBlocked.Size = new System.Drawing.Size(64, 17);
            this.btnBlocked.TabIndex = 0;
            this.btnBlocked.Text = "Blocked";
            this.btnBlocked.CheckedChanged += new System.EventHandler(this.btnBlocked_CheckedChanged);
            // 
            // btnPlayerSpawn
            // 
            this.btnPlayerSpawn.AutoSize = true;
            this.btnPlayerSpawn.Location = new System.Drawing.Point(59, 116);
            this.btnPlayerSpawn.Name = "btnPlayerSpawn";
            this.btnPlayerSpawn.Size = new System.Drawing.Size(90, 17);
            this.btnPlayerSpawn.TabIndex = 1;
            this.btnPlayerSpawn.Text = "Player Spawn";
            this.btnPlayerSpawn.CheckedChanged += new System.EventHandler(this.btnPlayerSpawn_CheckedChanged);
            // 
            // btnNone
            // 
            this.btnNone.AutoSize = true;
            this.btnNone.Checked = true;
            this.btnNone.Location = new System.Drawing.Point(59, 46);
            this.btnNone.Name = "btnNone";
            this.btnNone.Size = new System.Drawing.Size(51, 17);
            this.btnNone.TabIndex = 2;
            this.btnNone.TabStop = true;
            this.btnNone.Text = "None";
            this.btnNone.CheckedChanged += new System.EventHandler(this.btnNone_CheckedChanged);
            // 
            // btnWarp
            // 
            this.btnWarp.AutoSize = true;
            this.btnWarp.Location = new System.Drawing.Point(59, 93);
            this.btnWarp.Name = "btnWarp";
            this.btnWarp.Size = new System.Drawing.Size(51, 17);
            this.btnWarp.TabIndex = 3;
            this.btnWarp.Text = "Warp";
            this.btnWarp.CheckedChanged += new System.EventHandler(this.btnWarp_CheckedChanged);
            // 
            // btnNPCSpawn
            // 
            this.btnNPCSpawn.AutoSize = true;
            this.btnNPCSpawn.Location = new System.Drawing.Point(59, 139);
            this.btnNPCSpawn.Name = "btnNPCSpawn";
            this.btnNPCSpawn.Size = new System.Drawing.Size(83, 17);
            this.btnNPCSpawn.TabIndex = 4;
            this.btnNPCSpawn.Text = "NPC Spawn";
            this.btnNPCSpawn.CheckedChanged += new System.EventHandler(this.btnNPCSpawn_CheckedChanged);
            // 
            // DockMapAttributes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnNPCSpawn);
            this.Controls.Add(this.btnWarp);
            this.Controls.Add(this.btnNone);
            this.Controls.Add(this.btnPlayerSpawn);
            this.Controls.Add(this.btnBlocked);
            this.DockText = "Attributes";
            this.Name = "DockMapAttributes";
            this.Size = new System.Drawing.Size(213, 192);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DarkUI.Controls.DarkRadioButton btnBlocked;
        private DarkUI.Controls.DarkRadioButton btnPlayerSpawn;
        private DarkUI.Controls.DarkRadioButton btnNone;
        private DarkUI.Controls.DarkRadioButton btnWarp;
        private DarkUI.Controls.DarkRadioButton btnNPCSpawn;
    }
}
