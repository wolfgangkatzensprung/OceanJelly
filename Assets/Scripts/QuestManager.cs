using TMPro;

public class QuestManager : Singleton<QuestManager>
{
    public TextMeshProUGUI questText;

    private bool dungeonActive = false;
    private bool mermaidRescued = false;

    private void Start()
    {
        ResetQuest();

        GameController.Instance.onDungeonStarted += DungeonEntered;
        GameController.Instance.onDungeonFinished += DungeonExited;
    }

    public void ResetQuest()
    {
        SetQuestText("Find the Dungeon Entrance.");
    }

    public void DungeonEntered(Dungeon.DungeonType dt)
    {
        switch (dt)
        {
            default:
                SetQuestText("Survive.");
                break;
            case Dungeon.DungeonType.IceCave:
                SetQuestText("Solve the Ice Riddle");
                break;

        }
    }

    public void DungeonExited()
    {
        if (PlayerHealth.Instance.alive)
        {
            SetQuestText("Something changed...?!");
        }
        else
        {
            ResetQuest();
        }
    }

    public void BossfightStarted()
    {
        SetQuestText("Fight.");
    }

    public void MermaidFound()
    {
        SetQuestText("Escort the Mermaid safely to the exit.");
    }

    private void SetQuestText(string text)
    {
        questText.text = text;
    }
}
