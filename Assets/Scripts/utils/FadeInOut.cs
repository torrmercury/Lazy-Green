using UnityEngine;
using System.Collections;

/// <summary>
/// Simple script for an object that fades in or out using the alpha channel of its primary material.
/// </summary>
public class FadeInOut : MonoBehaviour {

	protected bool _fadingOut =false, _fadingIn = false;
	public bool fadingOut {
		get { return _fadingOut; }
	}
	public bool fadingIn {
		get { return _fadingIn; }
	}


	

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
