namespace Lunar.Editor.Controls
{
    partial class DockDialogueDocument
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DockDialogueDocument));
            this.darkToolStrip3 = new DarkUI.Controls.DarkToolStrip();
            this.buttonSave = new System.Windows.Forms.ToolStripButton();
            this.branchPanel = new DarkUI.Controls.DarkSectionPanel();
            this.darkSectionPanel2 = new DarkUI.Controls.DarkSectionPanel();
            this.darkToolStrip2 = new DarkUI.Controls.DarkToolStrip();
            this.btnAddResponse = new System.Windows.Forms.ToolStripButton();
            this.btnRemoveResponse = new System.Windows.Forms.ToolStripButton();
            this.lstResponses = new DarkUI.Controls.DarkListView();
            this.responsePanel = new DarkUI.Controls.DarkSectionPanel();
            this.cmbDisplayCond = new DarkUI.Controls.DarkComboBox();
            this.darkLabel3 = new DarkUI.Controls.DarkLabel();
            this.cmbNextBranch = new DarkUI.Controls.DarkComboBox();
            this.darkLabel2 = new DarkUI.Controls.DarkLabel();
            this.txtResponseText = new DarkUI.Controls.DarkTextBox();
            this.cmbResponseFunction = new DarkUI.Controls.DarkComboBox();
            this.darkLabel1 = new DarkUI.Controls.DarkLabel();
            this.txtBranchText = new DarkUI.Controls.DarkTextBox();
            this.darkSectionPanel1 = new DarkUI.Controls.DarkSectionPanel();
            this.darkToolStrip1 = new DarkUI.Controls.DarkToolStrip();
            this.buttonAddBranch = new System.Windows.Forms.ToolStripButton();
            this.buttonRemoveBranch = new System.Windows.Forms.ToolStripButton();
            this.lstBranches = new DarkUI.Controls.DarkListView();
            this.darkToolStrip3.SuspendLayout();
            this.branchPanel.SuspendLayout();
            this.darkSectionPanel2.SuspendLayout();
            this.darkToolStrip2.SuspendLayout();
            this.responsePanel.SuspendLayout();
            this.darkSectionPanel1.SuspendLayout();
            this.darkToolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // darkToolStrip3
            // 
            this.darkToolStrip3.AutoSize = false;
            this.darkToolStrip3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.darkToolStrip3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkToolStrip3.ImageScalingSize = new System.Drawing.Size(28, 28);
            this.darkToolStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonSave});
            this.darkToolStrip3.Location = new System.Drawing.Point(0, 0);
            this.darkToolStrip3.Name = "darkToolStrip3";
            this.darkToolStrip3.Padding = new System.Windows.Forms.Padding(5, 0, 1, 0);
            this.darkToolStrip3.Size = new System.Drawing.Size(1299, 28);
            this.darkToolStrip3.TabIndex = 24;
            this.darkToolStrip3.Text = "darkToolStrip3";
            // 
            // buttonSave
            // 
            this.buttonSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonSave.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.buttonSave.Image = global::Lunar.Editor.Icons.document_16xLG;
            this.buttonSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(40, 22);
            this.buttonSave.Text = "toolStripButton1";
            this.buttonSave.ToolTipText = "Save";
            this.buttonSave.Click += new System.EventHandler(this.ButtonSave_Click);
            // 
            // branchPanel
            // 
            this.branchPanel.Controls.Add(this.darkSectionPanel2);
            this.branchPanel.Controls.Add(this.responsePanel);
            this.branchPanel.Controls.Add(this.txtBranchText);
            this.branchPanel.Location = new System.Drawing.Point(373, 31);
            this.branchPanel.Name = "branchPanel";
            this.branchPanel.SectionHeader = "Dialogue Branch";
            this.branchPanel.Size = new System.Drawing.Size(895, 757);
            this.branchPanel.TabIndex = 26;
            this.branchPanel.Visible = false;
            // 
            // darkSectionPanel2
            // 
            this.darkSectionPanel2.Controls.Add(this.darkToolStrip2);
            this.darkSectionPanel2.Controls.Add(this.lstResponses);
            this.darkSectionPanel2.Location = new System.Drawing.Point(6, 271);
            this.darkSectionPanel2.Name = "darkSectionPanel2";
            this.darkSectionPanel2.SectionHeader = "Responses";
            this.darkSectionPanel2.Size = new System.Drawing.Size(200, 482);
            this.darkSectionPanel2.TabIndex = 25;
            // 
            // darkToolStrip2
            // 
            this.darkToolStrip2.AutoSize = false;
            this.darkToolStrip2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.darkToolStrip2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.darkToolStrip2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkToolStrip2.ImageScalingSize = new System.Drawing.Size(28, 28);
            this.darkToolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAddResponse,
            this.btnRemoveResponse});
            this.darkToolStrip2.Location = new System.Drawing.Point(1, 435);
            this.darkToolStrip2.Name = "darkToolStrip2";
            this.darkToolStrip2.Padding = new System.Windows.Forms.Padding(5, 0, 1, 0);
            this.darkToolStrip2.Size = new System.Drawing.Size(198, 46);
            this.darkToolStrip2.TabIndex = 25;
            this.darkToolStrip2.Text = "darkToolStrip2";
            // 
            // btnAddResponse
            // 
            this.btnAddResponse.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAddResponse.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.btnAddResponse.Image = ((System.Drawing.Image)(resources.GetObject("btnAddResponse.Image")));
            this.btnAddResponse.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.btnAddResponse.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddResponse.Name = "btnAddResponse";
            this.btnAddResponse.Size = new System.Drawing.Size(40, 40);
            this.btnAddResponse.Text = "toolStripButton1";
            this.btnAddResponse.ToolTipText = "Add Response";
            this.btnAddResponse.Click += new System.EventHandler(this.BtnAddResponse_Click);
            // 
            // btnRemoveResponse
            // 
            this.btnRemoveResponse.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRemoveResponse.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.btnRemoveResponse.Image = ((System.Drawing.Image)(resources.GetObject("btnRemoveResponse.Image")));
            this.btnRemoveResponse.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.btnRemoveResponse.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRemoveResponse.Name = "btnRemoveResponse";
            this.btnRemoveResponse.Size = new System.Drawing.Size(40, 40);
            this.btnRemoveResponse.Text = "toolStripButton2";
            this.btnRemoveResponse.ToolTipText = "Remove Response";
            this.btnRemoveResponse.Click += new System.EventHandler(this.BtnRemoveResponse_Click);
            // 
            // lstResponses
            // 
            this.lstResponses.Location = new System.Drawing.Point(4, 41);
            this.lstResponses.Name = "lstResponses";
            this.lstResponses.Size = new System.Drawing.Size(192, 383);
            this.lstResponses.TabIndex = 5;
            this.lstResponses.Text = "darkListView2";
            this.lstResponses.SelectedIndicesChanged += new System.EventHandler(this.LstResponses_SelectedIndicesChanged);
            // 
            // responsePanel
            // 
            this.responsePanel.Controls.Add(this.cmbDisplayCond);
            this.responsePanel.Controls.Add(this.darkLabel3);
            this.responsePanel.Controls.Add(this.cmbNextBranch);
            this.responsePanel.Controls.Add(this.darkLabel2);
            this.responsePanel.Controls.Add(this.txtResponseText);
            this.responsePanel.Controls.Add(this.cmbResponseFunction);
            this.responsePanel.Controls.Add(this.darkLabel1);
            this.responsePanel.Location = new System.Drawing.Point(212, 271);
            this.responsePanel.Name = "responsePanel";
            this.responsePanel.SectionHeader = "Response";
            this.responsePanel.Size = new System.Drawing.Size(634, 481);
            this.responsePanel.TabIndex = 3;
            this.responsePanel.Visible = false;
            // 
            // cmbDisplayCond
            // 
            this.cmbDisplayCond.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbDisplayCond.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbDisplayCond.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbDisplayCond.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbDisplayCond.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbDisplayCond.ButtonIcon = ((System.Drawing.Bitmap)(resources.GetObject("cmbDisplayCond.ButtonIcon")));
            this.cmbDisplayCond.DrawDropdownHoverOutline = false;
            this.cmbDisplayCond.DrawFocusRectangle = false;
            this.cmbDisplayCond.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cmbDisplayCond.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDisplayCond.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbDisplayCond.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbDisplayCond.FormattingEnabled = true;
            this.cmbDisplayCond.Location = new System.Drawing.Point(212, 347);
            this.cmbDisplayCond.Name = "cmbDisplayCond";
            this.cmbDisplayCond.Size = new System.Drawing.Size(282, 30);
            this.cmbDisplayCond.TabIndex = 9;
            this.cmbDisplayCond.Text = null;
            this.cmbDisplayCond.TextPadding = new System.Windows.Forms.Padding(2);
            this.cmbDisplayCond.SelectedIndexChanged += new System.EventHandler(this.CmbDisplayCond_SelectedIndexChanged);
            // 
            // darkLabel3
            // 
            this.darkLabel3.AutoSize = true;
            this.darkLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel3.Location = new System.Drawing.Point(56, 350);
            this.darkLabel3.Name = "darkLabel3";
            this.darkLabel3.Size = new System.Drawing.Size(150, 25);
            this.darkLabel3.TabIndex = 8;
            this.darkLabel3.Text = "Disp. Condition:";
            // 
            // cmbNextBranch
            // 
            this.cmbNextBranch.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbNextBranch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbNextBranch.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbNextBranch.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbNextBranch.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbNextBranch.ButtonIcon = ((System.Drawing.Bitmap)(resources.GetObject("cmbNextBranch.ButtonIcon")));
            this.cmbNextBranch.DrawDropdownHoverOutline = false;
            this.cmbNextBranch.DrawFocusRectangle = false;
            this.cmbNextBranch.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cmbNextBranch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbNextBranch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbNextBranch.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbNextBranch.FormattingEnabled = true;
            this.cmbNextBranch.Location = new System.Drawing.Point(212, 251);
            this.cmbNextBranch.Name = "cmbNextBranch";
            this.cmbNextBranch.Size = new System.Drawing.Size(282, 30);
            this.cmbNextBranch.TabIndex = 7;
            this.cmbNextBranch.Text = null;
            this.cmbNextBranch.TextPadding = new System.Windows.Forms.Padding(2);
            this.cmbNextBranch.SelectedValueChanged += new System.EventHandler(this.CmbNextBranch_SelectedValueChanged);
            // 
            // darkLabel2
            // 
            this.darkLabel2.AutoSize = true;
            this.darkLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel2.Location = new System.Drawing.Point(81, 251);
            this.darkLabel2.Name = "darkLabel2";
            this.darkLabel2.Size = new System.Drawing.Size(125, 25);
            this.darkLabel2.TabIndex = 6;
            this.darkLabel2.Text = "Next Branch:";
            // 
            // txtResponseText
            // 
            this.txtResponseText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtResponseText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtResponseText.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.14286F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtResponseText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtResponseText.Location = new System.Drawing.Point(29, 41);
            this.txtResponseText.Multiline = true;
            this.txtResponseText.Name = "txtResponseText";
            this.txtResponseText.Size = new System.Drawing.Size(581, 147);
            this.txtResponseText.TabIndex = 5;
            this.txtResponseText.Text = "Enter response text here...";
            this.txtResponseText.TextChanged += new System.EventHandler(this.TxtResponseText_TextChanged);
            // 
            // cmbResponseFunction
            // 
            this.cmbResponseFunction.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbResponseFunction.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbResponseFunction.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.cmbResponseFunction.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.cmbResponseFunction.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.cmbResponseFunction.ButtonIcon = ((System.Drawing.Bitmap)(resources.GetObject("cmbResponseFunction.ButtonIcon")));
            this.cmbResponseFunction.DrawDropdownHoverOutline = false;
            this.cmbResponseFunction.DrawFocusRectangle = false;
            this.cmbResponseFunction.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cmbResponseFunction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbResponseFunction.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbResponseFunction.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbResponseFunction.FormattingEnabled = true;
            this.cmbResponseFunction.Location = new System.Drawing.Point(212, 300);
            this.cmbResponseFunction.Name = "cmbResponseFunction";
            this.cmbResponseFunction.Size = new System.Drawing.Size(282, 30);
            this.cmbResponseFunction.TabIndex = 4;
            this.cmbResponseFunction.Text = null;
            this.cmbResponseFunction.TextPadding = new System.Windows.Forms.Padding(2);
            this.cmbResponseFunction.SelectedIndexChanged += new System.EventHandler(this.CmbResponseFunction_SelectedIndexChanged);
            // 
            // darkLabel1
            // 
            this.darkLabel1.AutoSize = true;
            this.darkLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel1.Location = new System.Drawing.Point(113, 305);
            this.darkLabel1.Name = "darkLabel1";
            this.darkLabel1.Size = new System.Drawing.Size(93, 25);
            this.darkLabel1.TabIndex = 3;
            this.darkLabel1.Text = "Function:";
            // 
            // txtBranchText
            // 
            this.txtBranchText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtBranchText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBranchText.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.14286F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBranchText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtBranchText.Location = new System.Drawing.Point(24, 43);
            this.txtBranchText.Multiline = true;
            this.txtBranchText.Name = "txtBranchText";
            this.txtBranchText.Size = new System.Drawing.Size(822, 199);
            this.txtBranchText.TabIndex = 0;
            this.txtBranchText.Text = "Enter branch dialogue text here...";
            this.txtBranchText.TextChanged += new System.EventHandler(this.TxtBranchText_TextChanged);
            // 
            // darkSectionPanel1
            // 
            this.darkSectionPanel1.Controls.Add(this.darkToolStrip1);
            this.darkSectionPanel1.Controls.Add(this.lstBranches);
            this.darkSectionPanel1.Location = new System.Drawing.Point(3, 31);
            this.darkSectionPanel1.Name = "darkSectionPanel1";
            this.darkSectionPanel1.SectionHeader = "Branches";
            this.darkSectionPanel1.Size = new System.Drawing.Size(364, 761);
            this.darkSectionPanel1.TabIndex = 28;
            // 
            // darkToolStrip1
            // 
            this.darkToolStrip1.AutoSize = false;
            this.darkToolStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.darkToolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.darkToolStrip1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkToolStrip1.ImageScalingSize = new System.Drawing.Size(28, 28);
            this.darkToolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonAddBranch,
            this.buttonRemoveBranch});
            this.darkToolStrip1.Location = new System.Drawing.Point(1, 706);
            this.darkToolStrip1.Name = "darkToolStrip1";
            this.darkToolStrip1.Padding = new System.Windows.Forms.Padding(5, 0, 1, 0);
            this.darkToolStrip1.Size = new System.Drawing.Size(362, 54);
            this.darkToolStrip1.TabIndex = 28;
            this.darkToolStrip1.Text = "darkToolStrip1";
            // 
            // buttonAddBranch
            // 
            this.buttonAddBranch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonAddBranch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.buttonAddBranch.Image = ((System.Drawing.Image)(resources.GetObject("buttonAddBranch.Image")));
            this.buttonAddBranch.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.buttonAddBranch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonAddBranch.Name = "buttonAddBranch";
            this.buttonAddBranch.Size = new System.Drawing.Size(40, 48);
            this.buttonAddBranch.Text = "toolStripButton1";
            this.buttonAddBranch.ToolTipText = "Add Branch";
            this.buttonAddBranch.Click += new System.EventHandler(this.ButtonAddBranch_Click);
            // 
            // buttonRemoveBranch
            // 
            this.buttonRemoveBranch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonRemoveBranch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.buttonRemoveBranch.Image = ((System.Drawing.Image)(resources.GetObject("buttonRemoveBranch.Image")));
            this.buttonRemoveBranch.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.buttonRemoveBranch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonRemoveBranch.Name = "buttonRemoveBranch";
            this.buttonRemoveBranch.Size = new System.Drawing.Size(40, 48);
            this.buttonRemoveBranch.Text = "toolStripButton2";
            this.buttonRemoveBranch.ToolTipText = "Remove Branch";
            this.buttonRemoveBranch.Click += new System.EventHandler(this.ButtonRemoveBranch_Click);
            // 
            // lstBranches
            // 
            this.lstBranches.Location = new System.Drawing.Point(6, 43);
            this.lstBranches.Name = "lstBranches";
            this.lstBranches.Size = new System.Drawing.Size(354, 652);
            this.lstBranches.TabIndex = 26;
            this.lstBranches.Text = "darkListView1";
            this.lstBranches.SelectedIndicesChanged += new System.EventHandler(this.LstBranches_SelectedIndicesChanged);
            // 
            // DockDialogueDocument
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.darkSectionPanel1);
            this.Controls.Add(this.darkToolStrip3);
            this.Controls.Add(this.branchPanel);
            this.Name = "DockDialogueDocument";
            this.Size = new System.Drawing.Size(1299, 808);
            this.darkToolStrip3.ResumeLayout(false);
            this.darkToolStrip3.PerformLayout();
            this.branchPanel.ResumeLayout(false);
            this.branchPanel.PerformLayout();
            this.darkSectionPanel2.ResumeLayout(false);
            this.darkToolStrip2.ResumeLayout(false);
            this.darkToolStrip2.PerformLayout();
            this.responsePanel.ResumeLayout(false);
            this.responsePanel.PerformLayout();
            this.darkSectionPanel1.ResumeLayout(false);
            this.darkToolStrip1.ResumeLayout(false);
            this.darkToolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private DarkUI.Controls.DarkToolStrip darkToolStrip3;
        private System.Windows.Forms.ToolStripButton buttonSave;
        private DarkUI.Controls.DarkSectionPanel branchPanel;
        private DarkUI.Controls.DarkSectionPanel responsePanel;
        private DarkUI.Controls.DarkComboBox cmbDisplayCond;
        private DarkUI.Controls.DarkLabel darkLabel3;
        private DarkUI.Controls.DarkComboBox cmbNextBranch;
        private DarkUI.Controls.DarkLabel darkLabel2;
        private DarkUI.Controls.DarkTextBox txtResponseText;
        private DarkUI.Controls.DarkComboBox cmbResponseFunction;
        private DarkUI.Controls.DarkLabel darkLabel1;
        private DarkUI.Controls.DarkTextBox txtBranchText;
        private DarkUI.Controls.DarkSectionPanel darkSectionPanel1;
        private DarkUI.Controls.DarkListView lstBranches;
        private DarkUI.Controls.DarkSectionPanel darkSectionPanel2;
        private DarkUI.Controls.DarkListView lstResponses;
        private DarkUI.Controls.DarkToolStrip darkToolStrip2;
        private System.Windows.Forms.ToolStripButton btnAddResponse;
        private System.Windows.Forms.ToolStripButton btnRemoveResponse;
        private DarkUI.Controls.DarkToolStrip darkToolStrip1;
        private System.Windows.Forms.ToolStripButton buttonAddBranch;
        private System.Windows.Forms.ToolStripButton buttonRemoveBranch;
    }
}
