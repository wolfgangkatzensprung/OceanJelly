﻿using UnityEngine;

public class PatrolEnemyIdle : StateMachineBehaviour
{
    EnemyPatrolMovement enemyMovement;

    float distanceX;
    float distanceY;
    [SerializeField] float aggroDistanceX;
    [SerializeField] float aggroDistanceY;



    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemyMovement = animator.GetComponent<EnemyPatrolMovement>();
        enemyMovement.isPatroling = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        //Aggro
        distanceX = animator.GetComponent<EnemyPatrolMovement>().rayPointHorizontal.transform.position.x - PlayerManager.Instance.playerPosition.x;
        distanceY = animator.transform.position.y - PlayerManager.Instance.playerPosition.y;
        distanceX = Mathf.Sqrt(distanceX * distanceX);
        distanceY = Mathf.Sqrt(distanceY * distanceY);
        if (distanceX < aggroDistanceX)
        {
            if (distanceY < aggroDistanceY)
            {
                Debug.Log("triggered by aggro distance");
                if (!animator.GetBool("isAttacking"))
                    animator.SetBool("isAttacking", true);
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemyMovement.isPatroling = false;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
