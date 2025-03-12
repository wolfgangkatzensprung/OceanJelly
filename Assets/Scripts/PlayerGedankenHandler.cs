using TMPro;
using UnityEngine;

public class PlayerGedankenHandler : MonoBehaviour
{
    public static PlayerGedankenHandler _instance;
    public static PlayerGedankenHandler Instance { get { return _instance; } }

    public TextMeshProUGUI gedankenText;
    public Animator gedankenAnim;

    private void Start()
    {
        if (_instance == null) _instance = this;
    }

    public void SetGedankenText(string gedankenText)
    {
        this.gedankenText.text = gedankenText;
    }

    public void StartGedanken()
    {
        gedankenAnim.SetBool("isThinking", true);
    }

    public void EndGedanken()
    {
        gedankenAnim.SetBool("isThinking", false);
    }
}
