using UnityEngine;
using System.Collections;

public class bg_move : MonoBehaviour {

	public int speed;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		float translation = Input.GetAxis("Horizontal") * speed;
		transform.Translate(translation, 0, 0);
	}
}
