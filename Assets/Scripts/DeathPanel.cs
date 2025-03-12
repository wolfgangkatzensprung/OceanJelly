using UnityEngine;

public class DeathPanel : MonoBehaviour
{
    public Animator anim;

    private void OnEnable()
    {
        anim.Play("FadeIn");
    }
}
