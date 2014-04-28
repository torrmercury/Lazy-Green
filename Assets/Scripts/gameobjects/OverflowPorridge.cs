using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OverflowPorridge : SimplePorridge {

	protected static IEnumerable<HexGrid.HexDir> overflowDirs(HexGrid.HexDir dir) {
		switch (dir) {
		case HexGrid.HexDir.UpRight:
			yield return HexGrid.HexDir.UpRight;
			yield return HexGrid.HexDir.Right;
			break;
		case HexGrid.HexDir.Right:
			yield return HexGrid.HexDir.Right;
			yield return HexGrid.HexDir.DownRight;
			break;
		case HexGrid.HexDir.DownRight:
			yield return HexGrid.HexDir.DownRight;
			yield return HexGrid.HexDir.DownLeft;
			break;
		case HexGrid.HexDir.DownLeft:
			yield return HexGrid.HexDir.DownLeft;
			yield return HexGrid.HexDir.Left;
			break;
		case HexGrid.HexDir.Left:
			yield return HexGrid.HexDir.Left;
			yield return HexGrid.HexDir.UpLeft;
			break;
		case HexGrid.HexDir.UpLeft:
			yield return HexGrid.HexDir.UpLeft;
			yield return HexGrid.HexDir.UpRight;
			break;
		}
	}


	public override void performSync ()
	{
		if (!_spawnedPorridgeThisTurn && _expandingNextTurn) {
			gameObject.tag = _startTag;
			_spriteRender.sprite = doubleStackSprite;
			_doubleStacked = true;
			_expandingNextTurn = false;
		}
		else if (!_spawnedPorridgeThisTurn && _spawningPorridgeNextTurn) {
			gameObject.tag = _startTag;

			foreach (HexGrid.HexDir dir in overflowDirs(_nextPorridgeDir)) {
				tryToSpawnPorridge(HexGrid.gridPosFromDir(_gridPos, dir), dir);
			}

			_spawningPorridgeNextTurn = false;
		}
	}

	protected override void tryToSpawnPorridge (Vector2 point, HexGrid.HexDir dir)
	{
		if (HexGrid.instance.inGrid(point)) {
			if (LittlePot.isGridPosExpanded(point))
				return;
			LittlePot.markGridPosAsExpanded(point);
		}
		base.tryToSpawnPorridge (point, dir);
	}

}
