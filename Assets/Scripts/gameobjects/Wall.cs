using UnityEngine;
using System.Collections;

public class Wall : HexGridPiece {

	// Use this for initialization
	public override void Start () {
		base.Start();
		_type = HexGridPiece.WALL_TYPE;
	}

}
