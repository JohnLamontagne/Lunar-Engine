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
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.folderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.itemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.animationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projectExplorerMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeProject
            // 
            this.treeProject.AllowMoveNodes = true;
            this.treeProject.ContextMenuStrip = this.projectExplorerMenu;
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
            this.newToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.projectExplorerMenu.Name = "darkContextMenu1";
            this.projectExplorerMenu.Size = new System.Drawing.Size(153, 70);
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.folderToolStripMenuItem,
            this.scriptToolStripMenuItem,
            this.mapToolStripMenuItem,
            this.itemToolStripMenuItem,
            this.animationToolStripMenuItem});
            this.newToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.newToolStripMenuItem.Text = "New";
            // 
            // folderToolStripMenuItem
            // 
            this.folderToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.folderToolStripMenuItem.Name = "folderToolStripMenuItem";
            this.folderToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.folderToolStripMenuItem.Text = "Folder";
            this.folderToolStripMenuItem.Click += new System.EventHandler(this.folderToolStripMenuItem_Click);
            // 
            // scriptToolStripMenuItem
            // 
            this.scriptToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.scriptToolStripMenuItem.Name = "scriptToolStripMenuItem";
            this.scriptToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.scriptToolStripMenuItem.Text = "Script";
            this.scriptToolStripMenuItem.Click += new System.EventHandler(this.scriptToolStripMenuItem_Click);
            // 
            // mapToolStripMenuItem
            // 
            this.mapToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.mapToolStripMenuItem.Name = "mapToolStripMenuItem";
            this.mapToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.mapToolStripMenuItem.Text = "Map";
            this.mapToolStripMenuItem.Click += new System.EventHandler(this.mapToolStripMenuItem_Click);
            // 
            // itemToolStripMenuItem
            // 
            this.itemToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.itemToolStripMenuItem.Name = "itemToolStripMenuItem";
            this.itemToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.itemToolStripMenuItem.Text = "Item";
            this.itemToolStripMenuItem.Click += new System.EventHandler(this.itemToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // animationToolStripMenuItem
            // 
            this.animationToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.animationToolStripMenuItem.Name = "animationToolStripMenuItem";
            this.animationToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.animationToolStripMenuItem.Text = "Animation";
            this.animationToolStripMenuItem.Click += new System.EventHandler(this.animationToolStripMenuItem_Click);
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
            this.ResumeLayout(false);

        }

        #endregion

        private DarkTreeView treeProject;
        private DarkContextMenu projectExplorerMenu;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem folderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scriptToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem itemToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem animationToolStripMenuItem;
    }
}
