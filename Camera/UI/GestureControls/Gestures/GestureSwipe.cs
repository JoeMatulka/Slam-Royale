using UnityEngine;

public class GestureSwipe : Gesture
{
    private GestureSwipeDirection m_direction;

    private float m_length;

    private Vector2 m_endLocation = new Vector2();

    public GestureSwipe(Vector2 location)
    {
        StartLocation = location;
    }

    public GestureSwipeDirection Direction
    {
        get { return m_direction; }
        set { m_direction = value; }
    }

    public float Length
    {
        get { return m_length; }
        set { m_length = value; }
    }

    public Vector2 EndLocation {
        get { return m_endLocation; }
        set { m_endLocation = value; }
    }
}
