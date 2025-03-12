using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class KnockBack : MonoBehaviour
{
    public Rigidbody2D rb;

    public float minKnockStrength = 10f;
    public float maxKnockStrength = 100f;

    public void Knockback(Vector2 playerPos, float strength)
    {
        strength = Mathf.Min(maxKnockStrength, strength);
        strength = Mathf.Max(minKnockStrength, strength);

        Vector2 direction = ((Vector2)transform.position - playerPos).normalized;
        rb.AddForce(direction * strength, ForceMode2D.Impulse);
        Debug.Log($"Knockback {gameObject.name} mit {strength}");
    }
}
