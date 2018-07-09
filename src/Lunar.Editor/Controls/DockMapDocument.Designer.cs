namespace Lunar.Editor.Controls
{
    partial class DockMapDocument
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DockMapDocument));
            this.mapToolStrip = new DarkUI.Controls.DarkToolStrip();
            this.fillBucketButton = new System.Windows.Forms.ToolStripButton();
            this.brushButton = new System.Windows.Forms.ToolStripButton();
            this.buttonAttribute = new System.Windows.Forms.ToolStripButton();
            this.eraserButton = new System.Windows.Forms.ToolStripButton();
            this.buttonSelectTileButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolSelectObjectButton = new System.Windows.Forms.ToolStripButton();
            this.buttonMapObject = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonSave = new System.Windows.Forms.ToolStripButton();
            this.scrollY = new DarkUI.Controls.DarkScrollBar();
            this.scrollX = new DarkUI.Controls.DarkScrollBar();
            this.mapView = new Lunar.Editor.Controls.View();
            this.mapToolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mapToolStrip
            // 
            this.mapToolStrip.AutoSize = false;
            this.mapToolStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.mapToolStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.mapToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fillBucketButton,
            this.brushButton,
            this.buttonAttribute,
            this.eraserButton,
            this.buttonSelectTileButton,
            this.toolStripSeparator1,
            this.toolSelectObjectButton,
            this.buttonMapObject,
            this.toolStripSeparator2,
            this.buttonSave});
            this.mapToolStrip.Location = new System.Drawing.Point(0, 0);
            this.mapToolStrip.Name = "mapToolStrip";
            this.mapToolStrip.Padding = new System.Windows.Forms.Padding(5, 0, 1, 0);
            this.mapToolStrip.Size = new System.Drawing.Size(895, 28);
            this.mapToolStrip.TabIndex = 16;
            this.mapToolStrip.Text = "darkToolStrip2";
            this.mapToolStrip.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mapToolStrip_KeyDown);
            // 
            // fillBucketButton
            // 
            this.fillBucketButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.fillBucketButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.fillBucketButton.Image = global::Lunar.Editor.Icons.Bucket;
            this.fillBucketButton.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.fillBucketButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.fillBucketButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.fillBucketButton.Name = "fillBucketButton";
            this.fillBucketButton.Size = new System.Drawing.Size(23, 25);
            this.fillBucketButton.Text = "toolStripButton1";
            this.fillBucketButton.ToolTipText = "Bucket Fill";
            this.fillBucketButton.Click += new System.EventHandler(this.fillBucketButton_Click);
            // 
            // brushButton
            // 
            this.brushButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.brushButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.brushButton.Image = global::Lunar.Editor.Icons.Brush;
            this.brushButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.brushButton.Name = "brushButton";
            this.brushButton.Size = new System.Drawing.Size(23, 25);
            this.brushButton.Text = "toolStripButton2";
            this.brushButton.ToolTipText = "Tile Brush";
            this.brushButton.Click += new System.EventHandler(this.brushButton_Click);
            // 
            // buttonAttribute
            // 
            this.buttonAttribute.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonAttribute.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.buttonAttribute.Image = global::Lunar.Editor.Icons.Stamp;
            this.buttonAttribute.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonAttribute.Name = "buttonAttribute";
            this.buttonAttribute.Size = new System.Drawing.Size(23, 25);
            this.buttonAttribute.Text = "toolStripButton1";
            this.buttonAttribute.ToolTipText = "Place Attribute";
            this.buttonAttribute.Click += new System.EventHandler(this.buttonAttribute_Click);
            // 
            // eraserButton
            // 
            this.eraserButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.eraserButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.eraserButton.Image = global::Lunar.Editor.Icons.Eraser;
            this.eraserButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.eraserButton.Name = "eraserButton";
            this.eraserButton.Size = new System.Drawing.Size(23, 25);
            this.eraserButton.Text = "eraserButton";
            this.eraserButton.ToolTipText = "Eraser";
            this.eraserButton.Click += new System.EventHandler(this.eraserButton_Click);
            // 
            // buttonSelectTileButton
            // 
            this.buttonSelectTileButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonSelectTileButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.buttonSelectTileButton.Image = global::Lunar.Editor.Icons.Select;
            this.buttonSelectTileButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonSelectTileButton.Name = "buttonSelectTileButton";
            this.buttonSelectTileButton.Size = new System.Drawing.Size(23, 25);
            this.buttonSelectTileButton.Text = "toolStripButton1";
            this.buttonSelectTileButton.ToolTipText = "Select";
            this.buttonSelectTileButton.Click += new System.EventHandler(this.selectButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.toolStripSeparator1.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 28);
            // 
            // toolSelectObjectButton
            // 
            this.toolSelectObjectButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolSelectObjectButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.toolSelectObjectButton.Image = ((System.Drawing.Image)(resources.GetObject("toolSelectObjectButton.Image")));
            this.toolSelectObjectButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolSelectObjectButton.Name = "toolSelectObjectButton";
            this.toolSelectObjectButton.Size = new System.Drawing.Size(23, 25);
            this.toolSelectObjectButton.Text = "toolStripButton1";
            this.toolSelectObjectButton.ToolTipText = "Map Object Select";
            this.toolSelectObjectButton.Click += new System.EventHandler(this.toolSelectObjectButton_Click);
            // 
            // buttonMapObject
            // 
            this.buttonMapObject.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonMapObject.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.buttonMapObject.Image = global::Lunar.Editor.Icons.RefactoringLog_12810;
            this.buttonMapObject.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonMapObject.Name = "buttonMapObject";
            this.buttonMapObject.Size = new System.Drawing.Size(23, 25);
            this.buttonMapObject.Text = "toolStripButton1";
            this.buttonMapObject.ToolTipText = "New Map Object";
            this.buttonMapObject.Click += new System.EventHandler(this.buttonMapObject_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.toolStripSeparator2.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 28);
            // 
            // buttonSave
            // 
            this.buttonSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonSave.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.buttonSave.Image = global::Lunar.Editor.Icons.document_16xLG;
            this.buttonSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(23, 25);
            this.buttonSave.Text = "toolStripButton1";
            this.buttonSave.ToolTipText = "Save";
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // scrollY
            // 
            this.scrollY.Dock = System.Windows.Forms.DockStyle.Right;
            this.scrollY.Location = new System.Drawing.Point(873, 28);
            this.scrollY.Name = "scrollY";
            this.scrollY.Size = new System.Drawing.Size(22, 756);
            this.scrollY.TabIndex = 17;
            this.scrollY.Text = "darkScrollBar1";
            this.scrollY.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrollY_ValueChanged);
            this.scrollY.KeyDown += new System.Windows.Forms.KeyEventHandler(this.scrollY_KeyDown);
            // 
            // scrollX
            // 
            this.scrollX.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.scrollX.Location = new System.Drawing.Point(0, 761);
            this.scrollX.Name = "scrollX";
            this.scrollX.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrollX.Size = new System.Drawing.Size(873, 23);
            this.scrollX.TabIndex = 18;
            this.scrollX.Text = "darkScrollBar2";
            this.scrollX.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrollX_ValueChanged);
            this.scrollX.KeyDown += new System.Windows.Forms.KeyEventHandler(this.scrollX_KeyDown);
            // 
            // mapView
            // 
            this.mapView.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.mapView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapView.Location = new System.Drawing.Point(0, 28);
            this.mapView.Name = "mapView";
            this.mapView.OnDraw = null;
            this.mapView.OnInitalize = null;
            this.mapView.OnUpdate = null;
            this.mapView.Size = new System.Drawing.Size(873, 733);
            this.mapView.SuspendOnFormInactive = false;
            this.mapView.TabIndex = 14;
            this.mapView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mapView_KeyDown);
            this.mapView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.mapView_MouseClick);
            this.mapView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mapView_MouseDown);
            this.mapView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mapView_MouseMove);
            this.mapView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.mapView_MouseUp);
            // 
            // DockMapDocument
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mapView);
            this.Controls.Add(this.scrollX);
            this.Controls.Add(this.scrollY);
            this.Controls.Add(this.mapToolStrip);
            this.Name = "DockMapDocument";
            this.Size = new System.Drawing.Size(895, 784);
            this.Load += new System.EventHandler(this.DockMapDocument_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DockMapDocument_KeyDown);
            this.mapToolStrip.ResumeLayout(false);
            this.mapToolStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private View mapView;
        private DarkUI.Controls.DarkToolStrip mapToolStrip;
        private System.Windows.Forms.ToolStripButton fillBucketButton;
        private System.Windows.Forms.ToolStripButton brushButton;
        private System.Windows.Forms.ToolStripButton eraserButton;
        private DarkUI.Controls.DarkScrollBar scrollY;
        private DarkUI.Controls.DarkScrollBar scrollX;
        private System.Windows.Forms.ToolStripButton buttonSelectTileButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton buttonMapObject;
        private System.Windows.Forms.ToolStripButton toolSelectObjectButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton buttonSave;
        private System.Windows.Forms.ToolStripButton buttonAttribute;
    }
}
