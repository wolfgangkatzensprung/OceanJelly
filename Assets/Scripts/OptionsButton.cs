using UnityEngine;
using UnityEngine.EventSystems;

public class OptionsButton : MonoBehaviour, IPointerDownHandler
{
    public void DisablePlayerMovement()
    {
        PlayerMovement.Instance.canMove = false;
    }

    public void ToggleOptionsPanel()
    {
        UIManager.Instance.ToggleOptions();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        DisablePlayerMovement();
    }
}
