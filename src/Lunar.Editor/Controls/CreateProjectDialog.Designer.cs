namespace Lunar.Editor.Controls
{
    partial class CreateProjectDialog
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
            this.txtServerDataDir = new DarkUI.Controls.DarkTextBox();
            this.darkLabel1 = new DarkUI.Controls.DarkLabel();
            this.btnBrowseServer = new DarkUI.Controls.DarkButton();
            this.btnBrowseClient = new DarkUI.Controls.DarkButton();
            this.darkLabel2 = new DarkUI.Controls.DarkLabel();
            this.txtClientDataDir = new DarkUI.Controls.DarkTextBox();
            this.SuspendLayout();
            // 
            // txtServerDataDir
            // 
            this.txtServerDataDir.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtServerDataDir.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtServerDataDir.Enabled = false;
            this.txtServerDataDir.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtServerDataDir.Location = new System.Drawing.Point(124, 26);
            this.txtServerDataDir.Name = "txtServerDataDir";
            this.txtServerDataDir.Size = new System.Drawing.Size(291, 20);
            this.txtServerDataDir.TabIndex = 2;
            // 
            // darkLabel1
            // 
            this.darkLabel1.AutoSize = true;
            this.darkLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel1.Location = new System.Drawing.Point(12, 28);
            this.darkLabel1.Name = "darkLabel1";
            this.darkLabel1.Size = new System.Drawing.Size(112, 13);
            this.darkLabel1.TabIndex = 3;
            this.darkLabel1.Text = "Server Data Directory:";
            // 
            // btnBrowseServer
            // 
            this.btnBrowseServer.Location = new System.Drawing.Point(421, 26);
            this.btnBrowseServer.Name = "btnBrowseServer";
            this.btnBrowseServer.Padding = new System.Windows.Forms.Padding(5);
            this.btnBrowseServer.Size = new System.Drawing.Size(30, 20);
            this.btnBrowseServer.TabIndex = 4;
            this.btnBrowseServer.Text = "...";
            this.btnBrowseServer.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnBrowseClient
            // 
            this.btnBrowseClient.Location = new System.Drawing.Point(421, 63);
            this.btnBrowseClient.Name = "btnBrowseClient";
            this.btnBrowseClient.Padding = new System.Windows.Forms.Padding(5);
            this.btnBrowseClient.Size = new System.Drawing.Size(30, 20);
            this.btnBrowseClient.TabIndex = 7;
            this.btnBrowseClient.Text = "...";
            this.btnBrowseClient.Click += new System.EventHandler(this.btnBrowseClient_Click);
            // 
            // darkLabel2
            // 
            this.darkLabel2.AutoSize = true;
            this.darkLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel2.Location = new System.Drawing.Point(12, 65);
            this.darkLabel2.Name = "darkLabel2";
            this.darkLabel2.Size = new System.Drawing.Size(107, 13);
            this.darkLabel2.TabIndex = 6;
            this.darkLabel2.Text = "Client Data Directory:";
            // 
            // txtClientDataDir
            // 
            this.txtClientDataDir.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtClientDataDir.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtClientDataDir.Enabled = false;
            this.txtClientDataDir.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtClientDataDir.Location = new System.Drawing.Point(124, 63);
            this.txtClientDataDir.Name = "txtClientDataDir";
            this.txtClientDataDir.Size = new System.Drawing.Size(291, 20);
            this.txtClientDataDir.TabIndex = 5;
            // 
            // CreateProjectDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(463, 155);
            this.Controls.Add(this.btnBrowseClient);
            this.Controls.Add(this.darkLabel2);
            this.Controls.Add(this.txtClientDataDir);
            this.Controls.Add(this.btnBrowseServer);
            this.Controls.Add(this.darkLabel1);
            this.Controls.Add(this.txtServerDataDir);
            this.MaximizeBox = false;
            this.Name = "CreateProjectDialog";
            this.Text = "Create Project";
            this.TopMost = true;
            this.Controls.SetChildIndex(this.txtServerDataDir, 0);
            this.Controls.SetChildIndex(this.darkLabel1, 0);
            this.Controls.SetChildIndex(this.btnBrowseServer, 0);
            this.Controls.SetChildIndex(this.txtClientDataDir, 0);
            this.Controls.SetChildIndex(this.darkLabel2, 0);
            this.Controls.SetChildIndex(this.btnBrowseClient, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DarkUI.Controls.DarkTextBox txtServerDataDir;
        private DarkUI.Controls.DarkLabel darkLabel1;
        private DarkUI.Controls.DarkButton btnBrowseServer;
        private DarkUI.Controls.DarkButton btnBrowseClient;
        private DarkUI.Controls.DarkLabel darkLabel2;
        private DarkUI.Controls.DarkTextBox txtClientDataDir;
    }
}
