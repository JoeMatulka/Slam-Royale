using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class SplashDamage : MonoBehaviour
{

    private BoxCollider col;

    private Damage dmg;

    private Actor[] ignoredActors;

    private Actor sourceActor;

    // Use this for initialization
    void Start()
    {
        col = GetComponent<BoxCollider>();
        col.isTrigger = true;
    }

    public void Spawn(Damage damage, Actor[] ignored, Actor source)
    {
        dmg = damage;
        dmg.Hurt /= 2f;
        ignoredActors = ignored;
        sourceActor = source;

        // For some reason, I have to re-retrieve this collider when I call this after instantiation
        col = GetComponent<BoxCollider>();

        Physics.IgnoreCollision(source.Collider, col, true);

        for (int i = 0; i < ignoredActors.Length; i++)
        {
            Physics.IgnoreCollision(ignoredActors[i].Collider, col, true);
        }

        col.size = new Vector3(
            dmg.Hurt,
            5,
            5
        );

        Invoke("Despawn", .25f);
    }

    private void Despawn()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.GetComponent<Actor>() != null)
        {
            Actor actor = col.gameObject.GetComponent<Actor>();

            actor.Damage(AttackType.HIT, dmg, sourceActor, col.transform.position);
        }
    }
}
