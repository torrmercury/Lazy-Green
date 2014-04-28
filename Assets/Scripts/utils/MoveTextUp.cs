using UnityEngine;
using System.Collections;

public class MoveTextUp : MonoBehaviour {

	private FlashText flashTextScript;
	private bool switchMe;

	void Start () {
		switchMe = false;
	}

	void Update () {
		transform.position = transform.position + new Vector3(0, 0.04f, 0);

		if(!switchMe){
			switchMe = true;
			gameObject.GetComponent<FlashText>().startFadeOut();
		}

	}

}
