using UnityEngine;
using System.Collections;

/*
 * Rewrite of the original GridPiece class with functions relevant to Porridge Purge specifically
 */
public class HexGridPiece : MonoBehaviour {

	// TYPES GO HERE
	// Types are bitwise values, so make sure they're powers of 2
	// e.g. (1, 2, 4, 8, 16 or 0x1, 0x2, 0x4, 0x8, 0x10)
	public const uint WALL_TYPE = 0x1;
	public const uint PLAYER_TYPE = 0x2;
	public const uint PORRIDGE_TYPE = 0x4;
	public const uint POT_TYPE = 0x8;
	public const uint BARRICADE_TYPE = 0x10;
	public const uint VOMIT_TYPE = 0x20;
	public const uint EXIT_TYPE = 0x40;
	public const uint CHEST_TYPE = 0x80;

	protected SpriteRenderer _spriteRender;

	protected Vector2 _velocity;
	protected Vector2 _currentMovementTarget;
	protected float _moveSpeed;
	// For moving along a path.
	protected bool _followingPath;
	public bool followingPath {
		get { return _followingPath; }
	}
	protected Vector2[] _pathPoints;
	public Vector2 lastPathPoint {
		get { return _pathPoints[_pathPoints.Length-1]; }
	}
	protected int _currentPathIndex;

	protected Vector2 _gridPos;
	public Vector2 gridPos {
		get { return _gridPos; }
		set { _gridPos = value; }
	}

	// Functions to tell whether we're moving after the sync step
	protected bool _moveAfterSync;
	public bool moveAfterSync {
		get { return _moveAfterSync; }
		set { _moveAfterSync = value; }
	}

	protected Vector2 _nextGridPos;
	public Vector2 nextGridPos {
		get { return _nextGridPos; }
		set { _nextGridPos = value; }
	}

	public bool moving {
		get { return _moveSpeed != 0; }
	}

	public float growSpeed = 5f;
	protected bool _growing = false;
	public bool growing {
		get { return _growing; }
		set { _growing = value; }
	}

	public float shrinkSpeed = 5f;
	protected bool _shrinking = false;
	public bool shrinking {
		get { return _shrinking; }
		set { _shrinking = value; }
	}
	public bool shrunk {
		get { return _shrinking || transform.localScale.x < 1; }
	}
	protected bool _dieAfterShrink = true;
	public bool dieAfterShrink {
		get { return _dieAfterShrink; }
		set { _dieAfterShrink = value; }
	}


	protected bool _shaking = false;
	public float shakeTime = 1.5f;
	public float shakeInterval = 0.1f;
	public float shakeAngle = 15f;


	// Just in case we die multiple times in a single frame, so it only registers once
	protected bool _alive = true;
	public bool alive {
		get { return _alive; }
		set { _alive = value; }
	}

	protected uint _type;
	public uint type {
		get { return _type; }
		set { _type = value; }
	}
	public bool hasType(uint testType) {
		return (_type & testType) != 0;
	}

	protected bool _syncPerformed;
	public bool syncPerformed {
		get { return _syncPerformed; }
		set { _syncPerformed = value; }
	}

	protected bool _isOurTurn = false;
	public bool isOurTurn {
		get { return _isOurTurn; }
	}


	// Easy access to 2D coordinates and simple values
	public float x {
		get { return transform.localPosition.x; }
		set { transform.localPosition = new Vector3(value, transform.localPosition.y, transform.localPosition.z); }
	}
	public float y {
		get { return transform.localPosition.y; }
		set { transform.localPosition = new Vector3(transform.localPosition.x, value, transform.localPosition.z); }
	}
	public float z {
		get { return transform.localPosition.z; }
		set { transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, value); }
	}
	public float red {
		get { return _spriteRender.color.r; }
		set { _spriteRender.color = new Color(value, _spriteRender.color.g, _spriteRender.color.b, _spriteRender.color.a); }
	}
	public float green {
		get { return _spriteRender.color.g; }
		set { _spriteRender.color = new Color(_spriteRender.color.r, value, _spriteRender.color.b, _spriteRender.color.a); }
	}
	public float blue {
		get { return _spriteRender.color.b; }
		set { _spriteRender.color = new Color(_spriteRender.color.r, _spriteRender.color.g, value, _spriteRender.color.a); }
	}
	public float alpha {
		get { return _spriteRender.color.a; }
		set { _spriteRender.color = new Color(_spriteRender.color.r, _spriteRender.color.g, _spriteRender.color.b, value); }
	}
	protected float _startAngle;


	// Use this for initialization
	public virtual void Start () {
		_spriteRender = (renderer as SpriteRenderer);

		// If we're laid out like a rect grid, have to correct that first
		if (HexGrid.instance.isRectGridLayout)
			_gridPos = HexGrid.instance.rectToGridCoord(x, y);
		else
			_gridPos = HexGrid.instance.toGridCoord(x, y);
		// Sync up with our current grid position
		Vector2 actualPos = HexGrid.instance.toActualCoord(_gridPos);
		x = actualPos.x;
		y = actualPos.y;

		_startAngle = transform.localRotation.eulerAngles.z;

		// Add ourselves to the official grid
		HexGrid.instance.addPiece(this);

		if (_growing)
			transform.localScale = Vector3.zero;
	}

	public virtual void preSync() {

	}

	public virtual void postSync() {

	}

	public virtual bool isSyncReady() {
		return _moveSpeed == 0 && !_shrinking && !_growing && !_shaking;
	}

	// This function is called once per frame whenever we're in a synchronized grid position (i.e. not moving in between grid positions, or scaling in or out)
	public virtual void performSync() {
		if (_isOurTurn)
			syncOnTurn();
	}

	// Explicitly called when we're synced with the grid
	// AND IT'S OUR TURN TO MOVE
	// For players, this means this will only be called when they can be selected
	// or are in the middle of a turn
	// FOR porridge pots, this means this will only be called on its individual turn
	public virtual void syncOnTurn() {


	}

	// Same as above, but called during the per-frame update if it's our turn
	public virtual void updateOnTurn() {

	}

	// Called by us when we want to officially end our turn
	public virtual void beginTurn() {
		_isOurTurn = true;
	}


	// Called by us when we want to officially end our turn
	public virtual void endTurn() {
		_isOurTurn = false;
	}




	public virtual void die() {
		_alive = false;
		HexGrid.instance.removeFromGame(this);
	}

	protected virtual void updateMotion() {
		x += _velocity.x*Time.deltaTime;
		y += _velocity.y*Time.deltaTime;
	}

	protected virtual void onBeginEnterPoint(Vector2 actualPos) {


	}

	protected virtual void updatePointMovement() {
		if (Vector2.Distance(_currentMovementTarget, transform.localPosition) < _moveSpeed*Time.deltaTime) {
			if (_followingPath) {
				x = _currentMovementTarget.x;
				y = _currentMovementTarget.y;
				_currentMovementTarget = _pathPoints[_currentPathIndex++];
				onBeginEnterPoint(_currentMovementTarget);
				_velocity = (_currentMovementTarget - (Vector2)transform.localPosition).normalized*_moveSpeed;
				if (_currentPathIndex >= _pathPoints.Length) {
					_followingPath = false;
				}
			}
			else
				_moveSpeed = 0;

		}
	}

	protected virtual void updateScale() {
		if (_growing) {
			float scale = transform.localScale.x;
			scale = Mathf.Min (1f, scale+Time.deltaTime*growSpeed);
			transform.localScale = Vector3.one*scale;
			if (scale == 1f)
				_growing = false;
		}
		if (_shrinking) {
			float scale = transform.localScale.x;
			scale = Mathf.Max(0f, scale-Time.deltaTime*shrinkSpeed);
			transform.localScale = Vector3.one*scale;
			if (scale == 0f) {
				_shrinking = false;
				if (_dieAfterShrink)
					die ();
			}

		}

	}

	public void followPath(Vector2[] path, float speed) {
		if (path.Length == 0)
			return;
		else if (path.Length == 1) {
			moveToPoint(path[0], speed);
		}
		else {
			_followingPath = true;
			_pathPoints = path;
			_currentPathIndex = 0;
			moveToPoint(_pathPoints[_currentPathIndex++], speed);
		}
	}

	public void moveToPoint(Vector2 point, float speed) {
		_currentMovementTarget = point;
		onBeginEnterPoint(_currentMovementTarget);
		_moveSpeed = speed;
		_velocity = (_currentMovementTarget - (Vector2)transform.localPosition).normalized*_moveSpeed;
	}

	// Update is called once per frame
	public virtual void Update () {
		//if (HexGrid.instance.isPaused)
		//	return;
		updateScale();
		updatePointMovement();
		if (_moveSpeed == 0) {
			// Figure out what our absolute grid coordinates are and lock to them
			Vector2 gridPos = HexGrid.instance.toGridCoord(x, y);
			Vector2 actualPos = HexGrid.instance.toActualCoord(gridPos);
			x = actualPos.x;
			y = actualPos.y;
			_velocity = Vector2.zero;
		}
		updateMotion();
		if (_isOurTurn)
			updateOnTurn();
	}

	public virtual bool canEnterPoint(Vector2 point) {
		if (!HexGrid.instance.inGrid(point))
			return false;

		foreach (HexGridPiece claimer in HexGrid.instance.currentGridInhabitants(point)) {
			if (!canOverlapWithPiece(claimer)) {
				// Check to see if this piece is moving
				if (!claimer.moveAfterSync || claimer.nextGridPos == _gridPos || claimer.nextGridPos == claimer.gridPos)
					return false;
			}
		}
		foreach (HexGridPiece claimer in HexGrid.instance.claimedGridInhabitants(point)) {
			if (!canOverlapWithPiece(claimer))
				return false;
		}

		return true;
	}

	public virtual bool canOverlapWithPiece(HexGridPiece piece) {
		return true;
	}

	public virtual void shake() {
		if (!_shaking)
			StartCoroutine(doShake());
	}

	protected virtual IEnumerator doShake() {
		_shaking = true;
		float t = 0;
		float shakeCounter = 0;
		float shakeMultiplier = 1;
		while (t < shakeTime) {
			yield return 0;
			t += Time.deltaTime;
			shakeCounter += Time.deltaTime;
			if (shakeCounter >= shakeInterval) {
				shakeMultiplier = -shakeMultiplier;
				transform.localRotation = Quaternion.Euler(0, 0, _startAngle+shakeAngle*shakeMultiplier);
				shakeCounter = 0;
			}
		}
		transform.localRotation = Quaternion.Euler(0, 0, _startAngle);
		_shaking = false;
		onShakeEnd();
	}

	protected virtual void onShakeEnd() {


	}




}
