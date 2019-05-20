using System;
using UnityEngine;

public class HitboxEventArgs : EventArgs {
    private Attack m_atk;
    private Hitbox m_hitter;
    private Hitbox m_reciever;
    private Collider m_col;

    public HitboxEventArgs(Attack atk, Hitbox hitter, Hitbox reciever, Collider col) {
        m_atk = atk;
        m_hitter = hitter;
        m_reciever = reciever;
        m_col = col;
    }

    public Attack Attack {
        get { return m_atk; }
    }

    public Hitbox Hitter {
        get { return m_hitter; }
    }

    public Hitbox Reciever {
        get { return m_reciever; }
    }

    public Collider Collider {
        get { return m_col; }
    } 
}
