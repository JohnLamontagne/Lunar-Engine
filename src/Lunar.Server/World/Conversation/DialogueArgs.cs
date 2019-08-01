using Lunar.Server.Utilities.Scripting;
using Lunar.Server.World.Actors;

namespace Lunar.Server.World.Conversation
{
    public class DialogueArgs : ServerArgs
    {
        public Dialogue Dialogue { get; }
        public Player Listener { get; }

        public DialogueArgs(Dialogue dialogue, Player listener)
        {
            this.Dialogue = dialogue;
            this.Listener = listener;
        }
    }
}