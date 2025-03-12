using UnityEngine;

public class GrassMovementTrigger : MonoBehaviour
{
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Entity"))
        {
            anim.SetTrigger("moveGrass");
        }
    }
}
