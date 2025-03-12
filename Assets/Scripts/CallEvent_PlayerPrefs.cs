using UnityEngine;
using UnityEngine.Events;

public class CallEvent_PlayerPrefs : MonoBehaviour
{
    public string playerPrefsKey = "";
    public UnityEvent eventToCall;

    private void Start()
    {
        int storedValue = PlayerPrefs.GetInt(playerPrefsKey, 0);

        if (storedValue == 0)
        {
            eventToCall.Invoke();
        }
    }
}
