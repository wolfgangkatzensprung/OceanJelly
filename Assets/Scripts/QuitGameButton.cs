using UnityEngine;

public class QuitGameButton : MonoBehaviour
{
    public void QuitGameAndRun()    // temp
    {
        Application.Quit(); // komplett Quit

    }
    public void QuitGame()
    {
        SceneManagerScript.Instance.UnloadGame();
        Application.Unload();   // Unity Player Quit. STATIC values bleiben gleich, daher cleanup VORHER
    }
}
