namespace Code
{
    public interface IEventFileHandler
    {
        public EventsQueue LoadEvents(bool needClearStorage = true);
        public void SaveEvents(EventsQueue eventsQueue);
    }
}