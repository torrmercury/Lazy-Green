using UnityEngine;
using System.Collections;

public class LittlePot : SimplePot {
	
	private ExitHexSideGUI exitHexSideGUIScript;

	// the little pot maintains a grid of where we can expand each turn
	protected static bool[,] expandedGridPositions = null;

	public static void markGridPosAsExpanded(Vector2 gridPos) {
		expandedGridPositions[(int)gridPos.x, (int)gridPos.y] = true;
	}

	public static bool isGridPosExpanded(Vector2 gridPos) {
		return expandedGridPositions[(int)gridPos.x, (int)gridPos.y];
	}

	protected bool _spawnedPorridgeThisTurn = false;
	public int minSyncsPerTurn = 6;
	protected int _syncCounter;


	public override void Start ()
	{
		exitHexSideGUIScript = GameObject.Find("SideUIExit").GetComponent<ExitHexSideGUI>();
		exitHexSideGUIScript.changeMeToLittlePot();

		if (expandedGridPositions == null) {
			expandedGridPositions = new bool[HexGrid.instance.gridWidth, HexGrid.instance.gridHeight];
		}
		base.Start ();
	}

	public override void beginTurn ()
	{
		base.beginTurn ();
		_spawnedPorridgeThisTurn = false;
		_syncCounter = minSyncsPerTurn;
		for (int i = 0; i < HexGrid.instance.gridWidth; i++) {
			for (int j = 0; j < HexGrid.instance.gridHeight; j++) {
				expandedGridPositions[i, j] = false;		
			}
		}
	}

	public override void die ()
	{
		if (_alive) {
			HexGrid.instance.onGameWin();
		}
		base.die ();
	}

	public override void syncOnTurn ()
	{
		if (!_spawnedPorridgeThisTurn && GameObject.FindGameObjectsWithTag("ExpandingPorridge").Length == 0) {
			foreach (HexGrid.HexDir dir in HexGrid.allDirs()) {
				tryToSpawnPorridge(HexGrid.gridPosFromDir(_gridPos, dir), dir);
			}
			_spawnedPorridgeThisTurn = true;
			shake ();
		}
		else if (_syncCounter <= 0 && GameObject.FindGameObjectsWithTag("ExpandingPorridge").Length == 0)
			endTurn();
		else
			shake ();

		_syncCounter--;
	}


	protected override bool tryToSpawnPorridge (Vector2 point, HexGrid.HexDir dir)
	{

		if (HexGrid.instance.inGrid(point)) {
			if (isGridPosExpanded(point))
				return false;
			markGridPosAsExpanded(point);
		}
		return base.tryToSpawnPorridge (point, dir);
	}



}
