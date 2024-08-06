using System;
using System.Collections.Generic;

namespace Code
{
    [Serializable]
    public class GameEvent
    {
        public string type;
        public string data;

        public GameEvent(string type, string data)
        {
            this.type = type;
            this.data = data;
        }
    }

    [Serializable]
    public class EventsQueue
    {
        public List<GameEvent> events = new();
    }

}