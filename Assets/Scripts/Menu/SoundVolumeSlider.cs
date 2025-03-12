using UnityEngine;
using UnityEngine.UI;

public class SoundVolumeSlider : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Slider>().value = PlayerPrefs.GetFloat("SoundVolume");
    }

    public void ChangeSoundVolume()
    {
        PlayerPrefs.SetFloat("SoundVolume", GetComponent<Slider>().value);
        SoundManager.Instance.SetVolume();
    }
}
