using UnityEngine;
using System.Collections;

public class ScreenShake : MonoBehaviour {


	protected bool _shaking = false;

	public void bigShake() {
		shake(0.8f, 0.4f, false);
	}

	public void shake(float shakePower, float time, bool vibrate) {
		if (!_shaking) {
			StartCoroutine(doShake(shakePower, time, vibrate));
		}
	}

	
	protected IEnumerator doShake(float shakePower, float time, bool vibrate) {
		// Vibrate if we're on a phone!
#if UNITY_IPHONE
		if (vibrate)
			Handheld.Vibrate();
#endif
		_shaking = true;
		Vector3 startPos = transform.position;
		// Since this will probably run during the Time.timescale == 0, gonna have to use real time
		float startTime = Time.realtimeSinceStartup;


		while ((Time.realtimeSinceStartup-startTime) < time) {
			transform.position = startPos + Vector3.left*Random.Range(-shakePower, shakePower) + Vector3.up*Random.Range(-shakePower, shakePower);
			yield return 0;
		}
		
		transform.position = startPos;
		_shaking = false;
	}
}
