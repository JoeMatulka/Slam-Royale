using UnityEngine;

public class RecoveryAnimation : StateMachineBehaviour {
    public bool m_IsInvulnerable;

    private ActionManager m_actionManager;

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        m_actionManager = animator.GetComponent<ActionManager>();
        m_actionManager.PreviousRecoveryState = m_actionManager.Actor.State;
        m_actionManager.Actor.UpdateState(ActorState.RECOVERING);
        m_actionManager.CurrentAttack = null;
        m_actionManager.Actor.Invulnerable = m_IsInvulnerable;
    }
}
