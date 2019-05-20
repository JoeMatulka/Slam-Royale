using UnityEngine;

public class WrestlerAttackManager : AttackManager
{
    // Attack for colliding actors
    public static Attack CONTACT = new Attack(AttackType.HIT, null);

    private static Attack THROW = new Attack(AttackType.THROW_GRAPPLE, new ActiveAttackFrame[]{
        new ActiveAttackFrame(0, null, Vector2.zero, false, true, true, false),
        new ActiveAttackFrame(20, new string[] {"throw"},
            Vector2.zero, false, true, true, false),
        new ActiveAttackFrame(30, null, Vector2.zero, false, false, false, true),
        });
    private static Attack BLOCK = new Attack(AttackType.BLOCK, new ActiveAttackFrame[]{
        new ActiveAttackFrame(0, null, Vector2.zero, false, true, true, false),
        new ActiveAttackFrame(10, null, Vector2.zero, false, false, false, true),
        });

    /**
    * GROUND ATTACKS
    */
    private static Attack PUNCH = new Attack(AttackType.HIT, new ActiveAttackFrame[]{
        new ActiveAttackFrame(0, null, Vector2.zero, false, false, true, false),
        new ActiveAttackFrame(11, new string[] {"upper_arm.L", "forearm.L", "hand.L", "chest"},
            new Vector2(40f, 0), false, false, true, false),
        new ActiveAttackFrame(13, null, Vector2.zero, false, false, true, false),
        new ActiveAttackFrame(20, null, Vector2.zero, false, false, false, true),
        });
    private static Attack GRND_GRAB = new Attack(AttackType.GRAB, new ActiveAttackFrame[]{
        new ActiveAttackFrame(0, null, Vector2.zero, false, false, true, false),
        new ActiveAttackFrame(19, new string[] {"chest", "upper_arm.L", "forearm.L",
            "upper_arm.R", "forearm.R"}, new Vector2(50f, 0), false, false, true, false),
        new ActiveAttackFrame(22, new string[] {"chest", "upper_arm.L", "forearm.L",
            "upper_arm.R", "forearm.R"}, Vector2.zero, false, false, true, false),
        new ActiveAttackFrame(23, null, Vector2.zero, false, false, false, true),
        });
    private static Attack SUPLEX = new Attack(AttackType.STRIKE_GRAPPLE, new ActiveAttackFrame[]{
        new ActiveAttackFrame(0, null, Vector2.zero, false, true, false, false),
        new ActiveAttackFrame(20, new string[] {"throw"}, Vector2.zero, false, true, false, false),
        new ActiveAttackFrame(40, null, Vector2.zero, false, false, false, true),
        });


    /**
    * AIR ATTACKS
    */
    private static Attack DROP_KICK = new Attack(AttackType.HIT, new ActiveAttackFrame[]{
        new ActiveAttackFrame(0, null, Vector2.zero, false, false, true, false),
        new ActiveAttackFrame(14, new string[] {"shin.L", "foot.L", "shin.R", "foot.R"},
            new Vector2(250f, 0), false, false, true, false),
        new ActiveAttackFrame(16, null, Vector2.zero, false, false, false, false),
        new ActiveAttackFrame(20, null, Vector2.zero, false, false, false, true),
        });
    private static Attack AIR_GRAB = new Attack(AttackType.GRAB, new ActiveAttackFrame[]{
        new ActiveAttackFrame(0, null, Vector2.zero, false, false, true, false),
        new ActiveAttackFrame(12, new string[] {"chest", "upper_arm.L", "forearm.L",
            "upper_arm.R", "forearm.R", }, new Vector2(150f, 0), false, false, true, false),
        new ActiveAttackFrame(17, new string[] {"chest", "upper_arm.L", "forearm.L",
            "upper_arm.R", "forearm.R"}, Vector2.zero, false, false, false, false),
        new ActiveAttackFrame(18, null, Vector2.zero, false, false, false, true),
        });
    private static Attack PILE_DRIVER = new Attack(AttackType.STRIKE_GRAPPLE, new ActiveAttackFrame[]{
        new ActiveAttackFrame(0, null, Vector2.zero, false, true, true, false),
        new ActiveAttackFrame(11, null, Vector2.zero, false, true, false, false),
        });

    // FORCES AND DAMAGES
    public static float THROW_DMG = 5f;
    public static float BASE_THROW_Y_FORCE = 400f;
    public static float BASE_THROW_X_FORCE = 40f;
    public static float STRIKE_THROW_DMG = 25f;
    public static Vector2 STRIKE_THROW_FORCE = new Vector2(30f, 15f);
    public const float PUNCH_DMG = 12.5f;
    public static Vector2 PUNCH_FORCE = new Vector2(40f, 0);
    public const float DROP_KICK_DMG = 30f;
    public static Vector2 DROP_KICK_FORCE = new Vector2(75f, 25f);

    public WrestlerAttackManager(ActionManager manager)
    {
        Manager = manager;
    }

    public Attack DetermineAttack(AttackType type, Vector2 aim)
    {
        Manager.Actor.Rigidbody.velocity = Vector3.zero;
        Manager.Actor.UpdateState(ActorState.ATTACKING);
        
        Attack attack = null;
        switch (type)
        {
            case AttackType.GRAB:
                attack = Manager.Actor.Grounded ? GRND_GRAB : AIR_GRAB;
                break;
            case AttackType.BLOCK:
                attack = BLOCK;
                break;
            case AttackType.THROW_GRAPPLE:
                attack = THROW;
                break;
            case AttackType.STRIKE_GRAPPLE:
                if (Manager.Actor.Grounded)
                {
                    attack = SUPLEX;
                    attack.Damage = new Damage(STRIKE_THROW_DMG, STRIKE_THROW_FORCE);
                }
                else {
                    attack = PILE_DRIVER;
                    // Modify Damage based off of height of the attack
                    float modDmg = (STRIKE_THROW_DMG * Manager.Actor.transform.position.y) / 5.5f;
                    modDmg = modDmg <= STRIKE_THROW_DMG ? STRIKE_THROW_DMG : modDmg;
                    
                    attack.Damage = new Damage(modDmg, STRIKE_THROW_FORCE);
                }
                break;
            default:
            case AttackType.HIT:
                if (Manager.Actor.Grounded)
                {
                    attack = PUNCH;
                    attack.Damage = new Damage(PUNCH_DMG, PUNCH_FORCE);
                }
                else {
                    attack = DROP_KICK;
                    attack.Damage = new Damage(DROP_KICK_DMG, DROP_KICK_FORCE);
                }
                break;
        }
        attack.Aim = aim;
        return attack;
    }
}
