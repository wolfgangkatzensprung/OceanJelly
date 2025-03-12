using UnityEngine;
using UnityEngine.UI;

public class LevelProgressBar : MonoBehaviour
{
    Image image;
    public float fadeSpeed = 0.5f; // Adjust the speed as needed

    private void Start()
    {
        image = GetComponent<Image>();

    }

    void Update()
    {
        float targetFill = Mathf.Lerp(.23f, .77f, PlayerAether.Instance.GetLevelProgressRatio());

        // Gradually update the fill amount towards the target value using Mathf.Lerp
        image.fillAmount = Mathf.Lerp(image.fillAmount, targetFill, fadeSpeed * Time.deltaTime);
    }

}
