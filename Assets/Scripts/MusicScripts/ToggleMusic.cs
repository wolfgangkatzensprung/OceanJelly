using UnityEngine;
using UnityEngine.UI;

public class ToggleMusic : MonoBehaviour
{
    public Text toggleMusicText;
    bool musicEnabled;

    void Start()
    {
        if (PlayerPrefs.GetInt("Music") == 0)
        {
            DisableMusic();
        }
        else
        {
            EnableMusic();
        }
    }

    public void SetMusicToggle()
    {
        if (!musicEnabled)
        {
            EnableMusic();
        }
        else if (musicEnabled)
        {
            DisableMusic();
        }
    }

    void EnableMusic()
    {
        toggleMusicText.text = "Music enabled";
        PlayerPrefs.SetInt("Music", 1);
        musicEnabled = true;
        MusicManager.Instance.UnmuteMusic();
    }
    void DisableMusic()
    {
        toggleMusicText.text = "Music disabled";
        PlayerPrefs.SetInt("Music", 0);
        musicEnabled = false;
        MusicManager.Instance.MuteMusic();
    }
}
