using UnityEngine;

public class DrawWireSphereGizmos : MonoBehaviour
{
    public Color color;
    public float radius = 10f;

    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
