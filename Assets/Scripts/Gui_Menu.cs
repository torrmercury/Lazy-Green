using UnityEngine;
using System.Collections;

public class Gui_Menu : MonoBehaviour {

	public string text_here;
	public string location_here;

	public bool gui_show = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}

	//turn GUI on when entering a specific area AND Vert axis is positive
	//NEEDS TRIGGERS!!!
	void OnTriggerStay () {
		if (Input.GetAxis ("Vertical") > 0) {
				gui_show = true;
		}

		if (Input.GetAxis ("Vertical") < 0) {
				gui_show = false;
		}
	}

	//Turn GUI off when exiting area
	//NEEDS TRIGGERS!!!
	void OnTriggerExit () {
		gui_show = false;
	}

	void OnGUI() {

		if (gui_show == true) {

			// Container Box
			GUI.Box (new Rect (15, 30, 475, 600), location_here);

			// Making buttons
			if (GUI.Button (new Rect (25, 80, 225, 125), "Button 1")) {
					Debug.Log ("button 1");
			}
			// Make the second button.
			if (GUI.Button (new Rect (25, 215, 225, 125), "Button 2")) {
					Debug.Log ("button 2");
			}
			// Repeat
			if (GUI.Button (new Rect (255, 80, 225, 125), "Button 3")) {
					Debug.Log ("button 3");
			}
			// Repeat
			if (GUI.Button (new Rect (255, 215, 225, 125), "Button 4")) {
					Debug.Log ("button 4");
			}
			//Text Area
			GUI.Box (new Rect (20, 350, 460, 270), text_here);

		}
	}
}
