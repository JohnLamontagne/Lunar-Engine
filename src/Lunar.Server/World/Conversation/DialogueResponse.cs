using System;

namespace Lunar.Server.World.Conversation
{
    public class DialogueResponse
    {
        public Guid UniqueID { get; }

        public string Text { get; set; }

        public string Next { get; set; }

        public string Function { get; set; }

        public string Condition { get; set; }

        public bool IsScripted { get; set; }

        public DialogueResponse()
        {
            this.UniqueID = Guid.NewGuid();
        }
    }
}