using UnityEngine;
using System.Collections;

public class DialogueWindow : MonoBehaviour {

	public ShadowText dialogueText;
	public GameObject leftPortrait, rightPortrait;
	
	public Sprite wernerSprite, johannSprite, olgaSprite;

	protected string _speakerName;

	protected DialogueSequence _currentSequence;

	public string olgaColor = "FF0000FF";
	public string wernerColor = "00FF00FF";
	public string johannColor = "0000FFFF";
	public string otherColor = "808080FF";

	protected string _speakerColor;

	protected bool _waitingForTween = false;

	public Vector2 leftUpPos = new Vector2(-14.5f, 11);
	public Vector2 leftDownPos = new Vector2(-14.5f, 0.25f);

	public Vector2 rightUpPos = new Vector2(14.5f, 11);
	public Vector2 rightDownPos = new Vector2(14.5f, 0.25f);

	// Use this for initialization
	void Start () {
		_speakerName = "Olga";
	}

	// Call this function to start up the dialogue effect. 
	public void ActivateDialogue(string dialogueFilename, float dialogueY) {

		_currentSequence = new DialogueSequence(dialogueFilename);
		advanceDialogue();
		_waitingForTween = true;
		rightPortrait.transform.localPosition = new Vector3(rightPortrait.transform.localPosition.x, rightDownPos.y, rightPortrait.transform.localPosition.z);
		leftPortrait.transform.localPosition = new Vector3(leftPortrait.transform.localPosition.x, leftUpPos.y, leftPortrait.transform.localPosition.z);
		
		iTween.MoveTo(gameObject, iTween.Hash("islocal", true, "y", dialogueY, "time", 1, "oncompletetarget", gameObject, "oncomplete", "enterFinished"));
		HexGrid.instance.currentState = HexGrid.GameState.Dialogue;

	}

	public void finishCloseDialogue() {
		HexGrid.instance.currentState = HexGrid.GameState.Run;
	}

	public void ActivateDialogue(string dialogueFilename) {
		ActivateDialogue(dialogueFilename, -16);

	}
	
	protected void advanceDialogue() {
		if (_currentSequence.hasNextLine()) {
			parseDialogueCommand(_currentSequence.nextLine());
		}
		else {
			// Deactivate the dialogue if we no longer have a next line.
			iTween.MoveTo(gameObject, iTween.Hash("islocal", true, "y", -64, "time", 0.5f, "oncompletetarget", gameObject, "oncomplete", "finishCloseDialogue"));
		}
	}

	public void enterFinished() {
		_waitingForTween = false;
	}

	public void tweenFinished() {
		_waitingForTween = false;
		advanceDialogue();
	}

	protected void parseDialogueCommand(string command) {
		if (command.StartsWith("<tutorialtrigger>") && HexGrid.instance.tutorial != null) {
			HexGrid.instance.tutorial.processDialogueEvent(command.Substring("<tutorialtrigger>".Length));
			advanceDialogue();
		}
		else if (command.StartsWith("<speaker>")) {
			_speakerName = command.Substring("<speaker>".Length);
			if (_speakerName == "Olga") {
				_speakerColor = olgaColor;
			}
			else if (_speakerName == "Werner") {
				_speakerColor = wernerColor;
			}
			else if (_speakerName == "Johann") {
				_speakerColor = johannColor;
			}
			else {
				_speakerColor = otherColor;
			}

			advanceDialogue();
		}
		else if (command.StartsWith("<leftup>")) {
			leftPortrait.transform.localPosition = new Vector3(leftPortrait.transform.localPosition.x, leftUpPos.y, leftPortrait.transform.localPosition.z);
			advanceDialogue();
		}
		else if (command.StartsWith("<moveleftup>")) {
			_waitingForTween = true;
			leftPortrait.transform.localPosition = new Vector3(leftPortrait.transform.localPosition.x, leftDownPos.y, leftPortrait.transform.localPosition.z);
			iTween.MoveTo(leftPortrait, iTween.Hash("islocal", true, "y", leftUpPos.y, "time", 0.2f, "oncompletetarget", gameObject, "oncomplete", "tweenFinished"));
		}
		else if (command.StartsWith("<leftdown>")) {
			leftPortrait.transform.localPosition = new Vector3(leftPortrait.transform.localPosition.x, leftDownPos.y, leftPortrait.transform.localPosition.z);
			advanceDialogue();
		}
		else if (command.StartsWith("<moveleftdown>")) {
			_waitingForTween = true;
			leftPortrait.transform.localPosition = new Vector3(leftPortrait.transform.localPosition.x, leftUpPos.y, leftPortrait.transform.localPosition.z);
			iTween.MoveTo(leftPortrait, iTween.Hash("islocal", true, "y", leftDownPos.y, "time", 0.2f, "oncompletetarget", gameObject, "oncomplete", "tweenFinished"));
		}
		else if (command.StartsWith("<rightup>")) {
			rightPortrait.transform.localPosition = new Vector3(rightPortrait.transform.localPosition.x, rightUpPos.y, rightPortrait.transform.localPosition.z);
			advanceDialogue();
		}
		else if (command.StartsWith("<moverightup>")) {
			_waitingForTween = true;
			rightPortrait.transform.localPosition = new Vector3(rightPortrait.transform.localPosition.x, rightDownPos.y, rightPortrait.transform.localPosition.z);
			iTween.MoveTo(rightPortrait, iTween.Hash("islocal", true, "y", rightUpPos.y, "time", 0.2f, "oncompletetarget", gameObject, "oncomplete", "tweenFinished"));
		}
		else if (command.StartsWith("<rightdown>")) {
			rightPortrait.transform.localPosition = new Vector3(rightPortrait.transform.localPosition.x, rightDownPos.y, rightPortrait.transform.localPosition.z);
			advanceDialogue();
		}
		else if (command.StartsWith("<moverightdown>")) {
			_waitingForTween = true;
			rightPortrait.transform.localPosition = new Vector3(rightPortrait.transform.localPosition.x, rightUpPos.y, rightPortrait.transform.localPosition.z);
			iTween.MoveTo(rightPortrait, iTween.Hash("islocal", true, "y", rightDownPos.y, "time", 0.2f, "oncompletetarget", gameObject, "oncomplete", "tweenFinished"));
		}
		else if (command.StartsWith("<leftportrait>")) {
			string portraitName = command.Substring("<leftportrait>".Length);
			if (portraitName == "Olga") {
				(leftPortrait.renderer as SpriteRenderer).sprite = olgaSprite;
			}
			else if (portraitName == "Werner") {
				(leftPortrait.renderer as SpriteRenderer).sprite = wernerSprite;
			}
			else if (portraitName == "Johann") {
				(leftPortrait.renderer as SpriteRenderer).sprite = johannSprite;
			}
			advanceDialogue();
		}
		else if (command.StartsWith("<rightportrait>")) {
			string portraitName = command.Substring("<rightportrait>".Length);
			if (portraitName == "Olga") {
				(rightPortrait.renderer as SpriteRenderer).sprite = olgaSprite;
			}
			else if (portraitName == "Werner") {
				(rightPortrait.renderer as SpriteRenderer).sprite = wernerSprite;
			}
			else if (portraitName == "Johann") {
				(rightPortrait.renderer as SpriteRenderer).sprite = johannSprite;
			}
			advanceDialogue();
		}
		else {
			// Standard dialogue.
			dialogueText.setText(string.Format("^C{0}{1}:^CFFFFFFFF\n{2}", _speakerColor, _speakerName, command));
		}
	}

	// Update is called once per frame
	void Update () {
		if (HexGrid.instance.currentState != HexGrid.GameState.Dialogue)
			return;
		// If the dialogue window is active, wait for a click to advance.
		if (HungryInput.mouseButtonClicked && !_waitingForTween) {
			audio.Play();
			advanceDialogue();

		}
	}
}
