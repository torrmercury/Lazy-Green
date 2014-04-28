using UnityEngine;
using System.Collections;

public class StopParticleSystem : MonoBehaviour {

	public float timeToStop = 1.0f;
	public float timeToKill = 2.5f;

	void Start () {
		Invoke("StopMe", timeToStop);
		Invoke("KillMe", timeToKill);
	}

	void StopMe(){
		particleEmitter.emit = false;
	}

	void KillMe(){
		Destroy(gameObject);
	}
}
