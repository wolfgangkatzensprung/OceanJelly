using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GlyphSequenceLight : MonoBehaviour
{
    public GlyphPuzzle.GlyphColor glyphColor;
    Light2D light;

    private void Start()
    {
        light = GetComponent<Light2D>();

        Disable();
    }

    internal void Enable()
    {
        light.enabled = true;
    }
    internal void Disable()
    {
        light.enabled = false;
    }
}
