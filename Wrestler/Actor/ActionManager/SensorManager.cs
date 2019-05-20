using UnityEngine;


public class SensorManager {

    // Sensors for making contact with other GameObjects
    public GameObject Front;
    public GameObject Back;

    private const float SENSOR_LENGTH = 12f;
    private Actor m_actor;

    public SensorManager(Actor actor) {
        m_actor = actor;
    }

    public void UpdateSensors()
    {
        Vector3 pos = m_actor.transform.position;

        Vector3 fwd = m_actor.transform.TransformDirection(Vector3.forward);
        RaycastHit fwdHit;

        Vector3 bwd = m_actor.transform.TransformDirection(Vector3.back);
        RaycastHit bwdHit;

        // Get front sensor
        if (Physics.Raycast(pos, fwd, out fwdHit, SENSOR_LENGTH, m_actor.TargetLayer, QueryTriggerInteraction.Ignore))
        {
            Front = fwdHit.transform.gameObject;
        }
        else {
            Front = null;
        }

        // Get back sensor
        if (Physics.Raycast(pos, bwd, out bwdHit, SENSOR_LENGTH, m_actor.TargetLayer, QueryTriggerInteraction.Ignore))
        {
            Back = bwdHit.transform.gameObject;
        }
        else {
            Back = null;
        }
    }
}
