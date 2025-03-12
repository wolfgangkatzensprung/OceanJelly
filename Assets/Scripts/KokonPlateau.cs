using UnityEngine;

public class KokonPlateau : MonoBehaviour
{
    Animator anim;
    public EntityHealth eh;
    bool isIdle = true;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isIdle && eh.GetHealth() < 0)
        {
            isIdle = false;
            anim.Play("Schluepf");
        }
    }
}
