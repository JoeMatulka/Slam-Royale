using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class RingRopes : MonoBehaviour
{

    private BoxCollider m_collider;

    public bool topContact = false;

    public bool leftContact = false;

    public bool rightContact = false;

    private const float ROPE_FORCE = 32f;

    /** Move the actor if they grab another actor on the ropes, 
    *   this is mainly a bug fix to prevent actors to push each other through the ropes

        This is calculated by 2f (location of grab hit box) + 1 (size of hitbox)
    */
    private const float ROPE_GRAB_CUSHION = 3f;

    void Start()
    {
        m_collider = GetComponent<BoxCollider>();
        m_collider.isTrigger = true;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.GetComponent<Actor>() != null)
        {
            Actor actor = col.gameObject.GetComponent<Actor>();

            if (actor.transform.position.x > actor.transform.TransformPoint(transform.position).x)
            {
                rightContact = true;
            }
            else {
                leftContact = true;
            }

            if (actor.State.Equals(ActorState.BEING_THROWN)) {
                Actor grabber = actor.GetComponent<ActionManager>().Grabber;
                if (grabber != null) {
                    actor = grabber;
                }
            }

            if (!actor.State.Equals(ActorState.GRABBED) && !actor.State.Equals(ActorState.GRABBING))
            {
                // Apply force into opposite direction
                Vector3 opposite = -actor.Rigidbody.velocity;
                if (opposite.x == 0) {
                    float force = gameObject.name.Contains("right")? 5 : -5;
                    opposite = new Vector3(
                            HedgeForceValues(opposite.x + force),
                            HedgeForceValues(opposite.y - Mathf.Abs(force)),
                            HedgeForceValues(opposite.z)
                        );
                }
                Vector3 ropeForce = opposite * ROPE_FORCE;

                if (ropeForce.x > 0)
                {
                    actor.Rigidbody.rotation = !actor.State.Equals(ActorState.DEFAULT) ?
                        Actor.FACE_LEFT : Actor.FACE_RIGHT;
                }
                else {
                    actor.Rigidbody.rotation = !actor.State.Equals(ActorState.DEFAULT) ?
                        Actor.FACE_RIGHT : Actor.FACE_LEFT;
                }
                
                actor.Rigidbody.AddForce(ropeForce, ForceMode.Impulse);
            }
            else if (actor.State.Equals(ActorState.GRABBED) && actor.Grounded)
            {
                // Very hacky, need to find a better way to protect against actors pushing themselves through ropes
                ActionManager manager = actor.GetComponent<ActionManager>();
                if (manager != null)
                {
                    Actor grabber = manager.Grabber;
                    if (gameObject.name.Contains("right"))
                    {
                        grabber.transform.position = new Vector2(
                                grabber.transform.position.x + ROPE_GRAB_CUSHION,
                                grabber.transform.position.y
                            );
                    }
                    else if (gameObject.name.Contains("left"))
                    {
                        grabber.transform.position = new Vector2(
                                grabber.transform.position.x - ROPE_GRAB_CUSHION,
                                grabber.transform.position.y
                            );
                    }
                }
            }
            else if (actor.State.Equals(ActorState.GRABBING) && actor.Grounded)
            {
                // Very hacky, need to find a better way to protect against actors pushing themselves through ropes
                if (gameObject.name.Contains("right"))
                {
                    actor.transform.position = new Vector2(
                            actor.transform.position.x + ROPE_GRAB_CUSHION,
                            actor.transform.position.y
                        );
                }
                else if (gameObject.name.Contains("left"))
                {
                    actor.transform.position = new Vector2(
                                actor.transform.position.x - ROPE_GRAB_CUSHION,
                                actor.transform.position.y
                            );
                }
            }
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.GetComponent<Actor>() != null)
        {
            Actor actor = col.gameObject.GetComponent<Actor>();

            if (actor.State.Equals(ActorState.BEING_THROWN))
            {
                Actor grabber = actor.GetComponent<ActionManager>().Grabber;
                if (grabber != null)
                {
                    actor = grabber;
                    if (gameObject.name.Contains("right"))
                    {
                        actor.transform.position = new Vector2(
                                actor.transform.position.x + ROPE_GRAB_CUSHION,
                                actor.transform.position.y
                            );
                    }
                    else if (gameObject.name.Contains("left"))
                    {
                        actor.transform.position = new Vector2(
                                    actor.transform.position.x - ROPE_GRAB_CUSHION,
                                    actor.transform.position.y
                                );
                    }
                }
            }

            if (actor.State.Equals(ActorState.GRABBED) && actor.Grounded)
            {
                // Very hacky, need to find a better way to protect against actors pushing themselves through ropes
                ActionManager manager = actor.GetComponent<ActionManager>();
                if (manager != null)
                {
                    Actor grabber = manager.Grabber;
                    if (gameObject.name.Contains("right"))
                    {
                        grabber.transform.position = new Vector2(
                                grabber.transform.position.x + ROPE_GRAB_CUSHION,
                                grabber.transform.position.y
                            );
                    }
                    else if (gameObject.name.Contains("left"))
                    {
                        grabber.transform.position = new Vector2(
                                grabber.transform.position.x - ROPE_GRAB_CUSHION,
                                grabber.transform.position.y
                            );
                    }
                }
            }
            else if (actor.State.Equals(ActorState.GRABBING) && actor.Grounded)
            {
                // Very hacky, need to find a better way to protect against actors pushing themselves through ropes
                if (gameObject.name.Contains("right"))
                {
                    actor.transform.position = new Vector2(
                            actor.transform.position.x + ROPE_GRAB_CUSHION,
                            actor.transform.position.y
                        );
                }
                else if (gameObject.name.Contains("left"))
                {
                    actor.transform.position = new Vector2(
                                actor.transform.position.x - ROPE_GRAB_CUSHION,
                                actor.transform.position.y
                            );
                }
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        topContact = false;
        leftContact = false;
        rightContact = false;
    }

    private float HedgeForceValues(float force) {
        if (force >= Actor.JUMP_POWER) {
            force = Actor.JUMP_POWER;
        }
        return force;
    }
}
