using UnityEngine;

public class GameBounds : MonoBehaviour
{

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.GetComponent<Actor>() != null)
        {
            Actor actor = col.gameObject.GetComponent<Actor>();
            actor.Rigidbody.velocity = Vector3.zero;
            actor.Collider.isTrigger = false;
        }
    }
}
