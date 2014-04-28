using UnityEngine;
using System.Collections;

/// <summary>
/// This script allows for quick camera follows where the camera quickly pans to a target and then stops following it.
/// </summary>
public class CameraFollow : MonoBehaviour {

	protected Transform _currentTarget;
	protected bool _followingTarget;

	public float followSpeed = 10f;
	public float tolerance = 0.1f;
	

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (HexGrid.instance.isPaused)
			return;
		if (_currentTarget == null)
			_followingTarget = false;

		if (_followingTarget) {
			Vector3 clampedTarget = new Vector3(Mathf.Clamp(_currentTarget.position.x, HexGrid.instance.cameraMinX, HexGrid.instance.cameraMaxX),
			                                    Mathf.Clamp(_currentTarget.position.y, HexGrid.instance.cameraMinY, HexGrid.instance.cameraMaxY),
			                                    transform.position.z);

			Vector3 newPos = Vector3.Lerp(transform.position, clampedTarget, Time.deltaTime*followSpeed);

			transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
			// If we're close enough, stop following. 
			if (Vector2.Distance((Vector2)transform.position, (Vector2)clampedTarget) < tolerance)
				_followingTarget = false;
		}

	}

	public void followNewTarget(Transform target) {
		_followingTarget = true;
		_currentTarget = target;
	}

}
