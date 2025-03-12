using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class AimAtPlayerStalagtit : MonoBehaviour
{
    Animator anim;

    public float maxDistance = 50f;
    Vector2 startPosition;

    float lerpSpeed = .1f;

    public GameObject stalagtitProjectile;
    bool isSpawningStalaProjectile;
    public Transform stalaProjectileSpawnPosition;

    private void Start()
    {
        anim = GetComponent<Animator>();
        startPosition = transform.position;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            anim.SetBool("Zitter", true);
            if (CheckIfBelowMaxDistance())
            {
                Aiming();
                MaybeSpawnStalagtitProjectile();
            }
        }
    }

    private void MaybeSpawnStalagtitProjectile()
    {
        if (!isSpawningStalaProjectile && Random.value > .9f)
        {
            StartCoroutine(SpawnStalagtitProjectile());

        }
    }

    IEnumerator SpawnStalagtitProjectile()
    {
        isSpawningStalaProjectile = true;
        Instantiate(stalagtitProjectile, stalaProjectileSpawnPosition.position, Quaternion.identity);
        yield return new WaitForEndOfFrame();
        isSpawningStalaProjectile = false;
    }

    private bool CheckIfBelowMaxDistance()
    {
        if (transform.position.x > (startPosition.x + maxDistance))
        {
            return false;
        }
        else return true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        anim.SetBool("Zitter", false);
    }
    void Aiming()
    {
        transform.position = new Vector3(Mathf.Lerp(transform.position.x, PlayerManager.Instance.player.transform.position.x, lerpSpeed), transform.position.y, 0);
    }
}
