using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Werner : SimplePlayer {


	protected bool _initiatingVomit = false;

	public static bool wernerSpawned = false;

	public static SimplePlayer theOneTrueWerner = null;

	// Use this for initialization
	public override void Start () {
		if (wernerSpawned) {
			Destroy(gameObject);
			return;
		}

		base.Start();
		wernerSpawned = true;
		theOneTrueWerner = this;
	}
	
	public virtual void vomitPressed() {
		audio.PlayOneShot(clickSound);
		// Vomit up all of our stomach
		if (currentStomach > 0) {
			_initiatingVomit = true;
			menuObj.SetActive(false);
			determineVomitTiles();
			foreach (MovePoint point in _highlightedPoints) {
				HexGrid.instance.highlightGridPos(point.point, point.sprite);
			}
			funcOnHexClick = vomitHexChosen;
		}
	}

	protected void vomitHexChosen(MovePoint point) {
		onDeselect();
		selectedPlayer = null;

		foreach (HexGridPiece piece in _justEatenPieces) {
			piece.die();
		}
		_justEatenPieces.Clear();

		_initiatingVomit = false;
		_nextVomitDir = HexGrid.dirBetweenPoints(_gridPos, point.point);
		_currentVomitCount = currentStomach;
		_currentStomach = 0;
		//_currentHealth--;
		//if (_currentHealth <= 0)
		//	die ();
	}

	protected override void openMenu ()
	{
		base.openMenu ();
		_initiatingVomit = false;
	}

	protected override bool aboutToVomit ()
	{
		return base.aboutToVomit () || _initiatingVomit;
	}

	protected void determineVomitTiles() {
		_gridPos = HexGrid.instance.toGridCoord(x, y);
		_highlightedPoints.Clear();
		// Let's let the player choose any direction to start the vomit for now.
		foreach (Vector2 neighbor in HexGrid.getNeighbors(_gridPos)) {
			if (HexGrid.instance.inGrid(neighbor))
				_highlightedPoints.Add(new MovePoint(neighbor, null, HexGrid.instance.greenHighlight));
		}

	}


	public override void eat (HexGridPiece piece)
	{
		base.eat(piece);
		if ( piece.hasType(HexGridPiece.VOMIT_TYPE)) {
			funcAfterMove = onSelect;
		}
	}


	protected override bool pieceIsEdible (HexGridPiece piece)
	{
		return base.pieceIsEdible (piece) || piece.hasType(HexGridPiece.VOMIT_TYPE);
	}

	protected override bool containsPorridge(Vector2 point) {
		foreach (HexGridPiece inhabitant in HexGrid.instance.currentGridInhabitants(point)) {
			if (inhabitant.hasType(HexGridPiece.PORRIDGE_TYPE | HexGridPiece.POT_TYPE | HexGridPiece.VOMIT_TYPE))
				return true; 
		}
		return false;
	}

	public override bool canMoveThroughPiece (HexGridPiece claimer)
	{
		return claimer.hasType(HexGridPiece.POT_TYPE | HexGridPiece.PLAYER_TYPE | HexGridPiece.BARRICADE_TYPE) || !claimer.hasType(HexGridPiece.WALL_TYPE);
	}

}
