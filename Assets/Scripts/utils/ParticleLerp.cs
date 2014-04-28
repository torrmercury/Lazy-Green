using UnityEngine;
using System.Collections;

public class ParticleLerp : MonoBehaviour {

	private Vector3 endPos;

	void Start() {
		endPos = GameObject.Find("OlgaStomach").GetComponent<Transform>().transform.position;
	}

	void Update(){
		ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleSystem.particleCount];
		int count = particleSystem.GetParticles(particles);
		for (int i=0; i<count; i++) {
			particles[i].position = Vector3.Lerp(particles[i].position, endPos , Time.deltaTime * 4);
		}
		particleSystem.SetParticles(particles, count);
	}
}
