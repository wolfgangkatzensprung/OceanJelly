using UnityEngine;

public class Ocean : MonoBehaviour
{
    private void Awake()
    {
        MusicManager.Instance.StopMusic();

        GameController.Instance.inDungeon = false;

        if (Ocean_AssetSpawner.Instance != null)
            Ocean_AssetSpawner.Instance.ClearListsAll();

        GameController.Instance.oceanBackground.SetActive(true);
        LightManager.Instance.SetGlobalLightIntensity(LightAreas.Instance.areaLights[0].intensity);
        DirectorScript.Instance.DisableConfiner();


        UIManager.Instance.FadeIn();
        GameController.Instance.UnpauseGame();

        GameController.Instance.inDungeon = false;
    }
}
