using System.IO;
using TMPro;
using UnityEngine;

public class DebugConsole : MonoBehaviour
{
    public DebugPrefabs dPrefabs;
    public TMP_InputField inputField;
    public TextMeshProUGUI outputText;

    Transform playerTrans;

    static FileInfo[] fileInfo;
    static string scenesDataPath;

    private void Start()
    {
        scenesDataPath = Application.dataPath + "/Scenes/";
        CheckExistingScenes();
        playerTrans = PlayerManager.Instance.GetPlayerTransform();
    }

    public void DoCommand(string input)
    {
        string[] splitInput = input.Split(' ');
        string baseCommand = splitInput[0];
        string suffixString = splitInput[splitInput.Length - 1];

        Debug.Log("input:" + input + "baseCommand: " + baseCommand + ". suffix: " + suffixString);

        switch (baseCommand)
        {
            default:
                break;

            case "help":
                SetOutputText("Commands:\nhelp : Show help info\nspawn prefabName : Spawn object by name\ntest levelName : Load level by name");
                break;

            case "spawn":
                bool objectExists = false;
                string spawnableObjectsNames = "";
                string[] prefabNames = dPrefabs.GetPrefabNames();
                foreach (string _prefabName in prefabNames)
                {
                    if (suffixString == _prefabName)
                        objectExists = true;
                    spawnableObjectsNames += _prefabName + "\n";
                }
                if (objectExists)
                {
                    SpawnPrefab(suffixString);
                    SetOutputText(suffixString + " spawned.");
                }
                else if (!objectExists)
                    SetOutputText("Prefab name not found. Spawnable prefabs:\n" + spawnableObjectsNames);
                break;

            case "test":
                bool sceneExists = false;
                string availableSceneNames = "";

                for (int i = 0; i < fileInfo.Length; i++)
                {
                    string fileName = fileInfo[i].Name.Split(new string[] { "." }, System.StringSplitOptions.RemoveEmptyEntries)[0].ToString();
                    availableSceneNames += fileName + "\n";

                    if (suffixString == "Master" || suffixString == "DEFAULT")
                        continue;

                    if (suffixString == fileName)
                        sceneExists = true;

                }
                if (sceneExists)
                {
                    SetOutputText("Loading " + suffixString + "...");
                    SceneManagerScript.Instance.LoadSceneAsync(suffixString);
                }
                else if (!sceneExists)
                    SetOutputText("Level name not found. Available levels:\n" + availableSceneNames);
                break;
        }

    }

    private void SetOutputText(string outputText)
    {
        this.outputText.text = outputText;
    }

    private void SpawnPrefab(string pName)
    {
        float offsetX = 5 * PlayerManager.Instance.GetFacingDirection();
        Vector3 spawnPosition = playerTrans.position + new Vector3(offsetX, 0, 0);

        foreach (DebugPrefabs.PrefabAsset prefab in dPrefabs.GetPrefabs())
        {
            if (prefab.GetName() == pName)
            {
                GameObject go = prefab.GetPrefab();
                if (go != null)
                    Instantiate(go, spawnPosition, Quaternion.identity);
            }
        }
    }

    static void CheckExistingScenes()
    {
        fileInfo = (new DirectoryInfo(scenesDataPath)).GetFiles("*.unity", SearchOption.AllDirectories);
    }
}
