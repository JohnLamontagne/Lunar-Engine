using DarkUI.Controls;
using DarkUI.Docking;

namespace Lunar.Editor.Controls
{
    partial class DockLayers
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DockLayers));
            this.lstLayers = new System.Windows.Forms.CheckedListBox();
            this.darkToolStrip1 = new DarkUI.Controls.DarkToolStrip();
            this.buttonAddLayer = new System.Windows.Forms.ToolStripButton();
            this.buttonRemoveLayer = new System.Windows.Forms.ToolStripButton();
            this.darkToolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstLayers
            // 
            this.lstLayers.AllowDrop = true;
            this.lstLayers.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.lstLayers.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstLayers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstLayers.ForeColor = System.Drawing.SystemColors.Menu;
            this.lstLayers.FormattingEnabled = true;
            this.lstLayers.Location = new System.Drawing.Point(0, 25);
            this.lstLayers.Name = "lstLayers";
            this.lstLayers.Size = new System.Drawing.Size(275, 258);
            this.lstLayers.TabIndex = 1;
            this.lstLayers.DragDrop += new System.Windows.Forms.DragEventHandler(this.lstLayers_DragDrop);
            this.lstLayers.DragOver += new System.Windows.Forms.DragEventHandler(this.lstLayers_DragOver);
            this.lstLayers.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lstLayers_MouseDown);
            this.lstLayers.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lstLayers_MouseMove);
            // 
            // darkToolStrip1
            // 
            this.darkToolStrip1.AutoSize = false;
            this.darkToolStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.darkToolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.darkToolStrip1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkToolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonAddLayer,
            this.buttonRemoveLayer});
            this.darkToolStrip1.Location = new System.Drawing.Point(0, 255);
            this.darkToolStrip1.Name = "darkToolStrip1";
            this.darkToolStrip1.Padding = new System.Windows.Forms.Padding(5, 0, 1, 0);
            this.darkToolStrip1.Size = new System.Drawing.Size(275, 28);
            this.darkToolStrip1.TabIndex = 22;
            this.darkToolStrip1.Text = "darkToolStrip1";
            // 
            // buttonAddLayer
            // 
            this.buttonAddLayer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonAddLayer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.buttonAddLayer.Image = ((System.Drawing.Image)(resources.GetObject("buttonAddLayer.Image")));
            this.buttonAddLayer.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.buttonAddLayer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonAddLayer.Name = "buttonAddLayer";
            this.buttonAddLayer.Size = new System.Drawing.Size(23, 25);
            this.buttonAddLayer.Text = "toolStripButton1";
            this.buttonAddLayer.ToolTipText = "Add Layer";
            this.buttonAddLayer.Click += new System.EventHandler(this.buttonAddLayer_Click);
            // 
            // buttonRemoveLayer
            // 
            this.buttonRemoveLayer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonRemoveLayer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.buttonRemoveLayer.Image = ((System.Drawing.Image)(resources.GetObject("buttonRemoveLayer.Image")));
            this.buttonRemoveLayer.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.buttonRemoveLayer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonRemoveLayer.Name = "buttonRemoveLayer";
            this.buttonRemoveLayer.Size = new System.Drawing.Size(23, 25);
            this.buttonRemoveLayer.Text = "toolStripButton2";
            this.buttonRemoveLayer.ToolTipText = "Remove Layer";
            // 
            // DockLayers
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.darkToolStrip1);
            this.Controls.Add(this.lstLayers);
            this.DefaultDockArea = DarkUI.Docking.DarkDockArea.Right;
            this.DockText = "Layers";
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = global::Lunar.Editor.Icons.Collection_16xLG;
            this.Name = "DockLayers";
            this.SerializationKey = "DockLayers";
            this.Size = new System.Drawing.Size(275, 283);
            this.darkToolStrip1.ResumeLayout(false);
            this.darkToolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckedListBox lstLayers;
        private DarkToolStrip darkToolStrip1;
        private System.Windows.Forms.ToolStripButton buttonAddLayer;
        private System.Windows.Forms.ToolStripButton buttonRemoveLayer;
    }
}
