using UnityEngine;

public class Fischspucker : MonoBehaviour
{
    public GameObject[] prefabs;
    public GameObject firePoint;
    Animator anim;
    Vector2 spitDirection;    // default schuss nach rechts
    bool facingRight = false;
    float timer = 0;
    public float reloadTime = 5;

    private void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        spitDirection = new Vector2(-1, 0);
        SpitFish(spitDirection);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > reloadTime)
        {
            timer = 0f;
            if (facingRight)
            {
                spitDirection = new Vector2(1, 0);
            }
            else if (!facingRight)
            {
                spitDirection = new Vector2(-1, 0);
            }
            SpitFish(spitDirection);
        }
    }

    void SpitFish(Vector2 dir)
    {
        anim.Play("Spuck");
        int random = Random.Range(0, prefabs.Length);
        Instantiate(prefabs[random], firePoint.transform.position, Quaternion.identity);
    }
}
