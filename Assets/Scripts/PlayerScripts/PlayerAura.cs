using System.Collections.Generic;
using UnityEngine;

public class PlayerAura : MonoBehaviour
{
    public static PlayerAura Instance;
    Rigidbody2D rb;

    public Transform firePoint;

    public List<GameObject> shields = new List<GameObject>();

    [Tooltip("Rotating Parent for Shields")]
    public Transform shieldHolder;

    public GameObject fireProjectile;   // Projektil
    public GameObject fireParticles;    // Particles
    public GameObject bigFlameBall;     // Channeled Missile

    public GameObject waterProjectile;
    public GameObject waterParticles;
    public GameObject bigNauticBall;

    public GameObject hopeParticles;

    [SerializeField] float knockBackStrength = 300f;

    float cooldownTimeLeft;
    bool projectileRdy;

    public static int aura; // 0 = keine Aura, 1 = Fire, 2 = Water, 3 = Äther, 4 = Envoy
    int maxAura = 2;  // 0 = man hat keine Aura, 1= man hat nur Feuer, 2 = man hat Feuer + Wasser, etc

    public float shootingCooldown = 0.4f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        rb = PlayerManager.Instance.GetRigidbody();
        projectileRdy = true;

        TurnOffAuraParticles(); // turn off particles
        SwapAuraVisuals(0);     // reset aura to 0
    }

    void Update()
    {
        //if (Input.GetButton("Shoot"))
        //{
        //    if (PlayerAura.Instance.GetAura() > 0)
        //    {
        //        Shoot();
        //    }
        //}

        if (cooldownTimeLeft > 0)
            ProjectileCooldown();
    }


    /// <summary>
    /// Set Player Aura: 0 = Off ; 1 = Flame ; 2 = Nautic
    /// </summary>
    /// <param name="auri"></param>
    public void SetAura(int auri)
    {
        aura = auri;
        SwapAuraVisuals(auri);
    }

    public int GetAura()
    {
        return aura;
    }

    void SwapAuraVisuals(int aur)
    {
        switch (aur)
        {
            case 0:
                TurnOffAuraParticles();
                break;
            case 1:
                TurnOffAuraParticles();
                fireParticles.SetActive(true);
                break;
            case 2:
                TurnOffAuraParticles();
                waterParticles.SetActive(true);
                break;
            case 3:
                TurnOffAuraParticles();
                hopeParticles.SetActive(true);
                break;
            case 4:
                //envoy
                break;
        }

    }

    void TurnOffAuraParticles()
    {
        fireParticles.SetActive(false);
        waterParticles.SetActive(false);
        hopeParticles.SetActive(false);
        //envoy
    }

    void Shoot()
    {
        if (projectileRdy)
        {
            DirectorScript.Instance.Screenshake();
            //Debug.Log(transform.localScale.x + " wenn > 0, shooting knockback");
            if (transform.localScale.x > 0f)        // if facing right
            {
                rb.AddForce(Vector2.left * knockBackStrength);
            }
            else
            {
                rb.AddForce(Vector2.right * knockBackStrength);
            }
            // DirectorScript.Instance.ShootingKnockback();    // waer vllt im lateUpdate besser

            switch (aura)
            {
                case 1:
                    if (PlayerChanneling.Instance.GetChargeRdy())
                    {
                        Instantiate(bigFlameBall, firePoint.position, firePoint.rotation);
                        PlayerChanneling.Instance.ResetCharge();
                    }
                    else
                    {
                        Instantiate(fireProjectile, firePoint.position, firePoint.rotation);
                    }
                    break;
                case 2:
                    if (PlayerChanneling.Instance.GetChargeRdy())
                    {
                        Instantiate(bigNauticBall, firePoint.position, firePoint.rotation);
                        PlayerChanneling.Instance.ResetCharge();
                    }
                    else
                    {
                        //Debug.Log("Shooting Nautic Projectile");
                        Instantiate(waterProjectile, firePoint.position, firePoint.rotation);
                    }
                    break;
            }
            cooldownTimeLeft = shootingCooldown;
        }
    }

    internal void AddShield(Item item)
    {
        item.transform.parent = shieldHolder;
        item.transform.localPosition = new Vector2(0, 10f);

        item.gameObject.layer = LayerMask.NameToLayer("Projectile");

        if (item.TryGetComponent(out Rigidbody2D rb))
        {
            Destroy(rb);
        }

        // Remove Item Script
        Destroy(item);
    }

    internal void AddSword(Item item)
    {
        item.transform.parent = shieldHolder;
        item.transform.localPosition = new Vector2(0, 10f);

        item.gameObject.layer = LayerMask.NameToLayer("Projectile");

        if (item.TryGetComponent(out Rigidbody2D rb))
        {
            Destroy(rb);
        }

        if (item.TryGetComponent(out FrostfireBlade fb))
        {
            fb.enabled = true;
        }

        // Remove Item Script
        Destroy(item);
    }

    void ProjectileCooldown()
    {
        cooldownTimeLeft -= Time.deltaTime;
        if (cooldownTimeLeft < 0.1f)
        {
            projectileRdy = true;
        }
        else
        {
            projectileRdy = false;
        }
    }
}
