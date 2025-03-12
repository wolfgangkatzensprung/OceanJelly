using TMPro;
using UnityEngine;

public class DebugConsoleInputField : MonoBehaviour
{
    public DebugConsole console;
    public TMP_InputField inputField;

    bool selected;

    //private void Update()
    //{
    //    if (Input.GetButtonDown("Submit"))
    //    {
    //        if (!selected)
    //        {
    //            SetSelected();
    //        }
    //        else if (selected)
    //        {
    //            SetDeselected();
    //        }
    //    }

    //    if (Input.GetButtonDown("Escape"))
    //    {
    //        SetDeselected();
    //    }
    //}

    void ClearInputField()
    {
        inputField.text = null;
    }

    public void SetSelected()
    {
        ClearInputField();
        inputField.Select();

        selected = true;
        GameController.Instance.PauseGame(freeze: false);
        UIManager.Instance.DisableCanToggleBools();
    }
    public void SetDeselected()
    {
        inputField.ReleaseSelection();

        selected = false;
        GameController.Instance.UnpauseGame();
        UIManager.Instance.EnableCanToggleBools();
        ClearInputField();
    }

    public void TriggerInputFieldFromUI()
    {
        console.DoCommand(inputField.text);
    }
}