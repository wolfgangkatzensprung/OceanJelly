using UnityEngine;

public class ReturnToGameButton : MonoBehaviour
{
    public void CloseOptionPanelButton()
    {
        UIManager.Instance.ToggleOptions();
    }
}
