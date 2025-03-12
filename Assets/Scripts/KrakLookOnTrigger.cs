using UnityEngine;

public class KrakLookOnTrigger : MonoBehaviour
{
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        anim.SetBool("isLooking", true);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        anim.SetBool("isLooking", false);
    }
}
