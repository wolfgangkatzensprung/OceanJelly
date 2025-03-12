using System.Collections;
using UnityEngine;

public class SelfDestructionTimer : MonoBehaviour
{
    public float selfDestructionTime = 10f;
    void Start()
    {
        StartCoroutine(SelfDestruction());
    }

    IEnumerator SelfDestruction()
    {
        yield return new WaitForSeconds(selfDestructionTime);
        Destroy(gameObject);
    }
}
