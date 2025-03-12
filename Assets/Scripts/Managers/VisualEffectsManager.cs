using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VisualEffectsManager : MonoBehaviour
{
    public static VisualEffectsManager _instance;
    public static VisualEffectsManager Instance { get { return _instance; } }

    [Header("References")]

    public VolumeProfile volume;
    public GameObject portalTransitionPanel;
    public ParticleSystem tapParticles;

    ColorAdjustments colorAdjustments = null;
    ChromaticAberration chromaticAberration = null;

    float startChroma;

    Color startColorFilter = Color.white;
    [Tooltip("Damaged Color for Color Filter Lerp")]
    public Color damagedColor = Color.red;

    [Tooltip("Post Exposure Value for initializing (overrides Post Exposure value of Volume")]
    public float startPostExposure = .5f;

    float damagedTimer = 0f;    // descending timer


    public float fadeDuration = 1f;
    public float startVignetteIntensity = 0f;
    public float targetVignetteIntensity = 1f;

    private Vignette vignette;
    private bool fading;

    [Tooltip("How long to wait until fading in after scene change")]
    public float blackscreenDelay = 1f;

    private bool portalActive;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

#if UNITY_EDITOR
    private void OnDisable()
    {
        ResetVisualEffects();
    }
#endif

    private void Start()
    {
        volume.TryGet(out colorAdjustments);
        volume.TryGet(out chromaticAberration);
        volume.TryGet(out vignette);

        startColorFilter = colorAdjustments.colorFilter.value;
        startChroma = chromaticAberration.intensity.value;
        startVignetteIntensity = vignette.intensity.value;

        PlayerHealth.Instance.onDeath += ResetPostProcessingEffects;
        PlayerInputManager.Instance.onTap += PlayTapParticles;
    }
    private void Update()
    {
        PlayDamagedVisuals();
    }

    private void PlayTapParticles(Vector2 touchPos)
    {
        tapParticles.transform.position = PlayerInputManager.Instance.TouchPositionWorld();
        tapParticles.Play();
    }

    private void ResetPostProcessingEffects()
    {
        damagedTimer = 0f;
        ResetVisualEffects();
    }


    private void PlayDamagedVisuals()   // fade from red to white
    {
        if (damagedTimer > 0f)
        {
            damagedTimer -= Time.unscaledDeltaTime;
            if (damagedTimer <= 0)
            {
                damagedTimer = 0f;
                ResetVisualEffects();
                return;
            }

            Color currentColor = Color.Lerp(startColorFilter, damagedColor, damagedTimer);
            float currentChroma = Mathf.Lerp(startChroma, 1f, damagedTimer);
            colorAdjustments.colorFilter.value = (Vector4)currentColor;
            chromaticAberration.intensity.value = currentChroma;
            //Debug.Log($"CurrentColor: {currentColor}");
        }
    }

    public void StartDamagedVFX(float dmgTime)
    {
        Debug.Log("Autsch! Dmg Visuals werden abgespielt");
        damagedTimer = dmgTime;
    }

    /// <summary>
    /// Restore Defaults of affected Post Processing values
    /// </summary>
    public void ResetVisualEffects()
    {
        if (colorAdjustments != null)
        {
            colorAdjustments.colorFilter.value = startColorFilter;
            chromaticAberration.intensity.value = startChroma;
        }
        if (vignette != null)
        {
            vignette.intensity.value = startVignetteIntensity;
        }
    }


    public void FadeOut()
    {
        if (fading)
            return;

        fading = true;
        StartCoroutine(FadeEffectRoutine(startVignetteIntensity, targetVignetteIntensity));
    }

    public void FadeIn()
    {
        if (fading)
            return;

        fading = true;
        StartCoroutine(FadeEffectRoutine(targetVignetteIntensity, startVignetteIntensity));
    }

    private IEnumerator FadeEffectRoutine(float startValue, float targetValue)  // Vignette Fade
    {
        float lerpTimer = 0f;

        while (lerpTimer < fadeDuration)
        {
            float t = lerpTimer / fadeDuration;
            float intensity = Mathf.Lerp(startValue, targetValue, t);
            vignette.intensity.Override(intensity);

            lerpTimer += Time.unscaledDeltaTime;

            yield return null;
        }

        vignette.intensity.Override(targetValue);
        fading = false;
    }

    //public void FadeOut()
    //{
    //    if (fadingIn)
    //        return;

    //    if (!fadingOut)
    //    {
    //        StartCoroutine(FadeOutRoutine());
    //    }
    //}
    //public void FadeIn()
    //{
    //    if (fadingOut)
    //        return;

    //    if (!fadingIn)
    //    {
    //        StartCoroutine(FadeInRoutine());
    //    }
    //}

    //IEnumerator FadeOutRoutine()
    //{
    //    fadingOut = true;
    //    yield return new WaitForSecondsRealtime(blackscreenDelay);

    //    float lerpTimer = 0;
    //    float maxLerpTime = 1f;

    //    while (fadingOut)
    //    {
    //            float t = lerpTimer / maxLerpTime;
    //            t = 1f - Mathf.Cos(t * Mathf.PI * 0.5f);
    //            lerpTimer += Time.unscaledDeltaTime;
    //            if (lerpTimer > maxLerpTime)
    //            {
    //                lerpTimer = maxLerpTime;
    //            }
    //            Debug.Log($"FadingOut. Lerping with t = {t}");

    //            colorAdjustments.postExposure.value = Mathf.Lerp(startPostExposure, .10f, t);
    //            yield return new WaitForSeconds(Time.unscaledDeltaTime);
    //            if (lerpTimer >= maxLerpTime)
    //            {
    //                colorAdjustments.postExposure.value = startPostExposure;
    //                fadingOut = false;
    //                Debug.Log("FadingOut finished");
    //            }
    //    }
    //}

    //IEnumerator FadeInRoutine()
    //{
    //    fadingIn = true;
    //    yield return new WaitForSecondsRealtime(blackscreenDelay);

    //    float lerpTimer = 0;
    //    float maxLerpTime = 1f;

    //    while (fadingIn)
    //    {
    //        float t = lerpTimer / maxLerpTime;
    //        t = 1f - Mathf.Cos(t * Mathf.PI * 0.5f);
    //        lerpTimer += Time.unscaledDeltaTime;
    //        if (lerpTimer > maxLerpTime)
    //        {
    //            lerpTimer = maxLerpTime;
    //        }
    //        Debug.Log($"FadingIn. Lerping with t = {t}");

    //        colorAdjustments.postExposure.value = Mathf.Lerp(-10f, startPostExposure, t);
    //        yield return new WaitForSeconds(Time.unscaledDeltaTime);
    //        if (colorAdjustments.postExposure.value > 0 - t)
    //        {
    //            colorAdjustments.postExposure.value = 0;
    //            fadingIn = false;
    //            Debug.Log("FadingIn finished");
    //        }
    //    }
    //}

    //public void TryStartPortalVisuals()
    //{
    //    if (!portalActive)
    //    {
    //        StartPortalVisuals();
    //    }
    //}

    //void StartPortalVisuals()
    //{
    //    portalActive = true;
    //    portalTransitionPanel.SetActive(true);
    //}

    //public void StopPortalVisuals()
    //{
    //    portalTransitionPanel.SetActive(false);
    //    portalActive = false;
    //}
}