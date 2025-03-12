using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsButton : MonoBehaviour
{
    public void ToSettingsMenu()
    {
        SceneManager.LoadScene("SettingsMenu");
    }
}
