using UnityEngine;
using System.Collections;

/// <summary>
/// Abstract class for carefully hard-coding tutorials alright.
/// </summary>
public abstract class Tutorial : MonoBehaviour {



	// Use this for initialization
	public virtual void Start () {
		HexGrid.instance.tutorial = this;
	}
	
	// Update is called once per frame
	public virtual void Update () {
	
	}

	public virtual void playerStartTurn(SimplePlayer player) {

	}

	public virtual void processInput() {
		// Tutorials process input before the playstate updates to ensure that we can catch and prevent the player from clicking on the incorrect objects. 
	}

	public virtual void processSync() {
		// Tutorials process synchronization steps before the sync happens to ensure that tutorial text pops up at the correct time. 
	}

	public virtual void processDialogueEvent(string eventName) {
		// Dialogue will sometimes send a trigger to the tutorial
	}

	public virtual bool playerSelected(SimplePlayer player) {
		// A tutorial can cancel a player selection (by returning false).
		return true;
	}

	public virtual bool hexClicked(Vector2 gridPos) {
		// A tutorial can cancel a hex click (by returning false).
		return true;
	}

	public virtual void justEaten(SimplePlayer player) {

	}

	public virtual void justPushed(SimplePlayer player) {

	}

	public virtual void aboutToVomit(SimplePlayer player) {
	
	}

	public virtual void menuOpened() {
		// So we can do stuff when the menu opens
	}

	public virtual void moveFinished() {
		// So we can do stuff when the move finishes.
	}

	public virtual bool buttonPressed(string buttonName) {
		// A tutorial can cancel a button press (by returning false).
		return true;
	}
}
