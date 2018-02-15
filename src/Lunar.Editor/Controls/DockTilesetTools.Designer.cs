namespace Lunar.Editor.Controls
{
    partial class DockTilesetTools
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DockTilesetTools));
            this.tilesetView = new Lunar.Editor.Controls.View();
            this.scrollY = new DarkUI.Controls.DarkScrollBar();
            this.darkToolStrip1 = new DarkUI.Controls.DarkToolStrip();
            this.buttonAddTileset = new System.Windows.Forms.ToolStripButton();
            this.buttonRemoveTileset = new System.Windows.Forms.ToolStripButton();
            this.comboTileset = new DarkUI.Controls.DarkComboBox();
            this.scrollX = new DarkUI.Controls.DarkScrollBar();
            this.darkToolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tilesetView
            // 
            this.tilesetView.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.tilesetView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tilesetView.Location = new System.Drawing.Point(0, 25);
            this.tilesetView.Name = "tilesetView";
            this.tilesetView.OnDraw = null;
            this.tilesetView.OnInitalize = null;
            this.tilesetView.OnUpdate = null;
            this.tilesetView.Size = new System.Drawing.Size(386, 637);
            this.tilesetView.SuspendOnFormInactive = false;
            this.tilesetView.TabIndex = 14;
            this.tilesetView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tilesetView_KeyDown);
            this.tilesetView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tilesetView_MouseDown);
            this.tilesetView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.tilesetView_MouseMove);
            this.tilesetView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tilesetView_MouseUp);
            // 
            // scrollY
            // 
            this.scrollY.Dock = System.Windows.Forms.DockStyle.Right;
            this.scrollY.Location = new System.Drawing.Point(365, 25);
            this.scrollY.Name = "scrollY";
            this.scrollY.Size = new System.Drawing.Size(21, 637);
            this.scrollY.TabIndex = 18;
            this.scrollY.Text = "scrollX";
            this.scrollY.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrollY_ValueChanged);
            // 
            // darkToolStrip1
            // 
            this.darkToolStrip1.AutoSize = false;
            this.darkToolStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.darkToolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.darkToolStrip1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkToolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonAddTileset,
            this.buttonRemoveTileset});
            this.darkToolStrip1.Location = new System.Drawing.Point(0, 634);
            this.darkToolStrip1.Name = "darkToolStrip1";
            this.darkToolStrip1.Padding = new System.Windows.Forms.Padding(5, 0, 1, 0);
            this.darkToolStrip1.Size = new System.Drawing.Size(365, 28);
            this.darkToolStrip1.TabIndex = 21;
            this.darkToolStrip1.Text = "darkToolStrip1";
            // 
            // buttonAddTileset
            // 
            this.buttonAddTileset.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonAddTileset.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.buttonAddTileset.Image = ((System.Drawing.Image)(resources.GetObject("buttonAddTileset.Image")));
            this.buttonAddTileset.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.buttonAddTileset.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonAddTileset.Name = "buttonAddTileset";
            this.buttonAddTileset.Size = new System.Drawing.Size(23, 25);
            this.buttonAddTileset.Text = "toolStripButton1";
            this.buttonAddTileset.ToolTipText = "Add Tileset(s)";
            this.buttonAddTileset.Click += new System.EventHandler(this.buttonAddTileset_Click);
            // 
            // buttonRemoveTileset
            // 
            this.buttonRemoveTileset.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonRemoveTileset.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.buttonRemoveTileset.Image = ((System.Drawing.Image)(resources.GetObject("buttonRemoveTileset.Image")));
            this.buttonRemoveTileset.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.buttonRemoveTileset.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonRemoveTileset.Name = "buttonRemoveTileset";
            this.buttonRemoveTileset.Size = new System.Drawing.Size(23, 25);
            this.buttonRemoveTileset.Text = "toolStripButton2";
            this.buttonRemoveTileset.ToolTipText = "Remove Tileset";
            this.buttonRemoveTileset.Click += new System.EventHandler(this.buttonRemoveTileset_Click);
            // 
            // comboTileset
            // 
            this.comboTileset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.comboTileset.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.comboTileset.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.comboTileset.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.comboTileset.ButtonIcon = ((System.Drawing.Bitmap)(resources.GetObject("comboTileset.ButtonIcon")));
            this.comboTileset.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.comboTileset.DrawDropdownHoverOutline = false;
            this.comboTileset.DrawFocusRectangle = false;
            this.comboTileset.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.comboTileset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboTileset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboTileset.ForeColor = System.Drawing.Color.Gainsboro;
            this.comboTileset.FormattingEnabled = true;
            this.comboTileset.Location = new System.Drawing.Point(0, 613);
            this.comboTileset.Name = "comboTileset";
            this.comboTileset.Size = new System.Drawing.Size(365, 21);
            this.comboTileset.TabIndex = 23;
            this.comboTileset.Text = null;
            this.comboTileset.TextPadding = new System.Windows.Forms.Padding(2);
            this.comboTileset.SelectedValueChanged += new System.EventHandler(this.comboTileset_SelectedValueChanged);
            // 
            // scrollX
            // 
            this.scrollX.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.scrollX.Location = new System.Drawing.Point(0, 590);
            this.scrollX.Name = "scrollX";
            this.scrollX.ScrollOrientation = DarkUI.Controls.DarkScrollOrientation.Horizontal;
            this.scrollX.Size = new System.Drawing.Size(365, 23);
            this.scrollX.TabIndex = 24;
            this.scrollX.Text = "scrollX";
            this.scrollX.ValueChanged += new System.EventHandler<DarkUI.Controls.ScrollValueEventArgs>(this.scrollX_ValueChanged);
            // 
            // DockTilesetTools
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.scrollX);
            this.Controls.Add(this.comboTileset);
            this.Controls.Add(this.darkToolStrip1);
            this.Controls.Add(this.scrollY);
            this.Controls.Add(this.tilesetView);
            this.DefaultDockArea = DarkUI.Docking.DarkDockArea.Right;
            this.DockText = "Tileset Tools";
            this.MinimumSize = new System.Drawing.Size(365, 633);
            this.Name = "DockTilesetTools";
            this.SerializationKey = "DockTilesetTools";
            this.Size = new System.Drawing.Size(386, 662);
            this.Load += new System.EventHandler(this.DockTilesetTools_Load);
            this.darkToolStrip1.ResumeLayout(false);
            this.darkToolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private View tilesetView;
        private DarkUI.Controls.DarkScrollBar scrollY;
        private DarkUI.Controls.DarkToolStrip darkToolStrip1;
        private System.Windows.Forms.ToolStripButton buttonAddTileset;
        private System.Windows.Forms.ToolStripButton buttonRemoveTileset;
        private DarkUI.Controls.DarkComboBox comboTileset;
        private DarkUI.Controls.DarkScrollBar scrollX;
    }
}
