namespace Lunar.Editor.Controls
{
    partial class WarpAttributeDialog
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
            this.panelWarpData = new System.Windows.Forms.Panel();
            this.lblWarpMapID = new DarkUI.Controls.DarkLabel();
            this.lblWarpY = new DarkUI.Controls.DarkLabel();
            this.lblWarpX = new DarkUI.Controls.DarkLabel();
            this.txtMapID = new DarkUI.Controls.DarkTextBox();
            this.txtWarpY = new DarkUI.Controls.DarkTextBox();
            this.txtWarpX = new DarkUI.Controls.DarkTextBox();
            this.btnSelectTile = new DarkUI.Controls.DarkButton();
            this.lblWarpLayer = new DarkUI.Controls.DarkLabel();
            this.txtWarpLayer = new DarkUI.Controls.DarkTextBox();
            this.panelWarpData.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelWarpData
            // 
            this.panelWarpData.Controls.Add(this.lblWarpLayer);
            this.panelWarpData.Controls.Add(this.txtWarpLayer);
            this.panelWarpData.Controls.Add(this.btnSelectTile);
            this.panelWarpData.Controls.Add(this.lblWarpMapID);
            this.panelWarpData.Controls.Add(this.lblWarpY);
            this.panelWarpData.Controls.Add(this.lblWarpX);
            this.panelWarpData.Controls.Add(this.txtMapID);
            this.panelWarpData.Controls.Add(this.txtWarpY);
            this.panelWarpData.Controls.Add(this.txtWarpX);
            this.panelWarpData.Location = new System.Drawing.Point(3, 12);
            this.panelWarpData.Name = "panelWarpData";
            this.panelWarpData.Size = new System.Drawing.Size(281, 265);
            this.panelWarpData.TabIndex = 0;
            // 
            // lblWarpMapID
            // 
            this.lblWarpMapID.AutoSize = true;
            this.lblWarpMapID.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.lblWarpMapID.Location = new System.Drawing.Point(27, 149);
            this.lblWarpMapID.Name = "lblWarpMapID";
            this.lblWarpMapID.Size = new System.Drawing.Size(45, 13);
            this.lblWarpMapID.TabIndex = 5;
            this.lblWarpMapID.Text = "Map ID:";
            // 
            // lblWarpY
            // 
            this.lblWarpY.AutoSize = true;
            this.lblWarpY.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.lblWarpY.Location = new System.Drawing.Point(55, 72);
            this.lblWarpY.Name = "lblWarpY";
            this.lblWarpY.Size = new System.Drawing.Size(17, 13);
            this.lblWarpY.TabIndex = 4;
            this.lblWarpY.Text = "Y:";
            // 
            // lblWarpX
            // 
            this.lblWarpX.AutoSize = true;
            this.lblWarpX.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.lblWarpX.Location = new System.Drawing.Point(55, 29);
            this.lblWarpX.Name = "lblWarpX";
            this.lblWarpX.Size = new System.Drawing.Size(17, 13);
            this.lblWarpX.TabIndex = 3;
            this.lblWarpX.Text = "X:";
            // 
            // txtMapID
            // 
            this.txtMapID.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtMapID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMapID.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtMapID.Location = new System.Drawing.Point(78, 147);
            this.txtMapID.Name = "txtMapID";
            this.txtMapID.Size = new System.Drawing.Size(132, 20);
            this.txtMapID.TabIndex = 2;
            // 
            // txtWarpY
            // 
            this.txtWarpY.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtWarpY.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtWarpY.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtWarpY.Location = new System.Drawing.Point(78, 70);
            this.txtWarpY.Name = "txtWarpY";
            this.txtWarpY.Size = new System.Drawing.Size(132, 20);
            this.txtWarpY.TabIndex = 1;
            // 
            // txtWarpX
            // 
            this.txtWarpX.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtWarpX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtWarpX.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtWarpX.Location = new System.Drawing.Point(78, 27);
            this.txtWarpX.Name = "txtWarpX";
            this.txtWarpX.Size = new System.Drawing.Size(132, 20);
            this.txtWarpX.TabIndex = 0;
            // 
            // btnSelectTile
            // 
            this.btnSelectTile.Location = new System.Drawing.Point(9, 185);
            this.btnSelectTile.Name = "btnSelectTile";
            this.btnSelectTile.Padding = new System.Windows.Forms.Padding(5);
            this.btnSelectTile.Size = new System.Drawing.Size(263, 41);
            this.btnSelectTile.TabIndex = 6;
            this.btnSelectTile.Text = "Select Tile";
            this.btnSelectTile.Click += new System.EventHandler(this.btnSelectTile_Click);
            // 
            // lblWarpLayer
            // 
            this.lblWarpLayer.AutoSize = true;
            this.lblWarpLayer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.lblWarpLayer.Location = new System.Drawing.Point(36, 108);
            this.lblWarpLayer.Name = "lblWarpLayer";
            this.lblWarpLayer.Size = new System.Drawing.Size(36, 13);
            this.lblWarpLayer.TabIndex = 8;
            this.lblWarpLayer.Text = "Layer:";
            // 
            // txtWarpLayer
            // 
            this.txtWarpLayer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtWarpLayer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtWarpLayer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtWarpLayer.Location = new System.Drawing.Point(78, 106);
            this.txtWarpLayer.Name = "txtWarpLayer";
            this.txtWarpLayer.Size = new System.Drawing.Size(132, 20);
            this.txtWarpLayer.TabIndex = 7;
            // 
            // TileAttributeDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(287, 321);
            this.Controls.Add(this.panelWarpData);
            this.Name = "TileAttributeDialog";
            this.Text = "Attribute Data";
            this.Controls.SetChildIndex(this.panelWarpData, 0);
            this.panelWarpData.ResumeLayout(false);
            this.panelWarpData.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelWarpData;
        private DarkUI.Controls.DarkLabel lblWarpMapID;
        private DarkUI.Controls.DarkLabel lblWarpY;
        private DarkUI.Controls.DarkLabel lblWarpX;
        private DarkUI.Controls.DarkTextBox txtMapID;
        private DarkUI.Controls.DarkTextBox txtWarpY;
        private DarkUI.Controls.DarkTextBox txtWarpX;
        private DarkUI.Controls.DarkButton btnSelectTile;
        private DarkUI.Controls.DarkLabel lblWarpLayer;
        private DarkUI.Controls.DarkTextBox txtWarpLayer;
    }
}
