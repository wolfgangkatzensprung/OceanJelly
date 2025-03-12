using UnityEngine;

public class RotateTransform : MonoBehaviour
{
    public float rotationSpeed = 10f;

    private void Update()
    {
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, transform.parent.rotation.z * -1.0f + rotationSpeed * Time.realtimeSinceStartup);
        //transform.Rotate(Vector3.forward);
    }
}