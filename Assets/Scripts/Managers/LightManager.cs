using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightManager : MonoBehaviour
{
    public static LightManager _instance;
    public static LightManager Instance { get { return _instance; } }

    //public UnityEngine.Rendering.Universal.Light2D globalLight;
    //public LightData lightData;
    public Light2D globalLight;

    private Color targetColor;
    private float targetIntensity;
    private float transitionDuration;
    private float transitionTimer;

    private void Start()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }
    private void Update()
    {
        if (transitionTimer < transitionDuration)
        {
            transitionTimer += Time.deltaTime;
            float t = Mathf.Clamp01(transitionTimer / transitionDuration);

            // Update light intensity
            if (globalLight.intensity != targetIntensity)
            {
                float newIntensity = Mathf.Lerp(globalLight.intensity, targetIntensity, t);
                globalLight.intensity = newIntensity;
            }

            // Update light color
            if (globalLight.color != targetColor)
            {
                Color newColor = Color.Lerp(globalLight.color, targetColor, t);
                globalLight.color = newColor;
            }
        }
    }

    public void TurnOnLight(Transform trans)
    {
        trans.GetComponent<Light>().enabled = true;
    }

    internal void SetGlobalLightIntensity(float v)
    {
        globalLight.intensity = v;
    }

    internal void SetGlobalLightColor(Color c)
    {
        globalLight.color = c;
    }

    internal void SetGlobalLightIntensity(float v, float duration)
    {
        targetIntensity = v;
        transitionDuration = duration;
        transitionTimer = 0f;
    }

    internal void SetGlobalLightColor(Color c, float duration)
    {
        targetColor = c;
        transitionDuration = duration;
        transitionTimer = 0f;
    }

    internal Color GetGlobalLightColor()
    {
        return globalLight.color;
    }
}
