using DarkUI.Forms;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ScintillaNET;
using System.Text.RegularExpressions;

namespace Lunar.Editor.Controls
{
    public partial class DockScriptDocument : SavableDocument
    {
        private string _regularDockText;
        private string _unsavedDockText;
        private bool _unsaved;
        private int _maxLineNumberCharLength;

        public DockScriptDocument(string text, Image icon, FileInfo file)
            : base(file)
        {
            InitializeComponent();

            this.InitalizeStyling();

            _regularDockText = text;
            _unsavedDockText = text + "*";

            DockText = text;
            Icon = icon;
        }

        public override void Close()
        {
            if (_unsaved)
            {
                var result = DarkMessageBox.ShowWarning(@"You will lose any unsaved changes. Continue?", @"Close document", DarkDialogButton.YesNo);
                if (result == DialogResult.No)
                    return;
            }

            base.Close();
        }

        private void DockLUADocument_Load(object sender, System.EventArgs e)
        {
            this.DockText = _regularDockText;
            _unsaved = false;

            // Load the script contents
            string text = File.ReadAllText(this.ContentFile.FullName);
            this.txtEditor.Text = text;
        }

        private void InitalizeStyling()
        {
            this.txtEditor.Lexer = Lexer.Python;

            this.txtEditor.StyleResetDefault();

            this.txtEditor.CaretForeColor = Color.White;

            this.txtEditor.Margins[0].Width = 16;
            //this.txtEditor.Margins[0].Type = MarginType.BackColor;

            this.txtEditor.Styles[Style.Default].Font = "Consolas";
            this.txtEditor.Styles[Style.Default].Size = 12;

            this.txtEditor.Styles[Style.Default].BackColor = Color.FromArgb(29, 31, 33);
            this.txtEditor.Styles[Style.Default].ForeColor = Color.FromArgb(197, 200, 198);

            this.txtEditor.StyleClearAll();

            this.txtEditor.Styles[Style.LineNumber].BackColor = Color.FromArgb(29, 31, 33);

            this.txtEditor.Styles[Style.Python.CommentLine].ForeColor = Color.FromArgb(181, 189, 104);
            this.txtEditor.Styles[Style.Python.CommentLine].Italic = true;
            this.txtEditor.Styles[Style.Python.CommentBlock].ForeColor = Color.FromArgb(181, 189, 104);
            this.txtEditor.Styles[Style.Python.CommentBlock].Italic = true;

            this.txtEditor.Styles[Style.Python.String].ForeColor = Color.FromArgb(222, 147, 95);

            this.txtEditor.Styles[Style.Python.Operator].ForeColor = Color.FromArgb(240, 198, 116);

            this.txtEditor.Styles[Style.Python.ClassName].ForeColor = Color.FromArgb(222, 147, 95);

            this.txtEditor.Styles[Style.Python.Number].ForeColor = Color.FromArgb(138, 190, 183);

            this.txtEditor.Styles[Style.Python.Identifier].ForeColor = Color.FromArgb(178, 148, 187);

            this.txtEditor.Styles[Style.Python.Word].ForeColor = Color.FromArgb(130, 239, 104);

            this.txtEditor.SetKeywords(0, "if and or def not return from import class");

            this.txtEditor.InsertCheck += TxtEditor_InsertCheck;
        }

        private void TxtEditor_InsertCheck(object sender, InsertCheckEventArgs e)
        {
            if ((e.Text.EndsWith("\r") || e.Text.EndsWith("\n")))
            {
                var curLine = this.txtEditor.LineFromPosition(e.Position);
                var curLineText = this.txtEditor.Lines[curLine].Text;

                var indent = Regex.Match(curLineText, @"^\s*");
                string txt = indent.Value.Replace("\r", "").Replace("\n", "");
                e.Text += txt;// Add indent following "\r\n"

                // Current line end with bracket?
                if (Regex.IsMatch(curLineText, @":\s*$") || this.txtEditor.GetCharAt(this.txtEditor.CurrentPosition - 1) == ':')
                    e.Text += '\t'; // Add tab
            }
        }

        public override void Save()
        {
            File.WriteAllText(this.ContentFile.FullName, this.txtEditor.Text);
            this.DockText = _regularDockText;
            _unsaved = false;
        }

        private void txtEditor_TextChanged(object sender, System.EventArgs e)
        {
            this.DockText = _unsavedDockText;
            _unsaved = true;

            var maxLineNumberCharLength = this.txtEditor.Lines.Count.ToString().Length;
            if (maxLineNumberCharLength == _maxLineNumberCharLength)
                return;

            // Calculate the width required to display the last line number
            // and include some padding for good measure.
            const int padding = 2;
            this.txtEditor.Margins[0].Width = this.txtEditor.TextWidth(Style.LineNumber, new string('9', maxLineNumberCharLength + 1)) + padding;
            _maxLineNumberCharLength = maxLineNumberCharLength;
        }

        private void txtEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control && e.KeyCode == Keys.S)
            {
                this.Save();
                e.SuppressKeyPress = true;
            }
        }

        private void buttonSave_Click(object sender, System.EventArgs e)
        {
            this.Save();
        }
    }
}