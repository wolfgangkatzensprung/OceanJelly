using Cinemachine;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] private float zoomSpeed = 0.1f;
    [SerializeField] private float minZoom = 1.0f;
    [SerializeField] private float maxZoom = 10.0f;

    private CinemachineVirtualCamera virtualCamera;

    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    //private void Update()
    //{
    //    if (isZooming)
    //    {
    //        float zoomValue = PlayerInputManager.Instance.zoomAction.ReadValue<float>();

    //        // Adjust camera size based on zoom input
    //        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - zoomValue * zoomSpeed, 1f, 10f);
    //    }
    //}
}
