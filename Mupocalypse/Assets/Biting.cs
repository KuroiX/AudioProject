using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Biting : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerExit = false;
    }

    public float range;

    private static readonly int PlayerExit = Animator.StringToHash("PlayerExit");
    private bool playerExit;

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var position = Player.Instance.transform.position;
        var position1 = animator.gameObject.transform.position;
        float dist = (position - position1).magnitude;
        float dir = position.x - position1.x;
        if (dir < 0)
        {
            animator.gameObject.GetComponent<SpriteRenderer>().flipX = false;
        }
        else
        {
            animator.gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }
        
        
        if (!playerExit &&  dist > range)
        {
            animator.SetTrigger(PlayerExit);
            playerExit = true;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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
