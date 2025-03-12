using UnityEngine;

public class Shipwreck : MonoBehaviour
{
    public EdgeCollider2D splineCollider;

    private void OnEnable()
    {
        GameController.Instance.inShipwreck = true;
    }

    private void OnDisable()
    {
        GameController.Instance.inShipwreck = false;
    }

    private void Start()
    {
        MusicManager.Instance.PlayMusic("Shipwreck");

        if (Ocean_AssetSpawner.Instance != null)
            Ocean_AssetSpawner.Instance.ClearListsAll();

        UIManager.Instance.FadeIn();
        DirectorScript.Instance.customDeadzone.enabled = false;
        DirectorScript.Instance.SetCameraColliderFromEdgeCollider(splineCollider);
        DirectorScript.Instance.EnableConfiner();

        GameController.Instance.UnpauseGame();
    }
}
