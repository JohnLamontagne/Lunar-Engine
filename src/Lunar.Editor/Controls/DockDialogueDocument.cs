using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using DarkUI.Controls;
using DarkUI.Forms;
using IronPython.Runtime;
using Lunar.Core.Utilities.Logic;
using Lunar.Server.Utilities.Scripting;
using Lunar.Server.World.Conversation;

namespace Lunar.Editor.Controls
{
    public partial class DockDialogueDocument : SavableDocument
    {
        private FileInfo _file;
        private Project _project;
        private Dialogue _dialogue;
        private DialogueBranch _selectedBranch;
        private DialogueResponse _selectedResponse;

        private ScriptManager _scriptManager;

        public DockDialogueDocument()
        {
        }

        public DockDialogueDocument(Project project, string text, Image icon, FileInfo file)
            : base(file)
        {
            InitializeComponent();

            _project = project;
            _file = file;

            _scriptManager = new ScriptManager(_project.ServerWorldDirectory + "/Scripts/", "");

            DockText = text;
            Icon = icon;

            _dialogue = _project.LoadDialogue(file.FullName);

            this.FillData();
        }

        private void LoadScript()
        {
            _dialogue.Script = _scriptManager.CreateScript(_project.ServerRootDirectory + "/" + _dialogue.ScriptPath);
            if (_dialogue.Script != null)
                _dialogue.Script.ScriptChanged += Script_ScriptChanged;
        }

        private void Script_ScriptChanged(object sender, EventArgs e)
        {
            // Recompile the script
            _dialogue.Script = _scriptManager.CreateScript(_project.ServerRootDirectory + "/" + _dialogue.ScriptPath);

            this.FillScriptData();
        }

        private void FillScriptData()
        {
            this.cmbDisplayCond.Items.Clear();
            this.cmbResponseFunction.Items.Clear();

            this.cmbDisplayCond.Items.Add("None");
            this.cmbResponseFunction.Items.Add("None");

            var functions = _dialogue.Script.GetVariables<PythonFunction>();

            foreach (var function in functions)
            {
                var code = function.Value.__code__;
                var argsProperty = code.GetType().GetProperty("ArgNames", BindingFlags.NonPublic | BindingFlags.Instance);
                string[] args = (string[])argsProperty.GetValue(code, null);

                if (args.Length == 1)
                {
                    this.cmbResponseFunction.Items.Add(function.Key);
                    this.cmbDisplayCond.Items.Add(function.Key);
                }
            }
        }

        private void OnResponseSelected(DialogueResponse response)
        {
            _selectedResponse = response;

            this.responsePanel.Show();

            this.txtResponseText.Text = response.Text;

            this.cmbNextBranch.Items.Clear();

            this.cmbNextBranch.Items.Add("None");

            foreach (var branch in _dialogue.Branches)
            {
                this.cmbNextBranch.Items.Add(branch.Name);
            }

            if (_dialogue.Script != null)
                this.FillScriptData();
            else
            {
                this.cmbDisplayCond.Items.Clear();
                this.cmbResponseFunction.Items.Clear();

                this.cmbDisplayCond.Items.Add("None");
                this.cmbResponseFunction.Items.Add("None");
            }

            if (!string.IsNullOrEmpty(response.Condition))
            {
                if (!this.cmbDisplayCond.Items.Contains(response.Condition))
                {
                    DarkMessageBox.ShowError($"Function {response.Condition} does not exist for response condition.", "Script Error!");
                    this.cmbDisplayCond.SelectedIndex = 0;
                }
                else
                {
                    this.cmbDisplayCond.SelectedItem = response.Condition;
                }
            }
            else
            {
                this.cmbDisplayCond.SelectedIndex = 0;
            }

            if (!string.IsNullOrEmpty(response.Function))
            {
                if (!this.cmbResponseFunction.Items.Contains(response.Function))
                {
                    DarkMessageBox.ShowError($"Function {response.Function} does not exist for response function.", "Script Error!");
                    this.cmbResponseFunction.SelectedIndex = 0;
                }
                else
                {
                    this.cmbResponseFunction.SelectedItem = response.Function;
                }
            }
            else
            {
                this.cmbResponseFunction.SelectedIndex = 0;
            }

            if (!string.IsNullOrEmpty(response.Next))
            {
                if (!this.cmbNextBranch.Items.Contains(response.Next))
                {
                    DarkMessageBox.ShowError($"Branch {response.Next} does not exist for response Next property.", "Dialogue Error!");
                    this.cmbNextBranch.SelectedIndex = 0;
                }
                else
                {
                    this.cmbNextBranch.SelectedItem = response.Next;
                }
            }
            else
            {
                this.cmbNextBranch.SelectedIndex = 0;
            }
        }

        private void AddResponseData(DialogueResponse response)
        {
            var responseItem = new DarkListItem(response.Text.Truncate(20))
            {
                Tag = response
            };
            this.lstResponses.Items.Add(responseItem);
        }

        private void OnBranchSelected(DialogueBranch branch)
        {
            _selectedBranch = branch;
            this.branchPanel.Show();

            this.txtBranchText.Text = branch.Text;

            this.lstResponses.Items.Clear();

            foreach (var response in branch.Responses)
            {
                this.AddResponseData(response);
            }

            if (branch.Responses.Count > 0)
                this.lstResponses.SelectItem(0);
        }

        private void AddBranchData(DialogueBranch branch)
        {
            var branchItem = new DarkListItem(branch.Name)
            {
                Tag = branch
            };

            this.lstBranches.Items.Add(branchItem);
        }

        private void FillData()
        {
            this.lstBranches.Items.Clear();
            this.lstResponses.Items.Clear();

            if (!string.IsNullOrEmpty(_dialogue.ScriptPath))
                this.LoadScript();

            foreach (var branch in _dialogue.Branches)
            {
                this.AddBranchData(branch);
            }

            if (_dialogue.Branches.Count > 0)
                this.lstBranches.SelectItem(0);
        }

        private void DarkTextBox2_TextChanged(object sender, EventArgs e)
        {
        }

        private void DarkSectionPanel2_Paint(object sender, PaintEventArgs e)
        {
        }

        private void ButtonAddBranch_Click(object sender, EventArgs e)
        {
            // Figure out the branch name
            string branchName = "Branch";
            int branchNum = 0;

            while (_dialogue.BranchExists($"{branchName}{branchNum}"))
            {
                branchNum++;
            }

            branchName = $"{branchName}{branchNum}";

            var branch = new DialogueBranch(_dialogue, branchName, "Enter your branch text here...");
            _dialogue.AddBranch(branch);

            this.AddBranchData(branch);
        }

        private void LstBranches_SelectedIndicesChanged(object sender, EventArgs e)
        {
            if (this.lstBranches.SelectedIndices.Count <= 0 || this.lstBranches.Items.Count <= this.lstBranches.SelectedIndices[0])
            {
                this.branchPanel.Hide();
                return;
            }

            this.OnBranchSelected((DialogueBranch)this.lstBranches.Items[this.lstBranches.SelectedIndices[0]].Tag);
        }

        private void LstResponses_SelectedIndicesChanged(object sender, EventArgs e)
        {
            if (this.lstResponses.SelectedIndices.Count <= 0 || this.lstResponses.Items.Count <= this.lstResponses.SelectedIndices[0])
            {
                this.responsePanel.Hide();
                return;
            }

            this.OnResponseSelected((DialogueResponse)this.lstResponses.Items[this.lstResponses.SelectedIndices[0]].Tag);
        }

        private void BtnAddResponse_Click(object sender, EventArgs e)
        {
            var response = new DialogueResponse()
            {
                Text = "Enter your response text here..."
            };

            if (_selectedBranch != null)
            {
                _selectedBranch.AddResponse(response);
                this.AddResponseData(response);
            }
        }

        private void CmbNextBranch_SelectedValueChanged(object sender, EventArgs e)
        {
            if (_selectedResponse == null)
                return;

            if (string.IsNullOrEmpty(this.cmbNextBranch.Text) || this.cmbNextBranch.Text == "None")
            {
                _selectedResponse.Next = string.Empty;
                return;
            }

            _selectedResponse.Next = this.cmbNextBranch.Text;
        }

        private void CmbResponseFunction_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_selectedResponse == null)
                return;

            if (string.IsNullOrEmpty(this.cmbResponseFunction.Text) || this.cmbResponseFunction.Text == "None")
            {
                _selectedResponse.Function = string.Empty;
                return;
            }

            _selectedResponse.Function = this.cmbResponseFunction.Text;
        }

        private void CmbDisplayCond_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_selectedResponse == null)
                return;

            if (string.IsNullOrEmpty(this.cmbDisplayCond.Text) || this.cmbDisplayCond.Text == "None")
            {
                _selectedResponse.Condition = string.Empty;
                return;
            }

            _selectedResponse.Condition = this.cmbDisplayCond.Text;
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            _project.SaveDialogue(_file.FullName, _dialogue);
        }

        private void TxtBranchText_TextChanged(object sender, EventArgs e)
        {
            if (_selectedBranch != null)
                _selectedBranch.Text = this.txtBranchText.Text;
        }

        private void TxtResponseText_TextChanged(object sender, EventArgs e)
        {
            if (_selectedResponse != null)
            {
                _selectedResponse.Text = this.txtResponseText.Text;
                this.lstResponses.Items[this.lstResponses.SelectedIndices[0]].Text = _selectedResponse.Text.Truncate(20);
            }
        }

        private void ButtonRemoveBranch_Click(object sender, EventArgs e)
        {
            if (_selectedBranch != null)
            {
                _dialogue.RemoveBranch(_selectedBranch);
                this.FillData();
            }
        }

        private void BtnRemoveResponse_Click(object sender, EventArgs e)
        {
            if (_selectedResponse != null)
            {
                _selectedBranch?.RemoveResponse(_selectedResponse);
                this.FillData();
            }
        }
    }
}