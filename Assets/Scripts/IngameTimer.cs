using TMPro;
using UnityEngine;

public class IngameTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    internal static float gameTimer = 0f;   // game time timer

    private void Start()
    {
        timerText = GetComponent<TextMeshProUGUI>();

        PlayerManager.Instance.onRespawn += ResetTime;
    }

    private void ResetTime() => gameTimer = 0f;

    void Update()
    {
        gameTimer += Time.deltaTime;
        int minutes = Mathf.FloorToInt(gameTimer / 60);
        int seconds = Mathf.FloorToInt(gameTimer % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
