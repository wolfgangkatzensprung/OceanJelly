using UnityEngine;

public class HealBurst : MonoBehaviour
{
    CircleCollider2D col;
    CircleCollider2D[] collidersInRange;
    float radius = 30;
    ContactFilter2D cf;

    float timer;

    private void Start()
    {
        timer = 0;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer > 1)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Green"))
        {

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
            int i = 0;
            while (i < hitColliders.Length)
            {
                hitColliders[i].SendMessage("Heal", 10f);   //TODO: refactor to not use SendMessage
                i++;
            }
        }

    }


}
