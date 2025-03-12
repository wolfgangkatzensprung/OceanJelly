using Cinemachine;
using UnityEngine;

public class WonderWall : MonoBehaviour
{
    public GameObject particlePrefab;
    public GameObject[] objectsToDestroy;
    public CinemachineVirtualCamera vcam;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        vcam.m_Priority = 20;
        SoundManager.Instance.PlaySound("Test");
        Instantiate(particlePrefab, transform.position, Quaternion.identity);
        GetComponent<Collider2D>().isTrigger = true;
        vcam.Follow = PlayerManager.Instance.player.transform;
        for (int i = 0; i < objectsToDestroy.Length; i++)
        {
            Destroy(objectsToDestroy[i]);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("OnTriggerExit WW");
        vcam.m_Priority = 0;
        DirectorScript.Instance.externalCamActive = false;
        DirectorScript.Instance.vcamObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("OnTriggerEnter WW");
        vcam.m_Priority = 20;
        DirectorScript.Instance.externalCamActive = true;
        DirectorScript.Instance.vcamObject.SetActive(false);
    }
}
