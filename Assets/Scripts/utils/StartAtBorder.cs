using UnityEngine;
using System.Collections;

// Simple script to move an object to the border of the screen when the game is launched
public class StartAtBorder : MonoBehaviour {

	public enum BorderDirection {
		Top,
		Right,
		Bottom,
		Left
	}
	public BorderDirection borderDirection = BorderDirection.Top;

	public Vector2 offset = Vector2.zero;

	// Use this for initialization
	void Start () {
		float cameraDistance = transform.position.z - Camera.main.transform.position.z;
		Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
		float screenX = screenPosition.x;
		float screenY = screenPosition.y;
		switch(borderDirection) {
		case BorderDirection.Top:
			transform.position = Camera.main.ScreenToWorldPoint(new Vector3(screenX, Screen.height, cameraDistance))+(Vector3)offset;
			break;
		case BorderDirection.Right:
			transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, screenY, cameraDistance))+(Vector3)offset;
			break;
		case BorderDirection.Bottom:
			transform.position = Camera.main.ScreenToWorldPoint(new Vector3(screenX, 0, cameraDistance))+(Vector3)offset;
			break;
		case BorderDirection.Left:
			transform.position = Camera.main.ScreenToWorldPoint(new Vector3(0, screenY, cameraDistance))+(Vector3)offset;
			break;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
