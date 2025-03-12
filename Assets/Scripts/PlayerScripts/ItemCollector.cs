using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ItemCollector : MonoBehaviour
{
    public static ItemCollector _instance;
    public static ItemCollector Instance { get { return _instance; } }

    public LayerMask itemLayer;
    public LayerMask shieldLayer;

    // Item Holder
    public Transform itemHolder;
    bool isHoldingItem;
    GameObject heldItem;

    // Scales and Money
    public int scaleCount;
    public int moneyCount;
    public int hopeCount = 1;
    public Text moneyText;

    Coroutine coTry;

    [Header("Item Pickup and Magnet")]
    [Tooltip("Distance to pick up items (sollte minimal groesser sein als player collider radius")]
    public float pickDistance = 5f;
    [Tooltip("Item Layer")]
    public LayerMask magnetMask;
    public float magnetStrength = 10f;
    public float magnetRadius = 10f;
    float[] magnetPower = { 55f, 70f, 85f, 100f, 115f, 130f, 145f, 160f, 170f, 180f, 190f, 200f, 210f, 220f, 230f, 240f, 250f, 260f, 270f, 280f, 290f, 300f,
        310f, 320f, 330f, 340f, 350f, 360f, 370f, 380f, 390f, 400f, 410f, 420f, 430f, 440f, 450f, 460f, 470f, 480f, 490f, 500f };    // magnet strength des entsprechenden level

    [Tooltip("Throw Item Strength. Default = 72")]
    public float throwStrength = 72f;

    bool itemRdyToDrop;
    private bool canPickup = true;

    public delegate void ItemCollectedDelegate(Item.Type itemType);
    public ItemCollectedDelegate onItemCollected;


    private void Awake()
    {
        if (_instance == null)
            _instance = this;
    }

    void Update()
    {
        if (!PlayerMovement.Instance.canMove)
            return;

        RadiusCheck();  // scans area around player for loot
    }

    private void RadiusCheck()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll((Vector2)transform.position, magnetRadius, magnetMask);

        // Iterate through all magnetizable objects
        foreach (Collider2D hit in hits)
        {
            // Get the distance between the player and the object
            float distance = Vector3.Distance(transform.position, hit.transform.position);

            if (distance < pickDistance)
            {
                if (hit.transform.CompareTag("Item"))
                {
                    if (hit.transform.TryGetComponent(out Item item))
                    {
                        PickupItem(item);
                    }
                }
            }

            // Get the direction of the magnet force
            Vector3 magnetForce = transform.position - hit.transform.position;

            // Normalize the force vector
            magnetForce = magnetForce.normalized;

            // Apply the magnet force to the object
            hit.GetComponent<Rigidbody2D>().AddForce(magnetForce * magnetStrength / Mathf.Max(distance, 0.000001f));
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Item"))
        {
            if (collision.collider.TryGetComponent(out Item item))
            {
                PickupItem(item);
            }
        }
    }

    private void PickupItem(Item item)  // NEW WAY (the Hopeful Jellyfish way)
    {
        if (!canPickup)
            return;

        canPickup = false;      // flag variable damit nicht onCollisionEnter2D und im RadiusCheck gleichzeitig im selben Frame das selbe Item doppelt eingesammelt wird

        if (item.itemType != Item.Type.Hope)
        {
            SoundManager.Instance.PlaySound("PickUp");
        }

        switch (item.itemType)
        {
            case Item.Type.FishEgg:
                PlayerHealth.Instance.HealPlayer(item.value * AbilityManager.Instance.healFactor);
                item.gameObject.SetActive(false);
                break;
            case Item.Type.Hope:
                CollectXP(item.value);
                if (Ocean_AssetSpawner.Instance != null)
                    Ocean_AssetSpawner.Instance.inactiveHope.Add(item.gameObject);
                item.gameObject.SetActive(false);
                MusicManager.Instance.PlayNextHopeMelody();
                break;
            case Item.Type.Shield:
                if (item.TryGetComponent(out Shield sm))
                {
                    sm.enabled = true;
                }
                else
                {
                    item.gameObject.AddComponent(typeof(Shield));
                }
                break;
            case Item.Type.Sword:
                if (item.TryGetComponent(out FrostfireBlade fb))
                {
                    fb.enabled = true;
                    if (!item.TryGetComponent(out DontDestroy dd))
                    {
                        item.gameObject.AddComponent<DontDestroy>();
                    }
                }
                else
                {
                    item.gameObject.AddComponent(typeof(FrostfireBlade));
                }
                break;
            case Item.Type.FlameVial:
                PlayerAura.Instance.SetAura(1);
                item.gameObject.SetActive(false);
                break;
            case Item.Type.NauticShard:
                PlayerAura.Instance.SetAura(2);
                item.gameObject.SetActive(false);
                break;
            case Item.Type.Flower:
                PlayerAura.Instance.SetAura(3);
                item.gameObject.SetActive(false);
                break;
        }

        onItemCollected?.Invoke(item.itemType);

        canPickup = true;
    }

    private void SetHopeUI()
    {
        //if (hopeComponent.xpAmount == 1)
        //{
        //    hopeCount += 1;
        //    UIManager.Instance.hopeAmountText.text = hopeCount.ToString();
        //}
    }

    internal void SetMagnetStrength(int abilityLevel)
    {
        if (abilityLevel < magnetPower.Length)
        {
            magnetStrength = magnetPower[abilityLevel];
            magnetRadius = 10 + abilityLevel * 2;
        }
    }

    public void SetItemRdyToDrop(bool isRdy = true)
    {
        if (!itemRdyToDrop)
            itemRdyToDrop = isRdy;
    }

    //public void TakeItemIntoHand(Item item)
    //{
    //    itemRdyToDrop = false;
    //    coTry = StartCoroutine(TakeItemRoutine(item));
    //}

    //IEnumerator TakeItemRoutine(Item item)
    //{
    //    Debug.Log($"Try Eqiup Item {item.GetItemType()}");

    //    switch (item.itemType)
    //    {
    //        case Item.Type.FlameVial:
    //            PlayerAura.Instance.SetAura(1);
    //            Destroy(item.gameObject);
    //            break;
    //        case Item.Type.NauticShard:
    //            PlayerAura.Instance.SetAura(2);
    //            Destroy(item.gameObject);
    //            break;
    //        case Item.Type.Shield:
    //            PlayerAura.Instance.AddShield(item);
    //            break;
    //        case Item.Type.Sword:
    //            PlayerAura.Instance.AddSword(item);
    //            break;
    //    }


    //    yield return new WaitForEndOfFrame();
    //}

    // Picking up Item and activating effect       - OLD SYSTEM -
    //public void PickupItem(Collider2D col)
    //{
    //    isHoldingItem = true;
    //    Debug.Log("Take Item");
    //    heldItem = col.gameObject;

    //    if (heldItem.TryGetComponent<ShopItem>(out ShopItem shopItemComponent))
    //    {
    //        UIManager.Instance.ToggleShopDialogPanel();
    //    }

    //    if (heldItem.TryGetComponent<Item>(out Item _item))
    //    {
    //        switch (_item.GetItemType())
    //        {
    //            case Item.Type.FlameVial:
    //                Debug.Log("Pick up FlameVial");
    //                PlayerAura.Instance.SetAura(1);
    //                break;
    //            case Item.Type.NauticShard:
    //                Debug.Log("Pick up NauticShard");
    //                PlayerAura.Instance.SetAura(2);
    //                break;
    //            case Item.Type.Shield:
    //                Debug.Log("Pick up Shield");
    //                AbilityManager.Instance.ActivateShieldItem(itemHolder.GetComponent<Collider2D>());
    //                break;
    //            case Item.Type.RecallStone:
    //                Debug.Log("Pick up Recall Stone");
    //                AbilityManager.Instance.ActivateRecallItem();
    //                break;
    //            case Item.Type.AetherCrystal:
    //                // aura 3 und so weiter
    //                break;
    //        }
    //    }

    //    Rigidbody2D itemRb = col.GetComponent<Rigidbody2D>();
    //    itemRb.bodyType = RigidbodyType2D.Kinematic;
    //    itemRb.velocity = Vector3.zero;
    //    itemRb.gravityScale = 0;
    //    col.GetComponent<Collider2D>().enabled = false;

    //    col.transform.position = itemHolder.transform.position;
    //    col.transform.SetParent(itemHolder);
    //}

    internal void SetIsHoldingItem(bool isHolding)
    {
        isHoldingItem = isHolding;
    }

    // Dropping Item
    public void TryDropItem()
    {
        Debug.Log("TryDropItem");
        if (itemRdyToDrop)
        {
            Debug.Log("ItemRdyToDrop");
            StartCoroutine(DropItemRoutine());
            itemRdyToDrop = false;
        }
    }
    public IEnumerator DropItemRoutine()
    {
        if (UIManager.Instance.shopDialogPanel.activeSelf)
        {
            Debug.Log("shop panel");
            UIManager.Instance.ToggleShopDialogPanel();
        }

        yield return new WaitForEndOfFrame();
        Rigidbody2D itemRb;
        if (!heldItem.name.Contains("Qualle"))
        {
            try
            {
                itemRb = heldItem.GetComponent<Rigidbody2D>();
                itemRb.bodyType = RigidbodyType2D.Dynamic;
                itemRb.gravityScale = 1;
                if (heldItem.CompareTag("Item"))
                {
                    if (heldItem.TryGetComponent<Item>(out Item _item))
                    {
                        switch (_item.GetItemType())
                        {
                            case Item.Type.RecallStone:
                                AbilityManager.Instance.DisableRecallItem();
                                break;
                            case Item.Type.Stone:
                                ThrowItem(itemRb);
                                break;
                            default:
                                Debug.Log("Drop held item.");
                                itemRb.velocity = Vector3.zero;
                                break;
                        }
                    }
                }
            }
            catch
            {
                // Item hat sich aufgeloest. Muss also gefangenes Missle gewesen sein. Achievement
                throw;
            }
        }

        heldItem.GetComponent<Collider2D>().enabled = true;

        heldItem.transform.position = itemHolder.transform.position;
        heldItem.transform.SetParent(null);
        PlayerAura.Instance.SetAura(0);
        isHoldingItem = false;
    }

    void ThrowItem(Rigidbody2D item_rb)
    {
        //if (Input.GetAxisRaw("Vertical") < -.5f)
        //{
        //    item_rb.velocity = Vector2.zero;
        //    return;
        //}
        //Vector3 throwVector = new Vector3(throwStrength, 0, 0);
        //if (PlayerManager.Instance.GetIsLookingLeft())
        //    throwVector *= -1f;
        //item_rb.velocity = throwVector;
    }

    public void PurchaseItem()
    {
        Destroy(heldItem.GetComponent<ShopItem>());
        Transform iTrans = heldItem.transform;
        for (int i = iTrans.childCount; i < 0; i--)
        {
            Destroy(iTrans.GetChild(i));
        }
    }

    public void DestroyItem()
    {
        Destroy(heldItem);
        isHoldingItem = false;
    }

    // Money
    public void CollectMoney(int gold)
    {
        moneyCount += gold;
        SaveMoneyCount();
    }

    public void LoseMoney(int mon)
    {
        moneyCount -= mon;
        SaveMoneyCount();
    }

    private void SaveMoneyCount()
    {
        PlayerPrefs.SetInt("Money", moneyCount);
    }

    // Collecting XP
    public void CollectXP(int xp)
    {
        PlayerAether.Instance.XpGain(xp);
    }

    // Get Set
    public void SetHeldItem(GameObject newHeldItem)
    {
        heldItem = newHeldItem;
        PlayerPrefs.SetString("heldItem", newHeldItem.name);
    }
    public GameObject GetHeldItem()
    {
        return heldItem;
    }
    public bool GetIsHoldingItem()
    {
        return isHoldingItem;
    }
}
