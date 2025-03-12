using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// LevelSet Room im Ozean
/// </summary>
public class Room : MonoBehaviour
{
    public Light2D roomLight;
    float startIntensity;
    private const float darkIntensity = .01f;

    [Tooltip("If player is inside the room")]
    public bool playerInside;

    bool initialized;

    private void OnEnable()
    {
        if (initialized)
        {
            roomLight.intensity = darkIntensity;
        }
    }

    private void Start()
    {
        roomLight = GetComponent<Light2D>();
        startIntensity = roomLight.intensity;
        roomLight.intensity = darkIntensity;

        initialized = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInside = true;
            roomLight.intensity = startIntensity;
            LightManager.Instance.SetGlobalLightIntensity(.1f, 1.2f);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInside = false;
            roomLight.intensity = darkIntensity;
            LightManager.Instance.SetGlobalLightIntensity(1f, 3f);
        }
    }
}
