using System;
using System.IO;
using System.Windows.Forms;
using DarkUI.Docking;

namespace Lunar.Editor.Controls
{
    public partial class SavableDocument : DarkDocument
    {
        private FileInfo _file;

        public event EventHandler Closed;

        public FileInfo ContentFile { get => _file; protected set { _file = value; } }

        public SavableDocument()
        {
            InitializeComponent();
        }

        public SavableDocument(FileInfo file)
        {
            InitializeComponent();
            _file = file;
        }

        public virtual void Save()
        {
        }

        public override void Close()
        {
            this.Closed?.Invoke(this, new EventArgs());

            base.Close();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Control | Keys.S:
                    this.Save();
                    break;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
