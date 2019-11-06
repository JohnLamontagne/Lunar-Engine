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
using Lunar.Core.Utilities.Data;

namespace Lunar.Core.World.Actor.Descriptors
{
    public class NPCModel : IActorModel
    {
        private int _level;
        private float _speed;
        private int _maximumHealth;
        private Rect _collisionBounds;
        private Vector _frameSize;
        private bool _aggressive;
        private int _aggresiveRange;
        private Vector _maxRoam;
        private int _attackRange;
        private List<string> _scripts;

        public string UniqueID { get; protected set; }

        public string Name { get; set; }

        public string TexturePath { get; set; }

        public int Level { get; set; }

        public Vector Position { get; set; }

        public Stats Stats { get; set; }

        public Stats StatBoosts { get; private set; }

        public float Speed { get; set; }

        public Rect CollisionBounds { get; set; }

        public int AggresiveRange { get; set; }

        public Vector MaxRoam { get; set; }

        public Vector FrameSize { get; set; }

        public Vector Reach { get; set; }

        public string Dialogue { get; set; }

        public string DialogueBranch { get; set; }

        public List<string> Scripts => _scripts;

        public Dictionary<string, object> CustomVariables { get; }

        protected NPCModel()
        {
            _scripts = new List<string>();
            this.CustomVariables = new Dictionary<string, object>();

            this.Stats = new Stats();
        }

        public static NPCModel Create(string uniqueID)
        {
            return new NPCModel()
            {
                Name = "",
                AggresiveRange = 0,
                Reach = new Vector(0, 0),
                TexturePath = "",
                UniqueID = uniqueID
            };
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}