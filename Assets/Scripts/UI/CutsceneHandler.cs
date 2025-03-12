using UnityEngine;

public class CutsceneHandler : MonoBehaviour
{
    public static CutsceneHandler _instance;
    public static CutsceneHandler Instance
    {
        get
        {
            return _instance;
        }
    }


    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    public void StartCutscene()
    {

    }

    public void EndCutscene()
    {

    }

}