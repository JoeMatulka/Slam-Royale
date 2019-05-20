using UnityEngine;

public abstract class EnemyActionManager : ActionManager
{
    // Offset the center of ropes so we can target above the ropes
    protected const float ROPE_CENTER_OFFSET = 10f;

    private float m_difficulty;
    private float m_aggression;
    private const float BASE_AGGRESSION = 5f;
    private const float MAX_AGRESSION = 1f;
    private float m_reaction;
    private const float BASE_REACT = .25f;
    private const float MAX_REACTION = 1;

    private float m_lastAtkTime = 0;
    private float m_lastReactTime = 0;
    protected float MAX_REACT_TIME = 2f;

    public static readonly float WALK_INPUT = 0.15f;
    public static readonly float RUN_INPUT = 1f;
    public static readonly float JUMP_INPUT = 1f;

    private float m_attackDist = 8f;
    private float m_runDist = 25f;

    private Player m_player;
    private ActionManager m_playerManager;

    private float m_xInput;

    private float m_yinput;

    private bool m_inAttackRange;

    private ActorBehaviorType m_behavior;

    protected void CalculateDifficulty(float difficulty)
    {
        m_difficulty = difficulty;
        m_aggression = Mathf.Abs(Mathf.Round(BASE_AGGRESSION / difficulty));
        if (m_aggression < MAX_AGRESSION) {
            m_aggression = MAX_AGRESSION;
        }
        m_reaction = BASE_REACT * (difficulty * .1f);
        if (m_reaction > MAX_REACTION) {
            m_reaction = MAX_REACTION;
        }

        // Adjust health according to difficulty
        float adjustHealth = 100 - (((m_difficulty - 1) * 5) + 25);
        if (adjustHealth > 0) {
            Actor.MinumumHealthPercentage = adjustHealth;
        }
    }

    protected void FacePlayer()
    {
        if (Actor.State.Equals(ActorState.DEFAULT) && Actor.Grounded)
        {
            if (Player.transform.position.x > Actor.transform.position.x)
            {
                Actor.Rigidbody.rotation = Actor.FACE_RIGHT;
            }
            else {
                Actor.Rigidbody.rotation = Actor.FACE_LEFT;
            }
        }
    }

    protected abstract void HandleMovement();

    protected abstract void HandleAttack();

    public abstract void Despawn();

    public Player Player
    {
        get { return m_player; }
        set { m_player = value; }
    }

    public ActionManager PlayerManager
    {
        get { return m_playerManager; }
        set { m_playerManager = value; }
    }

    public bool InAttackRange
    {
        get { return m_inAttackRange; }
        set { m_inAttackRange = value; }
    }

    protected float AttackDistance
    {
        get { return m_attackDist; }
        set { m_attackDist = value; }
    }

    protected float RunDistance
    {
        get { return m_runDist; }
        set { m_runDist = value; }
    }

    public float X_Input
    {
        get { return m_xInput; }
        set { m_xInput = value; }
    }

    public float Y_Input
    {
        get { return m_yinput; }
        set { m_yinput = value; }
    }

    public ActorBehaviorType Behavior
    {
        get { return m_behavior; }
        set { m_behavior = value; }
    }

    public float Difficulty
    {
        get { return m_difficulty; }
    }

    public float Aggression
    {
        get { return m_aggression; }
    }

    public float Reaction
    {
        get { return m_reaction; }
    }

    protected float LastAttackTime
    {
        get { return m_lastAtkTime; }
        set { m_lastAtkTime = value; }
    }

    protected float LastReactTime
    {
        get { return m_lastReactTime; }
        set { m_lastReactTime = value; }
    }
}
