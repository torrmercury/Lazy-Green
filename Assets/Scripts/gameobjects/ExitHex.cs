using UnityEngine;
using System.Collections;

public class ExitHex : HexGridPiece {

	public SpriteRenderer childSpriteRenderer;
	public MeshRenderer childMeshRenderer;
	public MeshRenderer childShadowMeshRenderer;

	// STATIC variables to keep players across scene loads.

	public static SimplePlayer ExitPlayer0;
	public static SimplePlayer ExitPlayer1;
	public static SimplePlayer ExitPlayer2;
	
	protected ShadowText _countdownText;

	public int exitIndex = 0;

	public override void Start ()
	{
		_type = HexGridPiece.EXIT_TYPE;
		base.Start();
		_countdownText = GetComponentInChildren<ShadowText>();
	}

	public override void Update ()
	{
		base.Update ();
		// Update our text with the current turn count.
		_countdownText.setText(HexGrid.instance.turnsLeft.ToString());
	}

	public void moveMeAway(){
		childSpriteRenderer.enabled = false;
		childMeshRenderer.enabled = false;
		childShadowMeshRenderer.enabled = false;
	}

	public override void performSync ()
	{
		base.performSync ();
		// Check if a player is on top of us.

		SimplePlayer player = null;
		foreach (HexGridPiece piece in HexGrid.instance.currentGridInhabitants(_gridPos)) {
			if (piece.hasType(HexGridPiece.PLAYER_TYPE)) {
				player = piece.GetComponent<SimplePlayer>();
			}
		}

		// Now assign the global value based on our index
		switch (exitIndex) {
		case 0:
			ExitPlayer0 = player;
			break;
		case 1:
			ExitPlayer1 = player;
			break;
		case 2:
			ExitPlayer2 = player;
			break;
		}
	}

}
