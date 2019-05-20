using UnityEngine;

public class Ring : MonoBehaviour {
    // Hard coding these because I couldn't retrieve the world position of the ropes for some reason
    public static Vector3 RIGHT_ROPES_POS = new Vector3(-50, 10, 0);
    public static Vector3 LEFT_ROPES_POS = new Vector3(50, 10, 0);

    private const string RIGHT_ROPE_RIGHT_CONTACT_ANIM_KEY = "r_rope_r_contact";
    private const string RIGHT_ROPE_LEFT_CONTACT_ANIM_KEY = "r_rope_l_contact";
    private const string RIGHT_ROPE_ABOVE_CONTACT_ANIM_KEY = "r_rope_above_contact";
    private const string LEFT_ROPE_RIGHT_CONTACT_ANIM_KEY = "l_rope_r_contact";
    private const string LEFT_ROPE_LEFT_CONTACT_ANIM_KEY = "l_rope_l_contact";
    private const string LEFT_ROPE_ABOVE_CONTACT_ANIM_KEY = "l_rope_above_contact";

    private const string RIGHT_ROPES_ID = "ropes.right";
    private const string LEFT_ROPES_ID = "ropes.left";

    private RingRopes m_rightRopes, m_leftRopes;

    private Animator animator;

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();

        // Get the ropes
        GameObject rightGO = transform.Search(RIGHT_ROPES_ID).gameObject;
        if (rightGO != null) {
            m_rightRopes = rightGO.GetComponent<RingRopes>();
        }
        GameObject leftGO = transform.Search(LEFT_ROPES_ID).gameObject;
        if (leftGO != null)
        {
            m_leftRopes = leftGO.GetComponent<RingRopes>();
        }
    }
	
	// Update is called once per frame
	void Update () {
        animator.SetBool(RIGHT_ROPE_RIGHT_CONTACT_ANIM_KEY, m_rightRopes.rightContact);
        animator.SetBool(RIGHT_ROPE_LEFT_CONTACT_ANIM_KEY, m_rightRopes.leftContact);
        animator.SetBool(RIGHT_ROPE_ABOVE_CONTACT_ANIM_KEY, m_rightRopes.topContact);
        animator.SetBool(LEFT_ROPE_RIGHT_CONTACT_ANIM_KEY, m_leftRopes.rightContact);
        animator.SetBool(LEFT_ROPE_LEFT_CONTACT_ANIM_KEY, m_leftRopes.leftContact);
        animator.SetBool(LEFT_ROPE_ABOVE_CONTACT_ANIM_KEY, m_leftRopes.topContact);
    }
}
