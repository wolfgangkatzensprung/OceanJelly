using System.Collections;
using UnityEngine;

public class Plankton : MonoBehaviour
{
    public int healAmount = 1;
    bool isHealing;

    private void OnParticleCollision(GameObject other)
    {
        if (!isHealing)
            StartCoroutine(HealPlayer());
    }


    IEnumerator HealPlayer()
    {
        isHealing = true;
        PlayerHealth.Instance.HealPlayer(healAmount);
        yield return new WaitForFixedUpdate();
        yield return new WaitForEndOfFrame();
        isHealing = false;
    }
}
