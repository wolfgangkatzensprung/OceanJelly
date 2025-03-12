using UnityEngine;

public class Leuchtbluemchen : MonoBehaviour
{
    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        anim.SetFloat("distanceToPlayer", PlayerManager.Instance.GetDistanceToPlayer(transform.position));
    }
}
