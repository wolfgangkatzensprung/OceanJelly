using UnityEngine;

public class Stein : MonoBehaviour
{
    public float stoneDmg = 1f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Entity"))
        {
            collision.gameObject.GetComponent<EntityHealth>().ApplyDamage(stoneDmg);
        }
    }
}
