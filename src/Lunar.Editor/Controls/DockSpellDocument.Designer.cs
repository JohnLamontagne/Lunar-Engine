namespace Lunar.Editor.Controls
{
    partial class DockSpellDocument
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
            this.darkLabel1 = new DarkUI.Controls.DarkLabel();
            this.txtName = new DarkUI.Controls.DarkTextBox();
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
            this.darkTextBox1 = new DarkUI.Controls.DarkTextBox();
            this.darkLabel2 = new DarkUI.Controls.DarkLabel();
            this.darkTextBox2 = new DarkUI.Controls.DarkTextBox();
            this.darkLabel3 = new DarkUI.Controls.DarkLabel();
            this.darkLabel10 = new DarkUI.Controls.DarkLabel();
            this.txtCastTime = new DarkUI.Controls.DarkTextBox();
            this.darkSectionPanel3 = new DarkUI.Controls.DarkSectionPanel();
            this.darkToolStrip1.SuspendLayout();
            this.darkSectionPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picTexture)).BeginInit();
            this.darkSectionPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // darkToolStrip1
            // 
            this.darkToolStrip1.AutoSize = false;
            this.darkToolStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.darkToolStrip1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkToolStrip1.ImageScalingSize = new System.Drawing.Size(28, 28);
            this.darkToolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonSave});
            this.darkToolStrip1.Location = new System.Drawing.Point(0, 0);
            this.darkToolStrip1.Name = "darkToolStrip1";
            this.darkToolStrip1.Padding = new System.Windows.Forms.Padding(5, 0, 1, 0);
            this.darkToolStrip1.Size = new System.Drawing.Size(1009, 28);
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
            this.buttonSave.Size = new System.Drawing.Size(32, 25);
            this.buttonSave.Text = "toolStripButton1";
            this.buttonSave.ToolTipText = "Save";
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // darkSectionPanel1
            // 
            this.darkSectionPanel1.Controls.Add(this.txtCastTime);
            this.darkSectionPanel1.Controls.Add(this.darkLabel10);
            this.darkSectionPanel1.Controls.Add(this.picTexture);
            this.darkSectionPanel1.Controls.Add(this.darkLabel9);
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
            this.picTexture.Location = new System.Drawing.Point(94, 87);
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
            this.darkLabel9.Location = new System.Drawing.Point(47, 96);
            this.darkLabel9.Name = "darkLabel9";
            this.darkLabel9.Size = new System.Drawing.Size(33, 15);
            this.darkLabel9.TabIndex = 11;
            this.darkLabel9.Text = "Icon:";
            // 
            // darkLabel1
            // 
            this.darkLabel1.AutoSize = true;
            this.darkLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel1.Location = new System.Drawing.Point(10, 41);
            this.darkLabel1.Name = "darkLabel1";
            this.darkLabel1.Size = new System.Drawing.Size(70, 15);
            this.darkLabel1.TabIndex = 5;
            this.darkLabel1.Text = "Spell Name:";
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
            // darkSectionPanel2
            // 
            this.darkSectionPanel2.Controls.Add(this.darkTextBox2);
            this.darkSectionPanel2.Controls.Add(this.darkLabel3);
            this.darkSectionPanel2.Controls.Add(this.darkTextBox1);
            this.darkSectionPanel2.Controls.Add(this.darkLabel2);
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
            this.darkSectionPanel2.Location = new System.Drawing.Point(13, 269);
            this.darkSectionPanel2.Name = "darkSectionPanel2";
            this.darkSectionPanel2.SectionHeader = "Cast Requirements";
            this.darkSectionPanel2.Size = new System.Drawing.Size(516, 150);
            this.darkSectionPanel2.TabIndex = 14;
            // 
            // txtHealth
            // 
            this.txtHealth.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtHealth.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtHealth.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtHealth.Location = new System.Drawing.Point(279, 65);
            this.txtHealth.Name = "txtHealth";
            this.txtHealth.Size = new System.Drawing.Size(51, 23);
            this.txtHealth.TabIndex = 19;
            // 
            // txtDef
            // 
            this.txtDef.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtDef.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDef.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtDef.Location = new System.Drawing.Point(279, 30);
            this.txtDef.Name = "txtDef";
            this.txtDef.Size = new System.Drawing.Size(51, 23);
            this.txtDef.TabIndex = 18;
            // 
            // txtDex
            // 
            this.txtDex.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtDex.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDex.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtDex.Location = new System.Drawing.Point(111, 102);
            this.txtDex.Name = "txtDex";
            this.txtDex.Size = new System.Drawing.Size(51, 23);
            this.txtDex.TabIndex = 17;
            // 
            // txtStr
            // 
            this.txtStr.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtStr.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtStr.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtStr.Location = new System.Drawing.Point(111, 30);
            this.txtStr.Name = "txtStr";
            this.txtStr.Size = new System.Drawing.Size(51, 23);
            this.txtStr.TabIndex = 16;
            // 
            // txtInt
            // 
            this.txtInt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtInt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtInt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtInt.Location = new System.Drawing.Point(111, 65);
            this.txtInt.Name = "txtInt";
            this.txtInt.Size = new System.Drawing.Size(51, 23);
            this.txtInt.TabIndex = 15;
            // 
            // darkLabel8
            // 
            this.darkLabel8.AutoSize = true;
            this.darkLabel8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel8.Location = new System.Drawing.Point(205, 67);
            this.darkLabel8.Name = "darkLabel8";
            this.darkLabel8.Size = new System.Drawing.Size(68, 15);
            this.darkLabel8.TabIndex = 14;
            this.darkLabel8.Text = "Req Health:";
            // 
            // darkLabel7
            // 
            this.darkLabel7.AutoSize = true;
            this.darkLabel7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel7.Location = new System.Drawing.Point(197, 32);
            this.darkLabel7.Name = "darkLabel7";
            this.darkLabel7.Size = new System.Drawing.Size(76, 15);
            this.darkLabel7.TabIndex = 13;
            this.darkLabel7.Text = "Req Defence:";
            // 
            // darkLabel6
            // 
            this.darkLabel6.AutoSize = true;
            this.darkLabel6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel6.Location = new System.Drawing.Point(25, 104);
            this.darkLabel6.Name = "darkLabel6";
            this.darkLabel6.Size = new System.Drawing.Size(80, 15);
            this.darkLabel6.TabIndex = 12;
            this.darkLabel6.Text = "Req Dexterity:";
            // 
            // darkLabel5
            // 
            this.darkLabel5.AutoSize = true;
            this.darkLabel5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel5.Location = new System.Drawing.Point(11, 67);
            this.darkLabel5.Name = "darkLabel5";
            this.darkLabel5.Size = new System.Drawing.Size(94, 15);
            this.darkLabel5.TabIndex = 11;
            this.darkLabel5.Text = "Req Intelligence:";
            // 
            // darkLabel4
            // 
            this.darkLabel4.AutoSize = true;
            this.darkLabel4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel4.Location = new System.Drawing.Point(27, 32);
            this.darkLabel4.Name = "darkLabel4";
            this.darkLabel4.Size = new System.Drawing.Size(78, 15);
            this.darkLabel4.TabIndex = 10;
            this.darkLabel4.Text = "Req Strength:";
            // 
            // darkTextBox1
            // 
            this.darkTextBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.darkTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.darkTextBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkTextBox1.Location = new System.Drawing.Point(453, 30);
            this.darkTextBox1.Name = "darkTextBox1";
            this.darkTextBox1.Size = new System.Drawing.Size(51, 23);
            this.darkTextBox1.TabIndex = 21;
            // 
            // darkLabel2
            // 
            this.darkLabel2.AutoSize = true;
            this.darkLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel2.Location = new System.Drawing.Point(371, 32);
            this.darkLabel2.Name = "darkLabel2";
            this.darkLabel2.Size = new System.Drawing.Size(72, 15);
            this.darkLabel2.TabIndex = 20;
            this.darkLabel2.Text = "Health Cost:";
            // 
            // darkTextBox2
            // 
            this.darkTextBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.darkTextBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.darkTextBox2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkTextBox2.Location = new System.Drawing.Point(453, 65);
            this.darkTextBox2.Name = "darkTextBox2";
            this.darkTextBox2.Size = new System.Drawing.Size(51, 23);
            this.darkTextBox2.TabIndex = 23;
            // 
            // darkLabel3
            // 
            this.darkLabel3.AutoSize = true;
            this.darkLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel3.Location = new System.Drawing.Point(371, 67);
            this.darkLabel3.Name = "darkLabel3";
            this.darkLabel3.Size = new System.Drawing.Size(67, 15);
            this.darkLabel3.TabIndex = 22;
            this.darkLabel3.Text = "Mana Cost:";
            // 
            // darkLabel10
            // 
            this.darkLabel10.AutoSize = true;
            this.darkLabel10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel10.Location = new System.Drawing.Point(292, 43);
            this.darkLabel10.Name = "darkLabel10";
            this.darkLabel10.Size = new System.Drawing.Size(59, 15);
            this.darkLabel10.TabIndex = 24;
            this.darkLabel10.Text = "Cast Time";
            // 
            // txtCastTime
            // 
            this.txtCastTime.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtCastTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCastTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtCastTime.Location = new System.Drawing.Point(357, 41);
            this.txtCastTime.Name = "txtCastTime";
            this.txtCastTime.Size = new System.Drawing.Size(64, 23);
            this.txtCastTime.TabIndex = 25;
            // 
            // darkSectionPanel3
            // 
            this.darkSectionPanel3.Location = new System.Drawing.Point(535, 31);
            this.darkSectionPanel3.Name = "darkSectionPanel3";
            this.darkSectionPanel3.SectionHeader = "Animation";
            this.darkSectionPanel3.Size = new System.Drawing.Size(383, 388);
            this.darkSectionPanel3.TabIndex = 15;
            // 
            // DockSpellDocument
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.darkSectionPanel3);
            this.Controls.Add(this.darkSectionPanel2);
            this.Controls.Add(this.darkSectionPanel1);
            this.Controls.Add(this.darkToolStrip1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.Name = "DockSpellDocument";
            this.Size = new System.Drawing.Size(1009, 449);
            this.Load += new System.EventHandler(this.DockItemEditor_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DockItemEditor_KeyDown);
            this.darkToolStrip1.ResumeLayout(false);
            this.darkToolStrip1.PerformLayout();
            this.darkSectionPanel1.ResumeLayout(false);
            this.darkSectionPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picTexture)).EndInit();
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
        private System.Windows.Forms.PictureBox picTexture;
        private DarkUI.Controls.DarkLabel darkLabel9;
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
        private DarkUI.Controls.DarkTextBox txtCastTime;
        private DarkUI.Controls.DarkLabel darkLabel10;
        private DarkUI.Controls.DarkTextBox darkTextBox2;
        private DarkUI.Controls.DarkLabel darkLabel3;
        private DarkUI.Controls.DarkTextBox darkTextBox1;
        private DarkUI.Controls.DarkLabel darkLabel2;
        private DarkUI.Controls.DarkSectionPanel darkSectionPanel3;
    }
}
