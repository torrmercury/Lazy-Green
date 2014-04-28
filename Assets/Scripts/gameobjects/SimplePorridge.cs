using UnityEngine;
using System.Collections;

public class SimplePorridge : HexGridPiece {

	public Sprite[] singlePorridgeSprites = new Sprite[6];
	public Sprite[] doublePorridgeSprites = new Sprite[6];
	private Sprite chosenSprite;

	public GameObject porridgePrefab;

	public GameObject flowParticleRight;
	public GameObject flowParticleUpRight;
	public GameObject flowParticleDownRight;
	public GameObject flowParticleLeft;
	public GameObject flowParticleUpLeft;
	public GameObject flowParticleDownLeft;

	public Sprite normalSprite;
	public Sprite doubleStackSprite;
	public bool _doubleStacked = false;
	protected bool _spawnedPorridgeThisTurn = false;

	protected bool _expandingNextTurn = false;
	protected bool _spawningPorridgeNextTurn;
	protected HexGrid.HexDir _nextPorridgeDir;

	// So we can return to our original tag
	protected string _startTag;

	public bool startGrowing = true;


	// Use this for initialization
	public override void Start () {
		_type = HexGridPiece.PORRIDGE_TYPE;
		if (startGrowing)
			_growing = true;
		base.Start();
		//_spriteRender.sprite = normalSprite;
		_startTag = gameObject.tag;
		if(!_doubleStacked){
			chosenSprite = singlePorridgeSprites[Random.Range(0, singlePorridgeSprites.Length)];
			gameObject.GetComponent<SpriteRenderer>().sprite = chosenSprite;
		}
		if (_doubleStacked){
			chosenSprite = doublePorridgeSprites[Random.Range(0, doublePorridgeSprites.Length)];
			gameObject.GetComponent<SpriteRenderer>().sprite = chosenSprite;
		}
	}

	public override void preSync ()
	{
		base.preSync();
		_spawnedPorridgeThisTurn = false;
	}

	public override void performSync ()
	{
		base.performSync ();
		if (!_spawnedPorridgeThisTurn && _expandingNextTurn) {
			gameObject.tag = _startTag;
			//_spriteRender.sprite = doubleStackSprite;

			//Randomize sprite
			chosenSprite = doublePorridgeSprites[Random.Range(0, doublePorridgeSprites.Length)];
			gameObject.GetComponent<SpriteRenderer>().sprite = chosenSprite;

			_doubleStacked = true;
			_expandingNextTurn = false;
		}
		else if (!_spawnedPorridgeThisTurn && _spawningPorridgeNextTurn) {
			gameObject.tag = _startTag;
			tryToSpawnPorridge(HexGrid.gridPosFromDir(_gridPos, _nextPorridgeDir), _nextPorridgeDir);
			_spawningPorridgeNextTurn = false;
		}
	}


	protected virtual void tryToSpawnPorridge(Vector2 point, HexGrid.HexDir dir) {
		// If the point is empty, spawn porridge. 
		// If the point has porridge, spread more porridge
		// If the point has a player, try to push/hurt the player
		
		// For now, just focus on the empty spot
		if (!HexGrid.instance.inGrid(point))
			return;
		
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
		
		bool canSpawn = !hasPlayer && !hasPorridge && !hasWall && !hasBarricade;

		if (hasWall)
			return;

		if (hasPorridge) {
			porridge.spawnPorridge(dir);
			return;
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

			//Randomize the sprite
			chosenSprite = singlePorridgeSprites[Random.Range(0, singlePorridgeSprites.Length)];
			porridgeObj.GetComponent<SpriteRenderer>().sprite = chosenSprite;

			porridgeObj.transform.parent = HexGrid.instance.transform;
			porridgeObj.GetComponent<HexGridPiece>().x = actualPos.x;
			porridgeObj.GetComponent<HexGridPiece>().y = actualPos.y;
			HexGrid.instance.addToCurrentGrid(porridgeObj.GetComponent<HexGridPiece>());
			porridgeObj.GetComponent<SimplePorridge>()._doubleStacked = false;
			porridgeObj.GetComponent<SimplePorridge>().startGrowing = true;

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
		}
	}


	public void spawnPorridge(HexGrid.HexDir dir) {
		// If the point is empty, spawn porridge. 
		// If the point has porridge, spread more porridge
		// If the point has a player, try to push/hurt the player
		if (_shrinking || _growing || _spawnedPorridgeThisTurn)
			return;
		
		_spawnedPorridgeThisTurn = true;
		if (!_doubleStacked) {
			gameObject.tag = "ExpandingPorridge";
			_expandingNextTurn = true;
			shake ();
		}
		// If we're doublestacked, need to try to spawn porridge on the next point down
		// On the next turn of course
		else {
			_spawningPorridgeNextTurn = true;
			_nextPorridgeDir = dir;
			gameObject.tag = "ExpandingPorridge";
			shake ();
		}
	}
	



}
