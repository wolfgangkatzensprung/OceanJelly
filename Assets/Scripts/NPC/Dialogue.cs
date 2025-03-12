using UnityEngine;

public class Dialogue
{
    [System.Serializable]
    public class Info
    {
        [Header("Sprecher")]

        public string name;
        public Sprite portrait;

        [TextArea(5, 16)]
        public string[] sentences;
        [TextArea(5, 16)]
        public string[] sentencesGer;
        [Tooltip("Delay pro Satz in Sekunden")]
        public float delay;
    }

    public Info[] dialogueInfo;
}
