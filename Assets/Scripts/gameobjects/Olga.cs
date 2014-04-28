using UnityEngine;
using System.Collections;

public class Olga : SimplePlayer {

	public GameObject barricadeObject;

	public AudioClip buildSound;

	public static SimplePlayer theOneTrueOlga = null;

	public static bool olgaSpawned = false;

	protected bool _builtBarricadeThisTurn;

	public int barricadeStomachThreshold = 2;

	// For cancelling our actions if we repaired a barricade instead of building one. 
	protected bool _repairedBarricadeThisTurn;
	protected SimpleBarricade _repairedBarricade;
	protected int _repairedBarricadeOldHealth;

	public GameObject barricadeMenuObject;
	protected tk2dSprite _barricadeButtonSprite;

	// Should just have a single flag for whether we can press the barricade button
	protected bool _canBuildBarricade = true;


	protected void setButtonColor(Color color) {
		if (_barricadeButtonSprite != null) {
			_barricadeButtonSprite.color = color;
			foreach (ShadowText text in _barricadeButtonSprite.GetComponentsInChildren<ShadowText>(true)) {
				text.setColor(color);
			}
		}
	}


	public override void Start ()
	{
		if (olgaSpawned) {
			Destroy (gameObject);
			return;
		}

		base.Start ();
		olgaSpawned = true;
		theOneTrueOlga = this;
	
		_barricadeButtonSprite = barricadeMenuObject.GetComponent<tk2dSprite>();
	}

	public override void syncOnTurn ()
	{
		base.syncOnTurn ();
		// Check to see if we've eaten enough to activate the barricade. 
		checkCanBuildBarricade();
	}


	public override void beginTurn ()
	{
		base.beginTurn ();
		_builtBarricadeThisTurn = false;
		_repairedBarricadeThisTurn = false;
	}

	public override void cancelPressed ()
	{
		base.cancelPressed ();
		_builtBarricadeThisTurn = false;
		if (_repairedBarricadeThisTurn) {
			_repairedBarricade.currentHealth = _repairedBarricadeOldHealth;
			_repairedBarricade.updateBarricadeSprite();
			_repairedBarricadeThisTurn = false;
		}

	}

	protected void deactivateBarricadeButton() {
		_canBuildBarricade = false;
		tk2dSprite sprite = barricadeMenuObject.GetComponent<tk2dSprite>();
		sprite.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
		foreach (ShadowText text in barricadeMenuObject.GetComponentsInChildren<ShadowText>(true)) {
			text.setColor(new Color(0.5f, 0.5f, 0.5f, 0.5f));
		}
	}

	protected void activateBarricadeButton() {
		_canBuildBarricade = true;
		tk2dSprite sprite = barricadeMenuObject.GetComponent<tk2dSprite>();
		sprite.color = Color.white;
		foreach (ShadowText text in barricadeMenuObject.GetComponentsInChildren<ShadowText>(true)) {
			text.setColor(Color.white);
		}
	}

	// Function that tells us whether we've met the conditions to be able to build a barricade. (called on the beginning of a turn or during a sync).
	protected void checkCanBuildBarricade() {

		if (_builtBarricadeThisTurn)
			return;

		bool canBuildBarricade = _currentStomach >= barricadeStomachThreshold;
		if (!canBuildBarricade) {
			ShadowText barricadeText = barricadeMenuObject.GetComponentInChildren<ShadowText>();
			if (barricadeText != null)
				barricadeText.setText(string.Format("{0}/{1}", _currentStomach, barricadeStomachThreshold));
		}
		else {
			ShadowText barricadeText = barricadeMenuObject.GetComponentInChildren<ShadowText>();
			if (barricadeText != null)
				barricadeText.setText("Build");
		}

		if (canBuildBarricade) {
			// Check to see if we're on top of another barricade.
			bool onFullBarricade = false;
			foreach (HexGridPiece piece in HexGrid.instance.currentGridInhabitants(_gridPos)) {
				if (piece.hasType(HexGridPiece.BARRICADE_TYPE)) {
					SimpleBarricade barricade = piece.GetComponent<SimpleBarricade>();
					if (barricade.currentHealth == barricade.maxHealth)
						canBuildBarricade = false;
				}
			}		
		}


		// Now, activate or deactivate our barricade based on what it currently is
		if (!_canBuildBarricade && canBuildBarricade) {
			activateBarricadeButton();
		}
		else if (_canBuildBarricade && !canBuildBarricade) {
			deactivateBarricadeButton();
		}

	}

	
	public void buildBarricade(){

		if (!_canBuildBarricade)
			return;

		if (!isSyncReady() || HexGrid.instance.isPaused)
			return;

		// Check our tutorial first
		if (HexGrid.instance.tutorial != null) {
			if (!HexGrid.instance.tutorial.buttonPressed("barricade"))
				return;
		}

		_turnInProgress = true;

		_builtBarricadeThisTurn = true;

		// Check to see if there's another barricade already there and whether it needs to be repaired. 
		bool onDamagedBarricade = false;
		SimpleBarricade damagedBarricade = null;
		foreach (HexGridPiece piece in HexGrid.instance.currentGridInhabitants(_gridPos)) {
			if (piece.hasType(HexGridPiece.BARRICADE_TYPE)) {
				SimpleBarricade barricade = piece.GetComponent<SimpleBarricade>();
				if (barricade.currentHealth < barricade.maxHealth) {
					onDamagedBarricade = true;
					damagedBarricade = barricade;
				}
			}
		}
		
		audio.PlayOneShot(clickSound);
		audio.PlayOneShot(buildSound);

		if (onDamagedBarricade) {
			_repairedBarricadeThisTurn = true;
			_repairedBarricade = damagedBarricade;
			_repairedBarricadeOldHealth = damagedBarricade.currentHealth;
			damagedBarricade.fixBarricade();
		}
		else {
			Vector2 actualPos = HexGrid.instance.toActualCoord(_gridPos);
			GameObject newBarricade = Instantiate(barricadeObject) as GameObject;
			newBarricade.transform.parent = HexGrid.instance.transform;
			newBarricade.GetComponent<HexGridPiece>().x = actualPos.x;
			newBarricade.GetComponent<HexGridPiece>().y = actualPos.y;
			newBarricade.GetComponent<HexGridPiece>().growing = true;
			HexGrid.instance.addToCurrentGrid(newBarricade.GetComponent<HexGridPiece>());
			_justBuiltPieces.Add(newBarricade.GetComponent<HexGridPiece>());
		}

		deactivateBarricadeButton();
	}

}
