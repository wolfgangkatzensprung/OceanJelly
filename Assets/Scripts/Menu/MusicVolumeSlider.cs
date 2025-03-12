using UnityEngine;
using UnityEngine.UI;

public class MusicVolumeSlider : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Slider>().value = PlayerPrefs.GetFloat("MusicVolume");
    }

    public void ChangeMusicVolume()
    {
        PlayerPrefs.SetFloat("MusicVolume", GetComponent<Slider>().value);
        MusicManager.Instance.SetVolume();
    }
}
