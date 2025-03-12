using UnityEngine;

public class ZitterOnTrigger : MonoBehaviour
{
    Animator anim;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            DoZitter();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            ResetZitter();
        }
    }
    void DoZitter()
    {
        anim.SetBool("Zitter", true);
    }
    void ResetZitter()
    {
        anim.SetBool("Zitter", false);
    }
}
