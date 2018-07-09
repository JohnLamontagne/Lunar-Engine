using System;
using System.Collections.Generic;
using Lunar.Core.Utilities;

namespace Lunar.Server.Utilities.Events
{
    public class GameEventListener : IGameEventSource, IService
    {
        private List<IGameEventSource> _eventSources;

        public GameEventListener()
        {
            _eventSources = new List<IGameEventSource>();
        }

        public void Initalize()
        {
        }

        public void Register(IGameEventSource eventSource)
        {
            eventSource.EventOccurred += EventSourceOnEventOccurred;

            _eventSources.Add(eventSource);
        }

        private void EventSourceOnEventOccurred(object invoker, GameEventArgs args)
        {
           this.EventOccurred?.Invoke(args.EventSource, args);
        }

        public event EventHandler<GameEventArgs> EventOccurred;

    }
}
