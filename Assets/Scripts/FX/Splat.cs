using UnityEngine;

public class Splat : MonoBehaviour
{

    public enum SplatLocation
    {
        Foreground,
        Background,
    }

    public Color backgroundTint;
    public float minSizeMod = 0.8f;
    public float maxSizeMod = 1.5f;

    public Sprite[] sprites;

    private SplatLocation splatLocation;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(SplatLocation splatLocation)
    {
        this.splatLocation = splatLocation;
        SetSprite();
        SetSize();
        SetRotation();
    }

    private void SetSprite()
    {
        int randomIndex = Random.Range(0, sprites.Length);
        spriteRenderer.sprite = sprites[randomIndex];
    }

    void SetSize()
    {
        float sizeMod = Random.Range(minSizeMod, maxSizeMod);
        transform.localScale *= sizeMod;
    }
    void SetRotation()
    {
        float randomRotation = Random.Range(-360, 360);
        transform.rotation = Quaternion.Euler(0f, 0f, randomRotation);
    }

    void SetLocationProperties()
    {
        switch (splatLocation)
        {
            case SplatLocation.Background:
                spriteRenderer.color = backgroundTint;
                spriteRenderer.sortingOrder = 0;
                break;

            case SplatLocation.Foreground:
                spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                spriteRenderer.sortingOrder = 3;
                break;
        }
    }

}
