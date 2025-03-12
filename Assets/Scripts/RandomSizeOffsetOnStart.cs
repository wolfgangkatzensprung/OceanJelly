using UnityEngine;

public class RandomSizeOffsetOnStart : MonoBehaviour
{
    private void Start()
    {
        transform.localScale = transform.localScale * Random.Range(.7f, 1.3f);
    }
}