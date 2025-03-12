using UnityEngine;

public class FischNPC : MonoBehaviour
{
    public Animator anim;

    public void TriggerMove()
    {
        anim.SetTrigger("Move");
    }
}
