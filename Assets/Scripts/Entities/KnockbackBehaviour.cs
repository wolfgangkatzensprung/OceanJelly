using UnityEngine;

public class KnockbackBehaviour : StateMachineBehaviour
{
    Transform playerTransform;
    Rigidbody2D rb;

    [SerializeField] float knockbackTime;
    Vector2 kdTargetLocation;
    public float knockbackForce;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerTransform = PlayerManager.Instance.GetPlayerTransform();
        kdTargetLocation = animator.transform.position - playerTransform.position;
        kdTargetLocation.Normalize();

        rb = animator.gameObject.GetComponent<Rigidbody2D>();
        rb.AddForce(kdTargetLocation * knockbackForce, ForceMode2D.Impulse);
        Debug.Log("force added");
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
