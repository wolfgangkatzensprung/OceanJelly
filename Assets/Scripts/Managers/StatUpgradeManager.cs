using UnityEngine;

public class StatUpgradeManager : MonoBehaviour
{
    public static StatUpgradeManager _instance;
    public static StatUpgradeManager Instance { get { return _instance; } }
    string[] keys;

    public void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    private void Start()
    {
        keys = new string[] { "dmgRaw", "dmgBonus", "dmgFactor", "projectileDmg" };  // alle stat keys
        CheckPlayerPrefs();
    }

    private void CheckPlayerPrefs()
    {
        foreach (string s in keys)
        {
            if (!PlayerPrefs.HasKey(s))
            {
                SetDefaultPref(s);
            }
        }
    }

    public void SetDefaultPref(string key)
    {
        // ToDo: in PlayerPrefsManager NewGame Funktion verschieben
        switch (key)
        {
            case "dmgRaw":
                PlayerPrefs.SetFloat(key, 2f);    // Default Schaden 2
                break;
            case "dmgBonus":
                PlayerPrefs.SetFloat(key, 1f);   // Default Additiver Bonus 1
                break;
            case "dmgFactor":
                PlayerPrefs.SetFloat(key, 1f);  // Default Multiplikator 1
                break;
            case "projectileDmg":
                PlayerPrefs.SetFloat(key, 1f);  // Default Projectile Dmg 1
                break;
        }
    }
}
