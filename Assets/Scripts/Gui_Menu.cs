using UnityEngine;
using System.Collections;

public class Gui_Menu : MonoBehaviour {

	public string text_here;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {
		// Container Box
		GUI.Box(new Rect(10,10,200,120), "Menu");
		
		// Making buttons
		if(GUI.Button(new Rect(20,40,80,20), "Button 1")) {
			Debug.Log ("button 1");
		}
		// Make the second button.
		if(GUI.Button(new Rect(20,70,80,20), "Button 2")) {
			Debug.Log ("button 2");
		}
		// Repeat
		if(GUI.Button(new Rect(120,40,80,20), "Button 3")) {
			Debug.Log ("button 3");
		}
		// Repeat
		if(GUI.Button(new Rect(120,70,80,20), "Button 4")) {
			Debug.Log ("button 4");
		}
		//Text Area
		GUI.Box(new Rect(15, 95, 190, 30), text_here);

	}
}
