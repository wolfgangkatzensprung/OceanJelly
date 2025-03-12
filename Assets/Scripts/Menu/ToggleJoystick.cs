using TMPro;
using UnityEngine;

public class ToggleJoystick : MonoBehaviour
{
    public TextMeshProUGUI joystickToggleText;

    bool joystickActive;

    private void Start()
    {
        // Initialisieren und korrekten Text anzeigen
        if (PlayerPrefs.GetInt("Joystick") == 0)
        {
            DisableJoystick();
        }
        else if (PlayerPrefs.GetInt("Joystick") == 1)
        {
            EnableJoystick();
        }
        else
        {
            joystickActive = false;
        }
    }

    public void SetJoystickToggle()
    {

        if (!joystickActive)
        {
            EnableJoystick();
        }
        else if (joystickActive)
        {
            DisableJoystick();
        }

    }

    void EnableJoystick()
    {
        joystickToggleText.text = "Joystick enabled";
        PlayerPrefs.SetInt("Joystick", 1);
        joystickActive = true;
    }
    void DisableJoystick()
    {
        joystickToggleText.text = "Joystick disabled";
        PlayerPrefs.SetInt("Joystick", 0);
        joystickActive = false;
    }

}
