using UnityEngine;
using System.Collections;

public class LinearPorridgePipe : SimplePot {

	public HexGrid.HexDir pipeDirection;

	protected bool _spawnedThisTurn;

	public int minSyncsPerTurn = 6;
	protected int _currentSyncCounter;

	public override void beginTurn ()
	{
		base.beginTurn ();
		_spawnedThisTurn = false;
		_currentSyncCounter = minSyncsPerTurn;
	}

	public override void syncOnTurn ()
	{

		if (!_spawnedThisTurn) {
			tryToSpawnPorridge(HexGrid.gridPosFromDir(_gridPos, pipeDirection), pipeDirection);
			// Start shaking
			shake();
			_spawnedThisTurn = true;
		}
		else if (GameObject.FindGameObjectsWithTag("ExpandingPorridge").Length == 0 && _currentSyncCounter <= 0)
			endTurn();
		else
			shake ();

		_currentSyncCounter--;
	}


}
