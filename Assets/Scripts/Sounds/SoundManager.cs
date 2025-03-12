using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    AudioSource audio;
    public AudioClip playerAttack;
    public AudioClip pickUp;

    private void Start()
    {
        Instance = this;
        audio = GetComponent<AudioSource>();
        SetVolume();
    }

    public void PlaySound(string sound)
    {
        if (PlayerPrefs.GetInt("Sound") == 1)
        {
            if (sound == "Test")
            {
                Debug.Log("Test Sound");
            }

            if (sound == "PlayerAttack")
            {
                audio.clip = playerAttack;
                audio.Play();
            }
            if (sound == "PickUp")
            {
                audio.clip = pickUp;
                audio.Play();
            }
        }
    }

    public void SetVolume()
    {
        audio.volume = PlayerPrefs.GetFloat("SoundVolume");
    }
}
