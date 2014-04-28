using UnityEngine;
using System.Collections;

public class AutoDestructParticle : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (!particleSystem.IsAlive())
			Destroy(gameObject);
	}
}
