using UnityEngine;

/// <summary>
/// SceneCommander aus Nautiscale
/// </summary>
public class SceneEventHandler : MonoBehaviour
{
    public static SceneEventHandler Instance;
    string[] eventsToHandle;

    [System.Serializable]
    public struct eventsToHandleList
    {
        public string eventName;
        public bool isReadyForTrigger;
        public bool isTriggered;
    }
    public eventsToHandleList[] eventsList;

    private void Awake()
    {
        Instance = this;    // da nur ein SceneCommander pro Level existiert muesste das gehn
        eventsToHandle = new string[eventsList.Length];

        for (int i = 0; i < eventsList.Length; i++)
        {
            eventsToHandle[i] = eventsList[i].ToString();

            Debug.Log(eventsToHandle[i]);
        }
    }

    void CallEvent(string eventToHandle)
    {
        for (int i = 0; i < eventsToHandle.Length; i++)
        {
            if (eventToHandle == eventsToHandle[i])
            {
                RequestEvent(eventToHandle);
            }
            else Debug.Log("Event " + eventsToHandle[i] + " not found.");
        }
    }

    void RequestEvent(string eventToRequest)
    {

    }

    void StartEvent(string eventToStart)
    {

    }
}
