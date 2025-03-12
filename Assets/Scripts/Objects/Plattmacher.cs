using System.Collections;
using UnityEngine;

public class Plattmacher : MonoBehaviour
{
    Animator playerAnim;
    public float stunTime = 1f;
    public float yOffset = 50f;
    float directionX = 0f;
    bool isPlatt;
    bool isCheckingDirection;

    private void Start()
    {
        playerAnim = PlayerManager.Instance.GetPlayerAnim();
    }

    private void Update()
    {
        if (isCheckingDirection)
            return;

        StartCoroutine(DirectionCheck());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            CheckPlayerPosition();
        }
    }

    IEnumerator DirectionCheck()
    {
        isCheckingDirection = true;
        float currentPositionX = transform.position.x;
        yield return new WaitForSeconds(1);
        SetDirectionX(currentPositionX);
        isCheckingDirection = false;
    }

    private void SetDirectionX(float xPos1secAgo)
    {
        directionX = transform.position.x - xPos1secAgo;
    }

    private void CheckPlayerPosition()
    {
        Vector2 playerPos = PlayerManager.Instance.player.transform.position;
        bool isPlayerInRollingDirection;
        if ((PlayerManager.Instance.CheckIfRightFromPlayer(transform.position) && directionX < 0) || (!PlayerManager.Instance.CheckIfRightFromPlayer(transform.position) && directionX > 0))
        {
            isPlayerInRollingDirection = true;
        }
        else
            isPlayerInRollingDirection = false;

        if (playerPos.y < transform.position.y - yOffset && !isPlatt && isPlayerInRollingDirection)
        {
            StartCoroutine(MachPlayerPlatt());
        }
    }

    IEnumerator MachPlayerPlatt()
    {
        isPlatt = true;
        CombatManager.Instance.TryStunPlayer(stunTime);
        CombatManager.Instance.DealDamageToPlayer(1);
        playerAnim.Play("Platt");
        yield return new WaitForSeconds(stunTime + Time.deltaTime);
        isPlatt = false;

    }

}
