using System.Collections;
using UnityEngine;

public class PlayerLiquo : MonoBehaviour
{
    public static PlayerLiquo _instance;
    public static PlayerLiquo Instance { get { return _instance; } }

    public Rigidbody2D rb;
    public Animator anim;

    internal bool isEnabled = true;

    // Liquo Gravity
    public float liquoTime = 1f; // die Zeit die die Liquo Gravity benoetigt um 0 zu werden
    float liquoTimer;       // Timer laeuft von liquoTime gegen 0 ; default ist von -1 nach 0
    bool isPlatsching = false;  // wenn Player ins Wasser eintritt, wird er einen kurzen Moment einplatschen. In diesem kurzen Moment wird Gravity mehrmals mit 0.8 oder so multipliziert
    Vector2 liquoForce;  // liquo force and direction

    private void Awake()
    {
        if (_instance == null) _instance = this;
    }

    private void Start()
    {
        liquoTimer = liquoTime;
    }

    private void Update()
    {
        if (!isEnabled)
        {
            return;
        }

        LiquoCheck();
    }

    private void FixedUpdate()
    {
        if (PlayerMovement.Instance.inLiquo)
        {
            LiquoPush();
        }
    }

    #region Checks

    private void LiquoCheck()
    {
        if (PlayerMovement.Instance.inLiquo)
        {
            if (liquoTime < 0)
            {
                liquoTimer += Time.deltaTime;
            }
        }
    }

    #endregion

    #region Liquo
    public void SetLiquoForce(Vector2 liquoForce)
    {
        this.liquoForce = liquoForce;
        Debug.Log("_LiquoFactor set to: " + liquoForce);
    }

    public void LiquoPush()
    {
        rb.AddForce(liquoForce);
    }

    public void Platsch(float plFactor, float minPlVel)
    {
        StartCoroutine(Platscher(plFactor, minPlVel));
    }
    public IEnumerator Platscher(float platschFactor, float minPlatschVel)
    {
        Debug.Log("Platsch");
        rb.velocity *= platschFactor;
        yield return new WaitForSeconds(.1f);
        yield return new WaitForFixedUpdate();
        {
            if (PlayerJump.Instance.rb.velocity.y < minPlatschVel)
            {
                Platsch(platschFactor, minPlatschVel);
            }
        }
    }

    #endregion
}
