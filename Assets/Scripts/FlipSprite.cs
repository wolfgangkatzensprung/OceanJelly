using UnityEngine;

public class FlipSprite : MonoBehaviour
{
    public SpriteRenderer sr;

    public void FlipX()
    {
        sr.flipX = !sr.flipX;
    }
    public void FlipY()
    {
        sr.flipY = !sr.flipY;
    }

}

