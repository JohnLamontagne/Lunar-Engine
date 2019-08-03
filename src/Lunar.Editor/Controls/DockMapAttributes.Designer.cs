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
            this.radioDialogueInit = new DarkUI.Controls.DarkRadioButton();
            this.SuspendLayout();
            // 
            // btnBlocked
            // 
            this.btnBlocked.AutoSize = true;
            this.btnBlocked.Location = new System.Drawing.Point(108, 127);
            this.btnBlocked.Margin = new System.Windows.Forms.Padding(6);
            this.btnBlocked.Name = "btnBlocked";
            this.btnBlocked.Size = new System.Drawing.Size(107, 29);
            this.btnBlocked.TabIndex = 0;
            this.btnBlocked.Text = "Blocked";
            this.btnBlocked.CheckedChanged += new System.EventHandler(this.btnBlocked_CheckedChanged);
            // 
            // btnPlayerSpawn
            // 
            this.btnPlayerSpawn.AutoSize = true;
            this.btnPlayerSpawn.Location = new System.Drawing.Point(108, 214);
            this.btnPlayerSpawn.Margin = new System.Windows.Forms.Padding(6);
            this.btnPlayerSpawn.Name = "btnPlayerSpawn";
            this.btnPlayerSpawn.Size = new System.Drawing.Size(158, 29);
            this.btnPlayerSpawn.TabIndex = 1;
            this.btnPlayerSpawn.Text = "Player Spawn";
            this.btnPlayerSpawn.CheckedChanged += new System.EventHandler(this.btnPlayerSpawn_CheckedChanged);
            // 
            // btnNone
            // 
            this.btnNone.AutoSize = true;
            this.btnNone.Checked = true;
            this.btnNone.Location = new System.Drawing.Point(108, 85);
            this.btnNone.Margin = new System.Windows.Forms.Padding(6);
            this.btnNone.Name = "btnNone";
            this.btnNone.Size = new System.Drawing.Size(84, 29);
            this.btnNone.TabIndex = 2;
            this.btnNone.TabStop = true;
            this.btnNone.Text = "None";
            this.btnNone.CheckedChanged += new System.EventHandler(this.btnNone_CheckedChanged);
            // 
            // btnWarp
            // 
            this.btnWarp.AutoSize = true;
            this.btnWarp.Location = new System.Drawing.Point(108, 172);
            this.btnWarp.Margin = new System.Windows.Forms.Padding(6);
            this.btnWarp.Name = "btnWarp";
            this.btnWarp.Size = new System.Drawing.Size(85, 29);
            this.btnWarp.TabIndex = 3;
            this.btnWarp.Text = "Warp";
            this.btnWarp.CheckedChanged += new System.EventHandler(this.btnWarp_CheckedChanged);
            // 
            // btnNPCSpawn
            // 
            this.btnNPCSpawn.AutoSize = true;
            this.btnNPCSpawn.Location = new System.Drawing.Point(108, 257);
            this.btnNPCSpawn.Margin = new System.Windows.Forms.Padding(6);
            this.btnNPCSpawn.Name = "btnNPCSpawn";
            this.btnNPCSpawn.Size = new System.Drawing.Size(145, 29);
            this.btnNPCSpawn.TabIndex = 4;
            this.btnNPCSpawn.Text = "NPC Spawn";
            this.btnNPCSpawn.CheckedChanged += new System.EventHandler(this.btnNPCSpawn_CheckedChanged);
            // 
            // radioDialogueInit
            // 
            this.radioDialogueInit.AutoSize = true;
            this.radioDialogueInit.Location = new System.Drawing.Point(108, 298);
            this.radioDialogueInit.Margin = new System.Windows.Forms.Padding(6);
            this.radioDialogueInit.Name = "radioDialogueInit";
            this.radioDialogueInit.Size = new System.Drawing.Size(160, 29);
            this.radioDialogueInit.TabIndex = 5;
            this.radioDialogueInit.Text = "Start Dialogue";
            this.radioDialogueInit.CheckedChanged += new System.EventHandler(this.RadioDialogueInit_CheckedChanged);
            // 
            // DockMapAttributes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.radioDialogueInit);
            this.Controls.Add(this.btnNPCSpawn);
            this.Controls.Add(this.btnWarp);
            this.Controls.Add(this.btnNone);
            this.Controls.Add(this.btnPlayerSpawn);
            this.Controls.Add(this.btnBlocked);
            this.DockText = "Attributes";
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "DockMapAttributes";
            this.Size = new System.Drawing.Size(391, 354);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DarkUI.Controls.DarkRadioButton btnBlocked;
        private DarkUI.Controls.DarkRadioButton btnPlayerSpawn;
        private DarkUI.Controls.DarkRadioButton btnNone;
        private DarkUI.Controls.DarkRadioButton btnWarp;
        private DarkUI.Controls.DarkRadioButton btnNPCSpawn;
        private DarkUI.Controls.DarkRadioButton radioDialogueInit;
    }
}
