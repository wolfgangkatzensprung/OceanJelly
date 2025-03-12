using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public HopeMelody hm;

    AudioSource audio;

    public AudioClip menuMusic;
    public AudioClip deathMusic;
    public AudioClip oceanMusic;
    public AudioClip dungeonMusic;
    public AudioClip anemonenDiscoMusic;
    public AudioClip frostfireBladeMusic;
    public AudioClip iceCaveBossfight;
    public AudioClip iceCave;

    private void Start()
    {
        Instance = this;
        audio = GetComponent<AudioSource>();
        SetVolume();
    }

    public void PlayMusic(string music)
    {
        AudioClip currentClip = audio.clip;

        if (PlayerPrefs.GetInt("Music") == 0)
        {
            MuteMusic();
        }
        else
        {
            switch (music)
            {
                case "Death":
                    audio.clip = deathMusic;
                    break;
                case "SettingsMenu":
                case "Menu":
                    audio.clip = menuMusic;
                    break;
                case "Shipwreck":
                    audio.clip = oceanMusic;
                    break;
                case "Dungeon":
                    audio.clip = dungeonMusic;
                    break;
                case "ActiniariaDisco":
                    audio.clip = anemonenDiscoMusic;
                    break;
                case "FrostfireBlade":
                    audio.clip = frostfireBladeMusic;
                    break;
                case "IceCaveBossfight":
                    audio.clip = iceCaveBossfight;
                    break;
                case "IceCave":
                    audio.clip = iceCave;
                    break;
            }
            if (audio.clip != null && audio.clip != currentClip)
                audio.Play();
        }
    }

    public void MuteMusic()
    {
        audio.mute = true;
    }
    public void UnmuteMusic()
    {
        audio.mute = false;
    }

    public void StopMusic()
    {
        audio.Stop();
    }

    public void PauseMusic()
    {
        audio.Pause();
    }
    public void ContinueMusic()
    {
        audio.UnPause();
    }

    public void SetVolume()
    {
        audio.volume = PlayerPrefs.GetFloat("MusicVolume");
    }

    internal void PlayNextHopeMelody()
    {
        hm.PlayNextMelodyNote();
    }
}
