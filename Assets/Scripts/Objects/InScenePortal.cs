using UnityEngine;

public class InScenePortal : MonoBehaviour
{
    public Transform portalAusgang;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerManager.Instance.player.transform.position = portalAusgang.position;
        }
    }

}
