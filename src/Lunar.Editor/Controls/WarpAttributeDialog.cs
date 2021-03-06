﻿using System;
using System.Windows.Forms;
using DarkUI.Forms;
using Lunar.Editor.World;

namespace Lunar.Editor.Controls
{
    public partial class WarpAttributeDialog : DarkDialog
    {
        private Map _mapSubject;
        private Form _parentForm;

        /// <summary>
        /// Map selected to place attribute on
        /// </summary>
        public Map MapSubject { get; private set; }

        public int WarpX
        {
            get => Convert.ToInt32(txtWarpX.Text);
            set => txtWarpX.Text = value.ToString();
        }

        public int WarpY
        {
            get => Convert.ToInt32(txtWarpY.Text);
            set => txtWarpY.Text = value.ToString();
        }

        public string WarpMapID
        {
            get => txtMapID.Text;
            set => txtMapID.Text = value;
        }

        public string WarpLayerName
        {
            get => this.txtWarpLayer.Text;
            set => this.txtWarpLayer.Text = value;
        }

        public WarpAttributeDialog(Form parentForm, Map mapSubject)
        {
            _parentForm = parentForm;

            this.btnOk.Click += BtnOk_Click;

            InitializeComponent();
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            this.Submitted?.Invoke(this, new EventArgs());
            this.Close();
        }

        private void btnSelectTile_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.SelectTile?.Invoke(this, new EventArgs());
        }

        public event EventHandler<EventArgs> Submitted;
        public event EventHandler<EventArgs> SelectTile;
    }
}
