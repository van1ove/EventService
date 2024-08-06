using System.IO;
using UnityEngine;

namespace Code
{
    public class EventFileHandler : IEventFileHandler
    {
        #region Variables

        private readonly string _filePath;

        #endregion

        #region Methods

        public EventFileHandler(string fileName)
        {
            _filePath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + fileName;
        }

        public EventsQueue LoadEvents(bool needClearStorage = true)
        {
            if (!File.Exists(_filePath))
            {
                Debug.LogWarning("Events file doesn't exist");
                return new EventsQueue();
            }
            string jsonString = File.ReadAllText(_filePath);
            EventsQueue eventsQueue = JsonUtility.FromJson<EventsQueue>(jsonString);
            
            if (needClearStorage)
                ClearStorage();
            
            return eventsQueue ?? new EventsQueue();
        }

        public void SaveEvents(EventsQueue eventsQueue)
        {
            if (eventsQueue.events.Count == 0)
                return;
            
            if (!File.Exists(_filePath))
            {
                Debug.LogWarning("Events file doesn't exist. Creating a new one");
                File.Create(_filePath);
            }
            else
            {
                GameEvent[] tmpQueue = new GameEvent[eventsQueue.events.Count];
                eventsQueue.events.CopyTo(tmpQueue);
                eventsQueue = LoadEvents();
                eventsQueue.events.AddRange(tmpQueue);
            }
            
            string jsonString = JsonUtility.ToJson(eventsQueue);
            File.WriteAllText(_filePath, jsonString);
        }
        
        public void SaveEventsByJson(string json)
        {
            EventsQueue eventsQueue = JsonUtility.FromJson<EventsQueue>(json);
            SaveEvents(eventsQueue);
        }

        public void ClearStorage() => File.WriteAllText(_filePath, "");

        #endregion
    }
}