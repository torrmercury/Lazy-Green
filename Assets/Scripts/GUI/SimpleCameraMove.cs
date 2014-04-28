using UnityEngine;
using System.Collections;

public class SimpleCameraMove : MonoBehaviour {

	public int moveSpeed = 2;

	// For returning from the limits of the map.
	public float springK = 5;
	public float dampingK = 20;

	public float thresholdTolerance = 5;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (HexGrid.instance.isPaused)
			return;

		Vector2 nextPos = transform.position;
		if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
			nextPos += Vector2.up*moveSpeed*Time.deltaTime;
		if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
			nextPos -= Vector2.right*moveSpeed*Time.deltaTime;
		if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) {
			if (rigidbody2D != null) {
				nextPos += Vector2.right*moveSpeed*Time.deltaTime;


				// Pull towards the right
				//rigidbody2D.AddForce(springK*(new Vector2((HexGrid.instance.cameraMaxX+thresholdTolerance-transform.position.x), 0)));
				//rigidbody2D.AddForce(-dampingK*(new Vector2(rigidbody2D.velocity.x, 0)));
			}
			else {
				nextPos += Vector2.right*moveSpeed*Time.deltaTime;
			}
		}

		if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey (KeyCode.S))
			nextPos -= Vector2.up*moveSpeed*Time.deltaTime;

	
		if (rigidbody2D == null) {
			nextPos = new Vector2(Mathf.Clamp(nextPos.x, HexGrid.instance.cameraMinX, HexGrid.instance.cameraMaxX),
		    	                  Mathf.Clamp(nextPos.y, HexGrid.instance.cameraMinY, HexGrid.instance.cameraMaxY));
		}
#if UNITY_EDITOR || !UNITY_IPHONE
		transform.position = new Vector3(nextPos.x, nextPos.y, transform.position.z);
#endif
		// Apply the force to correct the camera if necessary
		/*if (rigidbody2D != null) {
			if (transform.position.x > HexGrid.instance.cameraMaxX) {
				rigidbody2D.AddForce(springK*(new Vector2((HexGrid.instance.cameraMaxX-transform.position.x), 0)));
				rigidbody2D.AddForce(-dampingK*(new Vector2(rigidbody2D.velocity.x, 0)));
			}
		}*/



	}

}
