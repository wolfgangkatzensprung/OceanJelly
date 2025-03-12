using System.Collections;
using UnityEngine;

public class PlayerAether : MonoBehaviour
{
    int level = 0;

    float xp;
    float xpThreshold;
    float[] xpThresholds = { 100, 125, 150, 175, 200, 225, 250, 275, 300, 325, 350, 375, 400, 425, 450, 475, 500, 550, 600, 650, 700, 750, 800, 850, 900, 950, 1000,
        1100, 1200, 1300, 1400, 1500, 1600, 1700, 1800, 1900, 2000, 2150, 2300, 2450, 2600, 2750, 2900, 3000, 3250, 3500, 3750, 4000, 4250, 4500, 4750, 5000,
        5500, 6000, 6500, 7000, 7500, 8000, 8500, 9000, 9500, 10000, 20000, 30000, 40000, 50000, 60000, 70000, 80000, 90000, 100000 };

    public static PlayerAether Instance;

    public delegate void LevelUpDelegate();
    public LevelUpDelegate onLevelUp;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        //xp = PlayerPrefs.GetInt("XP");
        //level = PlayerPrefs.GetInt("Level");

        InitializeLevelAndXp();
        SetXpThreshold();

        PlayerHealth.Instance.onDeath += InitializeLevelAndXp;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            LevelUp();
        }
    }

    private void InitializeLevelAndXp()
    {
        xp = 0;
        level = 0;
    }

    private void SetXpThreshold()
    {
        xpThreshold = xpThresholds[level];
    }

    public void XpGain(float exp)
    {
        if (PlayerAura.Instance.GetAura() == 3)
        {
            exp *= 2;
        }
        xp += exp;
        Debug.Log($"+{exp} XP");
        DoLevelCheck();
        SaveLevelAndXp();
    }

    private void SaveLevelAndXp()
    {
        PlayerPrefs.SetInt("XP", (int)xp);
        PlayerPrefs.SetInt("Level", level);
    }

    public void DoLevelCheck()
    {
        //Debug.Log("DoLevelCheck()");
        if (xp >= xpThreshold)
        {
            LevelUp();
            return;
        }
        Debug.Log("LevelCheck done. Player Level: " + level);
    }

    private void LevelUp()
    {
        StartCoroutine(LevelUpRoutine());
    }

    private IEnumerator LevelUpRoutine()
    {
        level++;
        xp = xp - xpThreshold;
        xpThreshold = xpThresholds[Mathf.Min(level, xpThresholds.Length - 1)];
        Debug.Log("New Level: " + level + " // New xp " + xp + " // New xpThreshold " + xpThreshold);

        onLevelUp?.Invoke();

        yield return new WaitForSeconds(0.2f);
        yield return new WaitForEndOfFrame();

        DoLevelCheck();
    }

    public float GetLevelProgressRatio()
    {
        float progressRatio = xp / xpThreshold;
        return progressRatio;
    }
}
