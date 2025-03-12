using UnityEngine;

public class RollingFelsTrigger : MonoBehaviour
{
    public GameObject rollingFels;
    Animator anim;

    [SerializeField] bool setTrue = true;

    private void Start()
    {
        anim = rollingFels.GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            anim.SetBool("isTriggered", setTrue);
        }
    }
}
