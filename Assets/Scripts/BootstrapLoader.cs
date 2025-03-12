using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BootstrapLoader : MonoBehaviour
{
    [Tooltip("Loading Screen Image")]
    public Image img;

    public TextMeshProUGUI loadingText;

    float progress = 0f;
    private bool isTapped;

    [RuntimeInitializeOnLoadMethod]
    private void Start()
    {
        StartCoroutine(StartAsyncLoad());
    }
    void Update()
    {
        // Press the space key to start coroutine
        if (Input.touchCount > 0 || Input.GetKeyUp(KeyCode.Space))
        {
            isTapped = true;
        }
    }

    IEnumerator StartAsyncLoad()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Main");

        asyncLoad.allowSceneActivation = false;

        while (progress < 1f)
        {
            progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);

            img.fillAmount = progress;

            Debug.Log("Loading Progress: " + progress.ToString());

            loadingText.text = "Loading... " + (int)(progress * 100f) + "%";

            yield return null;
        }

        loadingText.text = "Tap to start.";

        while (!isTapped)
        {
            yield return null;
        }

        asyncLoad.allowSceneActivation = true;
    }
}
