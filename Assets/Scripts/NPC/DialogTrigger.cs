using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    [Header("Dialog Settings")]
    public Dialogue.Info[] dialogueInfo = new Dialogue.Info[1];

    int index = 0;  // index fuer dialogueInfo ; jeder index ist ein dialog abschnitt also eine Person. Wenn die naechste Person spricht, wird der index um 1 erhoeht
    private DialogManager dm;

    [Header("Dialog Settings")]
    [Tooltip("Deaktiviert Attack in DialogRange")]
    [SerializeField] bool disableAttack = true;

    [Tooltip("Deaktiviert Player Movement, Jump und Attack waehrend des Dialogs")]
    [SerializeField] bool disablePlayer = false;

    [Tooltip("Dialog wird nur 1x abgespielt")]
    [SerializeField] bool playDialogOnlyOneTime = false;

    [Tooltip("Startet Dialog in Start()")]
    [SerializeField] bool triggerAtStart = false;

    [Tooltip("Startet Dialog OnTriggerEnter(). PlayerControls werden deaktiviert.")]
    [SerializeField] bool startOnTriggerEnter = false;

    bool inRange;
    bool dontStartAgain = false;
    string _sentence;   // aktueller sentence

    private void Start()
    {
        dm = DialogManager.Instance;

        if (triggerAtStart)
            StartDialog();
    }

    private void Update()
    {
        //if (dm.GetDialogFinished())
        //{
        //    Debug.Log("FinishDialog() in DialogTrigger");
        //    FinishDialog();
        //}
        //else if (Input.GetButtonDown("Attack") && inRange)
        //{
        //    if (!PlayerManager.Instance.anim.GetBool("dialogActive"))
        //    {
        //        TryStartDialog();
        //    }
        //    else
        //    {
        //        ContinueDialog();
        //    }
        //}

        dm.dialogFinished = false;  // fuer naechsten dialog bereitmachen
    }

    private void TryStartDialog()
    {
        if (!dontStartAgain && !PlayerManager.Instance.anim.GetBool("dialogActive"))
        {
            StartDialog();
        }
    }

    public void StartDialog()
    {
        if (playDialogOnlyOneTime)
            dontStartAgain = true;

        Debug.Log("Trigger Dialog");
        dm.SetDialogTriggerHost(transform);
        dm.StartDialogue(dialogueInfo[index]);

        PlayerManager.Instance.anim.SetBool("dialogActive", true);

        if (disablePlayer)
        {
            PlayerManager.Instance.StopPlayer();
            PlayerManager.Instance.DisablePlayer();
        }

        if (disableAttack)
        {
            PlayerManager.Instance.DisableAttack();
        }
    }

    private void ContinueDialog()
    {
        if (!dm.sentenceFinished)
        {
            if (_sentence == null)
            {
                Debug.Log("Neuer _sentence zugewiesen");
                _sentence = dm.sentences.Dequeue();
            }

            if (dm.GetDialogText() == _sentence)
            {
                Debug.Log("sentence finished");
                dm.sentenceFinished = true;
            }
        }
        //if (Input.GetButtonDown("Attack"))
        //{
        //    Debug.Log("Attacked to ContinueDialog");
        //    if (!dm.sentenceFinished)
        //    {
        //        dm.FinishSentence(_sentence);
        //    }
        //    else if (dm.dialogTimer > dialogueInfo[index].delay)
        //    {
        //        dm.DisplayNextSentence();
        //        _sentence = null;   // damit neuer sentence reinpasst
        //    }
        //}
    }

    private void FinishDialog()
    {
        Debug.Log("FinishDialog()");
        PlayerManager.Instance.EnablePlayer();
        _sentence = null;   // damit neuer sentence reinpasst

        if (index < dialogueInfo.Length - 1)    // wenn das nächste Sprecherprofil benutzt werden soll
        {
            index++;
            StartDialog();
            PlayerManager.Instance.anim.SetBool("dialogActive", true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !inRange)
        {
            Debug.Log("In Range fuer Dialog");
            inRange = true;

            if (startOnTriggerEnter)
            {
                StartDialog();
                PlayerManager.Instance.anim.SetBool("dialogActive", true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            inRange = false;
            _sentence = null;
            if (disableAttack)
            {
                PlayerManager.Instance.EnableAttack();
            }
            dm.ResetDialog();
            PlayerManager.Instance.anim.SetBool("dialogActive", false);

            if (disablePlayer)
            {
                PlayerManager.Instance.EnablePlayer();
            }
        }
    }
}
