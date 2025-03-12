using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class PortalTransition : MonoBehaviour
{
    Image img;
    float alpha = 0f;

    private void OnEnable()
    {
        img = GetComponent<Image>();
    }

    private void Update()
    {
        if (!SceneManagerScript.Instance.loadingFinished)
            StartCoroutine(PortalTransitionRoutine());

    }

    IEnumerator PortalTransitionRoutine()
    {
        alpha += Time.unscaledDeltaTime;
        img.color = new Color(img.color.r, img.color.g, img.color.b, alpha);
        yield return new WaitForSeconds(Time.unscaledDeltaTime);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
