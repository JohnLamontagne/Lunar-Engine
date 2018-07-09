namespace Lunar.Server.Utilities.Events
{
    public class GameEventArgs
    {
        /// <summary>
        /// Name of the event which occurred
        /// </summary>
        public string EventName { get; }

        /// <summary>
        /// Object at which the game event occurred
        /// </summary>
        public IGameEventSource EventSource { get; }

        public GameEventArgs(string eventName, IGameEventSource invoker)
        {
            this.EventName = eventName;
            this.EventSource = invoker;
        }
    }
}
