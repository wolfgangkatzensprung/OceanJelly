using UnityEngine;

public class MusicVolume : MonoBehaviour
{
    public AudioLowPassFilter lowpass;
    public AudioSource source;

    public float maxVolume;
    public float lowpassFactor = 800;
    public float range = 700;
    public float innerRadius = 33;
    public float volumeFactor = .311f;

    private void Start()
    {
        maxVolume = PlayerPrefs.GetFloat("MusicVolume");
    }

    private void Update()
    {
        float distanceToPlayer = PlayerManager.Instance.GetDistanceToPlayer(transform.position);
        lowpass.cutoffFrequency = 22000 - lowpassFactor * Mathf.Sqrt(Mathf.Round(PlayerManager.Instance.GetDistanceToPlayer(transform.position)));
        source.volume = maxVolume / ((Mathf.Sqrt(Mathf.Abs(Mathf.Round((distanceToPlayer))))) * volumeFactor);

    }
}