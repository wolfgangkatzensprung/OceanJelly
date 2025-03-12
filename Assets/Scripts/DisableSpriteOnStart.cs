using UnityEngine;

public class DisableSpriteOnStart : MonoBehaviour
{
    void Start()
    {
        if (TryGetComponent<SpriteRenderer>(out SpriteRenderer sr))
        {
            sr.enabled = false;
        }
    }
}
