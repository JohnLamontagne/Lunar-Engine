namespace Lunar.Editor.Controls
{
    partial class DockAnimationEditor
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
            this.darkToolStrip1 = new DarkUI.Controls.DarkToolStrip();
            this.buttonSave = new System.Windows.Forms.ToolStripButton();
            this.darkSectionPanel1 = new DarkUI.Controls.DarkSectionPanel();
            this.txtSurfaceLoopCount = new DarkUI.Controls.DarkTextBox();
            this.darkLabel9 = new DarkUI.Controls.DarkLabel();
            this.btnSelectSurfaceTex = new DarkUI.Controls.DarkButton();
            this.darkLabel1 = new DarkUI.Controls.DarkLabel();
            this.txtSurfaceFrameHeight = new DarkUI.Controls.DarkTextBox();
            this.darkLabel5 = new DarkUI.Controls.DarkLabel();
            this.txtSurfaceFrameWidth = new DarkUI.Controls.DarkTextBox();
            this.darkLabel4 = new DarkUI.Controls.DarkLabel();
            this.txtSurfaceFrameTime = new DarkUI.Controls.DarkTextBox();
            this.darkLabel3 = new DarkUI.Controls.DarkLabel();
            this.txtSurfaceTexPath = new DarkUI.Controls.DarkTextBox();
            this.darkSectionPanel2 = new DarkUI.Controls.DarkSectionPanel();
            this.txtSubSurfaceLoopCount = new DarkUI.Controls.DarkTextBox();
            this.darkLabel10 = new DarkUI.Controls.DarkLabel();
            this.txtSubSurfaceFrameHeight = new DarkUI.Controls.DarkTextBox();
            this.darkLabel6 = new DarkUI.Controls.DarkLabel();
            this.txtSubSurfaceFrameWidth = new DarkUI.Controls.DarkTextBox();
            this.darkLabel7 = new DarkUI.Controls.DarkLabel();
            this.txtSubSurfaceFrameTime = new DarkUI.Controls.DarkTextBox();
            this.darkLabel8 = new DarkUI.Controls.DarkLabel();
            this.txtSubSurfaceTexPath = new DarkUI.Controls.DarkTextBox();
            this.btnSelectSubSurfaceTex = new DarkUI.Controls.DarkButton();
            this.darkLabel2 = new DarkUI.Controls.DarkLabel();
            this.darkSectionPanel3 = new DarkUI.Controls.DarkSectionPanel();
            this.surfaceAnimView = new Lunar.Editor.Controls.View();
            this.darkSectionPanel4 = new DarkUI.Controls.DarkSectionPanel();
            this.subSurfaceAnimView = new Lunar.Editor.Controls.View();
            this.darkLabel11 = new DarkUI.Controls.DarkLabel();
            this.darkLabel12 = new DarkUI.Controls.DarkLabel();
            this.darkToolStrip1.SuspendLayout();
            this.darkSectionPanel1.SuspendLayout();
            this.darkSectionPanel2.SuspendLayout();
            this.darkSectionPanel3.SuspendLayout();
            this.darkSectionPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // darkToolStrip1
            // 
            this.darkToolStrip1.AutoSize = false;
            this.darkToolStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.darkToolStrip1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkToolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonSave});
            this.darkToolStrip1.Location = new System.Drawing.Point(0, 0);
            this.darkToolStrip1.Name = "darkToolStrip1";
            this.darkToolStrip1.Padding = new System.Windows.Forms.Padding(5, 0, 1, 0);
            this.darkToolStrip1.Size = new System.Drawing.Size(841, 28);
            this.darkToolStrip1.TabIndex = 1;
            this.darkToolStrip1.Text = "darkToolStrip1";
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
            // darkSectionPanel1
            // 
            this.darkSectionPanel1.Controls.Add(this.darkLabel11);
            this.darkSectionPanel1.Controls.Add(this.txtSurfaceLoopCount);
            this.darkSectionPanel1.Controls.Add(this.darkLabel9);
            this.darkSectionPanel1.Controls.Add(this.btnSelectSurfaceTex);
            this.darkSectionPanel1.Controls.Add(this.darkLabel1);
            this.darkSectionPanel1.Controls.Add(this.txtSurfaceFrameHeight);
            this.darkSectionPanel1.Controls.Add(this.darkLabel5);
            this.darkSectionPanel1.Controls.Add(this.txtSurfaceFrameWidth);
            this.darkSectionPanel1.Controls.Add(this.darkLabel4);
            this.darkSectionPanel1.Controls.Add(this.txtSurfaceFrameTime);
            this.darkSectionPanel1.Controls.Add(this.darkLabel3);
            this.darkSectionPanel1.Controls.Add(this.txtSurfaceTexPath);
            this.darkSectionPanel1.Location = new System.Drawing.Point(13, 31);
            this.darkSectionPanel1.Name = "darkSectionPanel1";
            this.darkSectionPanel1.SectionHeader = "Surface Animation Properties";
            this.darkSectionPanel1.Size = new System.Drawing.Size(400, 264);
            this.darkSectionPanel1.TabIndex = 2;
            // 
            // txtSurfaceLoopCount
            // 
            this.txtSurfaceLoopCount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtSurfaceLoopCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSurfaceLoopCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtSurfaceLoopCount.Location = new System.Drawing.Point(116, 213);
            this.txtSurfaceLoopCount.Name = "txtSurfaceLoopCount";
            this.txtSurfaceLoopCount.Size = new System.Drawing.Size(76, 23);
            this.txtSurfaceLoopCount.TabIndex = 20;
            this.txtSurfaceLoopCount.TextChanged += new System.EventHandler(this.txtSurfaceLoopCount_TextChanged);
            this.txtSurfaceLoopCount.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSurfaceLoopCount_KeyPress);
            // 
            // darkLabel9
            // 
            this.darkLabel9.AutoSize = true;
            this.darkLabel9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel9.Location = new System.Drawing.Point(12, 215);
            this.darkLabel9.Name = "darkLabel9";
            this.darkLabel9.Size = new System.Drawing.Size(73, 15);
            this.darkLabel9.TabIndex = 19;
            this.darkLabel9.Text = "Loop Count:";
            // 
            // btnSelectSurfaceTex
            // 
            this.btnSelectSurfaceTex.Location = new System.Drawing.Point(291, 47);
            this.btnSelectSurfaceTex.Name = "btnSelectSurfaceTex";
            this.btnSelectSurfaceTex.Padding = new System.Windows.Forms.Padding(5);
            this.btnSelectSurfaceTex.Size = new System.Drawing.Size(30, 20);
            this.btnSelectSurfaceTex.TabIndex = 18;
            this.btnSelectSurfaceTex.Text = "...";
            this.btnSelectSurfaceTex.Click += new System.EventHandler(this.btnSelectSurfaceTex_Click);
            // 
            // darkLabel1
            // 
            this.darkLabel1.AutoSize = true;
            this.darkLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel1.Location = new System.Drawing.Point(12, 47);
            this.darkLabel1.Name = "darkLabel1";
            this.darkLabel1.Size = new System.Drawing.Size(75, 15);
            this.darkLabel1.TabIndex = 17;
            this.darkLabel1.Text = "Texture Path:";
            // 
            // txtSurfaceFrameHeight
            // 
            this.txtSurfaceFrameHeight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtSurfaceFrameHeight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSurfaceFrameHeight.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtSurfaceFrameHeight.Location = new System.Drawing.Point(116, 172);
            this.txtSurfaceFrameHeight.Name = "txtSurfaceFrameHeight";
            this.txtSurfaceFrameHeight.Size = new System.Drawing.Size(76, 23);
            this.txtSurfaceFrameHeight.TabIndex = 13;
            this.txtSurfaceFrameHeight.TextChanged += new System.EventHandler(this.txtSurfaceFrameHeight_TextChanged);
            this.txtSurfaceFrameHeight.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSurfaceFrameHeight_KeyPress);
            // 
            // darkLabel5
            // 
            this.darkLabel5.AutoSize = true;
            this.darkLabel5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel5.Location = new System.Drawing.Point(12, 174);
            this.darkLabel5.Name = "darkLabel5";
            this.darkLabel5.Size = new System.Drawing.Size(82, 15);
            this.darkLabel5.TabIndex = 12;
            this.darkLabel5.Text = "Frame Height:";
            // 
            // txtSurfaceFrameWidth
            // 
            this.txtSurfaceFrameWidth.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtSurfaceFrameWidth.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSurfaceFrameWidth.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtSurfaceFrameWidth.Location = new System.Drawing.Point(116, 130);
            this.txtSurfaceFrameWidth.Name = "txtSurfaceFrameWidth";
            this.txtSurfaceFrameWidth.Size = new System.Drawing.Size(76, 23);
            this.txtSurfaceFrameWidth.TabIndex = 11;
            this.txtSurfaceFrameWidth.TextChanged += new System.EventHandler(this.txtSurfaceFrameWidth_TextChanged);
            this.txtSurfaceFrameWidth.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSurfaceFrameWidth_KeyPress);
            // 
            // darkLabel4
            // 
            this.darkLabel4.AutoSize = true;
            this.darkLabel4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel4.Location = new System.Drawing.Point(12, 132);
            this.darkLabel4.Name = "darkLabel4";
            this.darkLabel4.Size = new System.Drawing.Size(78, 15);
            this.darkLabel4.TabIndex = 10;
            this.darkLabel4.Text = "Frame Width:";
            // 
            // txtSurfaceFrameTime
            // 
            this.txtSurfaceFrameTime.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtSurfaceFrameTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSurfaceFrameTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtSurfaceFrameTime.Location = new System.Drawing.Point(116, 89);
            this.txtSurfaceFrameTime.Name = "txtSurfaceFrameTime";
            this.txtSurfaceFrameTime.Size = new System.Drawing.Size(76, 23);
            this.txtSurfaceFrameTime.TabIndex = 9;
            this.txtSurfaceFrameTime.TextChanged += new System.EventHandler(this.txtSurfaceFrameTime_TextChanged);
            this.txtSurfaceFrameTime.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSurfaceFrameTime_KeyPress);
            // 
            // darkLabel3
            // 
            this.darkLabel3.AutoSize = true;
            this.darkLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel3.Location = new System.Drawing.Point(12, 91);
            this.darkLabel3.Name = "darkLabel3";
            this.darkLabel3.Size = new System.Drawing.Size(100, 15);
            this.darkLabel3.TabIndex = 8;
            this.darkLabel3.Text = "Frame Time (ms):";
            // 
            // txtSurfaceTexPath
            // 
            this.txtSurfaceTexPath.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtSurfaceTexPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSurfaceTexPath.Enabled = false;
            this.txtSurfaceTexPath.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtSurfaceTexPath.Location = new System.Drawing.Point(91, 44);
            this.txtSurfaceTexPath.Name = "txtSurfaceTexPath";
            this.txtSurfaceTexPath.Size = new System.Drawing.Size(194, 23);
            this.txtSurfaceTexPath.TabIndex = 5;
            // 
            // darkSectionPanel2
            // 
            this.darkSectionPanel2.Controls.Add(this.darkLabel12);
            this.darkSectionPanel2.Controls.Add(this.txtSubSurfaceLoopCount);
            this.darkSectionPanel2.Controls.Add(this.darkLabel10);
            this.darkSectionPanel2.Controls.Add(this.txtSubSurfaceFrameHeight);
            this.darkSectionPanel2.Controls.Add(this.darkLabel6);
            this.darkSectionPanel2.Controls.Add(this.txtSubSurfaceFrameWidth);
            this.darkSectionPanel2.Controls.Add(this.darkLabel7);
            this.darkSectionPanel2.Controls.Add(this.txtSubSurfaceFrameTime);
            this.darkSectionPanel2.Controls.Add(this.darkLabel8);
            this.darkSectionPanel2.Controls.Add(this.txtSubSurfaceTexPath);
            this.darkSectionPanel2.Controls.Add(this.btnSelectSubSurfaceTex);
            this.darkSectionPanel2.Controls.Add(this.darkLabel2);
            this.darkSectionPanel2.Location = new System.Drawing.Point(419, 31);
            this.darkSectionPanel2.Name = "darkSectionPanel2";
            this.darkSectionPanel2.SectionHeader = "Subsurface Animation Properties";
            this.darkSectionPanel2.Size = new System.Drawing.Size(400, 264);
            this.darkSectionPanel2.TabIndex = 3;
            // 
            // txtSubSurfaceLoopCount
            // 
            this.txtSubSurfaceLoopCount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtSubSurfaceLoopCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSubSurfaceLoopCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtSubSurfaceLoopCount.Location = new System.Drawing.Point(119, 213);
            this.txtSubSurfaceLoopCount.Name = "txtSubSurfaceLoopCount";
            this.txtSubSurfaceLoopCount.Size = new System.Drawing.Size(76, 23);
            this.txtSubSurfaceLoopCount.TabIndex = 24;
            this.txtSubSurfaceLoopCount.TextChanged += new System.EventHandler(this.txtSubSurfaceLoopCount_TextChanged);
            this.txtSubSurfaceLoopCount.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSubSurfaceLoopCount_KeyPress);
            // 
            // darkLabel10
            // 
            this.darkLabel10.AutoSize = true;
            this.darkLabel10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel10.Location = new System.Drawing.Point(13, 215);
            this.darkLabel10.Name = "darkLabel10";
            this.darkLabel10.Size = new System.Drawing.Size(73, 15);
            this.darkLabel10.TabIndex = 23;
            this.darkLabel10.Text = "Loop Count:";
            // 
            // txtSubSurfaceFrameHeight
            // 
            this.txtSubSurfaceFrameHeight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtSubSurfaceFrameHeight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSubSurfaceFrameHeight.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtSubSurfaceFrameHeight.Location = new System.Drawing.Point(119, 172);
            this.txtSubSurfaceFrameHeight.Name = "txtSubSurfaceFrameHeight";
            this.txtSubSurfaceFrameHeight.Size = new System.Drawing.Size(76, 23);
            this.txtSubSurfaceFrameHeight.TabIndex = 22;
            this.txtSubSurfaceFrameHeight.TextChanged += new System.EventHandler(this.txtSubSurfaceFrameHeight_TextChanged);
            this.txtSubSurfaceFrameHeight.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSubSurfaceFrameHeight_KeyPress);
            // 
            // darkLabel6
            // 
            this.darkLabel6.AutoSize = true;
            this.darkLabel6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel6.Location = new System.Drawing.Point(13, 174);
            this.darkLabel6.Name = "darkLabel6";
            this.darkLabel6.Size = new System.Drawing.Size(82, 15);
            this.darkLabel6.TabIndex = 21;
            this.darkLabel6.Text = "Frame Height:";
            // 
            // txtSubSurfaceFrameWidth
            // 
            this.txtSubSurfaceFrameWidth.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtSubSurfaceFrameWidth.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSubSurfaceFrameWidth.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtSubSurfaceFrameWidth.Location = new System.Drawing.Point(119, 130);
            this.txtSubSurfaceFrameWidth.Name = "txtSubSurfaceFrameWidth";
            this.txtSubSurfaceFrameWidth.Size = new System.Drawing.Size(76, 23);
            this.txtSubSurfaceFrameWidth.TabIndex = 20;
            this.txtSubSurfaceFrameWidth.TextChanged += new System.EventHandler(this.txtSubSurfaceFrameWidth_TextChanged);
            this.txtSubSurfaceFrameWidth.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSubSurfaceFrameWidth_KeyPress);
            // 
            // darkLabel7
            // 
            this.darkLabel7.AutoSize = true;
            this.darkLabel7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel7.Location = new System.Drawing.Point(13, 132);
            this.darkLabel7.Name = "darkLabel7";
            this.darkLabel7.Size = new System.Drawing.Size(78, 15);
            this.darkLabel7.TabIndex = 19;
            this.darkLabel7.Text = "Frame Width:";
            // 
            // txtSubSurfaceFrameTime
            // 
            this.txtSubSurfaceFrameTime.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtSubSurfaceFrameTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSubSurfaceFrameTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtSubSurfaceFrameTime.Location = new System.Drawing.Point(119, 89);
            this.txtSubSurfaceFrameTime.Name = "txtSubSurfaceFrameTime";
            this.txtSubSurfaceFrameTime.Size = new System.Drawing.Size(76, 23);
            this.txtSubSurfaceFrameTime.TabIndex = 18;
            this.txtSubSurfaceFrameTime.TextChanged += new System.EventHandler(this.txtSubSurfaceFrameTime_TextChanged);
            this.txtSubSurfaceFrameTime.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSubSurfaceFrameTime_KeyPress);
            // 
            // darkLabel8
            // 
            this.darkLabel8.AutoSize = true;
            this.darkLabel8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel8.Location = new System.Drawing.Point(13, 91);
            this.darkLabel8.Name = "darkLabel8";
            this.darkLabel8.Size = new System.Drawing.Size(100, 15);
            this.darkLabel8.TabIndex = 17;
            this.darkLabel8.Text = "Frame Time (ms):";
            // 
            // txtSubSurfaceTexPath
            // 
            this.txtSubSurfaceTexPath.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtSubSurfaceTexPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSubSurfaceTexPath.Enabled = false;
            this.txtSubSurfaceTexPath.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtSubSurfaceTexPath.Location = new System.Drawing.Point(94, 40);
            this.txtSubSurfaceTexPath.Name = "txtSubSurfaceTexPath";
            this.txtSubSurfaceTexPath.Size = new System.Drawing.Size(194, 23);
            this.txtSubSurfaceTexPath.TabIndex = 16;
            // 
            // btnSelectSubSurfaceTex
            // 
            this.btnSelectSubSurfaceTex.Location = new System.Drawing.Point(294, 43);
            this.btnSelectSubSurfaceTex.Name = "btnSelectSubSurfaceTex";
            this.btnSelectSubSurfaceTex.Padding = new System.Windows.Forms.Padding(5);
            this.btnSelectSubSurfaceTex.Size = new System.Drawing.Size(30, 20);
            this.btnSelectSubSurfaceTex.TabIndex = 10;
            this.btnSelectSubSurfaceTex.Text = "...";
            this.btnSelectSubSurfaceTex.Click += new System.EventHandler(this.btnSelectSubSurfaceTex_Click);
            // 
            // darkLabel2
            // 
            this.darkLabel2.AutoSize = true;
            this.darkLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel2.Location = new System.Drawing.Point(13, 45);
            this.darkLabel2.Name = "darkLabel2";
            this.darkLabel2.Size = new System.Drawing.Size(75, 15);
            this.darkLabel2.TabIndex = 9;
            this.darkLabel2.Text = "Texture Path:";
            // 
            // darkSectionPanel3
            // 
            this.darkSectionPanel3.Controls.Add(this.surfaceAnimView);
            this.darkSectionPanel3.Location = new System.Drawing.Point(13, 312);
            this.darkSectionPanel3.Name = "darkSectionPanel3";
            this.darkSectionPanel3.SectionHeader = "Surface Animation Preview";
            this.darkSectionPanel3.Size = new System.Drawing.Size(400, 333);
            this.darkSectionPanel3.TabIndex = 4;
            // 
            // surfaceAnimView
            // 
            this.surfaceAnimView.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.surfaceAnimView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.surfaceAnimView.Location = new System.Drawing.Point(1, 25);
            this.surfaceAnimView.Name = "surfaceAnimView";
            this.surfaceAnimView.OnDraw = null;
            this.surfaceAnimView.OnInitalize = null;
            this.surfaceAnimView.OnUpdate = null;
            this.surfaceAnimView.Size = new System.Drawing.Size(398, 307);
            this.surfaceAnimView.SuspendOnFormInactive = false;
            this.surfaceAnimView.TabIndex = 15;
            this.surfaceAnimView.Load += new System.EventHandler(this.surfaceAnimView_Load);
            // 
            // darkSectionPanel4
            // 
            this.darkSectionPanel4.Controls.Add(this.subSurfaceAnimView);
            this.darkSectionPanel4.Location = new System.Drawing.Point(419, 312);
            this.darkSectionPanel4.Name = "darkSectionPanel4";
            this.darkSectionPanel4.SectionHeader = "Subsurface Animation Preview";
            this.darkSectionPanel4.Size = new System.Drawing.Size(400, 333);
            this.darkSectionPanel4.TabIndex = 5;
            // 
            // subSurfaceAnimView
            // 
            this.subSurfaceAnimView.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.subSurfaceAnimView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.subSurfaceAnimView.Location = new System.Drawing.Point(1, 25);
            this.subSurfaceAnimView.Name = "subSurfaceAnimView";
            this.subSurfaceAnimView.OnDraw = null;
            this.subSurfaceAnimView.OnInitalize = null;
            this.subSurfaceAnimView.OnUpdate = null;
            this.subSurfaceAnimView.Size = new System.Drawing.Size(398, 307);
            this.subSurfaceAnimView.SuspendOnFormInactive = false;
            this.subSurfaceAnimView.TabIndex = 16;
            this.subSurfaceAnimView.Load += new System.EventHandler(this.subSurfaceAnimView_Load);
            // 
            // darkLabel11
            // 
            this.darkLabel11.AutoSize = true;
            this.darkLabel11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel11.Location = new System.Drawing.Point(198, 221);
            this.darkLabel11.Name = "darkLabel11";
            this.darkLabel11.Size = new System.Drawing.Size(71, 15);
            this.darkLabel11.TabIndex = 21;
            this.darkLabel11.Text = "0 for infinity";
            // 
            // darkLabel12
            // 
            this.darkLabel12.AutoSize = true;
            this.darkLabel12.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel12.Location = new System.Drawing.Point(201, 215);
            this.darkLabel12.Name = "darkLabel12";
            this.darkLabel12.Size = new System.Drawing.Size(71, 15);
            this.darkLabel12.TabIndex = 25;
            this.darkLabel12.Text = "0 for infinity";
            // 
            // DockAnimationEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.darkSectionPanel4);
            this.Controls.Add(this.darkSectionPanel3);
            this.Controls.Add(this.darkSectionPanel2);
            this.Controls.Add(this.darkSectionPanel1);
            this.Controls.Add(this.darkToolStrip1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "DockAnimationEditor";
            this.Size = new System.Drawing.Size(841, 689);
            this.Load += new System.EventHandler(this.DockItemEditor_Load);
            this.darkToolStrip1.ResumeLayout(false);
            this.darkToolStrip1.PerformLayout();
            this.darkSectionPanel1.ResumeLayout(false);
            this.darkSectionPanel1.PerformLayout();
            this.darkSectionPanel2.ResumeLayout(false);
            this.darkSectionPanel2.PerformLayout();
            this.darkSectionPanel3.ResumeLayout(false);
            this.darkSectionPanel4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private DarkUI.Controls.DarkToolStrip darkToolStrip1;
        private System.Windows.Forms.ToolStripButton buttonSave;
        private DarkUI.Controls.DarkSectionPanel darkSectionPanel1;
        private DarkUI.Controls.DarkTextBox txtSurfaceTexPath;
        private DarkUI.Controls.DarkSectionPanel darkSectionPanel2;
        private DarkUI.Controls.DarkButton btnSelectSubSurfaceTex;
        private DarkUI.Controls.DarkLabel darkLabel2;
        private DarkUI.Controls.DarkTextBox txtSurfaceFrameHeight;
        private DarkUI.Controls.DarkLabel darkLabel5;
        private DarkUI.Controls.DarkTextBox txtSurfaceFrameWidth;
        private DarkUI.Controls.DarkLabel darkLabel4;
        private DarkUI.Controls.DarkTextBox txtSurfaceFrameTime;
        private DarkUI.Controls.DarkLabel darkLabel3;
        private DarkUI.Controls.DarkTextBox txtSubSurfaceTexPath;
        private DarkUI.Controls.DarkTextBox txtSubSurfaceFrameHeight;
        private DarkUI.Controls.DarkLabel darkLabel6;
        private DarkUI.Controls.DarkLabel darkLabel7;
        private DarkUI.Controls.DarkTextBox txtSubSurfaceFrameTime;
        private DarkUI.Controls.DarkLabel darkLabel8;
        private DarkUI.Controls.DarkSectionPanel darkSectionPanel3;
        private DarkUI.Controls.DarkSectionPanel darkSectionPanel4;
        private DarkUI.Controls.DarkLabel darkLabel1;
        private View surfaceAnimView;
        private View subSurfaceAnimView;
        private DarkUI.Controls.DarkButton btnSelectSurfaceTex;
        private DarkUI.Controls.DarkTextBox txtSubSurfaceFrameWidth;
        private DarkUI.Controls.DarkTextBox txtSurfaceLoopCount;
        private DarkUI.Controls.DarkLabel darkLabel9;
        private DarkUI.Controls.DarkTextBox txtSubSurfaceLoopCount;
        private DarkUI.Controls.DarkLabel darkLabel10;
        private DarkUI.Controls.DarkLabel darkLabel11;
        private DarkUI.Controls.DarkLabel darkLabel12;
    }
}
