using UnityEngine;

public class HopeMelody : MonoBehaviour
{
    public GameObject melodyPoolParent;
    public float basePitch = 1f;
    public float semitone = 1f;
    public float octaveInterval = 12f; // Number of semitones in an octave

    private int currentScaleIndex = 0;
    private AudioSource[] melodyAudioSources;

    private int[] majorScale =
    {
        0, 2, 4, 5, 7, 9, 11, 12
    };

    private void Start()
    {
        // Initialize the pool of melody audio sources
        melodyAudioSources = melodyPoolParent.GetComponentsInChildren<AudioSource>();

        PlayerHealth.Instance.onDeath += ResetMelodyCycle;
    }

    private void ResetMelodyCycle()
    {
        currentScaleIndex = 0;
    }

    private AudioSource GetFreeMelodyAudioSource()
    {
        int numMelodyAudioSources = melodyAudioSources.Length;

        // Find a free melody audio source
        for (int i = 0; i < numMelodyAudioSources; i++)
        {
            AudioSource melodyAudioSource = melodyAudioSources[i];

            if (!melodyAudioSource.isPlaying)
            {
                return melodyAudioSource;
            }
        }

        // If all existing melody audio sources are playing, add a new one dynamically
        GameObject newMelodyAudioSourceObj = new GameObject("MelodyAudioSource");
        newMelodyAudioSourceObj.transform.parent = melodyPoolParent.transform;
        AudioSource newMelodyAudioSource = newMelodyAudioSourceObj.AddComponent<AudioSource>();

        // Set the audio clip of the new melody audio source to match the first child's audio clip
        newMelodyAudioSource.clip = melodyAudioSources[0].clip;

        melodyAudioSources = melodyPoolParent.GetComponentsInChildren<AudioSource>();
        return newMelodyAudioSource;
    }

    public void PlayNextMelodyNote()
    {
        AudioSource melodyAudioSource = GetFreeMelodyAudioSource();

        // Calculate the pitch adjustment based on the current scale degree and the corresponding note in the major scale
        float pitchAdjustment = basePitch * Mathf.Pow(2f, majorScale[currentScaleIndex] / octaveInterval);

        // Change the pitch of the melody audio source
        melodyAudioSource.pitch = pitchAdjustment;

        // Play the melody note
        melodyAudioSource.Play();
        Debug.Log($"Play HopeMelody Note from Source {melodyAudioSource} with Pitch {melodyAudioSource.pitch}");

        // Increment the scale degree and wrap around within the available notes
        currentScaleIndex++;
        currentScaleIndex %= majorScale.Length;
    }
}
