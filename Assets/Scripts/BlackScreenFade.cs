using UnityEngine;
using UnityEngine.UI;

public class BlackScreenFade : MonoBehaviour
{
    public Image img;

    private void Start()
    {
        img = GetComponent<Image>();
        SetBlackScreen();
    }

    public void SetBlackScreen()
    {
        Debug.Log("Blackscreen visible");
        img.color = new Color(1f, 1f, 1f, 1f);
    }
    public void UnsetBlackScreen()
    {
        Debug.Log("Blackscreen not visible");
        img.color = new Color(1f, 1f, 1f, 0f);
    }
}
