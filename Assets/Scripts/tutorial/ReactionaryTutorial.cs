using UnityEngine;
using System.Collections;

public class ReactionaryTutorial : Tutorial {

	public static bool gameStarted = false;
	public static bool vomitStarted = false;

	public static bool playerSelectedStarted = false;
	public static bool johannSelectedStarted = false;
	public static bool olgaSelectedStarted = false;
	public static bool wernerSelectedStarted = false;

	public static bool porridgeEaten = false;
	public static bool johannPorridgeEaten = false;
	public static bool olgaPorridgeEaten = false;
	public static bool wernerPorridgeEaten = false;

	public static bool aboutToVomitStarted = false;

	public static bool startVomitStarted = false;
	public static bool startJustPushed = false;
	public static bool startOlgaPushed = false;


	protected ExitHex[] exitTiles;

	public override void Start ()
	{
		base.Start ();
		exitTiles = GameObject.FindObjectsOfType<ExitHex>();
		if (!gameStarted) {
			HexGrid.instance.dialogueWindow.ActivateDialogue("dialogue/reactiontutstart");
			gameStarted = true;
		}
	}

	public override bool playerSelected (SimplePlayer player)
	{
		if (!johannSelectedStarted && player == Johann.theOneTrueJohann) {
			johannSelectedStarted = true;
			HexGrid.instance.dialogueWindow.ActivateDialogue("dialogue/reactiontutselectedj", -4);
		}
		if (!olgaSelectedStarted && player == Olga.theOneTrueOlga) {
			olgaSelectedStarted = true;
			HexGrid.instance.dialogueWindow.ActivateDialogue("dialogue/reactiontutselectedo", -4);
		}
		if (!wernerSelectedStarted && player == Werner.theOneTrueWerner) {
			wernerSelectedStarted = true;
			HexGrid.instance.dialogueWindow.ActivateDialogue("dialogue/reactiontutselectedw", -4);
		}
		return true;
	}

	public override void justEaten (SimplePlayer player)
	{
		if (!johannPorridgeEaten && player == Johann.theOneTrueJohann) {
			johannPorridgeEaten = true;
			HexGrid.instance.dialogueWindow.ActivateDialogue("dialogue/reactiontutjusteatenj");
		}
		if (!olgaPorridgeEaten && player == Olga.theOneTrueOlga) {
			olgaPorridgeEaten = true;
			HexGrid.instance.dialogueWindow.ActivateDialogue("dialogue/reactiontutjusteateno");
		}
		if (!wernerPorridgeEaten && player == Werner.theOneTrueWerner) {
			wernerPorridgeEaten = true;
			HexGrid.instance.dialogueWindow.ActivateDialogue("dialogue/reactiontutjusteatenw");
		}
	}

	public override void justPushed (SimplePlayer player)
	{

		if (!startOlgaPushed && player == Olga.theOneTrueOlga) {
			startOlgaPushed = true;
			startJustPushed = true;
			HexGrid.instance.dialogueWindow.ActivateDialogue("dialogue/reactiontutjustpushedo");
		}
		if (!startJustPushed) {
			startJustPushed = true;
			if (player == Johann.theOneTrueJohann) {
				HexGrid.instance.dialogueWindow.ActivateDialogue("dialogue/reactiontutjustpushedj");
			}
			else if (player == Werner.theOneTrueWerner) {
				HexGrid.instance.dialogueWindow.ActivateDialogue("dialogue/reactiontutjustpushedw");
			}
		}
	}


	public override void aboutToVomit (SimplePlayer player)
	{
		if (!aboutToVomitStarted) {
			aboutToVomitStarted = true;
			if (player == Johann.theOneTrueJohann) {
				HexGrid.instance.dialogueWindow.ActivateDialogue("dialogue/reactiontutabouttovomitj");
			}
			else if (player == Olga.theOneTrueOlga) {
				HexGrid.instance.dialogueWindow.ActivateDialogue("dialogue/reactiontutabouttovomito");
			}
			else if (player == Werner.theOneTrueWerner) {
				HexGrid.instance.dialogueWindow.ActivateDialogue("dialogue/reactiontutabouttovomitw");
			}
		}
	}

	public override void playerStartTurn (SimplePlayer player)
	{
		if (!startVomitStarted) {
			if (GameObject.FindObjectsOfType<Vomit>().Length > 0) {
				startVomitStarted = true;
				HexGrid.instance.dialogueWindow.ActivateDialogue("dialogue/reactiontutvomitonground");
			}
		}
	}



	public override void processDialogueEvent (string eventName)
	{
		if (eventName == "rafts") {
			foreach (ExitHex exit in exitTiles) {
				HexGrid.instance.highlightDeepGridPos(exit.gridPos, HexGrid.instance.normalHighlight);
				HexGrid.instance.flashDeepHighlight(exit.gridPos);
			}
		}
		else if (eventName == "endrafts") {
			foreach (ExitHex exit in exitTiles) {
				HexGrid.instance.disableDeepHighlight(exit.gridPos);
			}
		}
	}




	
}
