using UnityEngine;
using System.Collections;

public class PlayerGUI : MonoBehaviour {

	protected SimplePlayer _player;
	protected ShadowText _stomachText, _hpText;

	private FlashText flashTextScript;

	private bool firstRunUp = true;
	
	void Awake () {
		//_hpText = transform.FindChild("hptext").GetComponent<ShadowText>();
		//_stomachText.setText("");
		_player = transform.parent.GetComponent<SimplePlayer>();
		_stomachText = transform.FindChild("stomachtext").GetComponent<ShadowText>();
		flashTextScript = _stomachText.GetComponent<FlashText>();
	}

	void Update () {
		if(firstRunUp)
			flashTextScript.disappear();
		//_hpText.setText(string.Format("{0}/{1}", _player.currentHealth, _player.maxHealth));
		if (!_player.alive) {
			gameObject.SetActive(false);
			return;
		}
		_stomachText.setText(string.Format("{0}/{1}", _player.currentStomach, _player.maxStomach));
	}

	public void startFadingText(){
		flashTextScript.startFadeOut();
	}

	public void appearTextAgain(){
		flashTextScript.appear();
	}

	public void disappearMe(){
		flashTextScript.disappear();
	}

	public void stopFadeCorountine(){
		flashTextScript.stopMyCoroutines();
	}
}
