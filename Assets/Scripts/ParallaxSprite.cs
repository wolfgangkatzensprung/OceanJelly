using UnityEngine;

public class ParallaxSprite : MonoBehaviour
{
    Transform parent;

    private void Start()
    {
        parent = transform.parent;
    }

    private void OnBecameInvisible()
    {
        transform.parent?.SetParent(null);
    }

    private void OnBecameVisible()
    {
        transform.parent?.SetParent(parent);
    }
}
