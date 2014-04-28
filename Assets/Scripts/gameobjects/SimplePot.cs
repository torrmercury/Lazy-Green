using UnityEngine;
using System.Collections;

public class SimplePot : HexGridPiece {
	
	public GameObject porridgePrefab;

	public GameObject flowParticleRight;
	public GameObject flowParticleUpRight;
	public GameObject flowParticleDownRight;
	public GameObject flowParticleLeft;
	public GameObject flowParticleUpLeft;
	public GameObject flowParticleDownLeft;

	protected HexGrid.HexDir _nextPorridgeDir;
	protected int _porridgeSpawnCounter;


	public override void Start() {
		_type = WALL_TYPE | POT_TYPE;
		base.Start();
	}

	public override bool isSyncReady ()
	{
		return base.isSyncReady();
	}

	public override void beginTurn ()
	{
		base.beginTurn ();
		// FLAG: if you want the camera zooming to this pot on our turn
		Camera.main.GetComponent<CameraFollow>().followNewTarget(transform);

		_nextPorridgeDir = HexGrid.HexDir.UpRight;
		_porridgeSpawnCounter = 6;
		audio.Play();
	}

	public override void syncOnTurn ()
	{

		// If it's time for our turn, do our porridge thang

		// Spawn our next porridge
		if (_porridgeSpawnCounter > 0 && GameObject.FindGameObjectsWithTag("ExpandingPorridge").Length == 0) {
			bool porridgeSpawned = tryToSpawnPorridge(HexGrid.gridPosFromDir(_gridPos, _nextPorridgeDir), _nextPorridgeDir);
			_nextPorridgeDir = HexGrid.nextClockwiseDir(_nextPorridgeDir);
			_porridgeSpawnCounter--;
			if (porridgeSpawned)
				shake ();
		}
		else if (GameObject.FindGameObjectsWithTag("ExpandingPorridge").Length == 0)
			endTurn();
		else
			shake ();

	}


	protected virtual bool tryToSpawnPorridge(Vector2 point, HexGrid.HexDir dir) {
		// If the point is empty, spawn porridge. 
		// If the point has porridge, spread more porridge
		// If the point has a player, try to push/hurt the player
		
		// For now, just focus on the empty spot
		if (!HexGrid.instance.inGrid(point))
			return false;
		
		bool hasPlayer = false;
		SimplePlayer player = null;
		bool hasPorridge = false;
		SimplePorridge porridge = null;
		bool hasWall = false;
		bool hasBarricade = false;
		SimpleBarricade barricade = null;
		
		foreach (HexGridPiece inhabitant in HexGrid.instance.currentGridInhabitants(point)) {
			//PORRIDGE PARTICLES EFFECT
			Vector2 originPos = HexGrid.instance.toActualCoord(_gridPos);
			Vector3 finalOriginPos = new Vector3(originPos.x, originPos.y, -1.0f);
			if(dir == HexGrid.HexDir.Right){
				GameObject newFlow = Instantiate(flowParticleRight) as GameObject;
				newFlow.transform.parent = HexGrid.instance.transform;
				newFlow.transform.localPosition = finalOriginPos;
			} else if(dir == HexGrid.HexDir.Left){
				GameObject newFlow = Instantiate(flowParticleLeft) as GameObject;
				newFlow.transform.parent = HexGrid.instance.transform;
				newFlow.transform.localPosition = finalOriginPos;
			} else if(dir == HexGrid.HexDir.UpRight){
				GameObject newFlow = Instantiate(flowParticleUpRight) as GameObject;
				newFlow.transform.parent = HexGrid.instance.transform;
				newFlow.transform.localPosition = finalOriginPos;
			} else if(dir == HexGrid.HexDir.UpLeft){
				GameObject newFlow = Instantiate(flowParticleUpLeft) as GameObject;
				newFlow.transform.parent = HexGrid.instance.transform;
				newFlow.transform.localPosition = finalOriginPos;
			} else if(dir == HexGrid.HexDir.DownRight){
				GameObject newFlow = Instantiate(flowParticleDownRight) as GameObject;
				newFlow.transform.parent = HexGrid.instance.transform;
				newFlow.transform.localPosition = finalOriginPos;
			} else if(dir == HexGrid.HexDir.DownLeft){
				GameObject newFlow = Instantiate(flowParticleDownLeft) as GameObject;
				newFlow.transform.parent = HexGrid.instance.transform;
				newFlow.transform.localPosition = finalOriginPos;
			}

			if (inhabitant.hasType(HexGridPiece.BARRICADE_TYPE)) {
				hasBarricade = true;
				barricade = inhabitant.GetComponent<SimpleBarricade>();
			}
			else if (inhabitant.hasType(HexGridPiece.PLAYER_TYPE)) {
				hasPlayer = true;
				player = inhabitant.GetComponent<SimplePlayer>();
			}
			else if (inhabitant.hasType(HexGridPiece.PORRIDGE_TYPE)) {
				hasPorridge = true;
				porridge = inhabitant.GetComponent<SimplePorridge>();
			}
			else if (inhabitant.hasType(HexGridPiece.WALL_TYPE | HexGridPiece.EXIT_TYPE)) {
				hasWall = true;
			}
		}
		
		bool canSpawn = !hasPlayer && !hasPorridge && !hasBarricade;

		if (hasWall)
			return false;

		if (hasPorridge) {
			porridge.spawnPorridge(dir);
			return true;
		}
		if(hasBarricade) {
			barricade.takeDamage();
			// If the barricade was just destroyed, act as if there isn't a barricade
			if (barricade.shrinking) {
				hasBarricade = false;
				if (!hasPlayer)
					canSpawn = true;
			}
		}
		if (hasPlayer && !hasBarricade) {
			// See if we can push the player
			Vector2 playerPushPoint = HexGrid.gridPosFromDir(player.gridPos, dir);
			if (player.canBePushedIntoPoint(playerPushPoint)) {
				if (HexGrid.instance.tutorial != null)
					HexGrid.instance.tutorial.justPushed(player);
				player.nextGridPos = playerPushPoint;
				player.moveAfterSync = true;
				canSpawn = true;
			}
			else {
				player.takeDamage();
			}
		}


		
		if (canSpawn) {
			Vector2 actualPos = HexGrid.instance.toActualCoord(point);
			GameObject porridgeObj = Instantiate(porridgePrefab) as GameObject;
			porridgeObj.transform.parent = HexGrid.instance.transform;
			porridgeObj.GetComponent<HexGridPiece>().x = actualPos.x;
			porridgeObj.GetComponent<HexGridPiece>().y = actualPos.y;
			HexGrid.instance.addToCurrentGrid(porridgeObj.GetComponent<HexGridPiece>());

			//PORRIDGE PARTICLES EFFECT
			Vector2 originPos = HexGrid.instance.toActualCoord(_gridPos);
			Vector3 finalOriginPos = new Vector3(originPos.x, originPos.y, -1.0f);
			if(dir == HexGrid.HexDir.Right){
				GameObject newFlow = Instantiate(flowParticleRight) as GameObject;
				newFlow.transform.parent = HexGrid.instance.transform;
				newFlow.transform.localPosition = finalOriginPos;
			} else if(dir == HexGrid.HexDir.Left){
				GameObject newFlow = Instantiate(flowParticleLeft) as GameObject;
				newFlow.transform.parent = HexGrid.instance.transform;
				newFlow.transform.localPosition = finalOriginPos;
			} else if(dir == HexGrid.HexDir.UpRight){
				GameObject newFlow = Instantiate(flowParticleUpRight) as GameObject;
				newFlow.transform.parent = HexGrid.instance.transform;
				newFlow.transform.localPosition = finalOriginPos;
			} else if(dir == HexGrid.HexDir.UpLeft){
				GameObject newFlow = Instantiate(flowParticleUpLeft) as GameObject;
				newFlow.transform.parent = HexGrid.instance.transform;
				newFlow.transform.localPosition = finalOriginPos;
			} else if(dir == HexGrid.HexDir.DownRight){
				GameObject newFlow = Instantiate(flowParticleDownRight) as GameObject;
				newFlow.transform.parent = HexGrid.instance.transform;
				newFlow.transform.localPosition = finalOriginPos;
			} else if(dir == HexGrid.HexDir.DownLeft){
				GameObject newFlow = Instantiate(flowParticleDownLeft) as GameObject;
				newFlow.transform.parent = HexGrid.instance.transform;
				newFlow.transform.localPosition = finalOriginPos;
			}
			return true;
		}
		return false;
		
	}

}
