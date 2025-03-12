using UnityEngine;

public class SacrificeTrigger : MonoBehaviour
{
    /*
    List<GameObject> offerings = new List<GameObject>();
    bool rdyForOfferings;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!offerings.Contains(collision.gameObject))
        {
            offerings.Add(collision.gameObject);
        }
    }

    private void Update()
    {
        if ((TimeManager.Instance.timer3) > 2) // alle 3 sekunden
        {
            if (rdyForOfferings)
                AcceptOfferings();
        }
        else rdyForOfferings = true;
    }

    void AcceptOfferings()
    {
        Debug.Log("AcceptOfferings()");
        foreach (GameObject obj in offerings)
        {
            if (obj.CompareTag("Item") || obj.CompareTag("Nautic") || obj.CompareTag("Flame") || obj.CompareTag("Aether"))
            {
                ParticleManager.Instance.SpawnParticles("SacrificeParticles", obj.transform.position, Quaternion.identity);
                Destroy(obj);
            }
        }
        rdyForOfferings = false;
        offerings.Clear();
    }
    */
}
