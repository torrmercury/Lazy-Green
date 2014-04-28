using UnityEngine;
using System.Collections;

public class ExtraMoveItem : Item {


	// This is now a toggle item.


	// This item is deactivated before the player has moved. 
	protected bool _beforeMove = true;

	protected bool _selected = false;

	protected bool _usedThisTurn = false;

	protected tk2dSprite _mySprite;


	// Use this for initialization
	void Start () {
		_mySprite = GetComponent<tk2dSprite>();
	}

	protected void setButtonColor(Color color) {
		if (_mySprite != null) {
			_mySprite.color = color;
			foreach (ShadowText text in GetComponentsInChildren<ShadowText>(true)) {
				text.setColor(color);
			}
		}
	}

	void Update() {
		if (_beforeMove && _player.movedThisTurn) {
			_beforeMove = false;
			setButtonColor(Color.white);
		}
	}


	public override void beginTurn ()
	{
		base.beginTurn ();
		_usedThisTurn = false;
		_selected = false;
		_beforeMove = true;
		setButtonColor(new Color(0.5f, 0.5f, 0.5f, 0.5f));
	}

	public override void cancelPressed()
	{
		base.cancelPressed();
		_usedThisTurn = false;
		_selected = false;
		_beforeMove = true;
		setButtonColor(new Color(0.5f, 0.5f, 0.5f, 0.5f));
	}

	public override void pickedUpByPlayer (SimplePlayer player)
	{
		base.pickedUpByPlayer (player);
		player.menuObj.GetComponent<PlayerMenu>().insertNewMenuItem(transform);
	}


	protected void unselectButton() {
		_selected = false;
		setButtonColor(Color.white);
		_player.clearMovementHexes();
	}

	protected void selectButton() {
		_selected = true;
		setButtonColor(Color.yellow);
		// Cancel the movement tiles. 


	}

	protected void finishExtraMove(SimplePlayer.MovePoint point) {
		_usedThisTurn = true;
		setButtonColor(new Color(0.5f, 0.5f, 0.5f, 0.5f));
	}

	public override void itemPressed ()
	{
		base.itemPressed ();

		if (!_player.isSyncReady() || HexGrid.instance.isPaused)
			return;

		if (_usedThisTurn || _beforeMove)
			return;

		// Check to see if we're selected or not. 
		if (!_selected) {
			selectButton();
			_player.funcOnHexClick = _player.moveToHex;
			_player.funcOnHexClick+=finishExtraMove;
			_player.onSelect();
		}
		else {
			// Can cancel our move by clicking again. 
			unselectButton();
		}
	}

}
