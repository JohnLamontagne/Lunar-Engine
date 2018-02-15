namespace Lunar.Editor.Controls
{
    partial class DockMapObjectProperties
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
            this.mapObjectProperties = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // mapObjectProperties
            // 
            this.mapObjectProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapObjectProperties.Location = new System.Drawing.Point(0, 25);
            this.mapObjectProperties.Name = "mapObjectProperties";
            this.mapObjectProperties.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.mapObjectProperties.Size = new System.Drawing.Size(291, 432);
            this.mapObjectProperties.TabIndex = 0;
            // 
            // DockMapObjectProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mapObjectProperties);
            this.DefaultDockArea = DarkUI.Docking.DarkDockArea.Right;
            this.DockText = "Map Object";
            this.Name = "DockMapObjectProperties";
            this.Size = new System.Drawing.Size(291, 457);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid mapObjectProperties;
    }
}
