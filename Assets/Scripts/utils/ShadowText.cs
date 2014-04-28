using UnityEngine;
using System.Collections;

public class ShadowText : MonoBehaviour {

	protected tk2dTextMesh _frontText, _backText;

	// Use this for initialization
	void Start () {
		_frontText = GetComponent<tk2dTextMesh>();
		_backText = transform.FindChild("shadow").GetComponent<tk2dTextMesh>();
	}

	public void setColor(Color color) {
		if (_frontText == null) {
			_frontText = GetComponent<tk2dTextMesh>();
		}
		if (_frontText != null) {
			_frontText.color = color;
			_frontText.Commit();
		}
	}

	public void setText(string text) {
		if (_frontText != null && _backText != null) {
			_frontText.text = text;
			_backText.text = text;
			_frontText.Commit();
			_backText.Commit();
		}
	}
}
