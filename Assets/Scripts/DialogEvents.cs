using UnityEngine;
using UnityEngine.Events;

public class DialogEvents : MonoBehaviour
{
    public UnityEvent OnDialogStart;
    public UnityEvent OnSentenceFinished;
    public UnityEvent OnDialogFinished;

    public void TriggerEventAtDialogStart()
    {
        OnDialogStart.Invoke();
    }

    public void TriggerEventAfterDialog()
    {
        OnDialogFinished.Invoke();
    }

    public void TriggerEventAfterSentence()
    {
        OnSentenceFinished.Invoke();
    }
}
