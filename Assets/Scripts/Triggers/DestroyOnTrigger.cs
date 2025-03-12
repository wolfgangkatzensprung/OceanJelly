using UnityEngine;

public class DestroyOnTrigger : MonoBehaviour
{
    public GameObject objectToDestroy;  // additional

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (objectToDestroy != null)
            Destroy(objectToDestroy);
        Destroy(gameObject);
    }
}
