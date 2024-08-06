using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Code
{
    public class EventService : MonoBehaviour, IEventService
    {
        #region Variables
        
        [Header("Sending")]
        [SerializeField] private float cooldownBeforeSend = 1.0f;
        [SerializeField] private string serverUrl = "http://server-url.com";
        
        [Header("Saving")] 
        [SerializeField] private string fileName = "data.json";
        private bool _isCooldownActive;
        
        private EventFileHandler _eventFileHandler;
        private EventsQueue _eventsQueue;

        #endregion

        #region Methods

        private void Start()
        {
            _isCooldownActive = false;
            AppDomain.CurrentDomain.UnhandledException += HandleException;
            _eventFileHandler = new EventFileHandler(fileName);
            _eventsQueue = _eventFileHandler.LoadEvents();
            
            if (_eventsQueue.events.Count > 0)
                StartCoroutine(SendEventsAfterCooldown());
        }
        
        public void TrackEvent(string type, string data)
        {
            if (_eventsQueue.events.Count == 0)
                _eventFileHandler.LoadEvents(false);
            
            _eventsQueue.events.Add(new GameEvent(type, data));
            if (!_isCooldownActive) 
                StartCoroutine(SendEventsAfterCooldown());
        }

        private IEnumerator SendEventsAfterCooldown()
        {
            _isCooldownActive = true;
            yield return new WaitForSeconds(cooldownBeforeSend);

            if (_eventsQueue.events.Count > 0)
            {
                string json = JsonUtility.ToJson(_eventsQueue);
                StartCoroutine(SendDataToServer(json));
                _eventsQueue.events.Clear();
            }
            _isCooldownActive = false;
        }

        private IEnumerator SendDataToServer(string jsonPayload)
        {
            UnityWebRequest webRequest = new UnityWebRequest(serverUrl, "POST");
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonPayload);
            webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.responseCode == 200)
            {
                Debug.Log("Events sent successfully");
                _eventFileHandler.ClearStorage();
            }
            else
            {
                Debug.LogError("Failed to send events. Error: " + webRequest.error);
                _eventFileHandler.SaveEventsByJson(jsonPayload);
            }
            
            webRequest.Dispose();
        }

        private void HandleException(object sender, UnhandledExceptionEventArgs e)
        {
            _eventFileHandler.SaveEvents(_eventsQueue);
        }
        private void OnApplicationQuit()
        {
            _eventFileHandler.SaveEvents(_eventsQueue);
        }

        #endregion
    }
}