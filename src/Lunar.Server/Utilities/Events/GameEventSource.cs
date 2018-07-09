using System;

namespace Lunar.Server.Utilities.Events
{
    public interface IGameEventSource
    {
        event EventHandler<GameEventArgs> EventOccurred;
    }
}
