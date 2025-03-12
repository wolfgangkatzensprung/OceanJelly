using UnityEngine;

public class PlayerChanneling : MonoBehaviour
{
    public static PlayerChanneling _instance;
    public static PlayerChanneling Instance { get { return _instance; } }

    SpriteRenderer playerSr;

    public float channelingTime = 2f;
    float channelingTimer = 0f;
    bool channeling = false;
    bool dechanneling = false;
    bool charged = false;
    public float overchargeTime = 3f;       // zeit die man ueberchargen kann, wird dann hinzugefuegt zur charged zeit
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    private void Start()
    {
        playerSr = PlayerManager.Instance.GetSr();
    }

    private void Update()
    {
        if (playerSr == null)
            playerSr = PlayerManager.Instance.GetSr();

        playerSr.color = new Vector4(1 - channelingTimer, 1, 1, 1);
        //if (Input.GetButton("Channeling") || Input.GetAxis("Channeling") > 0f)
        //{
        //    Debug.Log("Is Channeling");
        //    if (!channeling)
        //        channeling = true;
        //}
        //if (Input.GetButtonUp("Channeling") || Input.GetAxis("Channeling") == 0f)
        //{
        //    channeling = false;
        //    dechanneling = true;
        //    if (channelingTimer < channelingTime * .25f)    // wenn nur noch ein viertel der channelingTime uebrig ist waehrend man loslaesst, so verpufft die Ladung komplett
        //    {
        //        ResetCharge();
        //    }
        //}
        //if (channeling)
        //{
        //    if (channelingTimer < channelingTime + overchargeTime)
        //    {
        //        channelingTimer += Time.deltaTime;
        //    }
        //}
        //else if (dechanneling)
        //{
        //    channelingTimer -= Time.deltaTime;
        //}
        //if (channelingTimer >= channelingTime)
        //{
        //    charged = true;
        //}
    }

    public void ResetCharge()
    {
        channeling = false;
        channelingTimer = 0f;
        charged = false;
    }

    public bool GetChargeRdy()
    {
        return charged;
    }
}
