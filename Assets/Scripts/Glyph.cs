using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Glyph : MonoBehaviour
{
    public GlyphPuzzle.GlyphColor glyphColor;
    Collider2D col;
    SpriteRenderer sr;
    Rigidbody2D rb;
    int startSortingOrder;
    bool unplacing;
    [Tooltip("Push Strength wie stark Glyphe vom Sockel runtergepusht wird, wenn sie nicht passt")]
    public float unplacingStrength = 15f;
    GlyphSlot placedOnSlot;

    private void Start()
    {
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        startSortingOrder = sr.sortingOrder;
    }

    internal void PlaceOnSlot(GlyphSlot glyphSlot)
    {
        col.enabled = false;
        sr.sortingOrder = -13;
        sr.color = Color.grey;
        rb.velocity = Vector2.zero;
        transform.position = glyphSlot.transform.position;
        placedOnSlot = glyphSlot;
    }
    internal void UnplaceFromSlot()
    {
        if (!unplacing)
        {
            StartCoroutine(UnplaceRoutine());
        }
    }

    IEnumerator UnplaceRoutine()
    {
        Debug.Log($"Unplace {glyphColor} Glyph from Slot {placedOnSlot.slotColor}");
        unplacing = true;

        placedOnSlot.ResetDefaultSprite();
        rb.AddForce(GetRandomDirection().normalized * unplacingStrength, ForceMode2D.Impulse);
        yield return new WaitForSeconds(1.5f);

        col.enabled = true;
        sr.color = Color.white;
        sr.sortingOrder = startSortingOrder;
        placedOnSlot = null;
        unplacing = false;
    }

    private Vector2 GetRandomDirection()
    {
        float angle = Random.Range(0f, 360f);
        return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
    }
}
