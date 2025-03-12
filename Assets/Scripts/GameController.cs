using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController _instance;
    public static GameController Instance { get { return _instance; } }

    public GameObject oceanBackground;

    internal bool isGamePaused = false;
    internal bool isInMenu = false;
    internal bool inDungeon = false;
    internal bool inShipwreck = false;

    public delegate void DungeonStartedDelegate(Dungeon.DungeonType dt);
    public DungeonStartedDelegate onDungeonStarted;

    public delegate void DungeonFinishedDelegate();
    public DungeonFinishedDelegate onDungeonFinished;

    private void Awake()
    {
        _instance = this;
    }

    /// <summary>
    /// Pause Game. Pass true to freeze Time (recommended)
    /// </summary>
    /// <param name="freeze"></param>
    public void PauseGame(bool freeze)
    {
        PlayerMovement.Instance.canMove = false;
        isGamePaused = true;
        if (freeze)
            TimeManager.Instance.Freeze();
    }

    public void UnpauseGame()
    {
        PlayerMovement.Instance.canMove = true;
        isGamePaused = false;
        TimeManager.Instance.ResetTimeScale();
    }

    public bool IsGamePaused()
    {
        return isGamePaused;
    }

    public bool GetIsInMenu()
    {
        return isInMenu;
    }
    public void SetInMenu(bool b)
    {
        isInMenu = b;
    }

    internal void StartDungeon(Dungeon d)
    {
        inDungeon = true;

        if (Ocean_AssetSpawner.Instance != null)
            Ocean_AssetSpawner.Instance.ClearListsAll();

        LightManager.Instance.SetGlobalLightIntensity(LightAreas.Instance.areaLights[2].intensity);
        DirectorScript.Instance.UpdateConfiner(d.mg.tilemap);
        oceanBackground.SetActive(false);

        UnpauseGame();

        onDungeonStarted?.Invoke(Dungeon.DungeonType.Default);
    }
    public void FinishDungeon()
    {
        LightManager.Instance.SetGlobalLightIntensity(LightAreas.Instance.areaLights[0].intensity);

        SceneManagerScript.Instance.ExitDungeon();

        onDungeonFinished?.Invoke();
    }

    internal void StartIceCave(IceCave ic)
    {
        Debug.Log("StartIceCave");
        inDungeon = true;

        if (Ocean_AssetSpawner.Instance != null)
            Ocean_AssetSpawner.Instance.ClearListsAll();

        LightManager.Instance.SetGlobalLightIntensity(LightAreas.Instance.areaLights[3].intensity);
        oceanBackground.SetActive(false);

        //Vector2[] points = ic.GetPoints();
        //DirectorScript.Instance.SetCameraColliderFromEdgeCollider(ic.GetComponentInChildren<EdgeCollider2D>(), ic.transform.position);
        //DirectorScript.Instance.EnableConfiner();

        DirectorScript.Instance.customDeadzone.enabled = true;

        UnpauseGame();

        onDungeonStarted?.Invoke(Dungeon.DungeonType.IceCave);
    }

    public void Respawn()
    {
        Debug.Log("Respawning");
        //Debug.Log("Respawning to Scene: " + PlayerPrefs.GetString("checkpointScene"));

        if (Ocean_MapHandler.Instance != null)
            Ocean_MapHandler.Instance.ResetMap();

        if (inDungeon)
        {
            FinishDungeon();
            //StartCoroutine(UnloadDungeonAsync());
        }
        else
        {
            LightManager.Instance.SetGlobalLightIntensity(LightAreas.Instance.areaLights[0].intensity);
            DirectorScript.Instance.DisableConfiner();
            UIManager.Instance.FadeOut();
            PauseGame(true);

            if (Ocean_AssetSpawner.Instance != null)
            {
                Ocean_AssetSpawner.Instance.ResetAssets();
            }
            if (Ocean_MapHandler.Instance != null)
                Ocean_MapHandler.Instance.ResetMap();

            SceneManagerScript.Instance.LoadScene("Shipwreck");
        }

        //SceneManagerScript.Instance.LoadCheckpoint();
    }

    internal void ResetGame()
    {
        IngameTimer.gameTimer = 0f;
    }
}
