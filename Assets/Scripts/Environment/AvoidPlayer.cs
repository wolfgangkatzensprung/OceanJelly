using System.Collections;
using UnityEngine;

public class AvoidPlayer : MonoBehaviour
{
    Animator anim;

    Vector2 startPosition;

    Vector2 zitterNoise;
    float zitterMultiplier = 1;

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
            AvoidingPlayer();
            StartCoroutine(UpdateZitterNoise());
            SetAvoidDirection();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        anim.SetBool("Zitter", false);
    }
    void AvoidingPlayer()
    {
        transform.localPosition += (Vector3)zitterNoise;
    }
    IEnumerator UpdateZitterNoise()
    {
        zitterNoise = new Vector2(zitterMultiplier * +Mathf.PerlinNoise(PlayerManager.Instance.playerPosition.x, PlayerManager.Instance.playerPosition.y), 0);
        yield return new WaitForEndOfFrame();
    }
    void SetAvoidDirection()
    {
        if (PlayerManager.Instance.CheckIfRightFromPlayer(startPosition))
            zitterMultiplier = 1;
        else
            zitterMultiplier = -1;
    }
}
