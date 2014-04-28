using UnityEngine;
using System.Collections;

public class FlashText : MonoBehaviour {

	// Our frequency of flashing in hertz
	public float frequency = 1f;

	protected tk2dTextMesh _text;

	protected bool _flashing = false;
	public bool flashing {
		get { return _flashing; }
	}

	public float currentAlphaScale;
	protected float _startTime;
	protected float _maxAlpha;
	// Frequency normalized for 2pi or whatever
	protected float _omega;

	public bool flashDuringPause = false;

	public bool startFlashing = false;

	public float fadeSpeed = 1f;

	void Start() {
		_text = GetComponent<tk2dTextMesh>();
		_maxAlpha = _text.color.a;
		_omega = Mathf.PI*2 / frequency;
		if (startFlashing)
			startFlash();
	}

	// Use this for initialization
	public void startFlash () {
		_startTime = flashDuringPause ? Time.realtimeSinceStartup : Time.time;
		_flashing = true;
		foreach (FlashText childText in GetComponentsInChildren<FlashText>()) {
			if (childText != this)
				childText.startFlash();
		}
	}

	public void stopFlash() {
		_text.color = new Color(_text.color.r, _text.color.g, _text.color.b, _maxAlpha);
		_flashing = false;
		foreach (FlashText childText in GetComponentsInChildren<FlashText>()) {
			if (childText != this)
				childText.stopFlash();
		}
	}

	public void startFadeOut() {
		_flashing = false;
		StartCoroutine(fadeOut());
		foreach (FlashText childText in GetComponentsInChildren<FlashText>()) {
			if (childText != this)
				childText.startFadeOut();
		}
	}

	protected IEnumerator fadeOut() {
		Color currentColor = _text.color;
		Color currentColor2 = _text.color2;
		float alphaScale = currentColor.a / _maxAlpha;
		float lastTime = Time.realtimeSinceStartup;
		while (alphaScale > 0) {
			yield return 0;
			float deltaTime = flashDuringPause ? Time.realtimeSinceStartup-lastTime : Time.deltaTime;
			lastTime = Time.realtimeSinceStartup;
			alphaScale -= deltaTime*fadeSpeed;
			if (alphaScale < 0)
				alphaScale = 0;
			_text.color = new Color(_text.color.r, _text.color.g, _text.color.b, alphaScale*_maxAlpha);
			_text.color2 = new Color(_text.color.r, _text.color.g, _text.color.b, alphaScale*_maxAlpha);
		}
	}

	void OnDestroy() {
		StopAllCoroutines();
	}

	public void disappear(){
		_flashing = false;
		Color curColor = _text.color;
		_text.color = new Color(curColor.r, curColor.g, curColor.b, 0);
		_text.color2 = new Color(curColor.r, curColor.g, curColor.b, 0);
		foreach (FlashText childText in GetComponentsInChildren<FlashText>()) {
			if (childText != this)
				childText.disappear();
		}
	}

	public void stopMyCoroutines(){
		StopAllCoroutines();
		foreach (FlashText childText in GetComponentsInChildren<FlashText>()) {
			if (childText != this)
				childText.StopAllCoroutines();
		}
	}

	public void appear(){
		_flashing = false;
		Color curColor = _text.color;
		_text.color = new Color(curColor.r, curColor.g, curColor.b, 1);
		_text.color2 = new Color(curColor.r, curColor.g, curColor.b, 1);
		foreach (FlashText childText in GetComponentsInChildren<FlashText>()) {
			if (childText != this)
				childText.appear();
		}
	}

	// Update is called once per frame
	void Update(){
		Color myCurrentColor = _text.color;
		currentAlphaScale = myCurrentColor.a / _maxAlpha;
		if (_flashing) {
			float t = flashDuringPause ? Time.realtimeSinceStartup-_startTime : Time.time-_startTime;
			float alphaScale = (Mathf.Cos(t*_omega)+1)/2f;
			Color currentColor = _text.color;
			Color currentColor2 = _text.color2;
			_text.color = new Color(currentColor.r, currentColor.g, currentColor.b, alphaScale*_maxAlpha);
			_text.color2 = new Color(currentColor2.r, currentColor2.g, currentColor2.b, alphaScale*_maxAlpha);
			_text.Commit();
		}
	}
}
