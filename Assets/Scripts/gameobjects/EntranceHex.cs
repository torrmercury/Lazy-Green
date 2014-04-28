using UnityEngine;
using System.Collections;

public class EntranceHex : HexGridPiece {

	public int entranceIndex = 0;

	public override void Start ()
	{
		base.Start ();
		_spriteRender.enabled = false;

		// Move the appropriate player to this entrance on the level load. 
		SimplePlayer maybePlayer = null;
		switch(entranceIndex) {
		case 0:
			maybePlayer = ExitHex.ExitPlayer0;
			break;
		case 1:
			maybePlayer = ExitHex.ExitPlayer1;
			break;
		case 2:
			maybePlayer = ExitHex.ExitPlayer2;
			break;
		}
		if (maybePlayer != null) {	

			maybePlayer.transform.parent = HexGrid.instance.transform;
			maybePlayer.x = x;
			maybePlayer.y = y;

			HexGrid.instance.addPiece(maybePlayer);
			maybePlayer.attachMenuToBottom();
			maybePlayer.beginTurn();

			Camera.main.GetComponent<CameraFollow>().followNewTarget(maybePlayer.transform);

		}

	}
}
