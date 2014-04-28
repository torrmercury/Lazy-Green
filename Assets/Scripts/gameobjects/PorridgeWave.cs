using UnityEngine;
using System.Collections;

public class PorridgeWave : MonoBehaviour {

	public float radius = 2f;
	public float period = 1f;

	protected float _startTime;
	protected float _omega;
	protected Vector2 _startPos;

	// Use this for initialization
	void Start () {
		_startTime = Time.time;
		_omega = 2*Mathf.PI/period;
		_startPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		float t = Time.time - _startTime;
		float x = _startPos.x + radius*Mathf.Cos(_omega*t);
		float y = _startPos.y + radius*Mathf.Sin(_omega*t);
		transform.position = new Vector3(x, y, transform.position.z);
	}
}
