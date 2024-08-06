namespace Code
{
    public interface IEventService
    {
        public void TrackEvent(string type, string data);
    }
}