using DarkUI.Config;
using DarkUI.Controls;
using DarkUI.Docking;

namespace Lunar.Editor.Controls
{
    partial class DockProject
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
            this.treeProject = new DarkUI.Controls.DarkTreeView();
            this.projectExplorerMenu = new DarkUI.Controls.DarkContextMenu();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.npcExplorerMenu = new DarkUI.Controls.DarkContextMenu();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.scriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fromTemplateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aggressiveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projectExplorerMenu.SuspendLayout();
            this.npcExplorerMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeProject
            // 
            this.treeProject.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeProject.Location = new System.Drawing.Point(0, 25);
            this.treeProject.MaxDragChange = 20;
            this.treeProject.MultiSelect = true;
            this.treeProject.Name = "treeProject";
            this.treeProject.ShowIcons = true;
            this.treeProject.Size = new System.Drawing.Size(280, 425);
            this.treeProject.TabIndex = 0;
            this.treeProject.Text = "treeProjectView";
            // 
            // projectExplorerMenu
            // 
            this.projectExplorerMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.projectExplorerMenu.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.projectExplorerMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem});
            this.projectExplorerMenu.Name = "darkContextMenu1";
            this.projectExplorerMenu.Size = new System.Drawing.Size(108, 26);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // npcExplorerMenu
            // 
            this.npcExplorerMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.npcExplorerMenu.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.npcExplorerMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.scriptToolStripMenuItem});
            this.npcExplorerMenu.Name = "darkContextMenu1";
            this.npcExplorerMenu.Size = new System.Drawing.Size(181, 70);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(107, 22);
            this.toolStripMenuItem1.Text = "Delete";
            // 
            // scriptToolStripMenuItem
            // 
            this.scriptToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.fromTemplateToolStripMenuItem,
            this.loadToolStripMenuItem});
            this.scriptToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.scriptToolStripMenuItem.Name = "scriptToolStripMenuItem";
            this.scriptToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.scriptToolStripMenuItem.Text = "Script";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.NewToolStripMenuItem_Click);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.loadToolStripMenuItem.Text = "Load";
            // 
            // fromTemplateToolStripMenuItem
            // 
            this.fromTemplateToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aggressiveToolStripMenuItem});
            this.fromTemplateToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.fromTemplateToolStripMenuItem.Name = "fromTemplateToolStripMenuItem";
            this.fromTemplateToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.fromTemplateToolStripMenuItem.Text = "Create From Template";
            // 
            // aggressiveToolStripMenuItem
            // 
            this.aggressiveToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.aggressiveToolStripMenuItem.Name = "aggressiveToolStripMenuItem";
            this.aggressiveToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.aggressiveToolStripMenuItem.Text = "Aggressive";
            // 
            // DockProject
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeProject);
            this.DefaultDockArea = DarkUI.Docking.DarkDockArea.Left;
            this.DockText = "Project Explorer";
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = global::Lunar.Editor.Icons.application_16x;
            this.Name = "DockProject";
            this.SerializationKey = "DockProject";
            this.Size = new System.Drawing.Size(280, 450);
            this.projectExplorerMenu.ResumeLayout(false);
            this.npcExplorerMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DarkTreeView treeProject;
        private DarkContextMenu projectExplorerMenu;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private DarkContextMenu npcExplorerMenu;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem scriptToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fromTemplateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aggressiveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
    }
}
