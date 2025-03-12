using UnityEngine;

public class SetRandomColorStart : MonoBehaviour
{
    [Header("Color Spectrum")]
    public Color startColor = Color.white;
    [Tooltip("50% Wahrscheinlichkeit diese Color")]
    public Color endColor = Color.blue;
    [Tooltip("50% Wahrscheinlichkeit diese Color")]
    public Color endColorVariation = Color.cyan;

    public SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (Random.value > .5f)
            sr.color = Color.Lerp(startColor, endColorVariation, Random.Range(0f, 1f));
        else
            sr.color = Color.Lerp(startColor, endColor, Random.Range(0f, 1f));
    }
}
