using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HexGrid : MonoBehaviour {

	public bool lastTurn = false;
	private GameObject[] exitHexes;

	// PUBLIC REFERENCE PREFABS GO HERE


	// For generating the static hex background
	public GameObject hexTilePrefab;

	public GameObject hexHighlightPrefab;

	public GameObject cursor;

	// For our level loader to spawn
	public GameObject porridgePrefab, doublePorridgePrefab, octoPotPrefab, rightPipePrefab, leftPipePrefab,
	rockPrefab, stumpPrefab;
	public GameObject overflowPorridgePrefab, doubleOverflowPorridgePrefab, littlePotPrefab;
	public GameObject exitPrefab, entrancePrefab;
	public GameObject johannPrefab, olgaPrefab, wernerPrefab;
	// Item chest prefabs (gonna be a lot of these)
	public GameObject potionPrefab, hpUpPrefab, stoUpPrefab, bootsPrefab;



	// END PUBLIC REFERENCE PREFABs

	// PUBLIC REFERENCE OBJECTS (in the hierarchy)

	public GameObject fadeObj;
	// The menu where we choose our actions
	public GameObject bottomMenu;
	// The pop up that informs us of a new item get
	public GameObject itemGetMenu;
	// The game over notification when all players are dead
	public GameObject gameOverMenu;
	// FLAG: temporary way of using the game over message to win the game
	public ShadowText doneText;
	// Where we do dialogue
	public DialogueWindow dialogueWindow;
	// The important bottom menu
	public BottomMenu dynamicBottomMenu;


	// END PUBLIC REFERENCE OBJECTs



	public string nextScene = "AlecSecondScene";



	public enum HexDir {
		UpRight,
		Right,
		DownRight,
		DownLeft,
		Left,
		UpLeft
	}


	public static Vector2 gridPosFromDir(Vector2 point, HexDir dir) {
		int q = (int)point.x, r = (int)point.y;
		switch(dir) {
		case HexDir.UpRight:
			return new Vector2(q+(r&1), r+1);
		case HexDir.Right:
			return new Vector2(q+1, r);
		case HexDir.DownRight:
			return new Vector2(q+(r&1), r-1);
		case HexDir.DownLeft:
			return new Vector2(q-1+(r&1), r-1);
		case HexDir.Left:
			return new Vector2(q-1, r);
		case HexDir.UpLeft:
			return new Vector2(q-1+(r&1), r+1);
		default:
			return new Vector2(q, r);
		}
	}

	// Lol, there is a much better way to do this automatically
	// That I'm not looking up right now.
	public static IEnumerable<HexDir> allDirs() {
		yield return HexDir.UpRight;
		yield return HexDir.Right;
		yield return HexDir.DownRight;
		yield return HexDir.DownLeft;
		yield return HexDir.Left;
		yield return HexDir.UpLeft;
	}

	public static IEnumerable<Vector2> getNeighbors(Vector2 point) {
		yield return gridPosFromDir(point, HexDir.UpRight);
		yield return gridPosFromDir(point, HexDir.Right);
		yield return gridPosFromDir(point, HexDir.DownRight);
		yield return gridPosFromDir(point, HexDir.DownLeft);
		yield return gridPosFromDir(point, HexDir.Left);
		yield return gridPosFromDir(point, HexDir.UpLeft);
	}

	public static HexDir dirBetweenPoints(Vector2 source, Vector2 dest) {
		int sX = (int)source.x, sY = (int)source.y, dX=(int)dest.x, dY=(int)dest.y;
		if (dest.x == source.x+1 && dest.y == source.y)
			return HexDir.Right;
		else if (dest.x == source.x-1 && dest.y == source.y)
			return HexDir.Left;
		else if (dX == sX+(sY&1) && dY == sY+1)
			return HexDir.UpRight;
		else if (dX == sX+(sY&1) && dY == sY-1)
			return HexDir.DownRight;
		else if (dX == sX-1+(sY&1) && dY == sY-1)
			return HexDir.DownLeft;
		else if (dX == sX-1+(sY&1) && dY == sY+1)
			return HexDir.UpLeft;

		// If the two points are not actually adjacent, ERROR
		throw new UnityException("Dir between points error. Points are not adjacent!");

	}

	public static HexDir nextClockwiseDir(HexDir dir) {
		switch(dir) {
		case HexDir.UpRight:
			return HexDir.Right;
		case HexDir.Right:
			return HexDir.DownRight;
		case HexDir.DownRight:
			return HexDir.DownLeft;
		case HexDir.DownLeft:
			return HexDir.Left;
		case HexDir.Left:
			return HexDir.UpLeft;
		case HexDir.UpLeft:
			return HexDir.UpRight;
		default:
			return dir;
		}
	}
	
	public enum GameState {
		Start,
		Paused,
		Run,
		Done,
		Dialogue
	}
	protected GameState _currentState = GameState.Run;
	public GameState currentState {
		get { return _currentState; }
		set { _currentState = value; }
	}
	public bool isPaused {
		get { return _currentState == GameState.Paused || _currentState == GameState.Start || _currentState == GameState.Dialogue; }
	}

	// Fuzzy Singleton (we don't really check to enforce there's only one so be careful)
	protected static HexGrid _instance;
	public static HexGrid instance {
		get { return _instance; }
	}

	// How many grid tiles can overlap at once
	protected const int GRID_DEPTH = 4;

	// For hexes, this value corresponds to the distance from the center to a vertex (NOT an edge)
	// Remember to take into account the PTM ratio
	public const float HEX_RADIUS = 1;
	public const float SQRT3 = 1.7320508f;
	public const float MOVE_SPEED = 5.7f;

	// This is a value that allows us to lay out the levels like a rectangular grid
	// and then correctly arrange the pieces so they're in an offset grid.
	public const float RECT_SIZE = 1;
	// Whether this level is initially laid out like a rect grid (i.e. should pieces reposition themselves)
	public bool isRectGridLayout = false;
	public Vector2 rectToGridCoord(float x, float y) {
		return new Vector2(Mathf.Floor(x / RECT_SIZE), Mathf.Floor(y / RECT_SIZE));
	}


	// Stuff for loading directly from levels
	public bool spawnFromOgmoLevel = false;
	public static int currentOgmoIndex = 0;
	public string[] ogmoLevelNames;
	protected Level _currentLevel;

	// Just a flag for it we've built our background yet.
	protected bool _backgroundBuilt = false;



	// The number of turns we have before the game ends and the rafts disappear. 
	public int turnsLeft = 5;

	// A straight up list of existing tiles
	List<HexGridPiece> _gridPieces = new List<HexGridPiece>();
	// The existing state of the grid (in rect coordinates)
	protected HexGridPiece[,,] _currentGrid;
	// The next state of the grid (updated during the turn phase, also in rect coordinates)
	protected HexGridPiece[,,] _claimedGrid;
	// The pieces that will be removed at the end of the turn
	protected List<HexGridPiece> _piecesToRemove = new List<HexGridPiece>();

	// A grid used for functions that need to keep track of which grid positions we've already checked
	// Useful for stuff like A* or flood fill etc.
	// WARNING: This only works since Unity is multithreaded.
	protected bool[,] _checkedGrid;
	protected void clearCheckedGrid() {
		for (int i = 0; i < gridWidth; i++) {
			for (int j = 0; j < gridHeight; j++) {
				_checkedGrid[i, j] = false;
			}
		}
	}

	public Sprite normalHighlight, redHighlight, greenHighlight, heavyGreenHighlight;
	// Grid of highlight tiles we can activate and deactivate at will
	// Has two layers to allow for multiple highlights (mainly for vomit hexes). 
	protected GameObject[,,] _highlightHexes;
	public void highlightGridPos(Vector2 point) {
		highlightGridPos(point, normalHighlight);
	}
	public void highlightGridPos(Vector2 point, Sprite sprite) {
		_highlightHexes[(int)point.x, (int)point.y, 0].renderer.enabled = true;
		(_highlightHexes[(int)point.x, (int)point.y, 0].renderer as SpriteRenderer).sprite = sprite;
	}
	public void highlightDeepGridPos(Vector2 point, Sprite sprite) {
		_highlightHexes[(int)point.x, (int)point.y, 1].renderer.enabled = true;
		(_highlightHexes[(int)point.x, (int)point.y, 1].renderer as SpriteRenderer).sprite = sprite;
	}
	public void flashHighlight(Vector2 point) {
		_highlightHexes[(int)point.x, (int)point.y, 0].GetComponent<FlashSprite>().startFlash();
	}
	public void flashDeepHighlight(Vector2 point) {
		_highlightHexes[(int)point.x, (int)point.y, 1].GetComponent<FlashSprite>().startFlash();
	}
	public void disableHighlight(Vector2 point) {
		_highlightHexes[(int)point.x, (int)point.y, 0].renderer.enabled = false;
		// Make sure that we don't flash when we're turned off.
		_highlightHexes[(int)point.x, (int)point.y, 0].GetComponent<FlashSprite>().stopFlash();
	}
	public void disableDeepHighlight(Vector2 point) {
		_highlightHexes[(int)point.x, (int)point.y, 1].renderer.enabled = false;
		_highlightHexes[(int)point.x, (int)point.y, 1].GetComponent<FlashSprite>().stopFlash();
	}



	// TURN MANAGEMENT STUFF GOES HERE
	public enum Turn {
		PlayerTurn,
		PorridgeTurn
	}
	protected Turn _currentTurn = Turn.PlayerTurn;
	public Turn currentTurn {
		get { return _currentTurn; }
	}
	// Keep lists of the players and the porridge pots so that we can tell when to swap turns

	// Player turns happen in any order.
	protected List<HexGridPiece> _players;

	// We move through the porridge pots one by one taking their turns
	protected List<HexGridPiece> _porridgePots;
	protected HexGridPiece _currentPot;
	protected int _currentPotIndex;


	// END TURN MANAGEMENT STUFF


	// The rect width and height of the grid
	public int gridWidth=4, gridHeight=4;
	// The Camera bounds of the grid
	protected float _cameraMinX=0, _cameraMaxX=0, _cameraMinY=0, _cameraMaxY;
	public float cameraMinX {
		get { return _cameraMinX; }
	}
	public float cameraMaxX {
		get { return _cameraMaxX; }
	}
	public float cameraMinY {
		get { return _cameraMinY; }
	}
	public float cameraMaxY {
		get { return _cameraMaxY; }
	}


	// ATTACHABLE TUTORIAL OBJECT
	protected Tutorial _tutorial = null;
	public Tutorial tutorial {
		get { return _tutorial; }
		set { _tutorial = value; }
	}


	public void addPiece(HexGridPiece piece) {
		if (piece.hasType(HexGridPiece.PLAYER_TYPE)) {

			//Make sure the player is visible again and that he is now alloed to take turns once more (after leaving on the raft last map)
			piece.GetComponent<SimplePlayer>().hasLeftOnRaft = false;
			piece.GetComponent<SpriteRenderer>().enabled = true;

			_players.Add(piece);
			if (_currentTurn == Turn.PlayerTurn) {
				piece.beginTurn();
			}
		}
		if (piece.hasType(HexGridPiece.POT_TYPE)) {
			_porridgePots.Add(piece);

		}
		_gridPieces.Add(piece);
	}



	// this is here so we can make it an iTween callback.
	public void loadNextLevel() {
		if (spawnFromOgmoLevel) {
			currentOgmoIndex++;
			if (currentOgmoIndex < ogmoLevelNames.Length) {
				Application.LoadLevel(Application.loadedLevel);
			}
			else {
				currentOgmoIndex = 0;
				Application.LoadLevel(nextScene);
			}
		}
		else
			Application.LoadLevel(nextScene);
	}

	public void loadMainMenu() {
		Johann.johannSpawned = false;
		Olga.olgaSpawned = false;
		Werner.wernerSpawned = false;
		currentOgmoIndex = 0;
		Application.LoadLevel("MainMenuScene");
	}

	public void quitButtonPressed() {
		_currentState = GameState.Paused;
		iTween.ColorTo(fadeObj, iTween.Hash("a", 1, "time", 1, "oncompletetarget", gameObject, "oncomplete", "loadMainMenu"));
	}

	public void onGameOver() {
		_currentState = GameState.Paused;
		gameOverMenu.transform.localPosition = new Vector3(6.5f, 64, gameOverMenu.transform.localPosition.z);
		iTween.MoveTo(gameOverMenu, iTween.Hash("x", 6.5f, "y", -8f, "time", 1, "easetype", iTween.EaseType.easeOutBack));
	}

	public void onGameWin() {
		_currentState = GameState.Paused;
		doneText.setText("You Win!");
		iTween.MoveTo(gameOverMenu, iTween.Hash("y", 0, "time", 1, "easetype", iTween.EaseType.easeOutBack));
	}

	public void revealItemGet() {
		// the notification scrolls in from the top
		itemGetMenu.transform.localPosition = new Vector3(6.5f, 64, itemGetMenu.transform.localPosition.z);
		iTween.MoveTo(itemGetMenu, iTween.Hash("isLocal", true, "position", new Vector3(6.5f, 1.5f, itemGetMenu.transform.localPosition.z), "time", 1, "easetype", iTween.EaseType.easeOutBack));
		_currentState = GameState.Paused;
	}

	public void confirmItemGet() {
		// the menu scrolls out
		Camera.main.audio.volume = 0.5f;
		iTween.MoveTo(itemGetMenu, iTween.Hash("isLocal", true, "position", new Vector3(6.5f, -64, itemGetMenu.transform.localPosition.z), "time", 1, "easetype", iTween.EaseType.easeInBack));
		_currentState = GameState.Run;
	}


	protected float worldGridHeight(int gridCellHeight) {
		float worldHeight = 0;
		for (int i = 0; i < gridCellHeight; i++) {
			if ((i & 1) == 0) {
				worldHeight+=2f*HEX_RADIUS;
			}
			else if (i == gridCellHeight-1) {
				worldHeight+=1.5f*HEX_RADIUS;
			}
			else {
				worldHeight+=HEX_RADIUS;
			}
		}
		return worldHeight;
	}

	public bool allowFlash = false;

	IEnumerator flashExitHexes(){
		allowFlash = true;
		yield return new WaitForSeconds(0.1f);
		allowFlash = false;
	}

	// Use this for initialization
	void Start () {
		_instance = this;

		_players = new List<HexGridPiece>();
		_porridgePots = new List<HexGridPiece>();

		// try loading from a level
		if (spawnFromOgmoLevel) {
			_currentLevel = new Level(string.Format("ogmolevels/{0}", ogmoLevelNames[currentOgmoIndex]));
			_currentLevel.loadFromLevel(this);
		}
		else {
			initGrids();
		}



		_checkedGrid = new bool[gridWidth, gridHeight];
		_highlightHexes = new GameObject[gridWidth, gridHeight, 2];

		_currentTurn = Turn.PlayerTurn;
		_currentState = GameState.Start;

		// Move us to the center of the camera
		float sidebarWidth = 5.25f;

		float x = -(transform.localScale.x*(gridWidth-0.5f)*HEX_RADIUS*SQRT3)/2+sidebarWidth;
		float y = -(transform.localScale.y*(gridHeight-1f)*HEX_RADIUS*1.5f)/2 + 5.5f;
		transform.position = new Vector3(x, y, transform.position.z);

		// Now figure out the bounds of the camera
		float worldWidth = (gridWidth+0.5f)*SQRT3*HEX_RADIUS*transform.localScale.x;
		float worldHeight = worldGridHeight(gridHeight)*transform.localScale.y + 5f;
		float viewWidth = (Screen.width/Constants.PTMRatio)*CameraResize.screenScale-sidebarWidth*2;
		float viewHeight = (Screen.height/Constants.PTMRatio)*CameraResize.screenScale - 11.0f;
		if (worldWidth < viewWidth) {
			Debug.Log (string.Format("{0}, {1}", worldWidth, viewWidth));
			_cameraMinX = 0;
			_cameraMaxX = 0;
		}
		else {
			float dif = worldWidth-viewWidth;
			_cameraMaxX = dif/2;
			_cameraMinX = -dif/2;
		}
		if (worldHeight < viewHeight) {
			_cameraMinY = 0;
			_cameraMaxY = 0;
		}
		else {
			float dif = worldHeight-viewHeight;
			_cameraMinY = -dif/2;
			_cameraMaxY = dif/2;
		}



		// Fade out our fade object
		fadeObj.renderer.material.color = Color.black;
		iTween.ColorTo(fadeObj, iTween.Hash("a", 0, "time", 1));



	}

	public void initGrids() {
		_currentGrid = new HexGridPiece[gridWidth, gridHeight, GRID_DEPTH];
		_claimedGrid = new HexGridPiece[gridWidth, gridHeight, GRID_DEPTH];
	}

	[HideInInspector] public string toMove;

	public void buildBackground() {
		_backgroundBuilt = true;
		GameObject spriteBatcherObj = new GameObject("bg_tiles");
		tk2dStaticSpriteBatcher batcher = spriteBatcherObj.AddComponent<tk2dStaticSpriteBatcher>();
		spriteBatcherObj.transform.parent = transform;
		spriteBatcherObj.transform.localPosition = Vector3.zero+Vector3.forward*0.1f;
		spriteBatcherObj.transform.localScale = Vector3.one;
		batcher.batchedSprites = new tk2dBatchedSprite[gridWidth*gridHeight];
		int listSize = gridWidth*gridHeight;
		for (int i = 0; i < gridWidth; i++) {
			for (int j = 0; j < gridHeight; j++) {
				bool hasExit = false;
				foreach (HexGridPiece piece in currentGridInhabitants(new Vector2(i, j))) {
					if (piece.hasType(HexGridPiece.EXIT_TYPE)) {
						hasExit = true;
					}
				}


				// If the pos is an exit pos, leave it.
				tk2dBatchedSprite bs = new tk2dBatchedSprite();
				bs.spriteCollection = hexTilePrefab.GetComponent<tk2dSprite>().Collection;
				if (hasExit)
					bs.color = new Color(1, 1, 1, 0);
				bs.spriteId = hexTilePrefab.GetComponent<tk2dSprite>().GetSpriteIdByName("bottom_hex");

				/*
				if (j == 0)
					bs.spriteId = hexTilePrefab.GetComponent<tk2dSprite>().GetSpriteIdByName("bottom_hex");
				else if (i == 0 && (j & 1) == 0)
					bs.spriteId = hexTilePrefab.GetComponent<tk2dSprite>().GetSpriteIdByName("bottomleft_hex");
				else if (i == gridWidth-1 && (j & 1) == 1)
					bs.spriteId = hexTilePrefab.GetComponent<tk2dSprite>().GetSpriteIdByName("bottomright_hex");
				else
					bs.spriteId = hexTilePrefab.GetComponent<tk2dSprite>().spriteId;*/

				Vector3 pos = toActualCoord(new Vector2(i, j));
				bs.relativeMatrix.SetTRS(pos, Quaternion.identity, Vector3.one);
				// Also build a hex highlight for each of the hexes
				GameObject hexHighlightObj = Instantiate(hexHighlightPrefab) as GameObject;
				hexHighlightObj.transform.parent = transform;
				hexHighlightObj.transform.localPosition = new Vector3(pos.x, pos.y, hexHighlightPrefab.transform.position.z);
				hexHighlightObj.transform.localScale = Vector3.one;
				hexHighlightObj.renderer.enabled = false;
				_highlightHexes[i, j, 0] = hexHighlightObj;
				// And a deep highlight for each of the hexes
				hexHighlightObj = Instantiate(hexHighlightPrefab) as GameObject;
				hexHighlightObj.transform.parent = transform;
				hexHighlightObj.transform.localPosition = new Vector3(pos.x, pos.y, hexHighlightPrefab.transform.position.z-0.1f);
				hexHighlightObj.transform.localScale = Vector3.one;
				hexHighlightObj.renderer.enabled = false;
				_highlightHexes[i, j, 1] = hexHighlightObj;
				batcher.batchedSprites[listSize-(j*gridWidth+i)-1] = bs;
			}
		}
		batcher.SetFlag(tk2dStaticSpriteBatcher.Flags.GenerateCollider, false);
		batcher.Build();

		//COLLECT DATA ON THE EXIT HEXES
		exitHexes = GameObject.FindGameObjectsWithTag("ExitTile");
		//Exits are either on the top or the bottom
		if(exitHexes[0].transform.localPosition.y == exitHexes[1].transform.localPosition.y || 
		   exitHexes[0].transform.localPosition.y == exitHexes[2].transform.localPosition.y ||
		   exitHexes[1].transform.localPosition.y == exitHexes[2].transform.localPosition.y){
			if(exitHexes[1].transform.localPosition.y == 0){
				//They are in the bottom!
				toMove = "Down";
				for(int j = 0; j < exitHexes.Length; j++)
					exitHexes[j].transform.localPosition = new Vector3(exitHexes[j].transform.localPosition.x, exitHexes[j].transform.localPosition.y, -0.1f);
			} else {
				//They are in the top!
				toMove = "Up";
				for(int j = 0; j < exitHexes.Length; j++)
					exitHexes[j].transform.localPosition = new Vector3(exitHexes[j].transform.localPosition.x, exitHexes[j].transform.localPosition.y, 0);
			}
		}
		//Exits are either on the left or the right
		else if(exitHexes[0].transform.localPosition.x == exitHexes[1].transform.localPosition.x ||
		        exitHexes[0].transform.localPosition.x == exitHexes[2].transform.localPosition.x ||
		        exitHexes[1].transform.localPosition.x == exitHexes[2].transform.localPosition.x){
			if(exitHexes[0].transform.localPosition.x == 0)
				toMove = "Left";
			else
				toMove = "Right";
		}

	}


	// Update is called once per frame
	void Update () {

#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.Z)) {
			Application.CaptureScreenshot(string.Format("Screenshots/{0}.png", System.Guid.NewGuid().ToString()));
		}
#endif
		if (!_backgroundBuilt) {
			buildBackground();
		}

		// First, if we've officially set everything up, we're no longer a grid
		isRectGridLayout = false;

		// First, check our tutorial to make sure we're not doing anything wrong
		if (_tutorial != null) {
			_tutorial.processInput();
		}


		if (_currentState == GameState.Start) {
			_currentState = GameState.Run;
			return;
		}
		if (_currentState == GameState.Paused) {

		}
		else if (_currentState == GameState.Run) {
			Vector2 checkMousePos = Camera.main.ScreenToWorldPoint(HungryInput.mousePosition);
			_mouseOverMenu = Physics2D.OverlapPoint(checkMousePos, 1 << LayerMask.NameToLayer("GUI"));


			foreach (HexGridPiece piece in _piecesToRemove) {
				if (piece != null) {
					_gridPieces.Remove(piece);
					if (piece.hasType(HexGridPiece.PLAYER_TYPE))
						_players.Remove(piece as SimplePlayer);
					
					// WARNING
					// If pots can be destroyed during the porridge turn, this could ruin everything.
					if (piece.hasType(HexGridPiece.POT_TYPE))
						_porridgePots.Remove(piece as SimplePot);
					
					Destroy (piece.gameObject);
				}
			}
			_piecesToRemove.Clear();
			
			
			// First, switch turns if it's time. 
			if (_currentTurn == Turn.PlayerTurn) {
				// Look at all the players and see if they're done
				// Also, look to see if the players are standing on exits, ready for a map transition
				bool switchTurn = true;
				bool exitMap = true;
				bool livingPlayer = false;
				foreach (HexGridPiece player in _players) {
					if (player.alive)
						livingPlayer = true;
					if (player.isOurTurn) {
						switchTurn = false;
					}
					if (player.alive && !player.GetComponent<SimplePlayer>().standingOnExit()) {
						exitMap = false;
					}
				}
				if (!livingPlayer && switchTurn && _players.Count > 0) {
					// If there are no living players right now, game over!
					onGameOver();
				}
				else if (livingPlayer && exitMap && switchTurn) {
					// First, detach all players from the grid so they don't get destroyed
					foreach (HexGridPiece player in _players) {
						if (player.alive) {
							player.transform.parent = null;
							player.GetComponent<SimplePlayer>().detachMenuFromBottom();
						}
					}
					// Time to start the fade out.
					iTween.ColorTo(fadeObj, iTween.Hash("a", 1, "time", 1, "oncompletetarget", gameObject, "oncomplete", "loadNextLevel"));
					_currentState = GameState.Paused;
					return;
				}
				else if (switchTurn) {
					// Our turns left decreases!
					turnsLeft--;

					if(turnsLeft < 4){
						lastTurn = true;
					} else
						lastTurn = false;

					//Let's flash the number to make people desperate!!
					StartCoroutine(flashExitHexes());


					if (turnsLeft <= 0) {
						onGameOver();
						return;
					}


					if (_porridgePots.Count > 0) {
						_currentTurn = Turn.PorridgeTurn;
						_currentPotIndex = 0;
						_currentPot = _porridgePots[_currentPotIndex++];
						_currentPot.beginTurn();
					}
					else {
						foreach (HexGridPiece player in _players) {
							player.beginTurn();
						}
					}
				}
			}
			else if (_currentTurn == Turn.PorridgeTurn) {
				// Check to see if our current pot is done
				if (!_currentPot.isOurTurn) {
					// See if there are any living players left.
					bool livingPlayer = false;
					foreach (HexGridPiece player in _players) {
						if (player.alive)
							livingPlayer = true;
					}
					if (!livingPlayer && _players.Count > 0) {
						onGameOver();
					}
					else if (_currentPotIndex < _porridgePots.Count) {
						_currentPot = _porridgePots[_currentPotIndex++];
						_currentPot.beginTurn();
					}
					else {
						// Switch to the players if they're around
						if (_players.Count > 0) {
							_currentTurn = Turn.PlayerTurn;
							foreach (HexGridPiece player in _players) {
								player.beginTurn();
							}
						}
						else {
							_currentPotIndex = 0;
							if (_porridgePots.Count > 0) {
								_currentPot = _porridgePots[0];
								_currentPot.beginTurn();
							}
						}
					}
					
				}
			}

			// Check for the mouse position
			Vector2 mousePos = mouseGridPos();
			if (HungryInput.mousePresent && inGrid(mousePos)) {
				Vector2 mouseActualPos = toActualCoord(mousePos);
				cursor.transform.localPosition = new Vector3(mouseActualPos.x, mouseActualPos.y, cursor.transform.localPosition.z);
				cursor.renderer.enabled = true;
			}
			else {
				cursor.renderer.enabled = false;
			}




			bool syncReady = true;
			foreach (HexGridPiece piece in _gridPieces) {
				if (!piece.isSyncReady())
					syncReady = false;
			}
			if (syncReady) {
				if (_tutorial != null) {
					_tutorial.processSync();
					if (_currentState != GameState.Run)
						return;
				}


				// Construct the two grids
				constructCurrentGrid();
				constructClaimedGrid();
				foreach (HexGridPiece piece in _gridPieces) {
					piece.syncPerformed = false;
					piece.moveAfterSync = false;
					piece.gridPos = toGridCoord(piece.x, piece.y);
				}

				foreach (HexGridPiece piece in _gridPieces) {
					piece.preSync();
				}
				
				// The actual turn
				// Not done in a foreach loop since pieces might be added to the list during this turn
				for (int i = 0; i < _gridPieces.Count; i++) {
					HexGridPiece piece = _gridPieces[i];
					performPieceSync(piece);
				}
				
				foreach (HexGridPiece piece in _gridPieces) {
					performSyncMovement(piece);
				}
				
				// Finally, the post turn 
				foreach (HexGridPiece piece in _gridPieces) {
					piece.postSync();
				}
			}

		}

	}

	public void performPieceSync(HexGridPiece piece) {
		if (!piece.syncPerformed) {
			piece.syncPerformed = true;
			piece.performSync();

			Vector2 nextPoint = piece.followingPath ? piece.lastPathPoint : piece.nextGridPos;
			if (!piece.followingPath && !piece.moveAfterSync)
				nextPoint = piece.gridPos;

			if (inGrid(nextPoint)) {
				// FLAG: UNCOMMENT THIS IF YOU WANT REALLY SIMPLE TURN HANDLING
				//removeFromCurrentGrid(piece);
				
				
				// Add to the claimed grid in the first available slot
				bool foundEmptySlot = false;
				int x = (int)nextPoint.x, y = (int)nextPoint.y;
				for (int z = 0; z < GRID_DEPTH; z++) {
					if (_claimedGrid[x, y, z] == null) {
						foundEmptySlot = true;
						_claimedGrid[x, y, z] = piece;
						break;
					}
				}
				if (!foundEmptySlot)
					throw new UnityException("Claimed Grid Depth Exceeded!");
			}
		}
	}
	
	public void performSyncMovement(HexGridPiece piece) {
		if (!piece.followingPath && piece.moveAfterSync && piece.nextGridPos != piece.gridPos) {
			float moveSpeed = MOVE_SPEED;
			Vector2 nextActualPoint = toActualCoord(piece.nextGridPos);
			piece.moveToPoint(nextActualPoint, moveSpeed);
		}
	}

	public void constructCurrentGrid() {
		for (int i = 0; i < gridWidth; i++) {
			for (int j = 0; j < gridHeight; j++) {
				for (int k = 0; k < GRID_DEPTH; k++) {
					_currentGrid[i, j, k] = null;
				}
			}
		}
		foreach (HexGridPiece piece in _gridPieces) {
			Vector2 gridPos = toGridCoord(piece.x, piece.y);
			if (inGrid(gridPos)) {
				bool foundEmptySlot = false;
				int x = (int)gridPos.x, y = (int)gridPos.y;
				// Find the first null entry along k
				for (int z = 0; z < GRID_DEPTH; z++) {
					if (_currentGrid[x, y, z] == null)	 {
						foundEmptySlot = true;
						_currentGrid[x, y, z] = piece;
						break;
					}
				}
				if (!foundEmptySlot)
					throw new UnityException("Current Grid Grid Depth Exceeded!");
			}
		}
	}

	public void addToCurrentGrid(HexGridPiece piece) {
		Vector2 gridPos = toGridCoord(piece.x, piece.y);
		if (inGrid(gridPos)) {
			bool foundEmptySlot = false;
			int x = (int)gridPos.x, y = (int)gridPos.y;
			for (int z = 0; z < GRID_DEPTH; z++) {
				if (_currentGrid[x, y, z] == null) {
					foundEmptySlot = true;
					_currentGrid[x, y, z] = piece;
					break;
				}
			}
			if (!foundEmptySlot)
				throw new UnityException("Current Grid Grid Depth Exceeded!");
		}
	}

	public void constructClaimedGrid() {
		for (int i = 0; i < gridWidth; i++) {
			for (int j = 0; j < gridHeight; j++) {
				for (int k = 0; k < GRID_DEPTH; k++) {
					_claimedGrid[i, j, k] = null;
				}
			}
		}
	}
	
	public void removeFromCurrentGrid(HexGridPiece piece) {
		int x = (int)piece.gridPos.x, y = (int)piece.gridPos.y;
		for (int z = 0; z < GRID_DEPTH; z++) {
			if (_currentGrid[x, y, z] == piece) {
				_currentGrid[x, y, z] = null;
				return;
			}
		}
	}


	// Mostly gonna use odd-r offset coordinates unless otherwise specified.
	public bool inGrid(Vector2 offsetPoint) {
		return offsetPoint.x >= 0 && offsetPoint.x < gridWidth && offsetPoint.y >= 0 && offsetPoint.y < gridHeight;
	}

	public Vector2 toGridCoord(float x, float y) {
		// Magical grid coordinate function. 
		// First, grab the fractional axial coordinates
		/*
		q = (1/3*sqrt(3) * x - 1/3 * y) / size
		r = 2/3 * y / size
		*/
		float axialQ = ((1/3f)*SQRT3*x - (1/3f)*y)/HEX_RADIUS;
		float axialR = (2/3f)*y/HEX_RADIUS;
		// Now convert to cube coordinates and round down
		/*
		x = q
		z = r
		y = -x-z
		 */
		float cubeX = axialQ;
		float cubeZ = axialR;
		float cubeY = -cubeX-cubeZ;

		int rx = Mathf.RoundToInt(cubeX);
		int ry = Mathf.RoundToInt(cubeY);
		int rz = Mathf.RoundToInt(cubeZ);

		float xDiff = Mathf.Abs(rx - cubeX);
		float yDiff = Mathf.Abs(ry - cubeY);
		float zDiff = Mathf.Abs(rz - cubeZ);

		if (xDiff > yDiff && xDiff > zDiff)
			rx = -ry-rz;
		else if (yDiff > zDiff)
			ry = -rx-rz;
		else
			rz = -rx-ry;

		// Finally, convert the cube coordinates back to offset coords
		int offsetQ = rx + (rz - (rz&1)) / 2;
		int offsetR = rz;
		return new Vector2(offsetQ, offsetR);
	}

	public Vector2 mouseGridPos() {
		Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(HungryInput.mousePosition);
		Vector3 mouseLocalPos = transform.InverseTransformPoint(mouseWorldPos);
		return toGridCoord(mouseLocalPos.x, mouseLocalPos.y);
	}

	protected bool _mouseOverMenu = false;
	public bool hexClicked() {
		if (_mouseOverMenu)
			return false;
		return HungryInput.mouseButtonClicked;
	}


	public Vector2 toActualCoord(Vector2 offsetPoint) {
		/*
		 x = size * sqrt(3) * (q + 0.5 * (r&1))
		 y = size * 3/2 * r
		 */
		int q = (int)offsetPoint.x, r = (int)offsetPoint.y;
		float x = HEX_RADIUS*SQRT3*(q+0.5f*(r&1));
		float y = HEX_RADIUS*(1.5f)*r;
		return new Vector2(x, y);
	}

	public IEnumerable<HexGridPiece> currentGridInhabitants(Vector2 point) {
		if (inGrid(point)) {
			int x = (int)point.x, y = (int)point.y;
			for (int z = 0; z < GRID_DEPTH; z++) {
				if (_currentGrid[x, y, z] != null)
					yield return _currentGrid[x, y, z];
			}
		}
		else
			yield break;
	}
	
	public IEnumerable<HexGridPiece> claimedGridInhabitants(Vector2 point) {
		if (inGrid(point)) {
			int x = (int)point.x, y = (int)point.y;
			for (int z = 0; z < GRID_DEPTH; z++) {
				if (_claimedGrid[x, y, z] != null)
					yield return _claimedGrid[x, y, z];
			}
		}
		else
			yield break;
	}

	public void removeFromGame(HexGridPiece item) {
		_piecesToRemove.Add(item);
	}

}
