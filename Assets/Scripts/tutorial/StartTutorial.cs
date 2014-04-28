using UnityEngine;
using System.Collections;

public class StartTutorial : Tutorial {

	// the object we use to flash on the hexes we want the player to click. 
	public GameObject flashyHex;

	public GameObject raft;
	public GameObject olga;

	public GameObject olgaMenu;
	public GameObject olgaWaitButton;
	public GameObject olgaBuildButton;

	public GameObject firstPorridge;

	public Vector2 secondPorridgePos = new Vector2(3, 2);
	public Vector2 pipePos = new Vector2(4, 2);

	protected enum TutorialPhase {
		FirstTurn,
		SecondTurn,
		ThirdTurn,
		EndingThirdTurn,
		FourthTurn
	}
	protected TutorialPhase _currentPhase = TutorialPhase.FirstTurn;

	protected Vector2 _targetHex;


	// Use this for initialization
	public override void Start () {
		base.Start();
		HexGrid.instance.dialogueWindow.ActivateDialogue("dialogue/tutorial1start");
		flashyHex.SetActive(false);
	}

	public override void processDialogueEvent (string eventName)
	{
		if (eventName == "raft" && (_currentPhase == TutorialPhase.FirstTurn || _currentPhase == TutorialPhase.FourthTurn)) {
			// Show off the raft
			moveFlashyHex(raft);
			flashyHex.SetActive(true);
		}
		else if (eventName == "clickplayer" && (_currentPhase == TutorialPhase.FirstTurn || _currentPhase == TutorialPhase.FourthTurn)) {
			moveFlashyHex(olga);
			flashyHex.SetActive(true);
		}
		else if (eventName == "firstporridge" && _currentPhase == TutorialPhase.FirstTurn) {
			moveFlashyHex(firstPorridge);
			_targetHex = HexGrid.instance.toGridCoord(firstPorridge.transform.localPosition.x, firstPorridge.transform.localPosition.y);
			flashyHex.SetActive(true);
		}
		else if (eventName == "secondporridge" && _currentPhase == TutorialPhase.SecondTurn) {
			moveFlashyHexToGridPos(secondPorridgePos);
			flashyHex.SetActive(true);
		}
		else if (eventName == "secondturnselect" && _currentPhase == TutorialPhase.SecondTurn) {
			moveFlashyHex(olga);
			flashyHex.SetActive(true);
		}
		else if (eventName == "pipe" && _currentPhase == TutorialPhase.ThirdTurn) {
			moveFlashyHexToGridPos(pipePos);
			flashyHex.SetActive(true);
		}
		else if (eventName == "thirdturnselect" && _currentPhase == TutorialPhase.ThirdTurn) {
			moveFlashyHex(olga);
			flashyHex.SetActive(true);
		}
	}

	protected void greyOutMenu(GameObject menu, GameObject ignoreObject) {
		// Grey out all the text and buttons in the menu except for a specific one we ignore
		foreach (tk2dSprite buttonSprite in menu.GetComponentsInChildren<tk2dSprite>(true)) {
			if (buttonSprite.gameObject == ignoreObject) {
				continue;
			}
			buttonSprite.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
			foreach (tk2dTextMesh buttonText in buttonSprite.GetComponentsInChildren<tk2dTextMesh>(true)) {
				buttonText.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
				buttonText.Commit();
			}
		}
	}

	protected void ungreyMenu(GameObject menu) {
		foreach (tk2dSprite buttonSprite in menu.GetComponentsInChildren<tk2dSprite>(true)) {
			buttonSprite.color = Color.white;
			foreach (tk2dTextMesh buttonText in buttonSprite.GetComponentsInChildren<tk2dTextMesh>(true)) {
				if (buttonText.name == "shadow")
					buttonText.color = Color.black;
				else
					buttonText.color = Color.white;
				buttonText.Commit();
			}
		}
	}

	protected void moveFlashyHex(GameObject target) {
		flashyHex.transform.localPosition = new Vector3(target.transform.localPosition.x, target.transform.localPosition.y, flashyHex.transform.localPosition.z);                                               
	}

	protected void moveFlashyHexToGridPos(Vector2 gridPos) {
		Vector2 actualPos = HexGrid.instance.toActualCoord(gridPos);
		flashyHex.transform.localPosition = new Vector3(actualPos.x, actualPos.y, flashyHex.transform.localPosition.z);
	}

	public override void playerStartTurn (SimplePlayer player)
	{
		Debug.Log (_currentPhase);
		if (_currentPhase == TutorialPhase.SecondTurn) {
			HexGrid.instance.dialogueWindow.ActivateDialogue("dialogue/tutorial1secondturn");
		}
		else if (_currentPhase == TutorialPhase.ThirdTurn) {
			HexGrid.instance.dialogueWindow.ActivateDialogue("dialogue/tutorial1thirdturn");
		}
		else if (_currentPhase == TutorialPhase.FourthTurn) {
			HexGrid.instance.dialogueWindow.ActivateDialogue("dialogue/tutorial1fourthturn");
		}
	}

	public override bool playerSelected (SimplePlayer player)
	{
		if (_currentPhase == TutorialPhase.FirstTurn) {
			flashyHex.SetActive(false);
			HexGrid.instance.dialogueWindow.ActivateDialogue("dialogue/tutorial1firstmove");
			return true;
		}
		else if (_currentPhase == TutorialPhase.SecondTurn) {
			moveFlashyHexToGridPos(secondPorridgePos);
			flashyHex.SetActive(true);
			return true;
		}
		else if (_currentPhase == TutorialPhase.ThirdTurn) {
			moveFlashyHexToGridPos(pipePos);
			flashyHex.SetActive(true);
			return true;
		}
		else if (_currentPhase == TutorialPhase.FourthTurn) {
			moveFlashyHex(raft);
			_targetHex = HexGrid.instance.toGridCoord(raft.transform.localPosition.x, raft.transform.localPosition.y);

			flashyHex.SetActive(true);
			return true;
		}
		else {
			return true;
		}
	}

	public override bool buttonPressed (string buttonName)
	{
		if (_currentPhase == TutorialPhase.FirstTurn) {
			if (buttonName != "wait") {
				return false;
			}
			else {
				_currentPhase = TutorialPhase.SecondTurn;
			}
		}
		else if (_currentPhase == TutorialPhase.SecondTurn) {
			if (buttonName != "barricade") {
				return false;
			}
			else {
				_currentPhase = TutorialPhase.ThirdTurn;
			}
		}
		else if (_currentPhase == TutorialPhase.EndingThirdTurn) {
			if (buttonName != "wait") {
				return false;
			}
			else {
				_currentPhase = TutorialPhase.FourthTurn;
			}
		}
		else if (_currentPhase == TutorialPhase.FourthTurn) {
			if (buttonName != "wait")
				return false;
			else {
				ungreyMenu(olgaMenu);
			}
		}
		return true;
	}


	public override void menuOpened ()
	{
		if (_currentPhase == TutorialPhase.FirstTurn) {
			// Open the dialogue so that only the end turn button is not greyed out.
			HexGrid.instance.dialogueWindow.ActivateDialogue("dialogue/tutorial1menuopen1", -4);
		}
		else if (_currentPhase == TutorialPhase.SecondTurn) {
			HexGrid.instance.dialogueWindow.ActivateDialogue("dialogue/tutorial1build", -4);
		}
	}

	public override bool hexClicked (Vector2 gridPos)
	{
		if (_currentPhase == TutorialPhase.FirstTurn) {
			// Check to see if we're clicking on the correct hex
			if (gridPos != _targetHex) {
				HexGrid.instance.dialogueWindow.ActivateDialogue("dialogue/tutorial1wronghex1");
				return false;
			}
			else {
				flashyHex.SetActive(false);
				greyOutMenu(olgaMenu, olgaWaitButton);
				return true;
			}
		}
		else if (_currentPhase == TutorialPhase.SecondTurn) {
			if (gridPos != secondPorridgePos) {
				HexGrid.instance.dialogueWindow.ActivateDialogue("dialogue/tutorial1wronghex2");
				return false;
			}
			else {
				flashyHex.SetActive(false);
				ungreyMenu(olgaMenu);
				greyOutMenu(olgaMenu, olgaBuildButton);
				return true;
			}
		}
		else if (_currentPhase == TutorialPhase.ThirdTurn) {
			if (gridPos != pipePos) {
				HexGrid.instance.dialogueWindow.ActivateDialogue("dialogue/tutorial1wronghex2");
				return false;
			}
			else {
				flashyHex.SetActive(false);
				ungreyMenu(olgaMenu);
				greyOutMenu(olgaMenu, olgaWaitButton);
				_currentPhase = TutorialPhase.EndingThirdTurn;
				return true;
			}
		}
		else if (_currentPhase == TutorialPhase.FourthTurn) {
			if (gridPos != _targetHex) {
				HexGrid.instance.dialogueWindow.ActivateDialogue("dialogue/tutorial1wronghex2");
				return false;
			}
			else {
				flashyHex.SetActive(false);
				ungreyMenu(olgaMenu);
				greyOutMenu(olgaMenu, olgaWaitButton);
				return true;
			}
		}

		return base.hexClicked (gridPos);
	}

	
	// Update is called once per frame
	public override void Update () {
		
	}
}
