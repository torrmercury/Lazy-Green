using UnityEngine;
using System.Collections;

public class ShakeObject : MonoBehaviour {


	public float shakeTime = 1.0f;
	public float shakeInterval = 0.1f;
	public float shakeAngle = 10f;
	public float minDelay = 4.0f;
	public float maxDelay = 8.0f;

	private float t = 0;
	private float shakeCounter = 0;
	private float shakeMultiplier = 1;

	protected float _startAngle;
	protected bool _shaking = false;

	void Update() {
		if (!_shaking)
			StartCoroutine(doShake());
	}
	
	IEnumerator doShake() {
		_shaking = true;
		yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
		t = 0;
		shakeCounter = 0;
		shakeMultiplier = 1;
		while (t < shakeTime) {
			yield return 0;
			t += Time.deltaTime;
			shakeCounter += Time.deltaTime;
			if (shakeCounter >= shakeInterval) {
				shakeMultiplier = -shakeMultiplier;
				transform.localRotation = Quaternion.Euler(0, 0, _startAngle+shakeAngle*shakeMultiplier);
				shakeCounter = 0;
			}
		}
		transform.localRotation = Quaternion.Euler(0, 0, _startAngle);
		onShakeEnd();
	}
	
	void onShakeEnd() {
		_shaking = false;
	}
}
