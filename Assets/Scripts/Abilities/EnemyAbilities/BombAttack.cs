using System.Collections;
using UnityEngine;

public class BombAttack : Enemy
{
    public GameObject explosionPrefab;
    SpriteRenderer sr;
    Animator anim;

    bool isExploding;
    float timer = 0f;   // exploding timer

    float startTime = 1f;  // warmup time from start. will not explode during this startup time.
    Color startColor = Color.white;
    bool initialized;

    private void OnEnable()
    {
        if (sr != null)
        {
            sr.color = Color.white;
        }
    }

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        startColor = sr.color;

        Invoke("Init", startTime);
    }

    private void Init()
    {
        initialized = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isExploding && initialized)
            StartExplosion();
    }

    public void StartExplosion()
    {
        if (!isExploding && initialized)
            StartCoroutine(ExplosionRoutine());
    }

    IEnumerator ExplosionRoutine()
    {
        isExploding = true;

        sr.color = startColor;
        yield return new WaitForSeconds(.5f);
        sr.color = Color.red;
        yield return new WaitForSeconds(.5f);
        sr.color = startColor;
        yield return new WaitForSeconds(.5f);
        sr.color = Color.red;
        yield return new WaitForSeconds(.5f);
        Explode();
    }


    void Explode()
    {
        timer = 0;
        Instantiate(explosionPrefab, transform.position, transform.rotation);
        ParticleManager.Instance.SpawnParticles("DeathFlecken", transform.position, Quaternion.identity);
        gameObject.SetActive(false);
        isExploding = false;
    }
}