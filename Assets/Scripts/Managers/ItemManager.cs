using System;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager _instance;
    public static ItemManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
    }

    internal Item.Type GetItemTypeByName(string itemName)
    {
        Item.Type itemType = (Item.Type)Enum.Parse(typeof(Item.Type), itemName);
        return itemType;
    }

    GameObject SpawnItem(GameObject itemToSpawn)
    {
        Vector3 playerPos = PlayerManager.Instance.player.transform.position;
        return Instantiate(itemToSpawn, playerPos + new Vector3(10 * PlayerManager.Instance.GetFacingDirection(), 0, 0), Quaternion.identity);
    }

    public void TryBuyItem()
    {
        int price = ItemCollector.Instance.GetHeldItem().GetComponent<ShopItem>().GetPrice();
        if (PlayerMoney.Instance.GetMoneyAmount() >= price)
        {
            ShopItemTrader.Instance.BuyItem(price);
            UIManager.Instance.ToggleShopDialogPanel();
        }
        else
        {
            ShopItemTrader.Instance.CantBuyItem();
            UIManager.Instance.ToggleShopDialogPanel();
        }
    }
}
