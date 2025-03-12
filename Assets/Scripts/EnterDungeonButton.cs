using UnityEngine;

public class EnterDungeonButton : MonoBehaviour
{
    public SceneManagerScript.SceneType dungeonType;

    public void EnterTheDungeon()
    {
        SceneManagerScript.Instance.EnterDungeon(dungeonType);
    }
}
