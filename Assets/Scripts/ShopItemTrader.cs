using UnityEngine;

public class ShopItemTrader : MonoBehaviour
{
    public static ShopItemTrader _instance;
    public static ShopItemTrader Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    public void BuyItem(int price)
    {
        PlayerMoney.Instance.SpendMoney(price);
        ItemCollector.Instance.PurchaseItem();
    }

    public void CantBuyItem()
    {
        Debug.Log("not enough money");
    }
}
