using UnityEngine;

public class QuitToMenuButton : MonoBehaviour
{
    public GameObject optionsPanel;

    public void QuitToMenu()
    {
        optionsPanel.SetActive(false);
        SceneManagerScript.Instance.LoadSceneAsync("Menu");
    }
}
