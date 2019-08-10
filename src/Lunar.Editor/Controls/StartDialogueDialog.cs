using System;
using System.IO;
using System.Windows.Forms;
using DarkUI.Forms;
using Lunar.Core.World.Actor.Descriptors;
using Lunar.Server.World.Conversation;

namespace Lunar.Editor.Controls
{
    public partial class StartDialogueDialog : DarkDialog
    {
        private Form _parentForm;
        private Project _project;
        private Dialogue _dialogue;

        public string Dialogue { get; private set; }

        public string Branch { get; private set; }

        public StartDialogueDialog(Form parentForm, Project project)
        {
            _parentForm = parentForm;
            _project = project;

            this.btnOk.Click += BtnOk_Click;

            InitializeComponent();

            this.cmbDialogue.Items.Clear();
            this.cmbDialogue.Items.Add("None");
            foreach (var dialogueFile in project.DialogueFiles)
            {
                var item = new DarkComboItem(Path.GetFileNameWithoutExtension(dialogueFile.Name))
                {
                    Tag = dialogueFile
                };

                this.cmbDialogue.Items.Add(item);
            }
            this.cmbDialogue.SelectedIndex = 0;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            this.Submitted?.Invoke(this, new EventArgs());
            this.Close();
        }

        public event EventHandler<EventArgs> Submitted;

        public event EventHandler<EventArgs> SelectTile;

        private void cmbDialogue_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbDialogue.SelectedIndex >= this.cmbDialogue.Items.Count)
                return;

            var dialogueName = this.cmbDialogue.SelectedItem.ToString();

            if (dialogueName == "None")
                return;

            // Load the dialogue file to get the branches
            var dialogue = _project.LoadDialogue(((this.cmbDialogue.SelectedItem as DarkComboItem).Tag as FileInfo).FullName);
            this.Dialogue = dialogue.Name;

            this.cmbBranch.Items.Clear();
            this.cmbBranch.Items.Add("None");
            foreach (var branch in dialogue.Branches)
            {
                this.cmbBranch.Items.Add(branch.Name);
            }
            this.cmbBranch.SelectedIndex = 0;
        }

        private void CmbBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmbBranch.SelectedIndex >= this.cmbBranch.Items.Count)
                return;

            this.Branch = this.cmbBranch.SelectedItem.ToString();
        }
    }
}