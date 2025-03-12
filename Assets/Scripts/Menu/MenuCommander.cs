using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuCommander : MonoBehaviour
{
    public static MenuCommander _instance;
    public static MenuCommander Instance { get { return _instance; } }

    public GameObject menuParticles;
    Coroutine co;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
    }

    void Start()
    {
        Cursor.visible = true;
        if (SceneManagerScript.Instance != null)
        {
            return;
        }
        if (gameObject.scene.buildIndex != -1)  // keine DontDestroyOnLoad Scene active
        {
            Debug.Log("CheckForMaster");
            co = StartCoroutine(CheckForMaster());
        }
    }

    IEnumerator CheckForMaster()
    {
        yield return new WaitForEndOfFrame();
        if (IsSceneActive("Master") || IsSceneActive("DontDestroyOnLoad"))    // ist beides master, weil die ja umbenannt wird nach verhindertem destroy versuch
        {
            Debug.Log("Master already active. Stopping Coroutine");
            StopCoroutine(co);
        }
        if (gameObject.scene.buildIndex != -1)  // kein DontDestroyOnLoad active
        {
            SceneManager.LoadScene("Master", LoadSceneMode.Additive);
            Debug.Log("MasterScene added");

        }
    }

    // Check if Scene is active

    bool IsSceneActive(string sceneName)
    {
        Debug.Log("sceneCount: " + SceneManager.sceneCount);
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

    internal void StartMenuParticles()
    {
        Vector2 vcamPosition = DirectorScript.Instance.GetActiveVcam().transform.position;
        float startMenuParticlesOffsetX = 30f;
        menuParticles.transform.position = new Vector2(vcamPosition.x - startMenuParticlesOffsetX, vcamPosition.y);
    }
}
