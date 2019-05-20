using UnityEngine;

public abstract class Gesture
{
    private Vector2 m_startLocation;

    public Vector2 StartLocation
    {
        get { return m_startLocation; }
        set { m_startLocation = value; }
    }
}