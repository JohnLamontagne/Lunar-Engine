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
using Lunar.Server.Content.Graphics;
using System;
using Lunar.Core.Utilities.Data;
using Lunar.Server.Utilities.Scripting;
using Lunar.Server.World.BehaviorDefinition;

namespace Lunar.Server.World.Actors
{
    public class NPCDescriptor
    {
        private string _name;
        private int _level;
        private float _speed;
        private int _maximumHealth;
        private Rect _collisionBounds;
        private Vector _frameSize;
        private int _aggresiveRange;
        private Sprite _sprite;
        private Vector _maxRoam;
        private int _attackRange;
        private ActorBehaviorDefinition _behaviorDefinition;
   
        public string Name => _name;
        public Sprite Sprite => _sprite;
        public int Level => _level;
        public float Speed => _speed;
        public int MaximumHealth => _maximumHealth;
        public Rect CollisionBounds => _collisionBounds;
        public int AggresiveRange => _aggresiveRange;
        public Vector MaxRoam => _maxRoam;
        public Vector FrameSize => _frameSize;
        public int AttackRange => _attackRange;
        public ActorBehaviorDefinition BehaviorDefinition => _behaviorDefinition;

        public static NPCDescriptor Load(string fileName)
        {
            string name = "";
            int level = 0;
            float speed = 0f;
            int maximumHealth = 0;
            Rect collisionBounds = new Rect();
            int aggresiveRange = 0;
            string spriteName = "";
            Vector maxRoam = new Vector();
            Vector frameSize = new Vector();
            int attackRange = 0;
            ActorBehaviorDefinition behaviorDefinition;


            Script script = Server.ServiceLocator.GetService<ScriptManager>().GetScript(Constants.FILEPATH_NPCS + fileName);
           

            // Load the NPC data from the lua NPC definition.
            var npcDef = script.GetTable("NPC");

            name = npcDef["Name"].ToString();
            spriteName = npcDef["Sprite"].ToString();
            frameSize = (Vector)npcDef["FrameSize"];
            aggresiveRange = Convert.ToInt32(npcDef["AggressiveRange"]);
            level = Convert.ToInt32(npcDef["Level"]);
            speed = Convert.ToSingle(npcDef["Speed"]);
            maximumHealth = Convert.ToInt32(npcDef["MaximumHealth"]);
            collisionBounds = (Rect)npcDef["CollisionBounds"];
            maxRoam = (Vector)npcDef["MaxRoam"];
            attackRange = Convert.ToInt32(npcDef["AttackRange"]);
            behaviorDefinition = (ActorBehaviorDefinition)npcDef["BehaviorDefinition"];



            NPCDescriptor desc = new NPCDescriptor()
            {
                _name = name,
                _sprite = new Sprite(spriteName),
                _frameSize = frameSize,
                _level = level,
                _speed = speed,
                _maximumHealth = maximumHealth,
                _collisionBounds = collisionBounds,
                _aggresiveRange = aggresiveRange,
                _maxRoam = maxRoam,
                _attackRange = attackRange,
                _behaviorDefinition = behaviorDefinition
            };

            script.ScriptChanged += desc.Script_ScriptChanged;

            return desc;
        }

        private void Script_ScriptChanged(object sender, EventArgs e)
        {
            var script = (Script) sender;

            // Load the NPC data from the lua NPC definition.
            var npcDef = script.GetTable("NPC");

            _behaviorDefinition = (ActorBehaviorDefinition)npcDef["BehaviorDefinition"];

            this.DefinitionChanged?.Invoke(this, new EventArgs());
        }

        public event EventHandler DefinitionChanged;
    }
}
