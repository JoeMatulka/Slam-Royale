using UnityEngine;

[RequireComponent(typeof(Actor))]
public class ActorEffectsManager : MonoBehaviour
{
    private Actor actor;
    private bool isEnemy;

    private TrailRenderer trail;

    // Use this for initialization
    void Start()
    {
        actor = GetComponent<Actor>();

        isEnemy = actor.name.Contains("Enemy");

        trail = createActorTrail();
    }

    private TrailRenderer createActorTrail()
    {
        Material trailMaterial = isEnemy ? ResourceManager.Instance.EnemyGroundContactMat : ResourceManager.Instance.PlayerGroundContactMat;
        TrailRenderer trail = new TrailRenderer();
        trail = gameObject.AddComponent<TrailRenderer>();
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        trail.receiveShadows = false;
        trail.startWidth = 1.5f;
        trail.endWidth = .25f;
        trail.time = .75f;
        trail.material = trailMaterial; 
        trail.enabled = false;
        trail.autodestruct = true;
        return trail;
    }

    public void ToggleTrail(bool enable) {
        if (trail != null) {
            trail.enabled = enable;
            if (!enable) {
                trail.Clear();
            }
        }
    }

    public void Hit(Vector3 pos, float magnitude)
    {
        Instantiate(ResourceManager.Instance.HitSpark,
                            pos,
                            Quaternion.identity);
    }

    public void GroundContact()
    {
        Vector2 pos = new Vector2(
                actor.transform.position.x,
                actor.transform.position.y - (actor.Collider.height / 2)
            );
        Quaternion rot = Quaternion.Euler(90, 0, 0);
        if (isEnemy)
        {
            Instantiate(ResourceManager.Instance.EnemyGroundContact,
                            pos,
                            rot);
        }
        else {
            Instantiate(ResourceManager.Instance.PlayerGroundContact,
                            pos,
                            rot);
        }
    }
}
