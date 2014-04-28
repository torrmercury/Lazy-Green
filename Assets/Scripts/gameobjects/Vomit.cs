using UnityEngine;
using System.Collections;

public class Vomit : HexGridPiece {
	

	// Use this for initialization
	public override void Start () {
		_type = VOMIT_TYPE;
		base.Start();
	}

	public override void performSync ()
	{
		// If we're not in a porridge space, start shrinking
		bool onWall = false;
		foreach (HexGridPiece inhabitant in HexGrid.instance.currentGridInhabitants(_gridPos)) {
			if (inhabitant.hasType(HexGridPiece.WALL_TYPE))
				onWall = true;
		}
		if (onWall) {
			_shrinking = true;
		}
	}

}
