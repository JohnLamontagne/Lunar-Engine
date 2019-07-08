using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DarkUI.Controls;
using DarkUI.Docking;
using IronPython;
using Lunar.Core.Utilities.Logic;
using Lunar.Server.World.Dialogue;

namespace Lunar.Editor.Controls
{
    public partial class DockDialogueDocument : SavableDocument
    {
        private Project _project;
        private Dialogue _dialogue;
        private DialogueBranch _selectedBranch;

        public DockDialogueDocument()
        {
        }

        public DockDialogueDocument(Project project, string text, Image icon, FileInfo file)
            : base(file)
        {
            InitializeComponent();

            _project = project;

            DockText = text;
            Icon = icon;

            _dialogue = _project.LoadDialogue(file.FullName);

            this.FillData();
        }

        private void AddResponseData(DialogueResponse response)
        {
            var responseItem = new DarkListItem(response.Text.Truncate(20))
            {
                Tag = new Action(() =>
                {
                    this.responsePanel.Show();

                    this.txtResponseText.Text = response.Text;

                    if (!string.IsNullOrEmpty(response.Condition))
                    {
                        if (!this.cmbDisplayCond.Items.Contains(response.Condition))
                            this.cmbDisplayCond.Items.Add(response.Condition);

                        this.cmbDisplayCond.SelectedItem = response.Condition;
                    }

                    if (!string.IsNullOrEmpty(response.Function))
                    {
                        if (!this.cmbResponseFunction.Items.Contains(response.Function))
                            this.cmbResponseFunction.Items.Add(response.Function);

                        this.cmbResponseFunction.SelectedItem = response.Function;
                    }

                    if (!string.IsNullOrEmpty(response.Next))
                    {
                        if (!this.cmbNextBranch.Items.Contains(response.Next))
                            this.cmbDisplayCond.Items.Add(response.Next);

                        this.cmbDisplayCond.SelectedItem = response.Next;
                    }
                })
            };
            this.lstResponses.Items.Add(responseItem);
        }

        private void AddBranchData(DialogueBranch branch)
        {
            var branchItem = new DarkListItem(branch.Name)
            {
                Tag = new Action(() =>
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
                })
            };

            this.lstBranches.Items.Add(branchItem);
        }

        private void FillData()
        {
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

            ((Action)this.lstBranches.Items[this.lstBranches.SelectedIndices[0]].Tag).Invoke();
        }

        private void LstResponses_SelectedIndicesChanged(object sender, EventArgs e)
        {
            if (this.lstResponses.SelectedIndices.Count <= 0 || this.lstResponses.Items.Count <= this.lstResponses.SelectedIndices[0])
            {
                this.responsePanel.Hide();
                return;
            }

            ((Action)this.lstResponses.Items[this.lstResponses.SelectedIndices[0]].Tag).Invoke();
        }

        private void BtnAddResponse_Click(object sender, EventArgs e)
        {
            var response = new DialogueResponse()
            {
                Text = "Enter your response text here..."
            };

            _selectedBranch.AddResponse(response);
        }
    }
}