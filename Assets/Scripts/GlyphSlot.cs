using UnityEngine;

public class GlyphSlot : MonoBehaviour
{
    SpriteRenderer sr;

    [Tooltip("Sprite fuer wenn Glyph placed wurde (=> Sockel ohne Eis)")]
    public Sprite endSprite;
    Sprite startSprite;
    public GlyphPuzzle.GlyphColor slotColor; // Color of the slot

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        startSprite = sr.sprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable"))
        {
            Debug.Log($"GlyphSlot Collision with Interactable {collision.name}");
            if (collision.TryGetComponent(out Glyph glyph))
            {
                Debug.Log($"GlyphSlot Collision with Glyph {collision.name}");
                PlaceGlyph(glyph);
            }
        }
    }

    public void PlaceGlyph(Glyph glyph)
    {
        Debug.Log($"Place {glyph.glyphColor} Glyph on {slotColor} GlyphSlot");
        glyph.PlaceOnSlot(this);
        if (glyph.glyphColor == slotColor)
        {
            Debug.Log($"Correct Glyph Color {slotColor}");
            GlyphPuzzle.Instance.AddGlyph(glyph);
            sr.sprite = endSprite;
        }
        else
        {
            Debug.Log($"Incorrect Glyph Color {glyph.glyphColor} on {slotColor} GlyphSlot");
            glyph.UnplaceFromSlot();
            ResetDefaultSprite();

            if (GlyphPuzzle.Instance.sequenceIndex > 0)
            {
                GlyphPuzzle.Instance.ResetGlyphs();
            }
        }
    }

    internal void ResetDefaultSprite()
    {
        sr.sprite = startSprite;
    }
}
