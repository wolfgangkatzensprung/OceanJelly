using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    public static SceneManagerScript _instance;
    public static SceneManagerScript Instance { get { return _instance; } }

    public GameObject player;

    AsyncOperation async;

    static bool loading; // for delayed loading routine

    public bool inMenu { get; private set; }
    public bool loadingFinished { get; private set; }

    string sceneToLoad = "PlateauStart";    // Scene die geladen werden soll. Default ist Plateau. Darf nicht Master sein

    private string[] stages;

    public enum SceneType
    {
        Ocean,
        Dungeon,
        IceCave
    }
    void OnDisable()
    {
        //Debug.Log("OnDisable");
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Awake()
    {
        if (_instance == null) _instance = this;
    }

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");


        SceneManager.LoadScene("Shipwreck", LoadSceneMode.Additive);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name + " SceneMode: " + mode);

        switch (scene.name)
        {
            case "Menu":
                Debug.Log("OnSceneLoaded Menu case");
                InitializeMenuScene();
                break;
            case "SettingsMenu":
                Debug.Log("OnSceneLoaded SettingsMenu case");
                InitializeSettingsMenuScene();
                break;
            case "Master":
                Debug.Log("OnSceneLoaded Master case");
                break;
            default:
                Debug.Log("OnSceneLoaded default case");
                scene = InitializeDefaultScene(scene);
                break;
        }

        StartCoroutine(InitializePlayer(scene));
    }


    // Default Scene is initialized when traveling to any not-menu scene
    private Scene InitializeDefaultScene(Scene scene)
    {
        inMenu = false;
        GameController.Instance.SetInMenu(false);
        ShowAreaText(scene.name);
        CheckTheft();
        return scene;
    }

    private static void ShowAreaText(string sceneName)
    {
        UIManager.Instance.ShowAreaText(sceneName);
    }

    private void CheckTheft()   // ob Player was ausm Shop geklaut hat
    {
        GameObject item = ItemCollector.Instance.GetHeldItem();
        if (item != null)
        {
            if (item.TryGetComponent<ShopItem>(out ShopItem sItem))
            {
                Debug.Log("Haltet den Dieb!");
            }
        }
    }

    private void InitializeSettingsMenuScene()
    {
        inMenu = true;
        GameController.Instance.SetInMenu(true);
        CutsceneHandler.Instance.EndCutscene();
    }

    private void InitializeMenuScene()
    {
        inMenu = true;
        GameController.Instance.SetInMenu(true);
        MenuCommander.Instance.StartMenuParticles();
    }

    IEnumerator InitializePlayer(Scene curScene)
    {
        yield return new WaitForEndOfFrame();

        if (player == null)
            player = PlayerManager.Instance.player;

        if (inMenu)
        {
            player.SetActive(false);
        }
        else if (!player.activeSelf)
        {
            player.SetActive(true);
        }
    }

    public string GetActiveSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    public void LoadScene(string sceneName)
    {
        if (loading)
            return;

        float delay = 1.5f;
        Debug.Log($"Load Scene {sceneName} after {delay} seconds delay");
        StartCoroutine(DelayedLoadScene(sceneName, delay));
    }

    private IEnumerator DelayedLoadScene(string sceneName, float delay)
    {
        Debug.Log($"DelayedLoadScene Coroutine started");

        loading = true;

        yield return new WaitForSecondsRealtime(delay);    // Blackscreen FadeOut Time
        Debug.Log($"Coroutine: Load Scene {sceneName} after {delay} seconds delay");
        loading = false;
        try
        {
            SceneManager.LoadScene(sceneName);
        }
        catch (Exception e)
        {
            Debug.LogError("Scene loading error: " + e.Message);
        }
    }

    public void LoadSceneAsync(string sceneName)
    {
        StartCoroutine(LoadAsyncScene(sceneName));
    }
    public void LoadSceneAsync(string sceneName, Vector2 targetPosition)
    {
        StartCoroutine(LoadAsyncScene(sceneName, targetPosition));
    }

    IEnumerator LoadAsyncScene(string sceneName)
    {
        Debug.Log("Start Loading Async Menu scene");
        loadingFinished = false;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            // lade anzeige
            yield return null;
        }
        yield return new WaitForEndOfFrame();
        Debug.Log("Loading Async finished");
        loadingFinished = true;
    }

    IEnumerator LoadAsyncScene(string sceneName, Vector2 targetPosition)
    {
        Debug.Log("Start Loading Async Ingame scene");
        loadingFinished = false;

        DirectorScript.Instance.DisableBrain();
        DirectorScript.Instance.DisableConfiner();

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        player.transform.position = targetPosition;
        //DirectorScript.Instance.InitializeCameraPositions(targetPosition);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            // lade anzeige
            yield return null;
        }
        yield return new WaitForEndOfFrame();
        Debug.Log("Loading Async finished");

        player.transform.position = targetPosition;
        //DirectorScript.Instance.InitializeCameraPositions(targetPosition);

        loadingFinished = true;

        DirectorScript.Instance.EnableBrain();
        DirectorScript.Instance.EnableConfiner();
    }

    // old way
    public void UsePortal(string targetLocation, Vector2 targetPosition)
    {
        if (player == null)
        {
            player = PlayerManager.Instance.GetPlayer();
        }
        player.SetActive(true);
        player.transform.position = targetPosition;
        player.SetActive(false);

        this.LoadSceneAsync(targetLocation, targetPosition);
    }

    internal void EnterDungeon(SceneType dungeonType)
    {
        DirectorScript.Instance.DisableConfiner();
        UIManager.Instance.FadeOut();
        GameController.Instance.PauseGame(true);

        if (Ocean_AssetSpawner.Instance != null)
        {
            Ocean_AssetSpawner.Instance.ResetAssets();
        }
        if (Ocean_MapHandler.Instance != null)
        {
            Ocean_MapHandler.Instance.ResetMap();
        }

        GameController.Instance.inDungeon = true;

        if (dungeonType.Equals(SceneType.Ocean))
        {
            LoadScene("Ocean");
        }
        else if (dungeonType.Equals(SceneType.IceCave))
        {
            LoadScene("IceCave");
        }
        else if (dungeonType.Equals(SceneType.Dungeon))
        {
            LoadScene("Dungeon");
        }
    }
    internal void ExitDungeon()
    {
        DirectorScript.Instance.DisableConfiner();
        UIManager.Instance.FadeOut();
        GameController.Instance.PauseGame(true);

        if (Ocean_AssetSpawner.Instance != null)
            Ocean_AssetSpawner.Instance.ResetAssets();
        if (Ocean_MapHandler.Instance != null)
            Ocean_MapHandler.Instance.ResetMap();
        GameController.Instance.inDungeon = false;
        LoadScene("Shipwreck");
    }

    internal void UnloadGame()
    {
        StartCoroutine(UnloadGameCoroutine());
    }

    private IEnumerator UnloadGameCoroutine()
    {
        string sceneName = GetActiveSceneName();

        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(sceneName);
        while (!asyncUnload.isDone)
        {
            yield return null;
        }

        GameController.Instance.ResetGame();

        yield break;
    }


    public bool IsSceneActive(string sceneName)
    {
        StartCoroutine(CheckScenes());

        bool returnBool = false;
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name == sceneName)
            {
                Debug.Log("sceneName: " + sceneName);
                returnBool = true;
            }
        }
        return returnBool;
    }

    IEnumerator CheckScenes()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.042f);
    }

    private IEnumerator UnloadDungeonAsync()
    {
        yield return new WaitForEndOfFrame();
        UIManager.Instance.loadingScreen.SetActive(true);
        Scene dungeonScene = SceneManager.GetSceneByName("Dungeon");
        Debug.Log($"Dungeon Scene: {dungeonScene}");
        AsyncOperation async = SceneManager.UnloadSceneAsync(dungeonScene);

        if (async != null)
        {

            while (!async.isDone)
            {
                Debug.Log("Unloading Dungeon Scene");
                float progress = Mathf.Clamp01(async.progress / 0.9f);
                UIManager.Instance.loadingScreenBar.fillAmount = progress;
                yield return null;
            }
        }
        Debug.Log("Dungeon unloaded");
    }

    // Checkpoint
    public void LoadCheckpoint()
    {
        // skip da im moment ausser main keine scenes geladen werden, Stand 220119
        return;

        SceneManager.LoadScene(PlayerPrefs.GetString("checkpointScene"));
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        player.transform.position = new Vector2(PlayerPrefs.GetFloat("checkpointX"), PlayerPrefs.GetFloat("checkpointY"));
    }

    public void SetCheckpoint()
    {
        // ich glaub diese funktion ist useless weil hier DontDestroyOnLoad als scene erkannt wird
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name.Contains("DontDestroyOnLoad"))
            {
                continue;
            }
            else return;
        }
        Debug.Log("Checkpoint set to " + gameObject.scene.name + transform.position.x + transform.position.y);
        PlayerPrefs.SetString("checkpointScene", gameObject.scene.name);
        PlayerPrefs.SetFloat("checkpointX", transform.position.x);
        PlayerPrefs.SetFloat("checkpointY", transform.position.y);
    }

    // Return to Main Menu

    public void SetMainMenu()
    {
        SceneManager.LoadScene("Menu");
        TimeManager.Instance.ResetTimeScale();
    }
}
