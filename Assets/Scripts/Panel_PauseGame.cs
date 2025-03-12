using UnityEngine;

public class Panel_PauseGame : MonoBehaviour
{
    private void OnEnable()
    {
        PlayerMovement.Instance.canMove = false;
        GameController.Instance.PauseGame(true);

    }
    private void OnDisable()
    {
        GameController.Instance.UnpauseGame();
        PlayerMovement.Instance.canMove = true;
    }
}
