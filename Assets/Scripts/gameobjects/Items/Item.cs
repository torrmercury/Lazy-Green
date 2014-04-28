using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {

	public string name;
	public Sprite sprite;

	protected SimplePlayer _player;

	// Called by our owning player so that we can update our use status at the start of a turn
	public virtual void beginTurn() {

	}

	public virtual void cancelPressed() {

	}

	public virtual void pickedUpByPlayer(SimplePlayer player) {
		_player = player;
		// Implement the actual item functionality here. 
	}

	public virtual void itemPressed() {
		// What happens when we press this item's button in the menu
	}


}
