using UnityEngine;
using System.Collections;

/// <summary>
/// Just a simple script to scale an object by a sin wave
/// </summary>
public class SinScale : MonoBehaviour {

	public float period = 1f;
	public float A = 1f;

	protected float _omega;

	protected float _startScale;

	// Use this for initialization
	void Start () {
		_startScale = transform.localScale.x;
		_omega = 2*Mathf.PI / period;
	}
	
	// Update is called once per frame
	void Update () {
		float scale = _startScale + A*Mathf.Sin(_omega*Time.time);
		transform.localScale = Vector3.one*scale;
	}
}
