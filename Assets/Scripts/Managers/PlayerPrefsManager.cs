using UnityEngine;

public class PlayerPrefsManager : MonoBehaviour
{
    public static PlayerPrefsManager _instance;
    public static PlayerPrefsManager Instance { get { return _instance; } }

    public Vector2 defaultCheckpoint = new Vector2(-0, 0);

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    private void Start()
    {
        if (!PlayerPrefs.HasKey("GameStarted"))  // game has been started at least 1 time
        {
            NewGame();
        }
    }

    public void NewGame()   // sets player prefs for the first time game start
    {
        float musicVolume = 1f;
        float soundVolume = 1f;

        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            musicVolume = PlayerPrefs.GetFloat("MusicVolume");
        }
        else
        {
            PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        }

        if (PlayerPrefs.HasKey("SoundVolume"))
        {
            soundVolume = PlayerPrefs.GetFloat("SoundVolume");
        }
        else
        {
            PlayerPrefs.SetFloat("SoundVolume", soundVolume);
        }

        SetNewGamePrefs(musicVolume, soundVolume);
    }

    private void SetNewGamePrefs(float musicVolume, float soundVolume)
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("All Player Prefs have been deleted!");

        PlayerPrefs.SetInt("GameStarted", 1);

        InitializeSoundPrefs(musicVolume, soundVolume);

        InitializePlotPrefs();

        InitializeSettingsPrefs();

        Debug.Log("NewGame Prefs have been set.");
    }

    #region InitializePrefs

    private void InitializePlotPrefs()
    {
        // 0 = untouched
        // 1 = triggered

        PlayerPrefs.SetInt("FishEggCollected", 0);
        PlayerPrefs.SetInt("DungeonCompleted", 0);
    }

    private static void InitializeHealthPrefs()
    {
        // Health
        PlayerPrefs.SetInt("MaxHp", 20);
    }

    private static void InitializeDamagePrefs()
    {
        // Damage
        PlayerPrefs.SetFloat("dmgRaw", 1);
        PlayerPrefs.SetFloat("dmgFactor", 1);
        PlayerPrefs.SetFloat("dmgBonus", 0);
    }

    private static void InitializeItemPrefs()
    {
        // Item Defaults
        PlayerPrefs.SetInt("MaxItemCount", 3);  // default wird 1, ist nur zum testen grad auf 3
        PlayerPrefs.SetString("ItemSlot1", "Empty");
        PlayerPrefs.SetString("ItemSlot2", "Empty");
        PlayerPrefs.SetString("ItemSlot3", "Empty");
        PlayerPrefs.SetString("heldItem", "None");
    }

    private void InitializePositionPrefs()
    {
        PlayerPrefs.SetFloat("startCheckpointX", defaultCheckpoint.x);
        PlayerPrefs.SetFloat("startCheckpointY", defaultCheckpoint.y);
        PlayerPrefs.SetFloat("checkpointX", defaultCheckpoint.x);
        PlayerPrefs.SetFloat("checkpointY", defaultCheckpoint.y);
        PlayerPrefs.SetString("checkpointScene", "PlateauStart");
    }

    private static void InitializeSoundPrefs(float musicVolume, float soundVolume)
    {
        PlayerPrefs.SetInt("Music", 1);
        PlayerPrefs.SetInt("Sound", 1);
        PlayerPrefs.SetInt("Screenshake", 1);

        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SoundVolume", soundVolume);
    }

    private void InitializeSettingsPrefs()
    {
        PlayerPrefs.SetInt("Joystick", 1);
    }

    #endregion

    #region Checks
    public void CheckPlayerPrefsDebug()
    {
        Debug.Log("WallJump: " + PlayerPrefs.GetInt("WallJump"));
    }

    //public bool CheckPlotPref(string plotPref)
    //{
    //    if (PlayerPrefs.GetInt(plotPref) == 1)
    //    {
    //        return true;
    //    }
    //    else return false;
    //}
    #endregion
}
