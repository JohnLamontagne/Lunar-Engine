/** Copyright 2018 John Lamontagne https://www.rpgorigin.com

	Licensed under the Apache License, Version 2.0 (the "License");
	you may not use this file except in compliance with the License.
	You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

	Unless required by applicable law or agreed to in writing, software
	distributed under the License is distributed on an "AS IS" BASIS,
	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	See the License for the specific language governing permissions and
	limitations under the License.
*/

using System.Collections.Generic;
using Lunar.Server.Utilities.Scripting;
using Lunar.Core.Utilities;
using System.Linq;
using Lunar.Core;
using Lunar.Server.World.Actors;

namespace Lunar.Server.World.Dialogue
{
    public class Dialogue
    {
        private Dictionary<string, DialogueBranch> _branches;
        private string _scriptPath;

        public string Name { get; }

        public string ScriptPath
        {
            get => _scriptPath;
            set
            {
                _scriptPath = value;

                if (this.Script != null)
                {
                    this.Script.Reload(Constants.FILEPATH_DATA + "/" + _scriptPath);
                }
            }
        }

        public Script Script { get; set; }

        public IList<DialogueBranch> Branches => _branches.Values.ToList();

        public Dialogue(string name)
        {
            this.Name = name;

            _branches = new Dictionary<string, DialogueBranch>();
        }

        public void AddBranch(DialogueBranch branch)
        {
            if (string.IsNullOrEmpty(branch.Name))
            {
                Engine.Services.Get<Logger>().LogEvent($"Unable to add branch to " +
                                    $"dialogue named {this.Name}: branch is not named!", LogTypes.ERROR);
                return;
            }

            if (_branches.ContainsKey(branch.Name))
            {
                Engine.Services.Get<Logger>().LogEvent($"Unable to add branch {branch.Name} to " +
                                    $"dialogue named {this.Name}: branch already exists!", LogTypes.ERROR);
                return;
            }

            _branches.Add(branch.Name, branch);
        }

        public void RemoveBranch(DialogueBranch branch)
        {
            _branches.Remove(branch.Name);
        }

        public bool BranchExists(string name)
        {
            return _branches.ContainsKey(name);
        }

        /// <summary>
        /// Plays the specified branch of the dialogue.
        /// </summary>
        /// <param name="branchName"></param>
        public void Play(string branchName, Player player)
        {
            if (!_branches.ContainsKey(branchName))
                Engine.Services.Get<Logger>().LogEvent($"Invalid dialogue branch {branchName}.", LogTypes.ERROR);

            _branches[branchName].Begin(player);
        }
    }
}