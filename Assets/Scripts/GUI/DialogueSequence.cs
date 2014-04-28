using UnityEngine;
using System.Collections;

public class DialogueSequence {
#if UNITY_STANDALONE_WIN
    public const string LINE_BREAK = "\r\n**\r\n";
#else
    public const string LINE_BREAK = "\n**\n";
#endif
	protected string[] _dialogueLines;
	protected int _dialogueIndex;

	public DialogueSequence(string dialogueFilename) {
		// Parse the text
		TextAsset textFile = Resources.Load (dialogueFilename, typeof(TextAsset)) as TextAsset;
		string fullDialogue = textFile.text;
		_dialogueLines = fullDialogue.Split(new string[] { LINE_BREAK }, System.StringSplitOptions.None);
		for (int i = 0; i < _dialogueLines.Length; i++) {
			_dialogueLines[i] = _dialogueLines[i].Trim();
		}
		_dialogueIndex = 0;
	}

	public void reset() {
		_dialogueIndex = 0;
	}

	public bool hasNextLine() {
		return _dialogueIndex < _dialogueLines.Length;
	}

	public string nextLine() {
		return _dialogueLines[_dialogueIndex++];
	}


}
