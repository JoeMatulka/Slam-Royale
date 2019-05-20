using System;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine;

[RequireComponent(typeof(PlayerActionManager))]
public class Player : Actor
{
    private const float HEALTH_RECOV_ON_BLOCK = 10f;

    private const float INPUT_RUN_THRESHOLD = 0.95f;
    private const float JUMP_DELAY = 0.1f;

    private const float RUN_SPEED = 1f;
    private const float WALK_SPEED = .5f;

    private float x_Input, y_Input;

    // Use this for initialization
    void Start()
    {
        SetUpTargetLayer();

        Animator = GetComponent<Animator>();
        AudioSource = GetComponent<AudioSource>();
        Rigidbody = GetComponent<Rigidbody>();
        Collider = GetComponent<CapsuleCollider>();
        HitBoxes = GetComponentsInChildren<Hitbox>();

        ActionManager = GetComponent<ActionManager>();
        EffectManager = GetComponent<ActorEffectsManager>();

        InvokeRepeating("RecoverOvertime", 1f, RECOVER_RATE);

        GameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    void FixedUpdate()
    {
        HandleControlInput();
        Move(x_Input, y_Input, false);
        SetBasicAnimationValues();
    }

    private void HandleControlInput()
    {
        x_Input = 0;
        y_Input = 0;
        if (ActionManager.Disabled)
        {
            return;
        }
        if (State.Equals(ActorState.DEFAULT) || State.Equals(ActorState.GRABBING) || State.Equals(ActorState.DOWN))
        {
            x_Input = CrossPlatformInputManager.GetAxis("Horizontal");
            y_Input = CrossPlatformInputManager.GetAxis("Vertical");

            // Throttle x input to manage proper acceleration
            if (Mathf.Abs(x_Input) >= INPUT_RUN_THRESHOLD)
            {
                x_Input = x_Input > 0 ? RUN_SPEED : -RUN_SPEED;
            }
            else if (Mathf.Abs(x_Input) > 0)
            {
                x_Input = x_Input > 0 ? WALK_SPEED : -WALK_SPEED;
            }
        }
    }


    protected override void OnDamage()
    {
        SoundManager.Instance.Grunt(AudioSource);
        ActionManager.OnDamage();
    }

    protected override void OnBlock()
    {
        // TODO, recover health and stun
    }

    protected override void OnGrabbed(Actor actor)
    {
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
