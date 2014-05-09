using UnityEngine;
using System.Collections;

public class Gui_Menu : MonoBehaviour {

	public string text_here;
	public string location_here;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {
		// Container Box
		GUI.Box(new Rect(15,30,475,600), location_here);
		
		// Making buttons
		if(GUI.Button(new Rect(25,80,225,125), "Button 1")) {
			Debug.Log ("button 1");
		}
		// Make the second button.
		if(GUI.Button(new Rect(25,215,225,125), "Button 2")) {
			Debug.Log ("button 2");
		}
		// Repeat
		if(GUI.Button(new Rect(255,80,225,125), "Button 3")) {
			Debug.Log ("button 3");
		}
		// Repeat
		if(GUI.Button(new Rect(255,215,225,125), "Button 4")) {
			Debug.Log ("button 4");
		}
		//Text Area
		GUI.Box(new Rect(20, 350, 460, 270), text_here);

	}
}
