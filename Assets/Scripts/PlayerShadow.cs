using System.Collections;
using UnityEngine;

public class PlayerShadow : MonoBehaviour
{
    bool rdy = true;

    private void Update()
    {
        if (rdy)
        {
            rdy = false;
            StartCoroutine(SetPositionAndDirection());
        }
    }

    IEnumerator SetPositionAndDirection()
    {
        transform.localScale = new Vector2(PlayerManager.Instance.GetFacingDirection(), 1);
        transform.position = PlayerManager.Instance.playerPosition;
        yield return new WaitForFixedUpdate();
        rdy = true;
    }
}