using UnityEngine;
using System.Collections;

public class MoveExitHexAway : MonoBehaviour {

	private float maxUpAndDown = 0.01f;
	private float speed = 150;
	
	protected float angle = 0;
	protected float toDegrees = Mathf.PI/180;

	[HideInInspector] public string directionToMove;
	
	void Awake(){
		directionToMove = HexGrid.instance.toMove;
	}

	void Update(){
		angle += speed * Time.deltaTime;
		if (angle > 360)
			angle -= 360;
		if(directionToMove == "Up"){
			transform.localPosition += new Vector3(maxUpAndDown * (Mathf.Sin(angle * toDegrees)) / 2, 0.03f, 0);
		} else if(directionToMove == "Down"){
			transform.localPosition += new Vector3(maxUpAndDown * (Mathf.Sin(angle * toDegrees)) / 2, -0.03f, 0);
		} else if(directionToMove == "Left"){
			transform.localPosition += new Vector3(-0.03f, maxUpAndDown * (Mathf.Sin(angle * toDegrees)) / 2, 0);
		} else if(directionToMove == "Right"){
			transform.localPosition += new Vector3(0.03f, maxUpAndDown * (Mathf.Sin(angle * toDegrees)) / 2, 0);
		}
	}

}