using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject canvasObject;
    public Animator blackScreenAnim;

    public AreaText areaText;

    [Header("Resources")]
    [Tooltip("0 = Rot, 1 = Blau, 2 = Gelb")]
    public Sprite[] artifacts;

    [Header("References")]
    public GameObject shopDialogPanel;
    public GameObject levelUpPanel;
    public GameObject optionsPanel;
    public GameObject dungeonEnterPanel;
    public GameObject iceCaveEnterPanel;
    [Tooltip("Already collected Artifacts, shown in the UI")]
    public GameObject artifactsDisplay;
    [Tooltip("Popup Panel nach Dungeon Finish mit gelungener Mermaid Rescue")]
    public ArtifactsPanel artifactsPanel;
    public GameObject deathSentence;

    [Header("HP Bar")]
    public RectTransform hpBarTrans;
    public RectTransform hpBarBackingTrans;

    [Header("Ingame Loading Screen")]
    public GameObject loadingScreen;
    public Image loadingScreenBar;

    public TextMeshProUGUI hopeAmountText;

    bool canToggleSkillPanel = true;
    bool canToggleOptionsPanel = true;

    Vector2 startSizeDelta;
    Vector2 backingStartSizeDelta;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        levelUpPanel.SetActive(false);
        optionsPanel.SetActive(false);
        shopDialogPanel.SetActive(false);
        artifactsPanel.gameObject.SetActive(false);

        foreach (Transform child in artifactsDisplay.transform)
        {
            child.gameObject.SetActive(false);
        }

        startSizeDelta = hpBarTrans.sizeDelta;

        PlayerAether.Instance.onLevelUp += ShowLevelUpPopup;
        PlayerHealth.Instance.onDeath += ResetArtifacts;
        PlayerManager.Instance.onRespawn += ResetHpBar;
    }

    private void ShowLevelUpPopup() => levelUpPanel.SetActive(true);
    internal void ShowDungeonPopup()
    {
        dungeonEnterPanel.SetActive(true);
    }
    internal void ShowDungeonPopup(SceneManagerScript.SceneType dungeonType)
    {
        if (dungeonType.Equals(SceneManagerScript.SceneType.IceCave))
        {
            iceCaveEnterPanel.SetActive(true);
        }
        else
        {
            dungeonEnterPanel.SetActive(true);
        }
    }
    internal void HideDungeonPopup()
    {
        if (dungeonEnterPanel != null && iceCaveEnterPanel != null)
        {
            dungeonEnterPanel.SetActive(false);
            iceCaveEnterPanel.SetActive(false);
        }
    }

    internal void AddArtifact(Mermaid.Variation v)
    {
        int i = GameProgress.Instance.artifacts.Count;

        Transform child = artifactsDisplay.transform.GetChild(i);
        if (child.TryGetComponent(out Image img))
        {
            img.sprite = artifacts[(int)v];
        }

        RectTransform rectTrans = child.GetComponent<RectTransform>();
        rectTrans.anchoredPosition = new Vector2(0, (i - 1) * -100f);
        child.gameObject.SetActive(true);
    }

    internal void ResetArtifacts()
    {
        foreach (Transform child in artifactsDisplay.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    private void ToggleSkillTree()
    {
        if (!levelUpPanel.activeSelf)
        {
            GameController.Instance.PauseGame(freeze: true);
            levelUpPanel.SetActive(true);
            canToggleOptionsPanel = false;
        }
        else if (levelUpPanel.activeSelf)
        {
            GameController.Instance.UnpauseGame();
            levelUpPanel.SetActive(false);
            canToggleOptionsPanel = true;
        }
    }

    public void ToggleOptions()
    {
        if (!optionsPanel.activeSelf)
        {
            GameController.Instance.PauseGame(freeze: true);
            optionsPanel.SetActive(true);
            canToggleSkillPanel = false;
        }
        else if (optionsPanel.activeSelf)
        {
            GameController.Instance.UnpauseGame();
            optionsPanel.SetActive(false);
            canToggleSkillPanel = true;
        }
    }

    public void ToggleShopDialogPanel()
    {
        if (!shopDialogPanel.activeSelf)
        {
            GameController.Instance.PauseGame(freeze: true);
            shopDialogPanel.SetActive(true);
        }
        else if (shopDialogPanel.activeSelf)
        {
            GameController.Instance.UnpauseGame();
            shopDialogPanel.SetActive(false);
        }
    }

    internal void GemPanelPopup(Mermaid.Variation variation)
    {
        artifactsPanel.gameObject.SetActive(true);
        artifactsPanel.PlayGemAnim(variation);
    }

    public void DisplayDeathSentence()
    {
        deathSentence.SetActive(true);
    }
    public void HideDeathSentence()
    {
        deathSentence.SetActive(false);
    }

    public void DisableCanToggleBools()
    {
        canToggleOptionsPanel = false;
        canToggleSkillPanel = false;
    }
    public void EnableCanToggleBools()
    {
        canToggleOptionsPanel = true;
        canToggleSkillPanel = true;
    }

    public void ShowAreaText(string sceneName)
    {
        areaText.ShowText(sceneName);
    }

    public void ShowSaveCircle()
    {
        // spiel gespeichert! kreis anzeigen
    }

    public void IncreaseHpBar(int add)
    {
        hpBarTrans.sizeDelta = new Vector2(hpBarTrans.sizeDelta.x + add, hpBarTrans.sizeDelta.y);
        hpBarBackingTrans.sizeDelta = new Vector2(hpBarBackingTrans.sizeDelta.x + add, hpBarBackingTrans.sizeDelta.y);
    }

    public void ResetHpBar()
    {
        hpBarTrans.sizeDelta = startSizeDelta;
        hpBarBackingTrans.sizeDelta = startSizeDelta;
    }

    internal void FadeIn()
    {
        Debug.Log("FadeIn");
        blackScreenAnim.Play("FadeIn");
        VisualEffectsManager.Instance.FadeIn();
    }

    internal void FadeOut()
    {
        Debug.Log("FadeOut");
        blackScreenAnim.Play("FadeOut");
        VisualEffectsManager.Instance.FadeOut();
    }
}
