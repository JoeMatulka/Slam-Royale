using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(ActionManager))]
public abstract class Actor : MonoBehaviour
{
    // ID of the actor
    protected Guid m_id;

    public static RigidbodyConstraints DEF_RB_CONSTRAINTS = RigidbodyConstraints.FreezePositionZ |
        RigidbodyConstraints.FreezeRotationX |
        RigidbodyConstraints.FreezeRotationY |
        RigidbodyConstraints.FreezeRotationZ;

    public static RigidbodyConstraints NO_GRAV_RB_CONSTRAINTS = RigidbodyConstraints.FreezePositionZ |
        RigidbodyConstraints.FreezePositionY |
        RigidbodyConstraints.FreezeRotationX |
        RigidbodyConstraints.FreezeRotationY |
        RigidbodyConstraints.FreezeRotationZ;

    public static string X_VEL_ANIM_KEY = "x_Vel";
    public static string X_INPUT_ANIM_KEY = "x_Input";
    public const string Y_VEL_ANIM_KEY = "y_Vel";
    public const string GROUNDED_ANIM_KEY = "grounded";
    private const string ACTOR_STATE_ANIM_KEY = "state";

    private const float GROUND_DRAG = 3f;
    private const float AIR_DRAG = 1f;
    private const float MAX_DOWNWARD_VELOCITY = -20f;
    private const float MAX_X_TO_IGNORE_COL = .9f;
    private const float APPLIED_GAVITY = -.5f;
    private const float ACTION_ADJ_GRAV = .175f;
    private const float ATK_ADJ_GRAV = 10f;

    private const float TRAIL_VEL = 5f;

    private float m_minimumHealthPercentage = 0;
    private float m_healthPercentage = 0;
    private const float HEAL_RECOVER_TICK = 1;
    private const float HEALTH_PERC_FORCE_MOD = 10f;
    private const float MAX_FORCE_VEL = 525f;
    private const float DMG_STUN_THRESHOLD = 100f;
    private const float MAX_HEALTH_PERC = 100;

    // Time over actors recover heath
    protected const float RECOVER_RATE = 1f;

    private CapsuleCollider m_collider;
    private const float DEF_COL_HEIGHT = 7;

    private Rigidbody m_rigidbody;

    private Animator m_animator;

    private AudioSource m_audioSource;

    private Hitbox[] m_hitBoxes;

    private readonly StateManager m_stateManager;
    // State time safeties
    private float timeRecoveryStarted;
    private float timeAtkStarted;
    private float timeThrowStarted;

    private float m_stunTime;
    private const float MAX_STUN_TIME = 1.75f;
    private const float STUN_RECOVER_TICK = .1f;
    private const float STUN_DMG_MOD = 350f;
    private bool m_canGetUp = false;

    private float x, y;

    public static float ROTATE_DURATION = .75f;
    public static Quaternion FACE_RIGHT = Quaternion.Euler(0, 90, 0);
    public static Quaternion FACE_LEFT = Quaternion.Euler(0, -90, 0);

    private const float SPEED = 800;

    public static float JUMP_POWER = 350;

    //Boolean to prevent jump spam, dampens jump calls
    private bool m_canJump = false;
    private const float CAN_JUMP_DAMPEN = .5f;

    private bool m_grounded;

    private bool m_lockGravity = false;

    private bool m_isInvulnerable = false;

    private bool m_isHyperArmor = false;

    private float m_groundCheckDistance;
    private const float GROUND_CHECK_MIN = 0f;
    private float m_origGroundCheckDistance;

    // Prevents the Actor from taking damage from the same source multiple times at once
    private Guid m_dmgRecieverId;

    // Used for ignoring own hitboxes as ground collisions
    private int m_targetLayer;

    private bool m_DestroyQueued = false;

    public ActionManager ActionManager;

    public ActorEffectsManager EffectManager;

    protected GameManager GameManager;

    public Actor()
    {
        m_origGroundCheckDistance = m_groundCheckDistance;
        m_stateManager = new StateManager();
    }

    private void ApplyStateEffects()
    {
        EffectManager.ToggleTrail(Rigidbody.velocity.y > TRAIL_VEL);
        if (State.Equals(ActorState.DEAD) && !m_DestroyQueued)
        {
            m_DestroyQueued = true;
            OnDestroy();
        }
        if (State.Equals(ActorState.DEFAULT))
        {
            m_isHyperArmor = false;
            m_isInvulnerable = false;
            m_lockGravity = false;
        }
        if (State.Equals(ActorState.GRABBED) ||
            State.Equals(ActorState.BEING_THROWN) ||
            State.Equals(ActorState.DEAD))
        {
            Rigidbody.isKinematic = true;
        }
        else {
            Rigidbody.isKinematic = false;
        }
        if (State.Equals(ActorState.DOWN) && !Grounded)
        {
            UpdateState(ActorState.STUNNED);
        }
        if (State.Equals(ActorState.DOWN) && Mathf.Abs(x) > 0 && m_canGetUp)
        {
            UpdateState(ActorState.DEFAULT);
        }
        if (!State.Equals(ActorState.DEFAULT) &&
            !State.Equals(ActorState.GRABBING) &&
            !State.Equals(ActorState.ATTACKING))
        {
            x = 0;
            y = 0;
        }
        // Safety Check for Stuck States
        if (State.Equals(ActorState.RECOVERING))
        {
            float timeRecovDiff = Time.fixedTime - timeRecoveryStarted;
            if (timeRecovDiff > MAX_STUN_TIME)
            {
                UpdateState(ActorState.DEFAULT);
                timeRecoveryStarted = 0;
            }
        }
        if (State.Equals(ActorState.ATTACKING))
        {
            float timeAtkDiff = Time.fixedTime - timeAtkStarted;
            if (timeAtkDiff > MAX_STUN_TIME + 1f)
            {
                UpdateState(ActorState.DEFAULT);
                timeAtkStarted = 0;
            }
        }
        if (State.Equals(ActorState.BEING_THROWN))
        {
            float timeThrownDiff = Time.fixedTime - timeThrowStarted;
            if (timeThrownDiff > MAX_STUN_TIME + 1f)
            {
                UpdateState(ActorState.DEFAULT);
                timeThrowStarted = 0;
            }
        }
        Collider.isTrigger = !Grounded && !State.Equals(ActorState.ATTACKING);
    }

    public void Move(float x_Input, float y_Input, bool ignoreRot)
    {
        x = x_Input;
        y = y_Input;

        ApplyStateEffects();

        Rigidbody.velocity.Normalize();
        // Check distance based off current collider height
        m_groundCheckDistance = m_collider.height / 2;

        HandleStun();
        if (LockGravity)
        {
            Rigidbody.constraints = NO_GRAV_RB_CONSTRAINTS;
        }
        else {
            Rigidbody.constraints = DEF_RB_CONSTRAINTS;
        }

        CheckGroundStatus();

        // Handles rotation of actor so appropriate force can be added
        if (!ignoreRot && State.Equals(ActorState.DEFAULT))
        {
            if (x < 0)
            {
                Rigidbody.rotation = FACE_RIGHT;
            }
            else if (x > 0)
            {
                Rigidbody.rotation = FACE_LEFT;
            }
        }

        if (Grounded)
        {
            Rigidbody.drag = GROUND_DRAG;

            m_canJump = true;
            if (!State.Equals(ActorState.GRABBING))
            {
                Rigidbody.AddForce((Mathf.Abs(x * SPEED)) * transform.forward, ForceMode.Force);
            }
            else
            {
                if (Rigidbody.rotation == Quaternion.Euler(FACE_RIGHT.eulerAngles))
                {
                    x *= -1;
                }
                Rigidbody.AddForce(x * (SPEED / 2) * transform.forward, ForceMode.Force);
            }
        }
        else if (!Grounded)
        {
            Rigidbody.drag = AIR_DRAG;

            if (!State.Equals(ActorState.GRABBING))
            {
                Rigidbody.AddForce(Mathf.Abs(x * (SPEED / 4)) * transform.forward, ForceMode.Force);
            }
            else
            {
                if (Rigidbody.rotation == Quaternion.Euler(FACE_RIGHT.eulerAngles))
                {
                    x *= -1;
                }
                Rigidbody.AddForce(x * (SPEED / 4) * transform.forward, ForceMode.Force);
            }

            if (Rigidbody.velocity.y > MAX_DOWNWARD_VELOCITY)
            {
                float adjAppliedGravity = SPEED * APPLIED_GAVITY;
                if (!State.Equals(ActorState.DEFAULT) && Rigidbody.velocity.y <= 0)
                {
                    if (State.Equals(ActorState.ATTACKING))
                    {
                        // For Pile Driving
                        adjAppliedGravity *= ATK_ADJ_GRAV;
                    }
                    else {
                        // Slow falling for stunned and grabbing
                        adjAppliedGravity *= ACTION_ADJ_GRAV;
                    }
                }

                Rigidbody.AddForce((adjAppliedGravity * transform.up), ForceMode.Force);
            }

            m_groundCheckDistance = Rigidbody.velocity.y < 0 ? m_origGroundCheckDistance : GROUND_CHECK_MIN;
        }
    }

    public void RecoverOvertime()
    {
        if (m_minimumHealthPercentage < HealthPercentage && State.Equals(ActorState.DEFAULT))
        {
            m_healthPercentage -= HEAL_RECOVER_TICK;
            // Headge value if it overflows
            m_healthPercentage = m_healthPercentage < 0 ? 0 : m_healthPercentage;
        }
        if (0 < StunTime && State.Equals(ActorState.DEFAULT) && HealthPercentage <= 50)
        {
            m_stunTime -= STUN_RECOVER_TICK;
            // Headge value if it overflows
            m_stunTime = m_stunTime < 0 ? 0 : m_stunTime;
        }
    }

    // Called on footstep
    public void Step()
    {
        SoundManager.Instance.Step(m_audioSource);
    }

    public void Jump()
    {
        if (Rigidbody != null && CanJump && State.Equals(ActorState.DEFAULT))
        {
            m_canJump = false;
            if (!Grounded)
            {
                Rigidbody.velocity = Vector3.zero;
            }
            float jump = JUMP_POWER;
            Rigidbody.AddForce((jump * Vector3.up), ForceMode.Impulse);
            EffectManager.GroundContact();
            //So we aren't grounded at take off
            m_groundCheckDistance = GROUND_CHECK_MIN;
        }
    }

    void OnCollisionEnter(Collision col)
    {
        Actor colActor = col.gameObject.GetComponent<Actor>();
        // Ignore collisions with down actors
        if (colActor != null)
        {
            if (colActor.State.Equals(ActorState.DOWN))
            {
                Physics.IgnoreCollision(colActor.Collider, Collider, true);
            }
        }
    }

    void OnCollisionExit(Collision col)
    {
        Actor colActor = col.gameObject.GetComponent<Actor>();
        // Ignore collisions with down actors
        if (colActor != null)
        {
            StartCoroutine(IgnoreCollision(colActor.Collider));
        }
    }


    public void OnDamageEvent(object sender, HitboxEventArgs e)
    {
        // Actor damaging this actor
        Actor atkActor = e.Hitter.Actor;
        Attack atk = e.Attack;

        Damage(atk.AttackType, atk.Damage, atkActor, e.Hitter.transform.position);
    }

    public void Damage(AttackType type, Damage dmg, Actor atkActor, Vector3 dmgPosition)
    {
        m_canJump = true;
        if (m_isInvulnerable &&
            !type.Equals(AttackType.GRAB))
        {
            return;
        }
        switch (type)
        {
            case AttackType.HIT:
                if (dmg.ID.Equals(DamageRecieverId))
                {
                    // Return because this damage is already being applied to this actor
                    return;
                }
                m_dmgRecieverId = dmg.ID;
                m_healthPercentage = dmg.Hurt + HealthPercentage > MAX_HEALTH_PERC ? MAX_HEALTH_PERC : HealthPercentage + dmg.Hurt;
                if (dmgPosition != Vector3.zero)
                {
                    EffectManager.Hit(dmgPosition, dmg.Hurt);
                    if (!type.Equals(AttackType.STRIKE_GRAPPLE)) {
                        SoundManager.Instance.Slap(AudioSource);
                    }
                }
                if (!m_isHyperArmor &&
                    !State.Equals(ActorState.GRABBING) &&
                    !State.Equals(ActorState.GRABBED) &&
                    !State.Equals(ActorState.BEING_THROWN))
                {
                    m_animator.Rebind();

                    m_canJump = true;

                    //Determine force
                    Vector2 force = new Vector2(dmg.Force.x * (HealthPercentage / HEALTH_PERC_FORCE_MOD),
                        dmg.Force.y * (HealthPercentage / HEALTH_PERC_FORCE_MOD));

                    if (atkActor != null && atkActor.transform.position.x >= transform.position.x)
                    {
                        force = new Vector2(force.x * -1, force.y);
                        Rigidbody.rotation = FACE_RIGHT;
                    }
                    else {
                        Rigidbody.rotation = FACE_LEFT;
                    }

                    if (force.y > 0)
                    {
                        m_groundCheckDistance = GROUND_CHECK_MIN;
                    }
                    // Stop Actor so crazy forces don't get applied
                    Rigidbody.velocity = Vector3.zero;

                    // Used mainly so throws on undamaged enemies actually seems like you can throw enemies
                    force.y = force.y < dmg.Force.y ? dmg.Force.y : force.y;

                    // Headge maximum velocities
                    force.y = force.y > MAX_FORCE_VEL ? MAX_FORCE_VEL : force.y;
                    force.x = force.x > MAX_FORCE_VEL ? MAX_FORCE_VEL : force.x;

                    Rigidbody.AddForce(new Vector3(force.x, force.y), ForceMode.Impulse);

                    AddToStunTime(dmg.Hurt);

                    if (HealthPercentage >= DMG_STUN_THRESHOLD && !State.Equals(ActorState.STUNNED))
                    {
                        // This is for a smooth tranistion to stunned animation
                        UpdateState(ActorState.DAMAGED);
                        UpdateState(ActorState.STUNNED);
                    }
                    else {
                        UpdateState(ActorState.DAMAGED);
                    }

                    if (dmg.Hurt > WrestlerAttackManager.PUNCH_DMG && !Grounded) {
                        StartCoroutine(GameManager.DynamicDamageView(force, atkActor, this));
                    }
                    OnDamage();
                }
                break;
            case AttackType.GRAB:
                if (atkActor != null &&
                    !State.Equals(ActorState.GRABBED) &&
                    !State.Equals(ActorState.GRABBING) &&
                    !State.Equals(ActorState.BEING_THROWN))
                {
                    m_animator.Rebind();
                    OnGrabbed(atkActor);
                }
                break;
        }
    }

    public IEnumerator IgnoreCollision(Collider col)
    {
        Physics.IgnoreCollision(Collider, col, true);
        // Re-enable collision when collision is the width of the collider away OR if attacking
        yield return new WaitUntil(
            () =>
            Vector3.Distance(transform.position, col.transform.position) > Collider.height ||
            State.Equals(ActorState.ATTACKING)
            );
        Actor actor = col.gameObject.GetComponent<Actor>();
        if (Collider != null)
        {
            Physics.IgnoreCollision(Collider, actor.Collider, false);
        }
    }

    protected abstract void OnGrounded();

    protected abstract void OnDamage();

    protected abstract void OnBlock();

    protected abstract void OnGrabbed(Actor actor);

    protected abstract void OnDestroy();

    private void HandleStun()
    {
        if (State.Equals(ActorState.STUNNED) || State.Equals(ActorState.DOWN))
        {
            if (StunTime <= 0)
            {
                UpdateState(ActorState.DEFAULT);
                m_stunTime = 0;
                Rigidbody.velocity = new Vector2(0, Rigidbody.velocity.y);
                m_canGetUp = false;
            }
            else {
                m_stunTime -= Time.deltaTime;
            }
        }
    }

    private void AddToStunTime(float dmg)
    {
        if (m_stunTime <= MAX_STUN_TIME)
        {
            m_stunTime += dmg / STUN_DMG_MOD * HealthPercentage;
            // Headge value if it overflows
            m_stunTime = m_stunTime >= MAX_STUN_TIME ? MAX_STUN_TIME : m_stunTime;
        }
    }

    protected void SetBasicAnimationValues()
    {
        Animator.SetFloat(X_VEL_ANIM_KEY, Mathf.Abs(Rigidbody.velocity.x));
        Animator.SetFloat(X_INPUT_ANIM_KEY, x);
        Animator.SetFloat(Y_VEL_ANIM_KEY, Rigidbody.velocity.y);
        Animator.SetBool(GROUNDED_ANIM_KEY, Grounded);
        Animator.SetInteger(ACTOR_STATE_ANIM_KEY, (int)State);
    }

    private void CheckGroundStatus()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, m_groundCheckDistance + 0.1f, m_targetLayer, QueryTriggerInteraction.Ignore))
        {
            // Ignore other actors as grounded
            if (hit.transform.gameObject.GetComponent<Actor>() == null &&
                !Grounded)
            {
                if (State.Equals(ActorState.DAMAGED) ||
                    State.Equals(ActorState.STUNNED) ||
                    State.Equals(ActorState.RECOVERING))
                {
                    // If damaged or stunned when hitting the ground, enter down state
                    UpdateState(ActorState.DOWN);
                    // Just incase it's a recovering frame with i-frames
                    Invulnerable = false;
                    m_canGetUp = false;
                    Invoke("SetCanGetUpTrue", 0.5f);
                }
                Grounded = true;
                SoundManager.Instance.SmallThud(m_audioSource);
                EffectManager.GroundContact();
                OnGrounded();
            }
        }
        else {

            Grounded = false;

            if (State.Equals(ActorState.DAMAGED))
            {
                UpdateState(ActorState.STUNNED);
            }
        }
    }

    public void SetCanGetUpTrue()
    {
        m_canGetUp = true;
    }

    public ActorState UpdateState(ActorState state)
    {
        if (state.Equals(ActorState.RECOVERING))
        {
            timeRecoveryStarted = Time.fixedTime;
        }
        if (state.Equals(ActorState.ATTACKING))
        {
            timeAtkStarted = Time.fixedTime;
        }
        if (state.Equals(ActorState.BEING_THROWN))
        {
            timeThrowStarted = Time.fixedTime;
        }
        return m_stateManager.UpdateState(state);
    }

    public Guid ID
    {
        get { return m_id; }
    }

    public ActorState State
    {
        get { return m_stateManager.State; }
    }

    public float XInput
    {
        get { return x; }
    }

    public float YInput
    {
        get { return y; }
    }

    public float HealthPercentage
    {
        get { return m_healthPercentage; }
    }

    public float MinumumHealthPercentage
    {
        set
        {
            m_minimumHealthPercentage = value;
            m_healthPercentage = value;
        }
    }

    public bool Grounded
    {
        get { return m_grounded; }
        set
        {
            if (!State.Equals(ActorState.ATTACKING) || !State.Equals(ActorState.BEING_THROWN))
            {
                m_grounded = value;
            }
        }
    }

    public CapsuleCollider Collider
    {
        get { return m_collider; }
        set
        {
            // Set up default collider
            value.height = DEF_COL_HEIGHT;
            value.radius = 1f;
            m_collider = value;
        }
    }

    public Rigidbody Rigidbody
    {
        get { return m_rigidbody; }
        set
        {
            //Set up default rigidbody
            value.constraints = DEF_RB_CONSTRAINTS;
            value.mass = 10;
            value.angularDrag = 0;
            value.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            m_rigidbody = value;
        }
    }

    public Animator Animator
    {
        get { return m_animator; }
        set { m_animator = value; }
    }

    public AudioSource AudioSource
    {
        get { return m_audioSource; }
        set { m_audioSource = value; }
    }

    public bool LockGravity
    {
        get { return m_lockGravity; }
        set { m_lockGravity = value; }
    }

    public bool Invulnerable
    {
        get { return m_isInvulnerable; }
        set { m_isInvulnerable = value; }
    }

    public bool HyperArmor
    {
        get { return m_isHyperArmor; }
        set { m_isHyperArmor = value; }
    }

    public float StunTime
    {
        get { return m_stunTime; }
    }

    public Guid DamageRecieverId
    {
        get { return m_dmgRecieverId; }
    }

    public int TargetLayer
    {
        get { return m_targetLayer; }
        set { m_targetLayer = value; }
    }

    protected bool CanJump
    {
        get { return m_canJump; }
        set { m_canJump = value; }
    }

    protected Hitbox[] HitBoxes
    {
        set
        {
            m_hitBoxes = value;

            // Set up hit box event Listeners
            foreach (Hitbox hitbox in m_hitBoxes)
            {
                hitbox.HitboxEvent += new Hitbox.HitboxEventHandler(OnDamageEvent);
            }
        }
    }

    protected abstract void SetUpTargetLayer();
}
