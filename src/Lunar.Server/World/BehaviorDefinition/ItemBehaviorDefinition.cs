using Lunar.Server.Utilities.Scripting;


namespace Lunar.Server.World.BehaviorDefinition
{
    public class ItemBehaviorDefinition
    {
        /// <summary>
        /// Invoked when the item is used
        /// </summary>
        public ScriptAction OnUse { get; set; }

        /// <summary>
        /// Invoked when the item is equipped
        /// </summary>
        public ScriptAction OnEquip { get; set; }

        /// <summary>
        /// Invoked when the item is acquired by an actor
        /// </summary>
        public ScriptAction OnAcquired { get; set; }

        /// <summary>
        /// Invoked when the item is dropped by an actor
        /// </summary>
        public ScriptAction OnDropped { get; set; }

        /// <summary>
        /// Invoked when the item is created within the gameworld
        /// </summary>
        public ScriptAction OnCreated { get; set; }
    }
}
