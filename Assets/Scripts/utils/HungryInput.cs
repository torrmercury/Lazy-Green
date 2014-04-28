using UnityEngine;
using System.Collections;

public class HungryInput : MonoBehaviour {

	protected static Vector3 lastMousePos;


	protected Vector2 lastTouchCenter = Vector2.zero;

	protected static bool panMode = false;
	protected int lastNumTouches = 0;

	protected const float PAN_THRESHOLD = 5f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
#if UNITY_IPHONE && !UNITY_EDITOR
		updatePan ();
#endif

	}

	protected void updatePan() {


		float fixedX=Camera.main.transform.position.x, fixedY=Camera.main.transform.position.y;
		float cameraX=Camera.main.transform.position.x, cameraY=Camera.main.transform.position.y;

		if (cameraX < HexGrid.instance.cameraMinX) {
			fixedX = Mathf.Min(cameraX+0.15f*(HexGrid.instance.cameraMinX-cameraX), HexGrid.instance.cameraMinX);
		}
		if (cameraX > HexGrid.instance.cameraMaxX) {
			fixedX = Mathf.Max(cameraX-0.15f*(cameraX-HexGrid.instance.cameraMaxX), HexGrid.instance.cameraMaxX);
		}
		if (cameraY < HexGrid.instance.cameraMinY) {
			fixedY = Mathf.Min(cameraY+0.15f*(HexGrid.instance.cameraMinY-cameraY), HexGrid.instance.cameraMinY);
		}
		if (cameraY > HexGrid.instance.cameraMaxY) {
			fixedY = Mathf.Max(cameraY-0.15f*(cameraY-HexGrid.instance.cameraMaxY), HexGrid.instance.cameraMaxY);
		}
		Camera.main.transform.position = new Vector3(fixedX, fixedY, Camera.main.transform.position.z);


		Vector2 curTouchCenter;

		if (Input.touches.Length == 0) {
			panMode = false;
			lastNumTouches = 0;
			return;
		}
		else if (Input.touches.Length == 1 && panMode && lastNumTouches == 2) {
			lastTouchCenter = Input.GetTouch(0).position;
			lastNumTouches = 1;
			return;
		}
		else if (Input.touches.Length == 1 && panMode && lastNumTouches == 1) {
			curTouchCenter = Input.GetTouch(0).position;
			Vector2 curTouch = Camera.main.ScreenToWorldPoint(curTouchCenter);
			Vector2 lastTouch = Camera.main.ScreenToWorldPoint(lastTouchCenter);
			if (Vector2.Distance(lastTouchCenter, curTouchCenter) >= PAN_THRESHOLD) {
				Camera.main.transform.position -= (new Vector3(curTouch.x-lastTouch.x,
				                                               curTouch.y-lastTouch.y, 0));
			}
			lastTouchCenter = curTouchCenter;
			lastNumTouches = 1;
			return;
		}
		else if (Input.touches.Length != 2) {
			lastNumTouches = Input.touches.Length;
			return;
		}

		panMode = true;

		Vector2 touch1Pos = Input.GetTouch(0).position;
		Vector2 touch2Pos = Input.GetTouch(1).position;
		curTouchCenter = (touch1Pos+touch2Pos)/2.0f;
		if (lastNumTouches == 2) {
			Vector2 curTouch = Camera.main.ScreenToWorldPoint(curTouchCenter);
			Vector2 lastTouch = Camera.main.ScreenToWorldPoint(lastTouchCenter);
			if (Vector2.Distance(lastTouchCenter, curTouchCenter) >= PAN_THRESHOLD) {
				Camera.main.transform.position -= (new Vector3(curTouch.x-lastTouch.x,
					                        				   curTouch.y-lastTouch.y, 0));
			}
		}
		lastTouchCenter = curTouchCenter;

		lastNumTouches = 2;
	}

	public static bool mousePresent {
		get { 
#if UNITY_IPHONE && !UNITY_EDITOR
			return !panMode;
#else
			return true;
#endif
		}
	}

	public static Vector3 mousePosition {
		get {
#if UNITY_IPHONE && !UNITY_EDITOR
			if (Input.touches.Length == 1) {
				lastMousePos = Input.touches[0].position;
			}
			return lastMousePos;
#else
			return Input.mousePosition;
#endif
		}
	}

	public static bool mouseButtonClicked {
		get {
#if UNITY_IPHONE && !UNITY_EDITOR
			return !panMode && Input.touches.Length == 1 && Input.touches[0].phase == TouchPhase.Ended;
#else
			return Input.GetMouseButtonDown(0);
#endif
		}
	}


	// Simple input commands.

}
