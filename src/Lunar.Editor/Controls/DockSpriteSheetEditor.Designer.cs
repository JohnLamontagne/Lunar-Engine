namespace Lunar.Editor.Controls
{
    partial class DockSpriteSheetEditor
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
            this.picTexture = new System.Windows.Forms.PictureBox();
            this.darkLabel9 = new DarkUI.Controls.DarkLabel();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.darkLabel3 = new DarkUI.Controls.DarkLabel();
            this.darkLabel2 = new DarkUI.Controls.DarkLabel();
            this.radioNotStackable = new DarkUI.Controls.DarkRadioButton();
            this.radioStackable = new DarkUI.Controls.DarkRadioButton();
            this.darkLabel1 = new DarkUI.Controls.DarkLabel();
            this.txtName = new DarkUI.Controls.DarkTextBox();
            this.darkSectionPanel3 = new DarkUI.Controls.DarkSectionPanel();
            this.txtEditor = new ScintillaNET.Scintilla();
            this.darkMenuStrip1 = new DarkUI.Controls.DarkMenuStrip();
            this.scriptsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.onUseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.onEquipToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.onAcquiredToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.onDroppedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.onCreatedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.darkSectionPanel4 = new DarkUI.Controls.DarkSectionPanel();
            this.cmbEquipmentSlot = new System.Windows.Forms.ComboBox();
            this.darkLabel10 = new DarkUI.Controls.DarkLabel();
            this.darkSectionPanel2 = new DarkUI.Controls.DarkSectionPanel();
            this.txtHealth = new DarkUI.Controls.DarkTextBox();
            this.txtDef = new DarkUI.Controls.DarkTextBox();
            this.txtDex = new DarkUI.Controls.DarkTextBox();
            this.txtStr = new DarkUI.Controls.DarkTextBox();
            this.txtInt = new DarkUI.Controls.DarkTextBox();
            this.darkLabel8 = new DarkUI.Controls.DarkLabel();
            this.darkLabel7 = new DarkUI.Controls.DarkLabel();
            this.darkLabel6 = new DarkUI.Controls.DarkLabel();
            this.darkLabel5 = new DarkUI.Controls.DarkLabel();
            this.darkLabel4 = new DarkUI.Controls.DarkLabel();
            this.darkToolStrip1.SuspendLayout();
            this.darkSectionPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picTexture)).BeginInit();
            this.darkSectionPanel3.SuspendLayout();
            this.darkMenuStrip1.SuspendLayout();
            this.darkSectionPanel4.SuspendLayout();
            this.darkSectionPanel2.SuspendLayout();
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
            this.darkToolStrip1.Size = new System.Drawing.Size(1221, 28);
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
            this.darkSectionPanel1.Controls.Add(this.picTexture);
            this.darkSectionPanel1.Controls.Add(this.darkLabel9);
            this.darkSectionPanel1.Controls.Add(this.cmbType);
            this.darkSectionPanel1.Controls.Add(this.darkLabel3);
            this.darkSectionPanel1.Controls.Add(this.darkLabel2);
            this.darkSectionPanel1.Controls.Add(this.radioNotStackable);
            this.darkSectionPanel1.Controls.Add(this.radioStackable);
            this.darkSectionPanel1.Controls.Add(this.darkLabel1);
            this.darkSectionPanel1.Controls.Add(this.txtName);
            this.darkSectionPanel1.Location = new System.Drawing.Point(13, 31);
            this.darkSectionPanel1.Name = "darkSectionPanel1";
            this.darkSectionPanel1.SectionHeader = "Core Properties";
            this.darkSectionPanel1.Size = new System.Drawing.Size(516, 232);
            this.darkSectionPanel1.TabIndex = 2;
            // 
            // picTexture
            // 
            this.picTexture.BackColor = System.Drawing.Color.White;
            this.picTexture.Location = new System.Drawing.Point(94, 163);
            this.picTexture.Name = "picTexture";
            this.picTexture.Size = new System.Drawing.Size(32, 32);
            this.picTexture.TabIndex = 12;
            this.picTexture.TabStop = false;
            this.picTexture.Click += new System.EventHandler(this.picTexture_Click);
            // 
            // darkLabel9
            // 
            this.darkLabel9.AutoSize = true;
            this.darkLabel9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel9.Location = new System.Drawing.Point(11, 163);
            this.darkLabel9.Name = "darkLabel9";
            this.darkLabel9.Size = new System.Drawing.Size(47, 15);
            this.darkLabel9.TabIndex = 11;
            this.darkLabel9.Text = "Picture:";
            // 
            // cmbType
            // 
            this.cmbType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbType.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Location = new System.Drawing.Point(94, 116);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(133, 23);
            this.cmbType.TabIndex = 10;
            this.cmbType.SelectedIndexChanged += new System.EventHandler(this.cmbType_SelectedIndexChanged);
            // 
            // darkLabel3
            // 
            this.darkLabel3.AutoSize = true;
            this.darkLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel3.Location = new System.Drawing.Point(10, 119);
            this.darkLabel3.Name = "darkLabel3";
            this.darkLabel3.Size = new System.Drawing.Size(62, 15);
            this.darkLabel3.TabIndex = 9;
            this.darkLabel3.Text = "Item Type:";
            // 
            // darkLabel2
            // 
            this.darkLabel2.AutoSize = true;
            this.darkLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel2.Location = new System.Drawing.Point(10, 86);
            this.darkLabel2.Name = "darkLabel2";
            this.darkLabel2.Size = new System.Drawing.Size(60, 15);
            this.darkLabel2.TabIndex = 8;
            this.darkLabel2.Text = "Stackable:";
            // 
            // radioNotStackable
            // 
            this.radioNotStackable.AutoSize = true;
            this.radioNotStackable.Checked = true;
            this.radioNotStackable.Location = new System.Drawing.Point(148, 84);
            this.radioNotStackable.Name = "radioNotStackable";
            this.radioNotStackable.Size = new System.Drawing.Size(51, 19);
            this.radioNotStackable.TabIndex = 7;
            this.radioNotStackable.TabStop = true;
            this.radioNotStackable.Text = "False";
            this.radioNotStackable.CheckedChanged += new System.EventHandler(this.radioNotStackable_CheckedChanged);
            // 
            // radioStackable
            // 
            this.radioStackable.AutoSize = true;
            this.radioStackable.Location = new System.Drawing.Point(94, 84);
            this.radioStackable.Name = "radioStackable";
            this.radioStackable.Size = new System.Drawing.Size(48, 19);
            this.radioStackable.TabIndex = 6;
            this.radioStackable.Text = "True";
            this.radioStackable.CheckedChanged += new System.EventHandler(this.radioStackable_CheckedChanged);
            // 
            // darkLabel1
            // 
            this.darkLabel1.AutoSize = true;
            this.darkLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel1.Location = new System.Drawing.Point(10, 41);
            this.darkLabel1.Name = "darkLabel1";
            this.darkLabel1.Size = new System.Drawing.Size(69, 15);
            this.darkLabel1.TabIndex = 5;
            this.darkLabel1.Text = "Item Name:";
            // 
            // txtName
            // 
            this.txtName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtName.Location = new System.Drawing.Point(94, 39);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(133, 23);
            this.txtName.TabIndex = 4;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // darkSectionPanel3
            // 
            this.darkSectionPanel3.Controls.Add(this.txtEditor);
            this.darkSectionPanel3.Controls.Add(this.darkMenuStrip1);
            this.darkSectionPanel3.Location = new System.Drawing.Point(535, 31);
            this.darkSectionPanel3.Name = "darkSectionPanel3";
            this.darkSectionPanel3.SectionHeader = "Scripting Information";
            this.darkSectionPanel3.Size = new System.Drawing.Size(683, 485);
            this.darkSectionPanel3.TabIndex = 4;
            // 
            // txtEditor
            // 
            this.txtEditor.Location = new System.Drawing.Point(4, 52);
            this.txtEditor.Name = "txtEditor";
            this.txtEditor.Size = new System.Drawing.Size(675, 429);
            this.txtEditor.TabIndex = 1;
            this.txtEditor.TextChanged += new System.EventHandler(this.txtEditor_TextChanged);
            this.txtEditor.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtEditor_KeyDown);
            // 
            // darkMenuStrip1
            // 
            this.darkMenuStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.darkMenuStrip1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.scriptsToolStripMenuItem});
            this.darkMenuStrip1.Location = new System.Drawing.Point(1, 25);
            this.darkMenuStrip1.Name = "darkMenuStrip1";
            this.darkMenuStrip1.Padding = new System.Windows.Forms.Padding(3, 2, 0, 2);
            this.darkMenuStrip1.Size = new System.Drawing.Size(681, 24);
            this.darkMenuStrip1.TabIndex = 2;
            this.darkMenuStrip1.Text = "darkMenuStrip1";
            // 
            // scriptsToolStripMenuItem
            // 
            this.scriptsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.onUseToolStripMenuItem,
            this.onEquipToolStripMenuItem,
            this.onAcquiredToolStripMenuItem,
            this.onDroppedToolStripMenuItem,
            this.onCreatedToolStripMenuItem});
            this.scriptsToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.scriptsToolStripMenuItem.Name = "scriptsToolStripMenuItem";
            this.scriptsToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.scriptsToolStripMenuItem.Text = "Scripts";
            // 
            // onUseToolStripMenuItem
            // 
            this.onUseToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.onUseToolStripMenuItem.Name = "onUseToolStripMenuItem";
            this.onUseToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.onUseToolStripMenuItem.Text = "OnUse";
            this.onUseToolStripMenuItem.Click += new System.EventHandler(this.onUseToolStripMenuItem_Click);
            // 
            // onEquipToolStripMenuItem
            // 
            this.onEquipToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.onEquipToolStripMenuItem.Name = "onEquipToolStripMenuItem";
            this.onEquipToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.onEquipToolStripMenuItem.Text = "OnEquip";
            this.onEquipToolStripMenuItem.Click += new System.EventHandler(this.onEquipToolStripMenuItem_Click);
            // 
            // onAcquiredToolStripMenuItem
            // 
            this.onAcquiredToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.onAcquiredToolStripMenuItem.Name = "onAcquiredToolStripMenuItem";
            this.onAcquiredToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.onAcquiredToolStripMenuItem.Text = "OnAcquired";
            this.onAcquiredToolStripMenuItem.Click += new System.EventHandler(this.onAcquiredToolStripMenuItem_Click);
            // 
            // onDroppedToolStripMenuItem
            // 
            this.onDroppedToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.onDroppedToolStripMenuItem.Name = "onDroppedToolStripMenuItem";
            this.onDroppedToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.onDroppedToolStripMenuItem.Text = "OnDropped";
            this.onDroppedToolStripMenuItem.Click += new System.EventHandler(this.onDroppedToolStripMenuItem_Click);
            // 
            // onCreatedToolStripMenuItem
            // 
            this.onCreatedToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.onCreatedToolStripMenuItem.Name = "onCreatedToolStripMenuItem";
            this.onCreatedToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.onCreatedToolStripMenuItem.Text = "OnCreated";
            this.onCreatedToolStripMenuItem.Click += new System.EventHandler(this.onCreatedToolStripMenuItem_Click);
            // 
            // darkSectionPanel4
            // 
            this.darkSectionPanel4.Controls.Add(this.cmbEquipmentSlot);
            this.darkSectionPanel4.Controls.Add(this.darkLabel10);
            this.darkSectionPanel4.Controls.Add(this.darkSectionPanel2);
            this.darkSectionPanel4.Location = new System.Drawing.Point(13, 269);
            this.darkSectionPanel4.Name = "darkSectionPanel4";
            this.darkSectionPanel4.SectionHeader = "Equipment Information";
            this.darkSectionPanel4.Size = new System.Drawing.Size(516, 306);
            this.darkSectionPanel4.TabIndex = 5;
            // 
            // cmbEquipmentSlot
            // 
            this.cmbEquipmentSlot.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.cmbEquipmentSlot.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbEquipmentSlot.ForeColor = System.Drawing.Color.Gainsboro;
            this.cmbEquipmentSlot.FormattingEnabled = true;
            this.cmbEquipmentSlot.Location = new System.Drawing.Point(107, 36);
            this.cmbEquipmentSlot.Name = "cmbEquipmentSlot";
            this.cmbEquipmentSlot.Size = new System.Drawing.Size(120, 23);
            this.cmbEquipmentSlot.TabIndex = 12;
            this.cmbEquipmentSlot.SelectedIndexChanged += new System.EventHandler(this.cmbEquipmentSlot_SelectedIndexChanged);
            // 
            // darkLabel10
            // 
            this.darkLabel10.AutoSize = true;
            this.darkLabel10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel10.Location = new System.Drawing.Point(10, 39);
            this.darkLabel10.Name = "darkLabel10";
            this.darkLabel10.Size = new System.Drawing.Size(91, 15);
            this.darkLabel10.TabIndex = 11;
            this.darkLabel10.Text = "Equipment Slot:";
            // 
            // darkSectionPanel2
            // 
            this.darkSectionPanel2.Controls.Add(this.txtHealth);
            this.darkSectionPanel2.Controls.Add(this.txtDef);
            this.darkSectionPanel2.Controls.Add(this.txtDex);
            this.darkSectionPanel2.Controls.Add(this.txtStr);
            this.darkSectionPanel2.Controls.Add(this.txtInt);
            this.darkSectionPanel2.Controls.Add(this.darkLabel8);
            this.darkSectionPanel2.Controls.Add(this.darkLabel7);
            this.darkSectionPanel2.Controls.Add(this.darkLabel6);
            this.darkSectionPanel2.Controls.Add(this.darkLabel5);
            this.darkSectionPanel2.Controls.Add(this.darkLabel4);
            this.darkSectionPanel2.Location = new System.Drawing.Point(237, 28);
            this.darkSectionPanel2.Name = "darkSectionPanel2";
            this.darkSectionPanel2.SectionHeader = "Stat Information";
            this.darkSectionPanel2.Size = new System.Drawing.Size(275, 150);
            this.darkSectionPanel2.TabIndex = 4;
            // 
            // txtHealth
            // 
            this.txtHealth.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtHealth.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtHealth.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtHealth.Location = new System.Drawing.Point(220, 63);
            this.txtHealth.Name = "txtHealth";
            this.txtHealth.Size = new System.Drawing.Size(51, 23);
            this.txtHealth.TabIndex = 19;
            this.txtHealth.TextChanged += new System.EventHandler(this.txtHealth_TextChanged);
            // 
            // txtDef
            // 
            this.txtDef.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtDef.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDef.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtDef.Location = new System.Drawing.Point(220, 30);
            this.txtDef.Name = "txtDef";
            this.txtDef.Size = new System.Drawing.Size(51, 23);
            this.txtDef.TabIndex = 18;
            this.txtDef.TextChanged += new System.EventHandler(this.txtDef_TextChanged);
            // 
            // txtDex
            // 
            this.txtDex.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtDex.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDex.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtDex.Location = new System.Drawing.Point(83, 102);
            this.txtDex.Name = "txtDex";
            this.txtDex.Size = new System.Drawing.Size(51, 23);
            this.txtDex.TabIndex = 17;
            this.txtDex.TextChanged += new System.EventHandler(this.txtDex_TextChanged);
            // 
            // txtStr
            // 
            this.txtStr.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtStr.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtStr.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtStr.Location = new System.Drawing.Point(83, 30);
            this.txtStr.Name = "txtStr";
            this.txtStr.Size = new System.Drawing.Size(51, 23);
            this.txtStr.TabIndex = 16;
            this.txtStr.TextChanged += new System.EventHandler(this.txtStr_TextChanged);
            // 
            // txtInt
            // 
            this.txtInt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtInt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtInt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtInt.Location = new System.Drawing.Point(83, 65);
            this.txtInt.Name = "txtInt";
            this.txtInt.Size = new System.Drawing.Size(51, 23);
            this.txtInt.TabIndex = 15;
            this.txtInt.TextChanged += new System.EventHandler(this.txtInt_TextChanged);
            // 
            // darkLabel8
            // 
            this.darkLabel8.AutoSize = true;
            this.darkLabel8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel8.Location = new System.Drawing.Point(163, 65);
            this.darkLabel8.Name = "darkLabel8";
            this.darkLabel8.Size = new System.Drawing.Size(45, 15);
            this.darkLabel8.TabIndex = 14;
            this.darkLabel8.Text = "Health:";
            // 
            // darkLabel7
            // 
            this.darkLabel7.AutoSize = true;
            this.darkLabel7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel7.Location = new System.Drawing.Point(163, 32);
            this.darkLabel7.Name = "darkLabel7";
            this.darkLabel7.Size = new System.Drawing.Size(53, 15);
            this.darkLabel7.TabIndex = 13;
            this.darkLabel7.Text = "Defence:";
            // 
            // darkLabel6
            // 
            this.darkLabel6.AutoSize = true;
            this.darkLabel6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel6.Location = new System.Drawing.Point(11, 104);
            this.darkLabel6.Name = "darkLabel6";
            this.darkLabel6.Size = new System.Drawing.Size(56, 15);
            this.darkLabel6.TabIndex = 12;
            this.darkLabel6.Text = "Dexterity:";
            // 
            // darkLabel5
            // 
            this.darkLabel5.AutoSize = true;
            this.darkLabel5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel5.Location = new System.Drawing.Point(11, 67);
            this.darkLabel5.Name = "darkLabel5";
            this.darkLabel5.Size = new System.Drawing.Size(71, 15);
            this.darkLabel5.TabIndex = 11;
            this.darkLabel5.Text = "Intelligence:";
            // 
            // darkLabel4
            // 
            this.darkLabel4.AutoSize = true;
            this.darkLabel4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel4.Location = new System.Drawing.Point(11, 32);
            this.darkLabel4.Name = "darkLabel4";
            this.darkLabel4.Size = new System.Drawing.Size(55, 15);
            this.darkLabel4.TabIndex = 10;
            this.darkLabel4.Text = "Strength:";
            // 
            // DockItemEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.darkSectionPanel4);
            this.Controls.Add(this.darkSectionPanel3);
            this.Controls.Add(this.darkSectionPanel1);
            this.Controls.Add(this.darkToolStrip1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "DockItemEditor";
            this.Size = new System.Drawing.Size(1221, 626);
            this.Load += new System.EventHandler(this.DockItemEditor_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DockItemEditor_KeyDown);
            this.darkToolStrip1.ResumeLayout(false);
            this.darkToolStrip1.PerformLayout();
            this.darkSectionPanel1.ResumeLayout(false);
            this.darkSectionPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picTexture)).EndInit();
            this.darkSectionPanel3.ResumeLayout(false);
            this.darkSectionPanel3.PerformLayout();
            this.darkMenuStrip1.ResumeLayout(false);
            this.darkMenuStrip1.PerformLayout();
            this.darkSectionPanel4.ResumeLayout(false);
            this.darkSectionPanel4.PerformLayout();
            this.darkSectionPanel2.ResumeLayout(false);
            this.darkSectionPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private DarkUI.Controls.DarkToolStrip darkToolStrip1;
        private System.Windows.Forms.ToolStripButton buttonSave;
        private DarkUI.Controls.DarkSectionPanel darkSectionPanel1;
        private DarkUI.Controls.DarkLabel darkLabel1;
        private DarkUI.Controls.DarkTextBox txtName;
        private DarkUI.Controls.DarkLabel darkLabel2;
        private DarkUI.Controls.DarkRadioButton radioNotStackable;
        private DarkUI.Controls.DarkRadioButton radioStackable;
        private System.Windows.Forms.ComboBox cmbType;
        private DarkUI.Controls.DarkLabel darkLabel3;
        private DarkUI.Controls.DarkSectionPanel darkSectionPanel3;
        private ScintillaNET.Scintilla txtEditor;
        private DarkUI.Controls.DarkMenuStrip darkMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem scriptsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem onUseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem onEquipToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem onAcquiredToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem onDroppedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem onCreatedToolStripMenuItem;
        private System.Windows.Forms.PictureBox picTexture;
        private DarkUI.Controls.DarkLabel darkLabel9;
        private DarkUI.Controls.DarkSectionPanel darkSectionPanel4;
        private System.Windows.Forms.ComboBox cmbEquipmentSlot;
        private DarkUI.Controls.DarkLabel darkLabel10;
        private DarkUI.Controls.DarkSectionPanel darkSectionPanel2;
        private DarkUI.Controls.DarkTextBox txtHealth;
        private DarkUI.Controls.DarkTextBox txtDef;
        private DarkUI.Controls.DarkTextBox txtDex;
        private DarkUI.Controls.DarkTextBox txtStr;
        private DarkUI.Controls.DarkTextBox txtInt;
        private DarkUI.Controls.DarkLabel darkLabel8;
        private DarkUI.Controls.DarkLabel darkLabel7;
        private DarkUI.Controls.DarkLabel darkLabel6;
        private DarkUI.Controls.DarkLabel darkLabel5;
        private DarkUI.Controls.DarkLabel darkLabel4;
    }
}
