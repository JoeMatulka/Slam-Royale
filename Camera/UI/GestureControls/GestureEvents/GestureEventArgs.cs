using System;
using UnityEngine;

public class GestureEventArgs : EventArgs
{
    private Gesture m_gesture;
    private TouchPhase m_touchPhase;
    private int m_touchId;

    public GestureEventArgs(Gesture gesture, TouchPhase touchPhase, int touchId)
    {
        m_gesture = gesture;
        m_touchPhase = touchPhase;
        m_touchId = touchId;
    }

    public Gesture Gesture
    {
        get { return m_gesture; }
    }

    public TouchPhase TouchPhase
    {
        get { return m_touchPhase; }
    }

    public int TouchId
    {
        get { return m_touchId; }
    }
}
