using UnityEngine;

public class ParticlesOnDestroy : MonoBehaviour
{
    public Color particleStartColor = Color.white;
    internal static bool quitting;
    private void OnApplicationQuit()
    {
        quitting = true;
    }

    private void OnDestroy()
    {
        if (quitting || SceneManagerScript.Instance.GetActiveSceneName() == "Master" || !SceneManagerScript.Instance.loadingFinished)
            return;
        ParticleManager.Instance.SpawnParticles("Triangles", transform.position, Quaternion.identity, particleStartColor);
    }
}
