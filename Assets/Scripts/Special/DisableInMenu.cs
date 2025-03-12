using UnityEngine;

public class DisableInMenu : MonoBehaviour
{
    void Start()
    {
        if (GameController.Instance.GetIsInMenu())
        {
            gameObject.SetActive(false);
        }
    }
}
