using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class OutOfRingSensor : MonoBehaviour
{
    void Start() {
        BoxCollider col = GetComponent<BoxCollider>();
        col.isTrigger = true;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.GetComponent<ActionManager>() != null)
        {
            ActionManager manager = col.gameObject.GetComponent<ActionManager>();
            manager.OutOfRing = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.GetComponent<ActionManager>() != null)
        {
            ActionManager manager = col.gameObject.GetComponent<ActionManager>();
            manager.OutOfRing = false;
        }
    }
}
