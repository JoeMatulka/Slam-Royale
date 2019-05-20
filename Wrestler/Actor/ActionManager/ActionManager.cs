using System;
using UnityEngine;

[RequireComponent(typeof(Actor))]
public abstract class ActionManager : MonoBehaviour
{
    public WrestlerAttackManager atkManager;
    public SensorManager sensorManager;

    public GameCamera gameCamera;

    protected static string ATTACK_TYPE_ANIM_KEY = "atk_Type";
    protected static string GRABBING_BOOL_ANIM_KEY = "grabbing";

    private static string GRAB_TRANSFORM_ID = "grab transform";

    private Actor m_actor;

    // Used for recovering
    private ActorState m_previousRecoveryState;

    // Actor that is grabbing this actor
    private Actor m_grabber;
    private ActionManager m_grabberActionManager;

    // Actor that this actor is grabbing
    private Actor m_grabbee;

    private Attack m_curAttack;
    private GestureEventArgs m_queuedGesture;

    private bool m_outOfRing;

    private bool m_disabled;

    public abstract void OnDamage();

    public abstract void ActivateAttackFrame(int frame);

    public abstract void ExecuteQueuedActions();

    protected void EndOfRecoveryFrame()
    {
        CurrentAttack = null;
        Actor.Invulnerable = false;
        if (Actor.State.Equals(ActorState.GRABBED) || Actor.State.Equals(ActorState.GRABBING))
        {
            return;
        }
        if (PreviousRecoveryState.Equals(ActorState.DAMAGED) &&
            !Actor.Grounded)
        {
            // Addresses issue with thrown actors, will initially start out on the ground
            Actor.UpdateState(ActorState.STUNNED);
            return;
        }
        if (PreviousRecoveryState.Equals(ActorState.STUNNED)) {
            Actor.UpdateState(ActorState.STUNNED);
            return;
        }
        Actor.UpdateState(ActorState.DEFAULT);
        ExecuteQueuedActions();
    }

    protected void UpdateActionAnimations()
    {
        if (CurrentAttack != null)
        {
            Actor.Animator.SetInteger(ATTACK_TYPE_ANIM_KEY, (int)CurrentAttack.AttackType);
        }
        if (Grabber != null)
        {
            Actor.Animator.SetFloat(Actor.X_VEL_ANIM_KEY, Mathf.Abs(Grabber.Rigidbody.velocity.x));
            Actor.Animator.SetFloat(Actor.X_INPUT_ANIM_KEY, Grabber.XInput);
            Actor.Animator.SetBool(Actor.GROUNDED_ANIM_KEY, Grabber.Grounded);
            if (m_grabberActionManager != null && m_grabberActionManager.CurrentAttack != null)
            {
                Actor.Animator.SetInteger(ATTACK_TYPE_ANIM_KEY,
                    (int)m_grabberActionManager.CurrentAttack.AttackType);
            }
        }
    }

    protected bool CheckCanAttack()
    {
        ActorState state = Actor.State;
        return state.Equals(ActorState.DEFAULT) || state.Equals(ActorState.GRABBING);
    }

    public void Throw()
    {
        if (CurrentAttack != null)
        {
            float aimX = CurrentAttack.Aim.x;
            float aimY = CurrentAttack.Aim.y;

            Actor grabbee = Grabbee;
            Damage dmg = new Damage(WrestlerAttackManager.THROW_DMG,
                new Vector2(
                     Math.Abs(aimX) * WrestlerAttackManager.BASE_THROW_X_FORCE,
                     aimY * WrestlerAttackManager.BASE_THROW_Y_FORCE
                ));
            // This is so the thrower doesn't affect the actor being thrown
            StartCoroutine(grabbee.IgnoreCollision(Actor.Collider));
            BreakGrab(false, false);
            // Hard set this here because Actor state is too slow, needs looking into...
            grabbee.Rigidbody.isKinematic = false;
            grabbee.Grounded = false;
            grabbee.Damage(AttackType.HIT, dmg, Actor, Vector3.zero);
        }
    }

    public void StrikeThrow()
    {
        Actor grabbee = Grabbee;

        // This is so the thrower doesn't affect the actor being thrown
        StartCoroutine(grabbee.IgnoreCollision(Actor.Collider));
        SoundManager.Instance.BigThud(Actor.AudioSource);
        BreakGrab(false, false);

        // Hard set this here because Actor state is too slow, needs looking into...
        grabbee.Rigidbody.isKinematic = false;
        grabbee.Grounded = false;
        grabbee.Damage(AttackType.HIT, CurrentAttack.Damage, Actor, 
            new Vector2(
                    grabbee.transform.position.x,
                    grabbee.transform.position.y - 3
                ));
        SpawnSplashDamage(CurrentAttack.Damage, grabbee);

        if (gameCamera != null) {
            gameCamera.Shake();
        }

        // Unset hyper armor here because pile driver is a non-self terminating attack
        Actor.HyperArmor = false;
        Actor.Invulnerable = false;
        Actor.LockGravity = false;
    }

    public void BreakGrab(bool fromGrabbee, bool cancelGrab)
    {
        if (!fromGrabbee && Grabbee != null)
        {
            if (cancelGrab)
            {
                Grabbee.UpdateState(ActorState.RECOVERING);
                Actor.UpdateState(ActorState.RECOVERING);
            }
            else {
                Grabbee.UpdateState(ActorState.DEFAULT);
                Actor.UpdateState(ActorState.DEFAULT);
            }
            ActionManager manager = Grabbee.GetComponent<ActionManager>();
            if (manager != null)
            {
                manager.CurrentAttack = null;
                manager.Grabbee = null;
                manager.Grabber = null;
            }
        }
        else if (fromGrabbee && Grabber)
        {
            ActionManager manager = Grabber.GetComponent<ActionManager>();
            if (manager != null)
            {
                manager.BreakGrab(false, false);
            }
        }
        Grabber = null;
        Grabbee = null;
    }

    private void SpawnSplashDamage(Damage dmg, Actor grabbee) {
        GameObject go = Instantiate(ResourceManager.Instance.SplashDamage,
                grabbee.transform.position,
                Quaternion.identity) as GameObject;
        //Swap x and y forces for splash
        dmg.Force = new Vector2(dmg.Force.y, dmg.Force.x);

        SplashDamage splash = go.GetComponent<SplashDamage>();
        splash.Spawn(dmg, new Actor[] { grabbee, Actor}, Actor);
    }

    public void NotifyGrabbeBeingThrown()
    {
        if (Grabbee != null)
        {
            Grabbee.Animator.Rebind();
            Grabbee.UpdateState(ActorState.BEING_THROWN);
            ActionManager manager = Grabbee.GetComponent<ActionManager>();
            if (manager != null) {
                Attack atk = WrestlerAttackManager.CONTACT;
                atk.Damage = new Damage(WrestlerAttackManager.PUNCH_DMG,
                    WrestlerAttackManager.STRIKE_THROW_FORCE);
                manager.atkManager.ActivateAllHitboxes(Grabbee, atk);
            }
        }
    }

    public Actor Actor
    {
        get { return m_actor; }
        set { m_actor = value; }
    }

    public bool Disabled
    {
        get { return m_disabled; }
        set { m_disabled = value; }
    }

    public bool OutOfRing
    {
        get { return m_outOfRing; }
        set { m_outOfRing = value; }
    }

    public Attack CurrentAttack
    {
        get { return m_curAttack; }
        set { m_curAttack = value; }
    }

    public GestureEventArgs QueuedGesture {
        get { return m_queuedGesture; }
        set { m_queuedGesture = value; }
    }

    public ActorState PreviousRecoveryState
    {
        get { return m_previousRecoveryState; }
        set { m_previousRecoveryState = value; }
    }

    public Actor Grabber
    {
        get { return m_grabber; }
        set
        {
            if (value != null)
            {
                Actor.Animator.Rebind();
                Actor.UpdateState(ActorState.GRABBED);
                Actor.transform.SetParent(value.transform);
                Actor.transform.localPosition = value.transform.Search(GRAB_TRANSFORM_ID).localPosition;
                Actor.transform.LookAt(value.transform);
                Actor.Invulnerable = false;
                Actor.HyperArmor = false;
                Actor.LockGravity = false;
                m_grabberActionManager = value.GetComponent<ActionManager>();
            }
            else {
                Actor.transform.SetParent(null);
                m_grabberActionManager = null;
            }
            m_grabber = value;
        }
    }

    public Actor Grabbee
    {
        get { return m_grabbee; }
        set
        {
            if (value != null)
            {
                Actor.UpdateState(ActorState.GRABBING);
                atkManager.DeactivateHitboxes(Actor);
                Actor.Rigidbody.velocity = Vector3.zero;
                Actor.Invulnerable = false;
                Actor.HyperArmor = false;
                Actor.LockGravity = false;
            }
            m_grabbee = value;
        }
    }
}
