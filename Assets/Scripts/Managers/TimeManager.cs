using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager _instance;
    public static TimeManager Instance { get { return _instance; } }

    bool slowmotionActive;
    float timer = 0f;    //absteigender timer
    float globalTimer = 0f;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
    }
    private void Start()
    {
        timer = 0f;
        globalTimer = 0f;
    }

    private void Update()
    {
        globalTimer += Time.deltaTime;

        if (timer > 0)
        {
            timer -= Time.unscaledDeltaTime;
        }
        else if (timer < 0)
        {
            timer = 0;
            ResetTimeScale();
        }
    }

    // Freeze TimeScale zB fuer Death

    public void Freeze()
    {
        Debug.Log("Freeze Time");
        Time.timeScale = 0f;
    }

    public void HitFreeze(float duration, float intensity = 0f) // intensity 0 heisst die zeit wird kurz komplett gestoppt. damit sichs nicht zu hart anfuehlt sollte das also etwas ueber 0f sein
    {
        Debug.Log("HitFreeze Effect");
        if (!slowmotionActive)
        {

            Time.timeScale = intensity;            // stoppt Zeit
            if (timer > 0)             // wenn timer schon verwendet wird, da mehrere Sachen gleichzeitig getroffen wurden
            {
                if (timer > duration)       // wenn der aktuelle timer größer ist als der neue wert, ist keine weitere timeScale-Veränderung notwendig und die methode endet
                {
                    return;
                }
                timer = duration;           // ansonsten wird die neue timeScale akzeptiert.

            }
            if (timer == 0)             // wenn timer noch frisch ist
            {
                timer = duration;           // dann kriegt er die gewünschte duration
            }
        }
    }

    public float GetGlobalTimer()
    {
        return globalTimer;
    }

    public void StartSlowMotion()
    {
        Debug.Log("StartSlowMotion");
        slowmotionActive = true;
        Time.timeScale = 0.5f;
    }
    public void EndSlowMotion()
    {
        Debug.Log("EndSlowMotion");
        Time.timeScale = 1f;
        slowmotionActive = false;
    }

    // Reset to normal Time
    public void ResetTimeScale()
    {
        Debug.Log("ResetTimeScale");
        Time.timeScale = 1;
    }
}
