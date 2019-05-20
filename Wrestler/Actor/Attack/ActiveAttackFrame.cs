using System.Collections.Generic;
using UnityEngine;

public class ActiveAttackFrame
{
    //Frames attack is active
    private int m_frame;
    //Hitboxes to activate in this frame
    private string[] m_activeHitboxesIds;
    //Motion applied on active frame
    private Vector2 m_motion;
    //Does frame have hyperarmor
    private bool m_activateHyperArmor;
    //Is the frame an invincibilty frame
    private bool m_isIframe;
    //Is gravity locked
    private bool m_lockGravity;
    //Is frame end of attack
    private bool m_endOfAttack;

    public ActiveAttackFrame(int frame, string[] activeHitboxIds,
        Vector2 appliedMotion, bool activateHyperArmor, bool activateIFrame, 
        bool isGravityLocked, bool endOfAttack)
    {
        m_activeHitboxesIds = activeHitboxIds;
        m_frame = frame;
        m_motion = appliedMotion;
        m_activateHyperArmor = activateHyperArmor;
        m_isIframe = activateIFrame;
        m_lockGravity = isGravityLocked;
        m_endOfAttack = endOfAttack;
    }

    public int Frame
    {
        get { return m_frame; }
    }

    public string[] ActiveHitBoxesIDs
    {
        get { return m_activeHitboxesIds; }
    }

    public Vector2 Motion
    {
        get { return m_motion; }
    }

    public bool ActivateHyperArmor
    {
        get { return m_activateHyperArmor; }
    }

    public bool ActivateIFrame
    {
        get { return m_isIframe; }
    }

    public bool LockGravity
    {
        get { return m_lockGravity; }
    }

    public bool EndOfAttack
    {
        get { return m_endOfAttack; }
    }
}
