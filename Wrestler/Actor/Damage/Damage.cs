using System;
using UnityEngine;

public class Damage
{
    // ID of the damage
    private Guid m_id;

    // Amount of health removed
    private float m_dmg;

    // Force applied to Actor recieving damage
    private Vector2 m_force;

    // If the Actor is knocked down from damage
    private bool m_knockdown;

    public Damage(float damage, Vector2 force)
    {
        m_id = Guid.NewGuid();
        m_dmg = damage;
        m_force = force;
    }

    public float Hurt
    {
        get { return m_dmg; }
        set { m_dmg = value; }
    }

    public Guid ID
    {
        get { return m_id; }
    }

    public Vector2 Force
    {
        get { return m_force; }
        set { m_force = value; }
    }
}
