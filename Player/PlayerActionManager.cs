using System.Collections;
using UnityEngine;

public class PlayerActionManager : ActionManager
{
    private GestureListener gestureListener;

    // Use this for initialization
    void Start()
    {
        gameCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameCamera>();

        Actor = GetComponent<Actor>();

        gestureListener = new GestureListener();
        gestureListener.GestureEvent += new GestureListener.GestureEventHandler(HandleGesture);

        atkManager = new WrestlerAttackManager(this);
        sensorManager = new SensorManager(Actor);
    }

    // Update is called once per frame
    void Update()
    {
        sensorManager.UpdateSensors();
        if (!Disabled)
        {
            gestureListener.HandleTouchInput();
        }
        UpdateActionAnimations();

        //Sanity check
        if (Actor.State.Equals(ActorState.DEFAULT))
        {
            Grabbee = null;
            Grabber = null;
        }
        // Clean up, can't figure why this isn't be cleared automatically
        if (CurrentAttack != null && !Actor.State.Equals(ActorState.ATTACKING))
        {
            CurrentAttack = null;
        }
        if (Actor.State.Equals(ActorState.GRABBING) && Grabbee == null)
        {
            BreakGrab(false, false);
        }
        if (Actor.State.Equals(ActorState.GRABBED))
        {
            if (Grabber == null)
            {
                BreakGrab(true, false);
            }
            if (Grabber != null &&
                Grabber.GetComponent<ActionManager>() != null &&
                Grabber.GetComponent<ActionManager>().Grabbee == null)
            {
                BreakGrab(false, false);
            }
        }
        if (Actor.State.Equals(ActorState.BEING_THROWN) &&
    Grabber != null && Grabber.State.Equals(ActorState.DEFAULT))
        {
            BreakGrab(false, false);
        }
    }

    private void HandleGesture(object sender, GestureEventArgs e)
    {
        switch (e.TouchPhase)
        {
            case TouchPhase.Began:
                break;
            case TouchPhase.Ended:
                if (!CheckCanAttack())
                {
                    if (QueuedGesture == null) {
                        QueuedGesture = e;
                    }
                    return;
                }
                TranslateGesture(e);
                break;
        }
    }

    private void TranslateGesture(GestureEventArgs e)
    {
        if (e.Gesture is GestureTap)
        {
            Actor.Rigidbody.velocity = Vector3.zero;
            if (!Actor.State.Equals(ActorState.GRABBING))
            {
                Actor.Animator.Rebind();
                CurrentAttack = atkManager.DetermineAttack(AttackType.HIT, Vector2.zero);
            }
            else {
                BreakGrab(false, true);
            }
        }
        else if (e.Gesture is GestureSwipe)
        {
            GestureSwipe gesture = e.Gesture as GestureSwipe;
            switch (gesture.Direction)
            {
                case GestureSwipeDirection.UP:
                    if (!Actor.State.Equals(ActorState.GRABBING))
                    {
                        Actor.Jump();
                    }
                    else {
                        CurrentAttack = atkManager.DetermineAttack(AttackType.THROW_GRAPPLE, (gesture.EndLocation - gesture.StartLocation).normalized);
                        Vector2 aim = CurrentAttack.Aim;
                        if (aim.x >= 0)
                        {
                            Actor.Rigidbody.rotation = Actor.FACE_LEFT;
                        }
                        else {
                            Actor.Rigidbody.rotation = Actor.FACE_RIGHT;
                        }
                    }
                    break;
                case GestureSwipeDirection.DOWN:
                    if (!Actor.State.Equals(ActorState.GRABBING))
                    {
                        CurrentAttack = atkManager.DetermineAttack(AttackType.BLOCK, Vector2.zero);
                    }
                    else {
                        CurrentAttack = atkManager.DetermineAttack(AttackType.STRIKE_GRAPPLE, Vector2.zero);
                    }
                    break;
                case GestureSwipeDirection.LEFT:
                    Actor.Rigidbody.rotation = Actor.FACE_RIGHT;
                    if (!Actor.State.Equals(ActorState.GRABBING))
                    {
                        Actor.Animator.Rebind();
                        CurrentAttack = atkManager.DetermineAttack(AttackType.GRAB, Vector2.zero);
                    }
                    else {
                        CurrentAttack = atkManager.DetermineAttack(AttackType.THROW_GRAPPLE, (gesture.EndLocation - gesture.StartLocation).normalized);
                    }
                    break;
                case GestureSwipeDirection.RIGHT:
                    Actor.Rigidbody.rotation = Actor.FACE_LEFT;
                    if (!Actor.State.Equals(ActorState.GRABBING))
                    {
                        Actor.Animator.Rebind();
                        CurrentAttack = atkManager.DetermineAttack(AttackType.GRAB, Vector2.zero);
                    }
                    else {
                        CurrentAttack = atkManager.DetermineAttack(AttackType.THROW_GRAPPLE, (gesture.EndLocation - gesture.StartLocation).normalized);
                    }
                    break;
            }
        }
    }

    public override void ActivateAttackFrame(int frame)
    {
        atkManager.ActivateAttackFrame(frame);
    }

    public override void OnDamage()
    {
        CurrentAttack = null;
        QueuedGesture = null;
        Actor.Invulnerable = false;
        Actor.LockGravity = false;
        atkManager.DeactivateHitboxes(Actor);
    }

    public override void ExecuteQueuedActions()
    {
        if (QueuedGesture != null) {
            StartCoroutine(ExecuteQueuedGesture());
        }
    }

    private IEnumerator ExecuteQueuedGesture() {
        yield return new WaitUntil(() => Actor.State.Equals(ActorState.DEFAULT));
        TranslateGesture(QueuedGesture);
        QueuedGesture = null;
    }
}
