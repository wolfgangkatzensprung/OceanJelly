using UnityEngine;

public class RandomSprite : MonoBehaviour
{
    [Tooltip("Sprites to choose from")]
    public Sprite[] sprites;
    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];
    }
}
