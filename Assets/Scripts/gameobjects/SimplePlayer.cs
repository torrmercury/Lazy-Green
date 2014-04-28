using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimplePlayer : HexGridPiece {

	private PlayerGUI myGUIScript;

	private SoundManager soundManagerScript;

	public GameObject invisibleWall;
	public GameObject movingRaftObject;
	public AudioClip letsGoSound;
	public bool hasLeftOnRaft = false;

	protected List<GameObject> _justPluggedPots;
	private GameObject recentlySpanwedPluggedPot;
	public GameObject pluggedPotObject;
	public AudioClip plugSound;
	public Sprite octopusSpritePlugged;
	public Sprite linearSpritePlugged;
	public Sprite littlePotSpritePlugged;

	public GameObject vomitParticleRight;
	public GameObject vomitParticleUpRight;
	public GameObject vomitParticleDownRight;
	public GameObject vomitParticleLeft;
	public GameObject vomitParticleUpLeft;
	public GameObject vomitParticleDownLeft;

	private void colorUpdate(Color newColor){
		_spriteRender.color = newColor;
	}

	// Simple struct for defining how we should expand the highlighted hexes for movement. 
	public struct MovePoint {
		public Vector2 point;
		public Vector2[] path;
		public Sprite sprite;
		public MovePoint(Vector2 point, Vector2[] path, Sprite sprite) {
			this.point = point;
			this.path = path;
			this.sprite = sprite;
		}
		public MovePoint(Vector2 point, Vector2[] path) : this(point, path, HexGrid.instance.normalHighlight) {
		}
	}

	// The currently selected player for performing commands. 
	protected static SimplePlayer _selectedPlayer = null;
	public SimplePlayer selectedPlayer {
		get { return _selectedPlayer; }
		set {  
			if (_selectedPlayer != value) {
				if (_selectedPlayer != null) {
					_selectedPlayer.selected = false;
					_selectedPlayer.onDeselect();
				}
				_selectedPlayer = value;
				if (_selectedPlayer != null) {
					_selectedPlayer.selected = true;
					_selectedPlayer.onSelect();
				}
			}
		}
	}

	// Whether this player is currently selected. 
	protected bool _selected;
	public bool selected {
		get { return _selected; }
		set { _selected = value; }
	}

	public bool amMoving = false;
	
	public AudioClip childrenHappy;
	public AudioClip biteSound;
	public AudioClip yesSound;
	public AudioClip eatSound;
	public AudioClip vomitSound;
	public AudioClip takeDamageSound;
	public AudioClip clickSound;
	public AudioClip clickToWalkSound;
	public AudioClip walkSound;

	public int maxHealth = 3;
	public int maxStomach = 2;

	public string name;

	private bool glowGreenAgain = true;

    protected int _currentHealth=0;
    public int currentHealth {
		get { return _currentHealth; }
		set { _currentHealth = value; }
	}
	protected int _currentStomach = 0;
	public int currentStomach {
		get { return _currentStomach; }
		set { _currentStomach = value; }
	}

	// Some values for effective vomit.
	protected int _currentVomitCount;
	protected HexGrid.HexDir _nextVomitDir;
	public GameObject vomitPrefab;

	protected bool _endTurnAfterShake = false;

	// Some values for vomit shakes
	protected float _vomitShakeCounter = 0;
	protected float _vomitShakeMultiplier = 1;

	// The last direction we moved in
	protected HexGrid.HexDir _lastMoveDir;


	// whether we've moved this turn yet
	protected bool _movedThisTurn = false;
	public bool movedThisTurn {
		get { return _movedThisTurn; }
	}


	// FLAG: Do we really need this?
	protected bool _shouldEndTurn = true;




	// The menu that pops up for this player when she has completed her movement
	public GameObject menuObj;
	// So we can change our wait button to an open button when necessary
	public GameObject waitButton, openButton;



	// FLAG: Needed?

	// Values to determine what we should do at the end of our move.
	protected bool _beginningAction = false;
	public bool beginningAction {
		get { return _beginningAction; }
	}

	// Determines if another character can start their turn right now.
	protected bool _turnInProgress = false;
	public bool turnInProgress {
		get { return _turnInProgress; }
	}


	// These public delegates are the primary way that items can control our movement behavior.

	// Called when we finish a movement
	public delegate void AfterMoveFunction();
	protected bool _hasAfterMoveFunc = false;
	protected AfterMoveFunction _funcAfterMove;
	public AfterMoveFunction funcAfterMove {
		get { return _funcAfterMove; }
		set {
			_funcAfterMove = value;
			if (_funcAfterMove != null)
				_hasAfterMoveFunc = true;
		}
	}

	// Called when we click on one of our available hexes
	public delegate void OnHexClickFunction(MovePoint point);
	protected bool _hasOnHexClickFunc = false;
	protected OnHexClickFunction _funcOnHexClick;
	public OnHexClickFunction funcOnHexClick {
		get { return _funcOnHexClick; }
		set { 
			_funcOnHexClick = value; 
			if (_funcOnHexClick != null)
				_hasOnHexClickFunc = true;
		}
	}

	// Called when we click on the player when hexes are available to click on.
	public delegate void OnHexClickCancel();
	protected bool _hasOnHexCancelFunc = false;
	protected OnHexClickCancel _funcOnHexCancel;
	public OnHexClickCancel funcOnHexCancel {
		get { return _funcOnHexCancel; }
		set {
			_funcOnHexCancel = value;
			if (_funcOnHexCancel != null)
				_hasOnHexCancelFunc = true;
		}
	}
	
	// For if we cancel our move after performing it. 
	protected Vector2 _lastGridPos;

	// FLAG
    // For Yohan's original grid position since he can currently move twice.
    protected Vector2 _lastYohanPos;
    
	bool haseaten = false;

	// The porridge we may or may not have eaten on this turn
	protected HexGridPiece _newlyEatenPorridge = null;
	// TODO: Use a dynamic list instead to properly record the total stuff we've eaten (since it might include porridge + vomit + other stuff).
	protected List<HexGridPiece> _justEatenPieces;

	protected List<HexGridPiece> _justBuiltPieces;

	//Variables used for digesting animation
	public GameObject fartingParticle;
	public GameObject digestText;

	//Variables for eating stuff (text actually)
	public GameObject eatTextPlusTwo;
	public GameObject eatTextPlusThree;

	public override void endTurn () {
		base.endTurn();
		_spriteRender.color = new Color32(101, 101, 101, 255);
		_turnInProgress = false;
		foreach (HexGridPiece piece in HexGrid.instance.currentGridInhabitants(_gridPos)) {
			if (piece.hasType(HexGridPiece.CHEST_TYPE)) {
				(piece as Chest).openChest(this);
			}

			// IF I END THE TURN ON A RAFT, MAKE ME GET THE HELL OUT!!
			if(piece.hasType(HexGridPiece.EXIT_TYPE)){
				piece.GetComponent<ExitHex>().moveMeAway();
				GameObject newMovingRaft = Instantiate(movingRaftObject) as GameObject;
				GameObject newWall = Instantiate(invisibleWall) as GameObject;
				audio.PlayOneShot(letsGoSound);
				newWall.transform.parent = HexGrid.instance.transform;
				newWall.transform.localPosition = transform.localPosition;
				newWall.transform.localScale = Vector3.one;
				newWall.GetComponent<SpriteRenderer>().enabled = false;
				newMovingRaft.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, piece.transform.position.z);
				newMovingRaft.transform.localScale = new Vector3(4,4,1);
				Sprite currentRaftSprite = piece.GetComponent<ExitHex>().childSpriteRenderer.sprite;
				newMovingRaft.GetComponent<SpriteRenderer>().sprite = currentRaftSprite;
				gameObject.GetComponent<SpriteRenderer>().enabled = false;
				hasLeftOnRaft = true;
			}
		}

	}

	public override void beginTurn ()
	{
		if (!_alive)
			return;

		if(!hasLeftOnRaft){
			base.beginTurn ();
			_movedThisTurn = false;
		}

		if (HexGrid.instance.tutorial != null) {
			HexGrid.instance.tutorial.playerStartTurn(this);
		}

		Camera.main.GetComponent<CameraFollow>().followNewTarget(transform);
		_spriteRender.color = new Color32(255, 255, 255, 255);
		_endTurnAfterShake = false;
		// Digestion!
		// Eventually want this to be more apparent
		if (_currentStomach > 0 && !hasLeftOnRaft){
			_currentStomach--;
			
			//Let's FART!!!
			GameObject newFartParticle = Instantiate(fartingParticle) as GameObject;
			GameObject newDigestText = Instantiate(digestText) as GameObject;
			newFartParticle.transform.parent = gameObject.transform;
			newDigestText.transform.parent = gameObject.transform;
			newFartParticle.transform.localPosition = new Vector3(-0.2f, -0.2f, 0.1f);
			newDigestText.transform.localPosition = new Vector3(0.0f, 0.1f, -0.1f);
			iTween.ScaleFrom(newDigestText, iTween.Hash("scale", new Vector3(3,3,1), "time", 1.5f, "oncomplete", "finishedScalling"));
			newDigestText.transform.parent = null;
			soundManagerScript = GameObject.Find("SoundManager").GetComponent<SoundManager>();
			soundManagerScript.playFartSound();
		}
		// Update our items
		foreach (Item item in menuObj.GetComponentsInChildren<Item>(true)) {
			item.beginTurn();
		}
	}

	protected List<MovePoint> _highlightedPoints;
	protected List<MovePoint> _deepHighlightedPoints;

	public virtual void onSelect() {
		// This is where we go through and activate all of our potential movement tiles. 
		determineMovementTiles();
		foreach (MovePoint point in _highlightedPoints) {
			HexGrid.instance.highlightGridPos(point.point, point.sprite);
		}

		if (_currentStomach > maxStomach) {
			if (HexGrid.instance.tutorial != null) {
				HexGrid.instance.tutorial.aboutToVomit(this);
			}
			_deepHighlightedPoints.Clear();
			// Figure out how much we're going to vomit
			HexGrid.HexDir nextVomitDir = _lastMoveDir;
			int numVomit = currentStomach/2;
			for (int i = 0; i < numVomit; i++) {
				Vector2 vomitPos = HexGrid.gridPosFromDir(_gridPos, nextVomitDir);
				if (HexGrid.instance.inGrid(vomitPos)) {
					_deepHighlightedPoints.Add(new MovePoint(vomitPos, null, HexGrid.instance.heavyGreenHighlight));
				}
				nextVomitDir = HexGrid.nextClockwiseDir(nextVomitDir);
			}
			foreach (MovePoint vomitPos in _deepHighlightedPoints) {
				HexGrid.instance.highlightDeepGridPos(vomitPos.point, vomitPos.sprite);
				HexGrid.instance.flashDeepHighlight(vomitPos.point);
			}
		}
	}

	public virtual void onDeselect() {
		foreach (MovePoint point in _highlightedPoints) {
			HexGrid.instance.disableHighlight(point.point);
		}
		foreach (MovePoint point in _deepHighlightedPoints) {
			HexGrid.instance.disableDeepHighlight(point.point);
		}
		_deepHighlightedPoints.Clear();
		_highlightedPoints.Clear();
	}

	protected const int MOVE_DEPTH = 3;

	protected void determineMovementTiles() {
		_gridPos = HexGrid.instance.toGridCoord(x, y);
		_highlightedPoints.Clear();
		HashSet<Vector2> closed = new HashSet<Vector2>();
		closed.Add(_gridPos);
		Queue<MovePoint> agenda = new Queue<MovePoint>();
		agenda.Enqueue(new MovePoint(_gridPos, new Vector2[0]));
		while (agenda.Count > 0) {
			MovePoint currentNode = agenda.Dequeue();
			int stepsUsed = currentNode.path.Length;
			if (currentNode.point != _gridPos && !containsPlayer(currentNode.point))
				_highlightedPoints.Add(currentNode);
			if (stepsUsed < MOVE_DEPTH) {
				foreach (Vector2 neighbor in HexGrid.getNeighbors(currentNode.point)) {
					if (!closed.Contains(neighbor) && HexGrid.instance.inGrid(neighbor) && canMoveThroughPoint(neighbor)) {
						Vector2[] neighborPath = new Vector2[currentNode.path.Length+1];
						for (int i = 0; i < neighborPath.Length; i++) {
							if (i < currentNode.path.Length)
								neighborPath[i] = currentNode.path[i];
							else
								neighborPath[i] = neighbor;
						}
						MovePoint neighborPoint = new MovePoint(neighbor, neighborPath, containsPorridge(neighbor) ? HexGrid.instance.redHighlight : HexGrid.instance.normalHighlight);
						if (!containsPorridge(neighbor))
							agenda.Enqueue(neighborPoint);
						else {
							_highlightedPoints.Add(neighborPoint);
						}
						closed.Add(neighbor);
					}
				}
			}

		}

	}

	protected virtual bool containsPorridge(Vector2 point) {
		foreach (HexGridPiece inhabitant in HexGrid.instance.currentGridInhabitants(point)) {
			if (!inhabitant.shrunk && inhabitant.hasType(HexGridPiece.PORRIDGE_TYPE | HexGridPiece.POT_TYPE))
				return true;
		}
		return false;
	}

	protected virtual bool containsPlayer(Vector2 point) {
		foreach (HexGridPiece inhabitant in HexGrid.instance.currentGridInhabitants(point)) {
			if (!inhabitant.shrunk && inhabitant.hasType(HexGridPiece.PLAYER_TYPE))
				return true;
		}
		return false;
	}

	protected virtual bool containsVomit(Vector2 point) {
		foreach (HexGridPiece inhabitant in HexGrid.instance.currentGridInhabitants(point)) {
			if (!inhabitant.shrunk && inhabitant.hasType(HexGridPiece.VOMIT_TYPE))
				return true;
		}
		return false;
	}

	protected virtual bool pieceIsEdible(HexGridPiece piece) {
		return piece.hasType(HexGridPiece.PORRIDGE_TYPE | HexGridPiece.POT_TYPE);
	}

	protected virtual int pieceStomachSize(HexGridPiece piece) {
		if (piece.hasType(HexGridPiece.PORRIDGE_TYPE)) {
			if (piece.GetComponent<SimplePorridge>()._doubleStacked)
				return 3;
			else
				return 2;
		}
		return 0;
	}
	

	public void takeDamage() {
		// Make sure we only take damage once per turn
		audio.PlayOneShot(takeDamageSound);
		iTween.ValueTo(gameObject, iTween.Hash("from", Color.red, "to", Color.white, "onupdatetarget", gameObject, "onupdate", "colorUpdate", "time", 1.0f));
		if (!_shaking) {
			shake();
			// Force eat the porridge
			int previousStomach = _currentStomach;
			_currentStomach+=2;
			if(_currentStomach - previousStomach == 2){
				GameObject newEatText = Instantiate(eatTextPlusTwo) as GameObject;
				newEatText.transform.parent = gameObject.transform;
				newEatText.transform.position = this.transform.position;
				newEatText.transform.position = new Vector3(transform.position.x, transform.position.y, -0.5f);
				iTween.ScaleFrom(newEatText, iTween.Hash("scale", new Vector3(3,3,1), "time", 1.5f));
			}
			else if(_currentStomach - previousStomach == 3){
				GameObject newEatText = Instantiate(eatTextPlusThree) as GameObject;
				newEatText.transform.parent = gameObject.transform;
				newEatText.transform.localPosition = this.transform.localPosition;
				newEatText.transform.position = new Vector3(transform.position.x, transform.position.y, -0.5f);
			}

			//myGUIScript.appearTextAgain();
			//myGUIScript.startFadingText();
			/*if (_currentHealth > 0)
				_currentHealth--;
			if (currentHealth <= 0)
			{
				die();
			}*/
		}
	}

	// Use this for initialization
	public override void Start () {
		GameObject.DontDestroyOnLoad(gameObject);

		hasLeftOnRaft = false;
		if(gameObject.GetComponent<SpriteRenderer>().enabled == false)
			gameObject.GetComponent<SpriteRenderer>().enabled = true;

		_type = HexGridPiece.PLAYER_TYPE | HexGridPiece.WALL_TYPE;

		if (_currentHealth == 0)
			_currentHealth = maxHealth;
		_highlightedPoints = new List<MovePoint>();
		_deepHighlightedPoints = new List<MovePoint>();
		_justEatenPieces = new List<HexGridPiece>();
		_justBuiltPieces = new List<HexGridPiece>();
		attachMenuToBottom();

		_justPluggedPots = new List<GameObject>();

		base.Start();
		GetComponent<Animator>().Play("Idle");

	}

	public override void die ()
	{
		// TODO: Have some kinda animation? I dunno
		// Corpses stay on the field, so we don't destroy ourselves here. 
		_alive = false;
	}


	public virtual void attachMenuToBottom() {
		menuObj.transform.parent = HexGrid.instance.bottomMenu.transform;
		menuObj.transform.localPosition = Vector3.zero;
	}

	public virtual void detachMenuFromBottom() {
		menuObj.transform.parent = transform;
	}


	protected virtual bool aboutToVomit() {
		return _currentStomach > maxStomach;
	}

	private void greenGlowAgain(){
		glowGreenAgain = true;
	}

	public override void Update ()
	{
		base.Update ();
		if(myGUIScript == null)
			myGUIScript = gameObject.GetComponentInChildren<PlayerGUI>();

		if (aboutToVomit() && !_shaking) {
			_vomitShakeCounter += Time.deltaTime;
			if (_vomitShakeCounter >= shakeInterval) {
				_vomitShakeMultiplier = -_vomitShakeMultiplier;
				transform.localRotation = Quaternion.Euler(0, 0, _startAngle+shakeAngle*_vomitShakeMultiplier);
				_vomitShakeCounter = 0;
				if(glowGreenAgain)
					iTween.ValueTo(gameObject, iTween.Hash("from", Color.green, "to", Color.white, "onupdatetarget", gameObject, "oncompletetarget", gameObject, "oncomplete", "greenGlowAgain", "onupdate", "colorUpdate", "time", 1.0f));
				glowGreenAgain = false;
			}
		}
		else {
			_vomitShakeCounter = 0;
			_vomitShakeMultiplier = 1;
			if (!_shaking) {
				transform.localRotation = Quaternion.Euler(0, 0, _startAngle);
			}
		}
	}


	public virtual bool standingOnExit() {
		// If we're on any exit tile
		foreach (HexGridPiece piece in HexGrid.instance.currentGridInhabitants(_gridPos)) {
			if (piece.hasType(HexGridPiece.EXIT_TYPE))
				return true;
		}
		return false;
	}

	public override bool isSyncReady ()
	{
		return base.isSyncReady () && !_shaking;
	}
	

	// AFTER MOVE FUNCTIONS

	protected virtual void openMenu() {
		/*_turnInProgress = true;
		menuObj.SetActive(true);
		if (HexGrid.instance.tutorial != null) {
			HexGrid.instance.tutorial.menuOpened();
		}*/
	}

	// END AFTER MOVE FUNCTIONS





	protected override void onBeginEnterPoint (Vector2 actualPos)
	{
		Vector2 nextGridPos = HexGrid.instance.toGridCoord(actualPos.x, actualPos.y);
		_lastMoveDir = HexGrid.dirBetweenPoints(HexGrid.instance.toGridCoord(x, y), nextGridPos);
		// Prepare to eat any of the edible objects
		foreach (HexGridPiece piece in HexGrid.instance.currentGridInhabitants(nextGridPos)) {
			// Check to see if the object is edible.
			if (!piece.shrunk && pieceIsEdible(piece)) {
				if(!piece.hasType(POT_TYPE))
					audio.PlayOneShot(eatSound);
				else if(piece.hasType(POT_TYPE))
					audio.PlayOneShot(plugSound);
				eat(piece);
			}
		}

	}

	public virtual void eat(HexGridPiece piece) {
		//Display the text above the character and fades it
		//myGUIScript.appearTextAgain();
		//myGUIScript.startFadingText();

		_justEatenPieces.Add(piece);
		int previousStomach = _currentStomach;
		_currentStomach+=pieceStomachSize(piece);
		if(_currentStomach - previousStomach == 2){
			GameObject newEatText = Instantiate(eatTextPlusTwo) as GameObject;
			newEatText.transform.parent = gameObject.transform;
			newEatText.transform.position = this.transform.position;
			newEatText.transform.position = new Vector3(transform.position.x, transform.position.y, -0.5f);
			iTween.ScaleFrom(newEatText, iTween.Hash("scale", new Vector3(3,3,1), "time", 1.5f));
		}
		else if(_currentStomach - previousStomach == 3){
			GameObject newEatText = Instantiate(eatTextPlusThree) as GameObject;
			newEatText.transform.parent = gameObject.transform;
			newEatText.transform.localPosition = this.transform.localPosition;
			newEatText.transform.position = new Vector3(transform.position.x, transform.position.y, -0.5f);
		}
		//CREATE PLUGGED POT GAME OBJECT HERE
		if (piece.hasType(POT_TYPE)){
			GameObject newPluggedPot = Instantiate(pluggedPotObject) as GameObject;
			newPluggedPot.transform.parent = HexGrid.instance.transform;
			newPluggedPot.transform.position = piece.transform.position;
			newPluggedPot.transform.localScale = new Vector3(1,1,1);
			_justPluggedPots.Add(newPluggedPot);

			//Check type of pot and, if it's linear, the direction
			if(piece.GetComponent<LittlePot>() != null){
				//If the pot if the LITTLE POT
				newPluggedPot.GetComponent<SpriteRenderer>().sprite = littlePotSpritePlugged;
			}else if(piece.GetComponent<LinearPorridgePipe>() == null){
				//If the pot if the OCTOPUT POT
				newPluggedPot.GetComponent<SpriteRenderer>().sprite = octopusSpritePlugged;
			} else if(piece.GetComponent<LinearPorridgePipe>().pipeDirection == HexGrid.HexDir.Right){
				//If the pot if the linear one TO THE RIGHT
				newPluggedPot.GetComponent<SpriteRenderer>().sprite = linearSpritePlugged;
			} else if(piece.GetComponent<LinearPorridgePipe>().pipeDirection == HexGrid.HexDir.Left){
				//If the pot if the linear one TO THE LEFT
				newPluggedPot.GetComponent<SpriteRenderer>().sprite = linearSpritePlugged;
				newPluggedPot.transform.localScale = new Vector3(-newPluggedPot.transform.localScale.x, newPluggedPot.transform.localScale.y, newPluggedPot.transform.localScale.z);
			}
		}
		piece.shrinking = true;
		piece.dieAfterShrink = false;
		if (piece.hasType(POT_TYPE)){
			audio.PlayOneShot(childrenHappy);
		}
	}

	protected override void onShakeEnd ()
	{
		base.onShakeEnd ();
		if (_isOurTurn && _endTurnAfterShake) {
			_endTurnAfterShake = false;
			endTurn();
		}

	}

	public virtual void clearMovementHexes() {
		foreach (MovePoint point in _highlightedPoints) {
			HexGrid.instance.disableHighlight(point.point);
		}
		_highlightedPoints.Clear();
	}


	public override void syncOnTurn () {
		amMoving = false;

		// Check to see if we're on a chest
		bool onChest = false;
		foreach (HexGridPiece piece in HexGrid.instance.currentGridInhabitants(_gridPos)) {
			if (piece.hasType(HexGridPiece.CHEST_TYPE)) {
				onChest = true;
			}
		}
		if (onChest) {
			openButton.SetActive(true);
			waitButton.SetActive(false);
		}
		else {
			waitButton.SetActive(true);
			openButton.SetActive(false);
		}

		// If it's time to begin an action, do that.
		if (_beginningAction) {
			if (HexGrid.instance.tutorial != null) {
				HexGrid.instance.tutorial.moveFinished();
				bool eatenPorridge = false;
				foreach (HexGridPiece eaten in _justEatenPieces) {
					if (eaten.hasType(HexGridPiece.PORRIDGE_TYPE))
						eatenPorridge = true;
				}
				if (eatenPorridge) {
					HexGrid.instance.tutorial.justEaten(this);
				}
			}




			if (_hasAfterMoveFunc) {
				_funcAfterMove();
				_hasAfterMoveFunc = false;
			}
			else {
				openMenu();

			}

			// When we're moving, check to see if we're about to vomit and highlight the correct tiles if so.
			if (_currentStomach > maxStomach) {
				if (HexGrid.instance.tutorial != null) {
					HexGrid.instance.tutorial.aboutToVomit(this);
				}
				_deepHighlightedPoints.Clear();
				// Figure out how much we're going to vomit
				HexGrid.HexDir nextVomitDir = _lastMoveDir;
				int numVomit = currentStomach/2;
				for (int i = 0; i < numVomit; i++) {
					Vector2 vomitPos = HexGrid.gridPosFromDir(_gridPos, nextVomitDir);
					if (HexGrid.instance.inGrid(vomitPos)) {
						_deepHighlightedPoints.Add(new MovePoint(vomitPos, null, HexGrid.instance.heavyGreenHighlight));
					}
					nextVomitDir = HexGrid.nextClockwiseDir(nextVomitDir);
				}
				foreach (MovePoint vomitPos in _deepHighlightedPoints) {
					HexGrid.instance.highlightDeepGridPos(vomitPos.point, vomitPos.sprite);
					HexGrid.instance.flashDeepHighlight(vomitPos.point);
				}
			}

			_beginningAction = false;

		}
		                   


		// If we're vomitting, do that
		if (_currentVomitCount > 0) {
			audio.PlayOneShot(vomitSound);
			_currentVomitCount--;
			shake();

			if (_currentVomitCount == 0) {
				_endTurnAfterShake = true;
			}
			// Spawn a vomit if the next location doesn't contain any. 
			Vector2 nextVomitPoint = HexGrid.gridPosFromDir(_gridPos, _nextVomitDir);
			//VOMIT PARTICLES EFFECT
			Vector2 originPos = HexGrid.instance.toActualCoord(_gridPos);
			Vector3 finalOriginPos = new Vector3(originPos.x, originPos.y, -1.0f);
			if(_nextVomitDir == HexGrid.HexDir.Right){
				GameObject newFlow = Instantiate(vomitParticleRight) as GameObject;
				newFlow.transform.parent = HexGrid.instance.transform;
				newFlow.transform.localPosition = finalOriginPos;
			} else if(_nextVomitDir == HexGrid.HexDir.Left){
				GameObject newFlow = Instantiate(vomitParticleLeft) as GameObject;
				newFlow.transform.parent = HexGrid.instance.transform;
				newFlow.transform.localPosition = finalOriginPos;
			} else if(_nextVomitDir == HexGrid.HexDir.UpRight){
				GameObject newFlow = Instantiate(vomitParticleUpRight) as GameObject;
				newFlow.transform.parent = HexGrid.instance.transform;
				newFlow.transform.localPosition = finalOriginPos;
			} else if(_nextVomitDir == HexGrid.HexDir.UpLeft){
				GameObject newFlow = Instantiate(vomitParticleUpLeft) as GameObject;
				newFlow.transform.parent = HexGrid.instance.transform;
				newFlow.transform.localPosition = finalOriginPos;
			} else if(_nextVomitDir == HexGrid.HexDir.DownRight){
				GameObject newFlow = Instantiate(vomitParticleDownRight) as GameObject;
				newFlow.transform.parent = HexGrid.instance.transform;
				newFlow.transform.localPosition = finalOriginPos;
			} else if(_nextVomitDir == HexGrid.HexDir.DownLeft){
				GameObject newFlow = Instantiate(vomitParticleDownLeft) as GameObject;
				newFlow.transform.parent = HexGrid.instance.transform;
				newFlow.transform.localPosition = finalOriginPos;
			}
			if (HexGrid.instance.inGrid(nextVomitPoint) && !containsVomit(nextVomitPoint)) {
				Camera.main.GetComponent<ScreenShake>().bigShake();
				Vector2 vomitPos = HexGrid.instance.toActualCoord(nextVomitPoint);
				GameObject vomitObj = Instantiate(vomitPrefab) as GameObject;
				vomitObj.transform.parent = HexGrid.instance.transform;
				HexGridPiece vomit = vomitObj.GetComponent<HexGridPiece>();
				vomit.x = vomitPos.x;
				vomit.y = vomitPos.y;
				vomit.GetComponent<Vomit>().growing = true;
			}
			_nextVomitDir = HexGrid.nextClockwiseDir(_nextVomitDir);
			return;
		}
		if (HexGrid.instance.hexClicked() && HexGrid.instance.mouseGridPos() == _gridPos) {
			if (!_selected) {
				// If another character is already taking a turn, cancel it. 
				if (selectedPlayer != null && selectedPlayer.turnInProgress) {
					selectedPlayer.cancelPressed();
				}


				// Check if this is cool with the tutorial first
				if (HexGrid.instance.tutorial != null) {
					if (!HexGrid.instance.tutorial.playerSelected(this))
						return;
				}


				// FLAG: If you want the camera zooming to the player
				//Camera.main.GetComponent<CameraFollow>().followNewTarget(transform);
				selectedPlayer = this;

				HexGrid.instance.dynamicBottomMenu.openPlayerMenu(menuObj);

				_lastGridPos = _gridPos;
				audio.PlayOneShot(yesSound);
				return;
			}
		}
		// Now if we're selected and we click somewhere where we can move, start the move. 
		if (_selected && HexGrid.instance.hexClicked()) {
			// Check to see if our mouse pos is one of our available tiles
			// FLAG: Depreceated. Now that the menu is always there, no longer need to click on the player to cancel.
			Vector2 mousePos = HexGrid.instance.mouseGridPos();
			/*if (mousePos == _gridPos) {
				// Check with our tutorial
				if (HexGrid.instance.tutorial != null) {
					if (!HexGrid.instance.tutorial.hexClicked(mousePos))
						return;
				}

				if (_hasOnHexCancelFunc) {
					_funcOnHexCancel();
					_hasOnHexCancelFunc = false;
				}
				onDeselect();
				openMenu();
			}*/
			if (mousePos != gridPos) {
				foreach (MovePoint point in _highlightedPoints) {
					if (point.point == mousePos) {
						if (HexGrid.instance.tutorial != null) {
							if (!HexGrid.instance.tutorial.hexClicked(mousePos))
								return;
						}

						audio.PlayOneShot(clickToWalkSound);
						if (_hasOnHexClickFunc) {
							_funcOnHexClick(point);
							_hasOnHexClickFunc = false;
						}
						else
							moveToHex(point);
						return;
					}
						
				}
			}

		}
		if (!_selected)
			GetComponent<Animator>().Play("Idle");
		else {
			GetComponent<Animator>().Play("Walk");
		}
		//GetComponent<Animator>().Play ("Idle");
	}

	// BEGIN On Hex Click functions (functions that are called when a specific hex is clicked)
	public virtual void moveToHex(MovePoint point) {
		amMoving = true;
		_hasAfterMoveFunc = false;
		_hasOnHexClickFunc = false;
		_turnInProgress = true;
		_movedThisTurn = true;
		Vector2[] actualPath = new Vector2[point.path.Length];
		for (int i = 0; i < actualPath.Length; i++) {
			actualPath[i] = HexGrid.instance.toActualCoord(point.path[i]);
		}
		
		_beginningAction = true;
		followPath(actualPath, HexGrid.MOVE_SPEED);
		_nextGridPos = point.path[point.path.Length-1];

		onDeselect();

		GetComponent<Animator>().Play ("Walk");
	}


	// END On Hex Click functions




	public bool canBePushedIntoPoint(Vector2 point) {
		if (!HexGrid.instance.inGrid(point))
			return false;
		
		foreach (HexGridPiece claimer in HexGrid.instance.currentGridInhabitants(point)) {
			if (!canOverlapWithPiece(claimer) || claimer.hasType(PORRIDGE_TYPE))
				return false;
		}
		foreach (HexGridPiece claimer in HexGrid.instance.claimedGridInhabitants(point)) {
			if (!canOverlapWithPiece(claimer) || claimer.hasType(PORRIDGE_TYPE))
				return false;
		}
		return true;
	}

	public bool canMoveThroughPoint(Vector2 point) {
		if (!HexGrid.instance.inGrid(point))
			return false;
		
		foreach (HexGridPiece claimer in HexGrid.instance.currentGridInhabitants(point)) {
			if (!canMoveThroughPiece(claimer))
				return false;
		}
		foreach (HexGridPiece claimer in HexGrid.instance.claimedGridInhabitants(point)) {
			if (!canMoveThroughPiece(claimer))
				return false;
		}
		return true;
	}

	public virtual bool canMoveThroughPiece(HexGridPiece claimer) {
		if (claimer.shrunk)
			return true;
		return claimer.hasType(HexGridPiece.POT_TYPE | HexGridPiece.PLAYER_TYPE | HexGridPiece.BARRICADE_TYPE) || !claimer.hasType(HexGridPiece.WALL_TYPE | HexGridPiece.VOMIT_TYPE);
	}

	public override bool canOverlapWithPiece (HexGridPiece piece)
	{
		if (piece.shrunk)
			return true;

		if (piece.hasType(HexGridPiece.WALL_TYPE))
			return false;
		return true;
	}


	public virtual void cancelPressed() {
		if (!isSyncReady() || HexGrid.instance.isPaused)
			return;


		myGUIScript.disappearMe();
		myGUIScript.stopFadeCorountine();

		// Check our tutorial first
		if (HexGrid.instance.tutorial != null) {
			if (!HexGrid.instance.tutorial.buttonPressed("cancel"))
				return;
		}

		_movedThisTurn = false;

		HexGrid.instance.dynamicBottomMenu.closeMenu();


		audio.PlayOneShot(clickSound);

		// Move back to the gridPos
		Vector2 actualPos = HexGrid.instance.toActualCoord(_lastGridPos);
        _nextGridPos = _lastGridPos;
		x = actualPos.x;
		y = actualPos.y;

		//Remove recently placed plugged pot
		foreach (GameObject piece in _justPluggedPots) {
			Destroy(piece);
		}
		//Destroy(recentlySpanwedPluggedPot);
		_justPluggedPots.Clear();

		foreach (HexGridPiece piece in _justEatenPieces) {
			piece.shrinking = false;
			piece.transform.localScale = Vector3.one;
			_currentStomach-=pieceStomachSize(piece);
		}
		_justEatenPieces.Clear();

		foreach (HexGridPiece piece in _justBuiltPieces) {
			piece.die();
		}
		_justBuiltPieces.Clear();


		HexGrid.instance.currentState = HexGrid.GameState.Run;
		selectedPlayer = this;

		foreach (Item item in menuObj.GetComponentsInChildren<Item>(true)) {
			item.cancelPressed();
		}

		menuObj.SetActive(false);
		selectedPlayer = null;
		_turnInProgress = false;
		_hasOnHexClickFunc = false;

		HexGrid.instance.constructCurrentGrid();

	}

	public virtual void waitPressed() {

		_justPluggedPots.Clear();

		if (!isSyncReady() || HexGrid.instance.isPaused)
			return;

		// Check our tutorial first
		if (HexGrid.instance.tutorial != null) {
			if (!HexGrid.instance.tutorial.buttonPressed("wait"))
				return;
		}
		GetComponent<Animator>().Play("Idle");

		HexGrid.instance.dynamicBottomMenu.closeMenu();

		audio.PlayOneShot(clickSound);

		HexGrid.instance.currentState = HexGrid.GameState.Run;
		menuObj.SetActive(false);

		foreach (HexGridPiece piece in _justEatenPieces) {
			piece.die();
		}
		_justEatenPieces.Clear();

		_justBuiltPieces.Clear();
        
		selectedPlayer = null;

		if (currentStomach > maxStomach) {
			/*if (_currentHealth > 0)
				_currentHealth--;
			if (currentHealth <= 0) {
				die();
			}*/
			_currentVomitCount = currentStomach/2;
			_currentStomach = 0;
			_nextVomitDir = _lastMoveDir;
		}
		else if (_shouldEndTurn) {
			endTurn();
		}
        if (haseaten == true)
        {
            haseaten = false;
        }
	}
		



}
