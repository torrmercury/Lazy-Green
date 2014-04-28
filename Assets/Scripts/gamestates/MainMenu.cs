using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

	public GameObject fadeObject;
	public string sceneName;

	protected bool _fading = false;

	// Use this for initialization
	void Start () {
		Application.targetFrameRate = 60;

		fadeObject.renderer.material.color = Color.black;
		iTween.ColorTo(fadeObject, iTween.Hash("a", 0, "time", 1));
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void finishStartPressed() {
		Application.LoadLevel(sceneName);
	}

	public void startPressed() {
		if (!_fading) {
			_fading = true;
			iTween.ColorTo(fadeObject, iTween.Hash("a", 1, "time", 1, "oncompletetarget", gameObject, "oncomplete", "finishStartPressed"));
			iTween.AudioTo(gameObject, iTween.Hash("volume", 0, "time", 1));
		}
	}
}
