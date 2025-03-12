using UnityEngine;

public class DebugCommander : MonoBehaviour
{
    private void Start()
    {
        PlayerManager.Instance.GetPlayerTransform().position = Vector3.zero;
    }
}