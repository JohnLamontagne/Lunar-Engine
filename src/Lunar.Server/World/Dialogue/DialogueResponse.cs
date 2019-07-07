namespace Lunar.Server.World.Dialogue
{
    public class DialogueResponse
    {
        public string Text { get; set; }

        public string Next { get; set; }

        public string Function { get; set; }

        public string Condition { get; set; }

        public bool IsScripted { get; set; }

        public DialogueResponse()
        {
        }
    }
}