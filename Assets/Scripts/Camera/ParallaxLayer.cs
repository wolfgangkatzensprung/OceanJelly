using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    float parallaxFactor = 0f;

    public void Move(float delta)
    {
        if (transform != null)
        {
            Vector3 newPos = transform.localPosition;
            newPos.x -= delta * parallaxFactor;
            newPos.y -= delta * parallaxFactor;
            transform.localPosition = newPos;
        }
    }

    public void SetParallaxFactor(float fac)
    {
        parallaxFactor = fac;
    }
}