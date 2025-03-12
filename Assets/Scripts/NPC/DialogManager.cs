using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance;

    public GameObject dialogWindow;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    Transform dialogTriggerHost;

    // Read Text
    public Queue<string> sentences;
    float textDelay = .01f;        // text delay bestimmt die text geschwindigkeit
    Coroutine typeRoutine;       // Coroutine die den Text schreibt

    // angezeigter Text ohne tags
    string currentText;
    bool ignoreChars;

    // Delay & Timer
    public float dialogTimer = 0;
    bool dialogTimerRunning;    // true solange der Dialog laeuft. Nachdem die delay zeit abgelaufen ist, wird dies false
    float delay;    // zeit die der dialogTimer mindestens pro Satz gelaufen sein muss
    Coroutine finishRoutine;

    public bool dialogFinished = false;     // true wenn dialog completed ist
    public bool sentenceFinished = false;   // wird in DialogTrigger auf true gesetzt wenn der sentence komplett dargestellt wird

    LanguageSetting.Language language;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        SetLanguageFromPlayerPref();
        sentences = new Queue<string>();
    }

    public void SetLanguageFromPlayerPref()
    {
        if (PlayerPrefs.GetInt("Language") == 0)
        {
            Debug.Log("Sprache ist Englisch");
            language = LanguageSetting.Language.English;
        }
        else if (PlayerPrefs.GetInt("Language") == 1)
        {
            Debug.Log("Sprache ist Deutsch");
            language = LanguageSetting.Language.German;
        }
    }

    private void Update()
    {
        if (dialogTimerRunning)
        {
            dialogTimer += Time.unscaledDeltaTime;
            if (dialogTimer > delay)
                dialogTimerRunning = false;
        }
    }

    public void StartDialogue(Dialogue.Info dialogue)
    {
        Debug.Log("StartDialogue");

        dialogTimer = 0;
        dialogTimerRunning = true;
        nameText.text = dialogue.name;
        sentences.Clear();
        dialogWindow.SetActive(true);
        delay = dialogue.delay;


        string[] theSentences = new string[0];
        if (language == LanguageSetting.Language.German)
        {
            theSentences = dialogue.sentencesGer;
        }
        else if (language == LanguageSetting.Language.English)
        {
            theSentences = dialogue.sentences;
        }
        foreach (string sentence in theSentences)
        {
            sentences.Enqueue(sentence);
        }
        DisplayNextSentence();
    }
    public void DisplayNextSentence()
    {
        dialogTimer = 0;
        dialogTimerRunning = true;
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        dialogueText.text = "";
        currentText = "";
        string sentence = sentences.Peek(); // wird dann in DialogTrigger Dequeue()ed wenn der Satz fertig ist

        sentenceFinished = false;
        typeRoutine = StartCoroutine(Type(sentence));
    }

    // Finish
    public void FinishSentence(string _sentence)
    {
        Debug.Log("FinishSentence()");
        dialogueText.text = _sentence;
        currentText = _sentence;
        StopCoroutine(typeRoutine);
        sentenceFinished = true;

        InvokeSentenceFinishedEvents();
    }

    private void InvokeSentenceFinishedEvents()
    {
        if (TryGetComponent<DialogEvents>(out DialogEvents events))
        {
            events.TriggerEventAfterSentence();
        }
    }

    public void EndDialogue()
    {
        Debug.Log("EndDialogue()");
        dialogTimerRunning = false;
        dialogTimer = 0;
        dialogWindow.SetActive(false);
        dialogFinished = true;
        InvokeDialogFinishedEvents();
    }

    private void InvokeDialogFinishedEvents()
    {
        Debug.Log("InvokeDialogFinishedEvents()");

        if (dialogTriggerHost != null)
        {
            PlayerManager.Instance.EnablePlayer();

            if (dialogTriggerHost.TryGetComponent(out DialogEvents eventsComponent))
                eventsComponent.TriggerEventAfterDialog();
        }
    }

    internal void ResetDialog()
    {
        Debug.Log("ResetDialog()");
        dialogTimerRunning = false;
        dialogTimer = 0;
        dialogueText.text = "";
        currentText = "";
        dialogWindow.SetActive(false);
        dialogFinished = true;
    }

    #region TypeDialogText

    IEnumerator Type(string sentence)
    {
        Debug.Log("Type Dialog Letters");

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += "<color=#ff000000>" + letter + "</color>";
        }
        for (int i = 0; i < dialogueText.text.Length; i++)
        {
            int rauteIndex;
            char[] charArray = dialogueText.text.ToCharArray();

            SetCurrentDialogText(i, charArray);

            if (charArray[i] == '#')
            {
                rauteIndex = i;
                dialogueText.text = dialogueText.text.Remove(rauteIndex + 1, 8);
                dialogueText.text = dialogueText.text.Insert(rauteIndex + 1, "ffffffff");
                i += 8;
            }
            else if (charArray[i] == '<')
            {
                i += 6;
            }
            else if (charArray[i] != '>')
            {
                SoundManager.Instance.PlaySound("Test");
                yield return new WaitForSeconds(textDelay);
                yield return new WaitForEndOfFrame();
                //Debug.Log(i + " " + (dialogueText.text.Length - 9));
                if (i == dialogueText.text.Length - 9)  // letzter Buchstabe
                {
                    if (dialogTimer > delay && !sentenceFinished)
                        FinishSentence(currentText);
                }
            }
        }
    }

    private void SetCurrentDialogText(int i, char[] charArray)
    {
        if (charArray[i] == '<')
            ignoreChars = true;
        else if (!ignoreChars)
            AddDialogLetter(charArray[i]);
        if (charArray[i] == '>')
            ignoreChars = false;
        //Debug.Log("Aktueller DialogText ohne Tags: " + GetDialogText());
    }

    // tags aus aktuell angezeigtem Text rausfiltern
    void AddDialogLetter(char letter)
    {
        currentText += letter;
    }

    // aktuell angezeigter Text ohne tags
    public string GetDialogText()
    {
        return currentText;
    }

    #endregion

    public bool GetDialogActive()
    {
        return dialogTimerRunning;
    }

    public bool GetDialogFinished()
    {
        return dialogFinished;
    }

    public void SetDialogTriggerHost(Transform trans)
    {
        dialogTriggerHost = trans;
    }
    public Transform GetDialogTriggerHost()
    {
        return dialogTriggerHost;
    }
    public void ResetDialogTriggerHost()
    {
        dialogTriggerHost = null;
    }
}
