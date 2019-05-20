using UnityEngine;

public class ParticleLife : MonoBehaviour {
	ParticleSystem[] ps;


	// Use this for initialization
	void Start () {
		ps = GetComponentsInChildren<ParticleSystem> ();
	}
	
	// Update is called once per frame
	void Update () {
		for(int i = 0; i < ps.Length; i++) {
			if (!ps [i].IsAlive()) {
				Destroy (gameObject);
			}
		}
	}
}
