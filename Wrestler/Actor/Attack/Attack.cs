using UnityEngine;

public class Attack
{
    private AttackType m_attackType;

    private Damage m_dmg;

    private Vector2 m_aim;

    private ActiveAttackFrame[] m_activeFrames;

    public Attack(AttackType type, ActiveAttackFrame[] activeFrames)
    {
        m_attackType = type;
        m_activeFrames = activeFrames;
    }

    public AttackType AttackType
    {
        get { return m_attackType; }
    }

    public Damage Damage
    {
        get { return m_dmg; }
        set { m_dmg = value; }
    }

    public ActiveAttackFrame[] ActiveFrames
    {
        get { return m_activeFrames; }
    }

    public Vector2 Aim {
        set { m_aim = value; }
        get { return m_aim; }
    }
}
