using UnityEngine;

public class Gems : MonoBehaviour
{
    void Start()
    {
        if (GameProgress.Instance.hasCollectedNewGem)
        {
            var gems = GameProgress.Instance.artifacts;
            UIManager.Instance.GemPanelPopup(gems.Peek());
        }
        else
        {
            GameController.Instance.UnpauseGame();
        }
    }
}
