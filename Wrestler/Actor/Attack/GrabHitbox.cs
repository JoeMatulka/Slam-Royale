using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class GrabHitbox : MonoBehaviour
{
    private static Vector3 DEF_CENTER = new Vector3(0, 0, 2.5f);
    private static Vector3 DEF_SIZE = new Vector3(2, 5, 2f);

    private Actor m_actor;
    private ActionManager m_manager;
    private BoxCollider m_collider;


    // Use this for initialization
    void Start()
    {
        m_actor = GetComponentInParent<Actor>();
        if (m_actor != null)
        {
            m_manager = m_actor.GetComponent<ActionManager>();
        }

        m_collider = GetComponent<BoxCollider>();
        m_collider.center = DEF_CENTER;
        m_collider.size = DEF_SIZE;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_manager.Grabbee != null && !m_manager.Grabbee.State.Equals(ActorState.BEING_THROWN))
        {
            Physics.IgnoreCollision(m_actor.Collider, m_collider, true);
            m_collider.isTrigger = false;
        }
        else {
            m_collider.isTrigger = true;
        }
    }
}
