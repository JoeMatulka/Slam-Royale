using System;
using UnityEngine;

[RequireComponent(typeof(EnemyWrestlerManager))]
public class EnemyWrestler : Actor
{
    private const float INPUT_RUN_THRESHOLD = 0.95f;
    private const float JUMP_DELAY = 0.1f;

    private const float RUN_SPEED = 1f;
    private const float WALK_SPEED = .5f;

    // Use this for initialization
    void Start()
    {
        m_id = Guid.NewGuid();

        SetUpTargetLayer();

        Animator = GetComponent<Animator>();
        AudioSource = GetComponent<AudioSource>();
        Rigidbody = GetComponent<Rigidbody>();
        Collider = GetComponent<CapsuleCollider>();
        HitBoxes = GetComponentsInChildren<Hitbox>();

        ActionManager = GetComponent<ActionManager>() as EnemyActionManager;
        EffectManager = GetComponent<ActorEffectsManager>();

        InvokeRepeating("RecoverOvertime", 1f, RECOVER_RATE);

        GameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    void FixedUpdate()
    {
        Move(((EnemyActionManager)ActionManager).X_Input,
            ((EnemyActionManager)ActionManager).Y_Input,
            false);
        SetBasicAnimationValues();
    }

    protected override void OnDamage()
    {
        SoundManager.Instance.Grunt(AudioSource);
        ActionManager.OnDamage();
    }

    protected override void OnBlock()
    {
        // Do nothing
    }

    protected override void OnGrabbed(Actor actor)
    {
        if (!actor.tag.Equals("Player"))
        {
            return;
        }

        if (ActionManager.CurrentAttack != null &&
        (ActionManager.CurrentAttack.AttackType.Equals(AttackType.STRIKE_GRAPPLE) ||
        ActionManager.CurrentAttack.AttackType.Equals(AttackType.THROW_GRAPPLE)))
        {
            return;
        }

        ActionManager actorManager = actor.GetComponent<ActionManager>();

        if (actorManager != null &&
            actorManager.Grabbee == null)
        {
            if (Grounded)
            {
                SoundManager.Instance.SmallThud(AudioSource);
            }
            ActionManager.OnDamage();
            ActionManager.Grabber = actor;
            actorManager.Grabbee = this;
        }
    }

    protected override void OnDestroy()
    {
        ((EnemyActionManager)ActionManager).Despawn();
    }

    protected override void OnGrounded()
    {
        if (State.Equals(ActorState.ATTACKING) &&
            ActionManager.CurrentAttack.AttackType.Equals(AttackType.STRIKE_GRAPPLE))
        {
            ActionManager.StrikeThrow();
            ActionManager.CurrentAttack = null;
        }
        if (State.Equals(ActorState.GRABBING) && ActionManager.Grabbee != null)
        {
            // TODO: Set up a system to update values for grabbed actors
            ActionManager.Grabbee.Grounded = true;
        }
    }

    protected override void SetUpTargetLayer()
    {
        TargetLayer = 1 << LayerMask.NameToLayer("Default");
    }
}
