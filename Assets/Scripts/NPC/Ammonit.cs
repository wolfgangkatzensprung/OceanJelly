using UnityEngine;

public class Ammonit : MonoBehaviour
{
    Animator anim;
    public GameObject schutthaufen;
    EntityHealth eh;

    private void Start()
    {
        anim = GetComponent<Animator>();
        eh = schutthaufen.GetComponent<EntityHealth>();
        eh.onDeath += TriggerHappy;
    }

    void TriggerHappy()
    {
        anim.SetBool("isHappy", true);
    }

    private void OnDestroy()
    {
        eh.onDeath -= TriggerHappy;
    }
}
