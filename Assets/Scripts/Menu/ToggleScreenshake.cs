using TMPro;
using UnityEngine;

public class ToggleScreenshake : MonoBehaviour
{
    public TextMeshProUGUI screenshakeToggleText;

    bool screenshakeActive;

    private void Start()
    {
        // Initialisieren und korrekten Text anzeigen
        if (PlayerPrefs.GetInt("Screenshake") == 0)
        {
            DisableScreenshake();
        }
        else
        {
            EnableScreenshake();
        }
    }

    public void SetScreenshakeToggle()
    {

        if (!screenshakeActive)
        {
            EnableScreenshake();
        }
        else if (screenshakeActive)
        {
            DisableScreenshake();
        }

    }

    void EnableScreenshake()
    {
        screenshakeToggleText.text = "Screenshake enabled";
        PlayerPrefs.SetInt("Screenshake", 1);
        screenshakeActive = true;
    }
    void DisableScreenshake()
    {
        screenshakeToggleText.text = "Screenshake disabled";
        PlayerPrefs.SetInt("Screenshake", 0);
        screenshakeActive = false;
    }

}
