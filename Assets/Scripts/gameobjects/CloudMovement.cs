using UnityEngine;
using System.Collections;

public class CloudMovement : MonoBehaviour {

	private float maxUpAndDown = 0.002f;
	private float speed = 150;

	public bool allowedToRespawn;
	
	protected float angle = 0;
	protected float toDegrees = Mathf.PI/180;

	void Update () {
		angle += speed * Time.deltaTime;
		if (angle > 360)
			angle -= 360;
		transform.position += new Vector3(0.01f, maxUpAndDown * (Mathf.Sin(angle * toDegrees)) / 2, 0);
	}
}
