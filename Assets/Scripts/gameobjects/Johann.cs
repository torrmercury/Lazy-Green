using UnityEngine;
using System.Collections;

public class Johann : SimplePlayer {

	public static bool johannSpawned = false;
	public static SimplePlayer theOneTrueJohann = null;

	protected bool _eatenThisTurn = false;

	public override void Start ()
	{
		// Make sure there's only one of us at a time.
		if (johannSpawned) {
			Destroy(gameObject);
			return;
		}

		base.Start ();
		johannSpawned = true;
		theOneTrueJohann = this;
	}

	public override void eat (HexGridPiece piece)
	{
		base.eat (piece);
		if (!_eatenThisTurn && piece.hasType(HexGridPiece.PORRIDGE_TYPE)) {
			_eatenThisTurn = true;
			funcAfterMove = onSelect;
			_movedThisTurn = false;
		}
	}

	public override void beginTurn ()
	{
		base.beginTurn ();
		_eatenThisTurn = false;
	}

	public override void cancelPressed ()
	{
		base.cancelPressed ();

		if (!isSyncReady() || HexGrid.instance.isPaused)
			return;

		_eatenThisTurn = false;
	}


}
