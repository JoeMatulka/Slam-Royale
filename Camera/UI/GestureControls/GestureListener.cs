using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GestureListener
{
    private const float FINGER_MOVE_BUFFER = 50.0f;

    private Gesture m_gesture;

    public delegate void GestureEventHandler(object sender, GestureEventArgs e);

    private event GestureEventHandler m_gestureEvent;

    //We set them to -1 because 0 is a valid finger id
    private int m_curCtrlFingerId = -1;

    public GestureListener() { }

    private void HandleGestures(Touch touch)
    {
        switch (touch.phase)
        {
            case TouchPhase.Began:
                if (m_gesture == null)
                {
                    m_gesture = new GestureTap(touch.position);
                    GestureEventArgs e = new GestureEventArgs(m_gesture, touch.phase, touch.fingerId);
                    OnGesture(e);
                }
                break;
            case TouchPhase.Moved:
                if (m_gesture != null)
                {
                    if (m_gesture is GestureTap)
                    {
                        m_gesture = new GestureSwipe(touch.position);
                    }
                    else {
                        Vector2 direction = touch.position - m_gesture.StartLocation;

                        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                        {
                            if (direction.x > 0)
                                ((GestureSwipe)m_gesture).Direction = GestureSwipeDirection.RIGHT;
                            else
                                ((GestureSwipe)m_gesture).Direction = GestureSwipeDirection.LEFT;
                        }
                        else {
                            if (direction.y > 0)
                                ((GestureSwipe)m_gesture).Direction = GestureSwipeDirection.UP;
                            else
                                ((GestureSwipe)m_gesture).Direction = GestureSwipeDirection.DOWN;
                        }
                    }
                }
                break;
            case TouchPhase.Ended:
                if (m_gesture != null)
                {
                    // Add length if Gesture was a swipe
                    if (m_gesture is GestureSwipe) {
                        Vector2 final = touch.position - ((GestureSwipe)m_gesture).StartLocation;
                        ((GestureSwipe)m_gesture).Length = final.magnitude;
                        ((GestureSwipe)m_gesture).EndLocation = touch.position;
                        // To give buffer for twitchy fingers
                        if (((GestureSwipe)m_gesture).Length <= FINGER_MOVE_BUFFER) {
                            m_gesture = new GestureTap(m_gesture.StartLocation);
                        }
                    }
                    GestureEventArgs e = new GestureEventArgs(m_gesture, touch.phase, touch.fingerId);
                    OnGesture(e);
                    m_gesture = null;
                }
                break;
            case TouchPhase.Canceled:
                if (m_gesture != null)
                {
                    m_gesture = null;
                }
                break;
            default:
                break;
        }
    }

    public void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch[] touches = Input.touches;
            List<Touch> attackTouches = new List<Touch>();

            for (int i = 0; i < touches.Length; i++)
            {
                if (EventSystem.current.IsPointerOverGameObject(touches[i].fingerId) && touches[i].phase == TouchPhase.Began)
                {
                    m_curCtrlFingerId = touches[i].fingerId;
                }
                if (touches[i].fingerId != m_curCtrlFingerId)
                {
                    attackTouches.Add(touches[i]);
                }
            }

            //Handle first non-event gesture in
            if (attackTouches.Count > 0)
            {
                HandleGestures(attackTouches[0]);
            }
        }
        else {
            //We set them to -1 because 0 is a valid finger id
            m_curCtrlFingerId = -1;
        }
    }

    private void OnGesture(GestureEventArgs e)
    {
        GestureEventHandler handler = GestureEvent;
        if (handler != null)
        {
            handler(this, e);
        }
    }

    public GestureEventHandler GestureEvent
    {
        get { return m_gestureEvent; }
        set { m_gestureEvent = value; }
    }
}
