using Lunar.Server.Utilities.Scripting;
using Lunar.Server.World.Actors;
using System;


namespace Lunar.Server.World.Structure
{
    public class MapObjectBehaviorDefinition
    {
        public ScriptAction OnEntered { get; set; }

        public ScriptAction OnLeft { get; set; }

        public ScriptAction OnInteract { get; set; }

        public ScriptAction Update { get; set; }
    }
}
