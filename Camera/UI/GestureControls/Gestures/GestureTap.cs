using UnityEngine;

public class GestureTap : Gesture
{
    private float m_duration;

    public GestureTap(Vector2 location)
    {
        StartLocation = location;
    }

    public float Duration
    {
        get { return m_duration; }
        set { m_duration = value; }
    }
}
