using UnityEngine;

public static class ActorBehavior
{
    // These values are between 0.0 to 1.0
    public static readonly float WALK_INPUT = 0.25f;
    public static readonly float RUN_INPUT = 1f;
    public static readonly float JUMP_INPUT = 1f;

    private const float RUN_BUFFER = 3f;

    public static Vector2 Act(ActorBehaviorType behavior, Actor actor, Player target, float actDistance)
    {
        Vector2 move = Vector2.zero;
        if (target == null) {
            return move;
        }
        switch (behavior)
        {
            case ActorBehaviorType.AGGRESSIVE:
                move = AggressiveMove(actor, target, actDistance);
                break;
            case ActorBehaviorType.AVOID:
                move = AvoidMove(actor, target, actDistance);
                break;
        }
        return move;
    }

    private static Vector2 AggressiveMove(Actor actor, Player target, float actDistance)
    {
        EnemyActionManager manager = actor.GetComponent<EnemyActionManager>();
        SensorManager sensors = manager.sensorManager;
        SensorManager playerSensors = manager.PlayerManager.sensorManager;
        Actor player = manager.Player;

        float x = 0, y = 0;
        float input = WALK_INPUT;
        if (GetDistToPlayer(target, actor) > actDistance || !actor.Grounded)
        {
            input = RUN_INPUT;
        }
        if (target.transform.position.x > actor.transform.position.x)
        {
            x = -input;
        }
        else {
            x = input;
        }

        if (sensors.Front != null && sensors.Front.tag.Equals("Enemy"))
        {
            x = 0;
        }

        if (sensors.Front != null && sensors.Front.tag.Equals("Enemy") &&
            (playerSensors.Front == null || playerSensors.Back == null)) {
            y = 1;
        }

        float dot = Vector3.Dot(actor.transform.forward, (player.transform.position - actor.transform.position).normalized);

        if (sensors.Front != null &&
            sensors.Front.tag.Equals("Player") &&
            player.State.Equals(ActorState.GRABBING) && 
            playerSensors.Back == null &&
            dot > 0.7f) {
            y = 1;
            // Extra help to get over actors
            actor.Rigidbody.AddForce((50 * Vector3.up), ForceMode.Impulse);
        }

        return new Vector2(x, y);
    }

    private static Vector2 AvoidMove(Actor actor, Player target, float actDistance)
    {
        EnemyActionManager manager = actor.GetComponent<EnemyActionManager>();
        SensorManager sensors = manager.sensorManager;
        float x = 0, y = 0;
        float distToPlayer = GetDistToPlayer(target, actor);

        if (distToPlayer < actDistance - RUN_BUFFER)
        {
            float input = RUN_INPUT;

            x = target.transform.position.x > actor.transform.position.x ? input : -input;

            if (sensors.Front != null && sensors.Front.tag.Equals("Enemy") && actor.Grounded)
            {
                y = 1;
            }

            if (sensors.Back != null && sensors.Back.name.Contains("rope") ||
                sensors.Front != null && sensors.Front.name.Contains("rope"))
            {
                x = 0;
                y = 0;
            }
        }
        else if (distToPlayer > actDistance + RUN_BUFFER)
        {
            float input = WALK_INPUT;
            x = target.transform.position.x < actor.transform.position.x ? input : -input;

            if (sensors.Front != null && sensors.Front.tag.Equals("Enemy"))
            {
                x = 0;
            }
        }

        return new Vector2(x, y);
    }

    private static float GetDistToPlayer(Player player, Actor actor)
    {
        return Vector3.Distance(player.transform.position, actor.transform.position);
    }
}
