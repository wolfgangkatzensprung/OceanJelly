using UnityEngine;

public class HintManager : MonoBehaviour
{
    public static HintManager _instance;
    public static HintManager Instance { get { return _instance; } }

    public GameObject hintHolder;
    public Sprite[] xboxButtonSprites;
    public SpriteRenderer hintSpriteRenderer;

    bool buttonHintShown = false;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    public void ShowButtonHint()
    {
        hintSpriteRenderer.enabled = true;
    }
    public void ShowButtonHint(string bName)
    {
        hintSpriteRenderer.enabled = true;
        SetButtonHintSprite(bName);
    }
    public void HideButtonHint()
    {
        hintSpriteRenderer.enabled = false;
    }

    public void SetButtonHintSprite(string buttonName)
    {
        switch (buttonName)
        {
            case "B":
                hintSpriteRenderer.sprite = xboxButtonSprites[0];
                break;
            case "A":
                hintSpriteRenderer.sprite = xboxButtonSprites[1];
                break;
            case "Y":
                hintSpriteRenderer.sprite = xboxButtonSprites[2];
                break;
            case "X":
                hintSpriteRenderer.sprite = xboxButtonSprites[3];
                break;
        }
    }
}
