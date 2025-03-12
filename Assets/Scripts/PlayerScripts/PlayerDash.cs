using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    /*
    public GameObject dashLeft;
    public GameObject dashRight;

    Rigidbody2D rb;

    bool dashing = false;
    public float dashSpeed;
    public float dashTime = .22f;
    float dashTimer = 0f;
    Vector2 direction;

    void Start()
    {
        TurnOffParticles();
        rb = GetComponentInChildren<Rigidbody2D>();
        dashTimer = 0f;
    }

    private void Update()
    {
        if (PlayerPrefs.GetInt("canDash") == 1)
        {
            if (PlayerChanneling.Instance.GetChargeRdy())
            {
                if (Input.GetAxisRaw("Horizontal") < -0.8f)    // guckt nach links
                {
                    if (Input.GetAxis("Dash") > .1f || Input.GetButtonDown("Dash"))   // drückt RT
                    {
                        StartDash(Vector2.left);                       // direction wird übergeben und gesetted
                        dashLeft.SetActive(true);           // particles
                    }
                }
                else if (Input.GetAxisRaw("Horizontal") > 0.8f)    // guckt nach rechts
                {
                    if (Input.GetAxis("Dash") > .1f || Input.GetButtonDown("Dash"))   // drückt RT
                    {
                        StartDash(Vector2.right);
                        dashRight.SetActive(true);
                    }
                }
            }

            if (dashing)
            {
                PlayerManager.Instance.DisableMovement();
                rb.velocity = new Vector2(direction.x * dashSpeed, 0);
                dashTimer += Time.deltaTime;
                if (dashTimer > dashTime)
                {
                    dashing = false;
                    dashTimer = 0f;
                    PlayerManager.Instance.EnableMovement();
                    TurnOffParticles();
                    rb.velocity = new Vector2(0, 0);
                }
            }
        }
    }


    void StartDash(Vector2 dir)
    {
        PlayerChanneling.Instance.ResetCharge();      // verbraucht den Charge
        PlayerManager.Instance.DisableMovement();
        direction = dir;
        dashing = true;
    }

    void TurnOffParticles()
    {
        dashLeft.SetActive(false);
        dashRight.SetActive(false);
    }
    */
}
