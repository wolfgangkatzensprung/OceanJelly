using UnityEngine;

public class StickToCamera : MonoBehaviour
{
    Camera cam;

    public Vector3 offset = Vector3.zero;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (cam == null)
            return;

        if (transform.position != cam.transform.position)
            transform.position = cam.transform.position + offset;
    }
}
