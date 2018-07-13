/** Copyright 2018 John Lamontagne https://www.mmorpgcreation.com

	Licensed under the Apache License, Version 2.0 (the "License");
	you may not use this file except in compliance with the License.
	You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

	Unless required by applicable law or agreed to in writing, software
	distributed under the License is distributed on an "AS IS" BASIS,
	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	See the License for the specific language governing permissions and
	limitations under the License.
*/
using DarkUI.Forms;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ScintillaNET;

namespace Lunar.Editor.Controls
{
    public partial class DockLUADocument : SavableDocument
    {
        private FileInfo _file;
        private string _regularDockText;
        private string _unsavedDockText;
        private bool _unsaved;

        public FileInfo LUAFile => _file;

        public DockLUADocument()
        {
            InitializeComponent();

           
            
            this.InitalizeStyling();
        }

        public DockLUADocument(string text, Image icon, FileInfo file)
            : this()
        {
            _regularDockText = text;
            _unsavedDockText = text + "*";

            DockText = text;
            Icon = icon;

            _file = file;
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
            // Load the file contents.
            this.txtEditor.Text = File.ReadAllText(_file.FullName);

            this.DockText = _regularDockText;
            _unsaved = false;
        }

        private void InitalizeStyling()
        {
            this.txtEditor.Lexer = Lexer.Lua;

            this.txtEditor.StyleResetDefault();

            this.txtEditor.Styles[Style.Default].Font = "Consolas";
            this.txtEditor.Styles[Style.Default].Size = 12;

            this.txtEditor.Styles[Style.Default].BackColor = Color.FromArgb(29, 31, 33);
            this.txtEditor.Styles[Style.Default].ForeColor = Color.FromArgb(197, 200, 198);

            this.txtEditor.StyleClearAll();

            this.txtEditor.Styles[Style.Lua.Comment].ForeColor = Color.FromArgb(181, 189, 104);
            this.txtEditor.Styles[Style.Lua.CommentLine].ForeColor = Color.FromArgb(181, 189, 104);
            this.txtEditor.Styles[Style.Lua.CommentLine].Italic = true;

            this.txtEditor.Styles[Style.Lua.String].ForeColor = Color.FromArgb(222, 147, 95); 

            this.txtEditor.Styles[Style.Lua.Operator].ForeColor = Color.FromArgb(240, 198, 116); 

            this.txtEditor.Styles[Style.Lua.Number].ForeColor = Color.FromArgb(138, 190, 183);

            this.txtEditor.Styles[Style.Lua.Preprocessor].ForeColor = Color.FromArgb(129, 162, 190);

            this.txtEditor.Styles[Style.Lua.Identifier].ForeColor = Color.FromArgb(178, 148, 187);

            this.txtEditor.Styles[Style.Lua.Word].ForeColor = Color.FromArgb(130, 239, 104);

            this.txtEditor.SetKeywords(0, "if then end not function");
        }

        public void Save()
        {
            File.WriteAllText(_file.FullName, this.txtEditor.Text);
            this.DockText = _regularDockText;
            _unsaved = false;
        }

        private void txtEditor_TextChanged(object sender, System.EventArgs e)
        {
            this.DockText = _unsavedDockText;
            _unsaved = true;
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
