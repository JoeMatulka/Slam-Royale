using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Hitbox : MonoBehaviour
{
    //Id of the hitbox
    private string m_name;
    //If the hitbox is actively attacking
    private bool m_isActive;
    //Collider for the hitbox
    private BoxCollider m_collider;
    //Actor that owns the hitbox
    private Actor m_actor;
    //Attack associated with hitbox
    private Attack m_attack;

    //Used to emit damage events from hitboxs
    public delegate void HitboxEventHandler(object sender, HitboxEventArgs e);
    private event HitboxEventHandler m_hitboxEvent;

    // Use this for initialization
    void Start()
    {
        m_name = gameObject.name;
        m_collider = GetComponent<BoxCollider>();
        m_collider.isTrigger = true;
        m_actor = GetComponentInParent<Actor>();
    }


    void OnTriggerEnter(Collider col)
    {
        Hitbox incHitBox = col.GetComponent<Hitbox>();
        if (incHitBox != null && incHitBox.Active)
        {
            if (!CheckIfHitIsValid(incHitBox))
            {
                // Return if hit is invalid
                return;
            }
            NotifiyDamage(incHitBox, incHitBox.Attack, col);
        }
    }

    private bool CheckIfHitIsValid(Hitbox hit)
    {
        return hit.Actor == null || !ReferenceEquals(hit.Actor.gameObject, Actor.gameObject);
    }

    private void NotifiyDamage(Hitbox incHitBox, Attack attack, Collider col)
    {
        HitboxEventArgs e = new HitboxEventArgs(attack, incHitBox, this, col);
        HitboxEventHandler handler = HitboxEvent;
        if (handler != null)
        {
            handler(this, e);
        }
    }

    public bool Active
    {
        get { return m_isActive; }
        set { m_isActive = value; }
    }

    public Actor Actor
    {
        get { return m_actor; }
    }

    public BoxCollider Collider
    {
        get { return m_collider; }
    }

    public string Name
    {
        get { return m_name; }
    }

    public Attack Attack
    {
        get { return m_attack; }
        set { m_attack = value; }
    }

    public HitboxEventHandler HitboxEvent
    {
        get { return m_hitboxEvent; }
        set { m_hitboxEvent = value; }
    }
}
