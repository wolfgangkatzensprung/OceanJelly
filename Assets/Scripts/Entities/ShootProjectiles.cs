using UnityEngine;

public class ShootProjectiles : MonoBehaviour
{
    public GameObject[] prefabsToShoot;
    public GameObject firePoint;
    Animator anim;

    public bool facingRight = false;    // if sprite is facing right

    // Shooting
    bool shooting;
    Vector2 shootDirection;
    public bool alwaysShoot = false;    // if false, turret will only shoot when player is in trigger range
    float timer = 0;                    // time since shot from 0 to reloadTime
    public float reloadTime = 5;

    private void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        if (facingRight)
            shootDirection = new Vector2(1, 0);
        else
            shootDirection = new Vector2(-1, 0);
        if (alwaysShoot)
            shooting = true;
        if (shooting)
            ShootProjectile(shootDirection);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > reloadTime)
        {
            timer = 0f;
            if (shooting)
            {
                if (facingRight)
                {
                    shootDirection = new Vector2(1, 0);
                }
                else if (!facingRight)
                {
                    shootDirection = new Vector2(-1, 0);
                }
                ShootProjectile(shootDirection);
            }
        }
    }

    void ShootProjectile(Vector2 dir)
    {
        anim.Play("Shoot");
        int random = Random.Range(0, prefabsToShoot.Length);
        Instantiate(prefabsToShoot[random], firePoint.transform.position, Quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!shooting)
            {
                shooting = true;
                ShootProjectile(shootDirection);

            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!alwaysShoot)
                if (shooting)
                    shooting = false;
        }
    }
}
