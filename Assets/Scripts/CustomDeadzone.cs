using Cinemachine;
using UnityEngine;

public class CustomDeadzone : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public float deadzoneRadius = 2f;
    public float damping = 0.5f;
    public Color debugColor = Color.red;

    private CinemachineCameraOffset cameraOffset;
    private Transform player;
    private Camera mainCamera;
    private Vector2 currentOffsetVelocity;
    private bool resetOffset;

    private void Start()
    {
        cameraOffset = GetComponent<CinemachineCameraOffset>();
        player = PlayerManager.Instance.playerTrans;
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (cameraOffset == null || player == null || mainCamera == null)
            return;

        Vector2 playerScreenPos = mainCamera.WorldToScreenPoint(player.position);
        Vector2 centerPos = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 cameraOffsetVector = playerScreenPos - centerPos;
        float centerDistance = Vector2.Distance(playerScreenPos, centerPos);

        //Debug.Log("Camera Offset Vector: " + cameraOffsetVector);
        //Debug.Log("Player Distance to Center: " + centerDistance);

        if (centerDistance > deadzoneRadius || cameraOffsetVector.magnitude > deadzoneRadius)
        {
            Vector2 desiredOffset = cameraOffsetVector.normalized * (cameraOffsetVector.magnitude - deadzoneRadius);
            Vector2 smoothedOffset = Vector2.SmoothDamp(cameraOffset.m_Offset, desiredOffset, ref currentOffsetVelocity, damping);
            cameraOffset.m_Offset = smoothedOffset;

            //Debug.Log("Desired Offset: " + desiredOffset);

            resetOffset = false; // Reset the resetOffset flag since the offset is actively adjusting
        }
        else if (!resetOffset)
        {
            resetOffset = true; // Set the resetOffset flag if it's not already set

            // Smoothly reset the offset to zero
            Vector2 targetOffset = Vector2.zero; // Target offset is always (0, 0)
            Vector2 smoothedOffset = Vector2.SmoothDamp(cameraOffset.m_Offset, targetOffset, ref currentOffsetVelocity, damping, Mathf.Infinity, Time.deltaTime);
            cameraOffset.m_Offset = smoothedOffset;

            if (cameraOffset.m_Offset.magnitude >= 0.01f)
            {
                resetOffset = false; // Reset the resetOffset flag to allow further updates
            }
        }

    }

    private void OnDrawGizmos()
    {
        if (mainCamera == null)
            return;

        Gizmos.color = debugColor;
        float worldRadius = deadzoneRadius * mainCamera.orthographicSize * 2f / Screen.height;
        Vector3 centerPoint = mainCamera.transform.position;
        Gizmos.DrawWireSphere(centerPoint, worldRadius);
    }
}
