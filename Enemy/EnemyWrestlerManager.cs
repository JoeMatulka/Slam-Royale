using System;
using UnityEngine;

public class EnemyWrestlerManager : EnemyActionManager
{
    private const float HEALTH_BREAKPOINT = 75;
    private const float PLAYER_HEALTH_BREAKPOINT = 40;
    private const float PLAYER_Y_TRANS_JUMP = 8f;

    private bool initiatedThrow = false;

    // Prevent Spam Jumping
    private bool canJump = true;
    private const float JUMP_BUFFER = 1.5f;

    private GameManager gameManager;

    // Use this for initialization
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        gameCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameCamera>();

        Actor = GetComponent<Actor>();
        atkManager = new WrestlerAttackManager(this);
        sensorManager = new SensorManager(Actor);

        // Set range at which the enemy will run towards the player
        RunDistance = UnityEngine.Random.Range(18f, 25f);
        // Start out agressive
        Behavior = ActorBehaviorType.AGGRESSIVE;

        // Get the player for targeting
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        PlayerManager = Player.GetComponent<ActionManager>();

        CalculateDifficulty(gameManager.Score + 1);
    }

    // Update is called once per frame
    void Update()
    {
        sensorManager.UpdateSensors();
        UpdateActionAnimations();
        HandleMovement();
        HandleAttack();

        // Clean up, can't figure why this isn't be cleared automatically
        if (CurrentAttack != null && !Actor.State.Equals(ActorState.ATTACKING))
        {
            CurrentAttack = null;
        }

        //Sanity check
        if (Actor.State.Equals(ActorState.DEFAULT))
        {
            Grabbee = null;
            Grabber = null;
        }
        if (Actor.State.Equals(ActorState.GRABBING) && Grabbee == null)
        {
            BreakGrab(false, false);
        }
        if (Actor.State.Equals(ActorState.GRABBED))
        {
            if (Grabber == null) {
                BreakGrab(true, false);
            }
            if (Grabber != null &&
                Grabber.GetComponent<ActionManager>() != null &&
                Grabber.GetComponent<ActionManager>().Grabbee == null) {
                BreakGrab(false, false);
            }
        }
        if (Actor.State.Equals(ActorState.BEING_THROWN) &&
            Grabber != null && Grabber.State.Equals(ActorState.DEFAULT))
        {
            BreakGrab(false, false);
        }
    }

    public override void ActivateAttackFrame(int frame)
    {
        atkManager.ActivateAttackFrame(frame);
    }

    public override void OnDamage()
    {
        CurrentAttack = null;
        atkManager.DeactivateHitboxes(Actor);
        Actor.Invulnerable = false;
        Actor.LockGravity = false;
    }

    protected override void HandleMovement()
    {
        if (Actor.State.Equals(ActorState.DEFAULT) || Actor.State.Equals(ActorState.GRABBING))
        {
            if (Disabled || PlayerManager.Disabled)
            {
                X_Input = 0;
                Y_Input = 0;
                return;
            }
            if (Actor.HealthPercentage >= 75 && Difficulty >= 6 ||
                (Player.State.Equals(ActorState.GRABBED) && Actor.State.Equals(ActorState.DEFAULT)))
            {
                // Be a huge coward at high percentages of damage or when player is grabbed
                Behavior = ActorBehaviorType.AVOID;
            }
            else {
                Behavior = ActorBehaviorType.AGGRESSIVE;
            }

            if (Player.State.Equals(ActorState.STUNNED) || OutOfRing)
            {
                Behavior = ActorBehaviorType.AGGRESSIVE;
            }

            if (Actor.State.Equals(ActorState.GRABBING) && Player.HealthPercentage > PLAYER_HEALTH_BREAKPOINT)
            {
                // Avoid will take us closer to the ropes
                Behavior = ActorBehaviorType.AVOID;
            }

            float distToPlayer = Vector3.Distance(Player.transform.position, Actor.transform.position);
            InAttackRange = distToPlayer < AttackDistance;

            Vector2 move = ActorBehavior.Act(Behavior, Actor, Player, RunDistance);
            if (InAttackRange &&
                Behavior.Equals(ActorBehaviorType.AGGRESSIVE) &&
                !Actor.State.Equals(ActorState.DOWN) &&
                !Actor.State.Equals(ActorState.STUNNED))
            {
                move.x = 0;
            }
            X_Input = move.x;
            if (X_Input == 0)
            {
                FacePlayer();
            }
            Y_Input = move.y;
            if (OutOfRing)
            {
                Actor.Jump();
            }
            if (Y_Input > 0)
            {
                if (canJump)
                {
                    canJump = false;
                    Invoke("toggleCanJump", UnityEngine.Random.Range(JUMP_BUFFER, JUMP_BUFFER + 1));
                    Y_Input = 0;
                    Actor.Jump();
                }
            }
        }
        else {
            X_Input = 0;
            Y_Input = 0;
        }
    }

    private void Enable()
    {
        Disabled = false;
    }

    public override void Despawn()
    {
        if (Actor != null && ResourceManager.Instance != null) {
            BreakGrab(false, false);
            Instantiate(ResourceManager.Instance.Confetti,
                                Actor.transform.position,
                                Quaternion.identity);
            gameManager.DestroyEnemy(Actor as EnemyWrestler);
        }
    }

    private void InitiateThrow()
    {
        float delay = UnityEngine.Random.Range(0.5f, 1);
        Disabled = true;
        Invoke("ThrowPlayer", delay);
    }

    private void ThrowPlayer()
    {
        if (!Actor.State.Equals(ActorState.ATTACKING))
        {
            Actor.Rigidbody.velocity = Vector3.zero;
            // Aim
            float x, y;
            // Determine which edge is closer and how close
            float distToRight = Vector3.Distance(Actor.transform.position, Ring.RIGHT_ROPES_POS);
            float distToLeft = Vector3.Distance(Actor.transform.position, Ring.LEFT_ROPES_POS);

            // Detemine throw angle
            if (distToRight > distToLeft)
            {
                // Throw Right
                Actor.Rigidbody.rotation = Actor.FACE_RIGHT;

                y = (Actor.transform.position.y - Ring.LEFT_ROPES_POS.y);

                float xx = Ring.LEFT_ROPES_POS.x - Actor.transform.position.x;
                float xz = Ring.LEFT_ROPES_POS.z - Actor.transform.position.z;
                x = Mathf.Sqrt(xx * xx + xz * xz);
            }
            else {
                // Throw Left
                Actor.Rigidbody.rotation = Actor.FACE_LEFT;

                y = (Actor.transform.position.y - Ring.RIGHT_ROPES_POS.y);

                float xx = Ring.RIGHT_ROPES_POS.x - Actor.transform.position.x;
                float xz = Ring.RIGHT_ROPES_POS.z - Actor.transform.position.z;
                x = Mathf.Sqrt(xx * xx + xz * xz);
            }
            // Throw Player
            CurrentAttack = atkManager.DetermineAttack(AttackType.THROW_GRAPPLE,
                new Vector2(x, Mathf.Abs(y + ROPE_CENTER_OFFSET)).normalized);
            initiatedThrow = false;
            Invoke("Enable", 1f);
        }
    }

    protected override void HandleAttack()
    {
        if (Disabled || initiatedThrow || PlayerManager.Disabled ||
            (Player.State.Equals(ActorState.GRABBING) &&
            PlayerManager.sensorManager.Back != null && 
            PlayerManager.sensorManager.Back.name.Contains("ropes")))
        {
            return;
        }

        if (Player.transform.position.y - Actor.transform.position.y >= PLAYER_Y_TRANS_JUMP &&
                UnityEngine.Random.value < Reaction)
        {
            if (canJump)
            {
                canJump = false;
                Invoke("toggleCanJump", UnityEngine.Random.Range(JUMP_BUFFER, JUMP_BUFFER + 1));
                Actor.Jump();
            }
        }

        if (InAttackRange &&
            CheckCanAttack())
        {
            if (Player.State.Equals(ActorState.ATTACKING) &&
                (sensorManager.Front != null && !sensorManager.Front.tag.Equals("Enemy")))
            {
                float timeSinceLastReact = Time.fixedTime - LastReactTime;
                if (timeSinceLastReact >= MAX_REACT_TIME)
                {
                    if (UnityEngine.Random.value < Reaction)
                    {
                        LastReactTime = Time.fixedTime;
                        if (PlayerManager.CurrentAttack != null)
                        {
                            Actor.Rigidbody.velocity = Vector3.zero;
                            FacePlayer();
                            switch (PlayerManager.CurrentAttack.AttackType)
                            {
                                case AttackType.HIT:
                                    CurrentAttack = atkManager.DetermineAttack(AttackType.BLOCK, Vector2.zero);
                                    return;
                                case AttackType.GRAB:
                                    CurrentAttack = atkManager.DetermineAttack(AttackType.HIT, Vector2.zero);
                                    return;
                                case AttackType.BLOCK:
                                    CurrentAttack = atkManager.DetermineAttack(AttackType.GRAB, Vector2.zero);
                                    return;
                            }
                        }
                    }
                }
            }

            float timeSinceLastAtk = Time.fixedTime - LastAttackTime;

            if (Actor.State.Equals(ActorState.GRABBING))
            {
                if (Player.HealthPercentage < PLAYER_HEALTH_BREAKPOINT)
                {
                    Actor.Rigidbody.velocity = Vector3.zero;
                    CurrentAttack = atkManager.DetermineAttack(AttackType.STRIKE_GRAPPLE, Vector2.zero);
                }
                else {
                    float delay = UnityEngine.Random.Range(1, 3);
                    delay = !Actor.Grounded ? 0 : delay;
                    if (!initiatedThrow)
                    {
                        initiatedThrow = true;
                        Invoke("InitiateThrow", delay);
                    }
                }
            }

            if (timeSinceLastAtk > Aggression &&
                !Player.State.Equals(ActorState.GRABBED) &&
                !Player.State.Equals(ActorState.DOWN) &&
                (sensorManager.Front != null &&
                !sensorManager.Front.tag.Equals("Enemy")))
            {
                LastAttackTime = Time.fixedTime;
                FacePlayer();
                Actor.Rigidbody.velocity = Vector3.zero;
                if (UnityEngine.Random.value > 0.5)
                {
                    Actor.Animator.Rebind();
                    CurrentAttack = atkManager.DetermineAttack(AttackType.HIT, Vector2.zero);
                    return;
                }
                else {
                    Actor.Animator.Rebind();
                    CurrentAttack = atkManager.DetermineAttack(AttackType.GRAB, Vector2.zero);
                    return;
                }
            }
        }
    }

    private void toggleCanJump()
    {
        canJump = true;
    }

    public override void ExecuteQueuedActions()
    {
        // Since AI has precise reaction, no need for queuing attacks
    }
}
