using UnityEngine;
using UnityEngine.UI;

public class PlayerMoney : MonoBehaviour
{
    public static PlayerMoney _instance;
    public static PlayerMoney Instance { get { return _instance; } }
    public Text moneyText;

    public int money;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    private void Start()
    {
        if (!PlayerPrefs.HasKey("Money"))
        {
            PlayerPrefs.SetInt("Money", 0);
        }
        money = PlayerPrefs.GetInt("Money");
    }

    public void CollectMoney(int cash)
    {
        money += cash;
        SaveMoneyCount();
        UpdateMoneyText();
    }
    public void SpendMoney(int mon)
    {
        money -= mon;
        SaveMoneyCount();
        UpdateMoneyText();
    }

    private void UpdateMoneyText()
    {
        moneyText.text = "Money: " + money;
    }


    private void SaveMoneyCount()
    {
        PlayerPrefs.SetInt("Money", money);
    }


    public int GetMoneyAmount()
    {
        return money;
    }

}
