using TMPro;
using UnityEngine;

public class AreaText : MonoBehaviour
{
    TextMeshProUGUI areaText;
    Animator anim;

    string currentAreaName;

    private void Start()
    {
        anim = GetComponent<Animator>();
        areaText = GetComponent<TextMeshProUGUI>();
    }

    public void ShowText(string sceneName)
    {
        string areaName = GetAreaNameBySceneName(sceneName);
        if (AreaChanged(areaName))
        {
            areaText.text = GetAreaNameBySceneName(sceneName);
            anim.Play("ShowAreaText");
        }

        SetCurrentAreaName(areaName);
    }

    private void SetCurrentAreaName(string areaName)
    {
        currentAreaName = areaName;
    }

    bool AreaChanged(string areaName)
    {
        if (areaName.Equals(currentAreaName))
            return false;
        else return true;
    }
    private string GetAreaNameBySceneName(string sceneName)
    {
        string areaName = "";

        if (PlayerPrefs.GetInt("Language") == 0)
        {
            switch (sceneName)
            {
                default:
                    areaName = "Home";
                    break;
                case "Ocean":
                    areaName = "The Ocean";
                    break;
                case "Dungeon":
                    areaName = "Dungeon";
                    break;
                case "IceCave":
                    areaName = "IceCave";
                    break;
            }
        }

        else if (PlayerPrefs.GetInt("Language") == 1)
        {
            areaName = "Der Ozean";
        }
        return areaName;
    }
}
