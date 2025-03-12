using TMPro;
using UnityEngine;

public class LanguageButton : MonoBehaviour
{
    public TextMeshProUGUI langText;

    private void Start()
    {
        if (PlayerPrefs.GetInt("Language") == 0)
        {
            SetButtonText("English");
        }
        else if (PlayerPrefs.GetInt("Language") == 1)
        {
            SetButtonText("German");
        }

    }

    private void SetButtonText(string lt)
    {
        if (lt == null)
            return;

        langText.text = lt;
    }

    public void ToggleLanguage()
    {
        if (PlayerPrefs.GetInt("Language") == 1)
        {
            Debug.Log("Sprache ist Englisch");
            PlayerPrefs.SetInt("Language", 0);
            SetButtonText("English");
            DialogManager.Instance.SetLanguageFromPlayerPref();
        }
        else if (PlayerPrefs.GetInt("Language") == 0)
        {
            Debug.Log("Sprache ist Deutsch");
            PlayerPrefs.SetInt("Language", 1);
            SetButtonText("German");
            DialogManager.Instance.SetLanguageFromPlayerPref();
        }
    }
}
