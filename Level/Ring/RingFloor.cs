using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class RingFloor : MonoBehaviour
{
    private GameManager gameManager;
    // Use this for initialization
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        BoxCollider col = GetComponent<BoxCollider>();
        col.isTrigger = false;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.GetComponent<ActionManager>() != null)
        {
            ActionManager manager = col.gameObject.GetComponent<ActionManager>();
            manager.Disabled = true;
            manager.Actor.UpdateState(ActorState.DEAD);
            if (col.gameObject.name.Contains("Player"))
            {
                gameManager.GameOver();
            }
        }
    }
}
