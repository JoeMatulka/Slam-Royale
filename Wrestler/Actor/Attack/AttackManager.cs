using UnityEngine;

public abstract class AttackManager
{
    private ActionManager m_manager;

    public void ActivateAttackFrame(int frame)
    {
        if (Manager.CurrentAttack != null && Manager != null)
        {
            foreach (ActiveAttackFrame activeFrame in Manager.CurrentAttack.ActiveFrames)
            {
                if (activeFrame.Frame == frame)
                {
                    if (activeFrame.ActiveHitBoxesIDs != null && activeFrame.ActiveHitBoxesIDs.Length > 0)
                    {
                        switch (Manager.CurrentAttack.AttackType)
                        {
                            case AttackType.HIT:
                            case AttackType.GRAB:
                                // Activate Hit boxes for HIT attacks
                                ActivateHitboxes(Manager.Actor, activeFrame.ActiveHitBoxesIDs, Manager.CurrentAttack);
                                break;
                            case AttackType.THROW_GRAPPLE:
                                Manager.Throw();
                                break;
                            case AttackType.STRIKE_GRAPPLE:
                                Manager.StrikeThrow();
                                break;
                        }
                    }
                    else {
                        // Deactive Hit boxes
                        DeactivateHitboxes(Manager.Actor);
                    }
                    Manager.Actor.LockGravity = activeFrame.LockGravity;
                    Manager.Actor.Invulnerable = activeFrame.ActivateIFrame;
                    Manager.Actor.HyperArmor = activeFrame.ActivateHyperArmor;
                    if (activeFrame.Motion != Vector2.zero)
                    {
                        Manager.Actor.Move(activeFrame.Motion.x, 0, true);

                    }
                    else {
                        Manager.Actor.Rigidbody.velocity = Vector3.zero;
                    }
                    if (activeFrame.EndOfAttack)
                    {
                        if (Manager.CurrentAttack.AttackType.Equals(AttackType.GRAB) &&
                            Manager.Actor.State.Equals(ActorState.GRABBING))
                        {
                            Manager.Actor.UpdateState(ActorState.GRABBING);
                        }
                        else if (Manager.CurrentAttack.AttackType.Equals(AttackType.GRAB))
                        {
                            // Missed so recover
                            Manager.Actor.UpdateState(ActorState.RECOVERING);
                        }
                        else {
                            Manager.Actor.UpdateState(ActorState.DEFAULT);
                            Manager.Actor.ActionManager.ExecuteQueuedActions();
                        }
                        Manager.CurrentAttack = null;
                    }
                }
            }
        }
    }

    public ActionManager Manager
    {
        get { return m_manager; }
        set { m_manager = value; }
    }

    private void ActivateHitboxes(Actor actor, string[] activeHitboxIds, Attack attack)
    {
        foreach (string id in activeHitboxIds)
        {
            Transform tr = actor.transform.Search(id);
            if (tr != null)
            {
                Hitbox hitBox = tr.GetComponent<Hitbox>();
                hitBox.Attack = attack;
                hitBox.Active = true;
            }
        }
    }

    public void ActivateAllHitboxes(Actor actor, Attack attack)
    {
        if (actor != null && actor.GetComponentsInChildren<Hitbox>() != null)
        {
            Hitbox[] hitboxes = actor.GetComponentsInChildren<Hitbox>();

            foreach (Hitbox hitBox in hitboxes)
            {
                hitBox.Attack = attack;
                hitBox.Active = true;
            }
        }
    }

    public void DeactivateHitboxes(Actor actor)
    {
        Hitbox[] hitBoxes = actor.GetComponentsInChildren<Hitbox>();
        foreach (Hitbox hitBox in hitBoxes)
        {
            hitBox.Attack = null;
            hitBox.Active = false;
        }
    }
}
