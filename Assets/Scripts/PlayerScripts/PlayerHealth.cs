using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance { get { return _instance; } }
    public static PlayerHealth _instance;

    [Header("References")]
    public GameObject player;
    public Text healthText;

    public bool alive = true;
    public int hp { get; private set; }
    public int maxHp { get; private set; }

    int startHp = 50;

    public bool rdyForRespawn { get; private set; }

    [Header("Settings")]
    public int itemDropDmgThreshold = 2;
    float lowestPossibleY = -3333f;
    public int damageFactor = 1;   // dmg faktor. zB halber dmg waer .5f . doppelter dmg 2f

    public float respawnDelay = .3f;

    public delegate void DeathDelegate();
    public DeathDelegate onDeath;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }
    void Start()
    {
        InitializeHP();

        AbilityManager.Instance.onAbilityIncrease += MaxHealthUpgrade;
        onDeath += InitializeHP;
    }
    void Update()
    {
        DeathCheck();
    }

    public void InitializeHP()
    {
        if (!PlayerPrefs.HasKey("MaxHp"))
        {
            PlayerPrefs.SetInt("MaxHp", startHp);
        }

        maxHp = PlayerPrefs.GetInt("MaxHp");
        hp = maxHp;
    }


    private void MaxHealthUpgrade(AbilityManager.Abilities a)
    {
        if (a.Equals(AbilityManager.Abilities.Health))
        {
            maxHp += 10;
            UIManager.Instance.IncreaseHpBar(10);
            HealPlayer(10);
        }
    }

    private void DeathCheck()
    {
        if (alive)
        {
            if (hp <= 0f)
            {
                Death();
            }
        }
    }

    public void ReceiveDmg(int dmg)
    {
        PlayerManager.Instance.anim.Play("Damaged");
        VisualEffectsManager.Instance.StartDamagedVFX(.6f);

        //if (PlayerChanneling.Instance.GetChargeRdy())
        //{
        //    Debug.Log("Charge cancelled");
        //    PlayerChanneling.Instance.ResetCharge();
        //}

        //if (dmg > itemDropDmgThreshold || AbilityManager.Instance.IsRecalling())    // bei mehr als 2 Dmg droppt man das gehaltene Item
        //{
        //    if (ItemCollector.Instance.GetIsHoldingItem())
        //    {
        //        ItemCollector.Instance.TryDropItem();
        //    }
        //}
        hp = Mathf.Max(hp - dmg * damageFactor, 0);

        DeathCheck();
    }

    internal void HealPlayer(int healAmount)
    {
        //PlayerManager.Instance.anim.Play("Healed");

        hp = Mathf.Min(hp + healAmount, maxHp);
    }

    public void Death()
    {
        Debug.Log("Death");
        UIManager.Instance.DisplayDeathSentence();
        PlayerManager.Instance.Die();

        alive = false;

        onDeath?.Invoke();

        StartCoroutine(DelayedRespawn());
    }

    IEnumerator DelayedRespawn()
    {
        yield return new WaitForSecondsRealtime(respawnDelay);
        PlayerInputManager.Instance.onTouchPressStart += Respawn;
    }


    void Respawn(Vector2 touchPos)
    {
        PlayerInputManager.Instance.onTouchPressStart -= Respawn;
        Debug.Log("Respawn");

        hp = PlayerPrefs.GetInt("MaxHp");
        rdyForRespawn = false;
        UIManager.Instance.HideDeathSentence();
        PlayerManager.Instance.Respawn();
        alive = true;
    }
}
