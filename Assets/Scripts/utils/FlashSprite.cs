using UnityEngine;
using System.Collections;

// Utility script for flashing a sprite while the Time.timeScale == 0
public class FlashSprite : MonoBehaviour {

	// Our frequency of flashing in hertz
	public float frequency = 1f;

	protected bool _flashing = false;
	public bool flashing {
		get { return _flashing; }
	}

	protected float _startTime;
	protected float _maxAlpha;
	// Frequency normalized for 2pi or whatever
	protected float _omega;

	public bool flashDuringPause = true;
	public bool startFlashing = true;

	public float fadeSpeed = 1f;

	// Use this for initialization
	void Start () {
		_maxAlpha = (renderer as SpriteRenderer).color.a;
		_omega = Mathf.PI*2 / frequency;
		if (startFlashing)
			startFlash();
	}

	public void startFlash() {
		_startTime = flashDuringPause ? Time.realtimeSinceStartup : Time.time;
		_flashing = true;
		foreach (FlashSprite childSprite in GetComponentsInChildren<FlashSprite>()) {
			if (childSprite != this)
				childSprite.startFlash();
		}
	}

	public void stopFlash() {
		Color curColor = (renderer as SpriteRenderer).color;
		(renderer as SpriteRenderer).color = new Color(curColor.r, curColor.g, curColor.b, _maxAlpha);
		_flashing = false;
		foreach (FlashSprite childSprite in GetComponentsInChildren<FlashSprite>()) {
			if (childSprite != this)
				childSprite.stopFlash();
		}
	}

	public void startFadeOut() {
		_flashing = false;
		StartCoroutine(fadeOut());
		foreach (FlashSprite childSprite in GetComponentsInChildren<FlashSprite>()) {
			if (childSprite != this)
				childSprite.startFadeOut();
		}
	}

	public void disappear() {
		_flashing = false;
		Color curColor = (renderer as SpriteRenderer).color;
		(renderer as SpriteRenderer).color = new Color(curColor.r, curColor.g, curColor.b, 0);
		foreach (FlashSprite childSprite in GetComponentsInChildren<FlashSprite>()) {
			if (childSprite != this)
				childSprite.disappear();
		}
	}

	protected IEnumerator fadeOut() {
		Color curColor = (renderer as SpriteRenderer).color;
		float alphaScale = curColor.a / _maxAlpha;
		float lastTime = Time.realtimeSinceStartup;
		while (alphaScale > 0) {
			yield return 0;
			float deltaTime = flashDuringPause ? Time.realtimeSinceStartup-lastTime : Time.deltaTime;
			lastTime = Time.realtimeSinceStartup;
			alphaScale -= deltaTime*fadeSpeed;
			if (alphaScale < 0)
				alphaScale = 0;
			(renderer as SpriteRenderer).color = new Color(curColor.r, curColor.g, curColor.b, alphaScale*_maxAlpha);
		}
	}

	
	// Update is called once per frame
	void Update () {
		if (_flashing) {
			float t = flashDuringPause ? Time.realtimeSinceStartup-_startTime : Time.time-_startTime;
			float alphaScale = (Mathf.Cos(t*_omega)+1)/2f;
			Color currentColor = (renderer as SpriteRenderer).color;
			(renderer as SpriteRenderer).color = new Color(currentColor.r, currentColor.g, currentColor.b, alphaScale*_maxAlpha);
		}
	}
}
