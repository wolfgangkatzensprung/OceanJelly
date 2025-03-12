using UnityEngine;

public class PlayButton : MonoBehaviour
{
    public void PlayGame()
    {
        GameController.Instance.UnpauseGame();

        string sceneToLoad = PlayerPrefs.GetString("checkpointScene");
        Vector2 continuePosition = new Vector2(PlayerPrefs.GetFloat("checkpointX"), PlayerPrefs.GetFloat("checkpointY"));

        SceneManagerScript.Instance.LoadSceneAsync(sceneToLoad, continuePosition);
    }

    public void NewGame()
    {
        GameController.Instance.UnpauseGame();
        PlayerPrefsManager.Instance.NewGame();
        Vector2 startPosition = new Vector2(PlayerPrefs.GetFloat("startCheckpointX"), PlayerPrefs.GetFloat("startCheckpointY"));

        SceneManagerScript.Instance.LoadSceneAsync("PlateauStart", startPosition);
    }

    public void LoadDebugScene()
    {
        GameController.Instance.UnpauseGame();
        string sceneToLoad = "Debug";
        SceneManagerScript.Instance.UsePortal(sceneToLoad, new Vector2(0, 0));
    }

}
