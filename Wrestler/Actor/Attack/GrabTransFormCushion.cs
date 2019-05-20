using UnityEngine;

/**
* Class used to push actors out from between two grabbed actors
**/
[RequireComponent(typeof(BoxCollider))]
public class GrabTransFormCushion : MonoBehaviour
{
    private static Vector3 DEF_CENTER = new Vector3(0, 0, 1f);
    private static Vector3 DEF_SIZE = new Vector3(1, 5f, 5f);

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
        m_collider.isTrigger = true;
    }

    private void MoveActor(Collider col)
    {
        if (m_actor.State.Equals(ActorState.GRABBING))
        {
            Actor actor = col.gameObject.GetComponent<Actor>();
            if (actor != null && !actor.ID.Equals(m_manager.Grabbee.ID))
            {
                actor.Rigidbody.isKinematic = true;
                if (actor.transform.position.x > m_actor.transform.position.x)
                {
                    actor.transform.position = new Vector2(
                                    actor.transform.position.x + (DEF_SIZE.z / 2.5f),
                                    actor.transform.position.y
                                );
                }
                else {
                    actor.transform.position = new Vector2(
                                    actor.transform.position.x - (DEF_SIZE.z / 2.5f),
                                    actor.transform.position.y
                                );
                }
                actor.Rigidbody.isKinematic = false;
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        MoveActor(col);
    }

    void OnTriggerStay(Collider col)
    {
       MoveActor(col);
    }
}
