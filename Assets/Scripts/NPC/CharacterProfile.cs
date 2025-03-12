using UnityEngine;

[CreateAssetMenu(fileName = "New Profile", menuName = "Character Profiles")]
public class CharacterProfile : ScriptableObject
{
    public string myName;
    private Sprite myPortrait;
    public AudioClip myVoice;
    public Font myFont;

    public Sprite MyPortrait
    {
        get
        {
            // add portrait

            return myPortrait;
        }
    }

    [System.Serializable]
    public class EmotionPortraits
    {
        public Sprite standard;
        public Sprite happy;
        public Sprite angry;
    }

    public EmotionPortraits emotionPortraits;

    public enum EmotionType
    {
        Standard,
        Happy,
        Angry
    }
}
