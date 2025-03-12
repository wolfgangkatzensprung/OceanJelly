using UnityEngine;

public class InfiniteTexture : MonoBehaviour
{
    SpriteRenderer sr;
    MeshRenderer mr;
    Material mat;

    public float parallaxFactor = 2f;

    private void Start()
    {
        if (TryGetComponent(out SpriteRenderer sr))
        {
            this.sr = sr;
            mat = sr.material;
        }
        else if (TryGetComponent(out MeshRenderer mr))
        {
            this.mr = mr;
            mat = mr.material;
        }
    }

    private void Update()
    {
        Vector3 offset = mat.mainTextureOffset;

        offset.x = transform.position.x / transform.localScale.x * parallaxFactor;
        offset.y = transform.position.y / transform.localScale.y * parallaxFactor;

        mat.mainTextureOffset = offset;
    }
}
