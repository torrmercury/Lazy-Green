using UnityEngine;
using System.Collections;

public class SideGUI : MonoBehaviour {

	private int OlgaCurrentStomach;
	private int OlgaMaxStomach;
	private int JohannCurrentStomach;
	private int JohannMaxStomach;
	private int WernerCurrentStomach;
	private int WernerMaxStomach;

	public Sprite sadFace;
	public Sprite happyFace;
	public Sprite heartFace;

	protected bool _shakeOlga = false;
	protected bool _shakeJohann = false;
	protected bool _shakeWerner = false;
	protected bool _blockShakeTest = false;
	protected bool _pulseOlga = false;
	protected bool _pulseJohann = false;
	protected bool _pulseWerner = false;

	protected float _startAngle;
	public float shakeTime = 1.5f;
	public float shakeInterval = 0.1f;
	public float shakeAngle = 15f;

	protected ShadowText _johannStomachText;
	protected ShadowText _olgaStomachText;
	protected ShadowText _wernerStomachText;

	protected SpriteRenderer _johannBalloon;
	protected SpriteRenderer _olgaBalloon;
	protected SpriteRenderer _wernerBalloon;

	public GameObject olgaSelectedHex;
	public GameObject johannSelectedHex;
	public GameObject wernerSelectedHex;
	public FlashSprite olgaFlashHexScript;
	public FlashSprite johannFlashHexScript;
	public FlashSprite wernerFlashHexScript;

	private bool OlgaHexFlashingYet = false;
	private bool JohannHexFlashingYet = false;
	private bool WernerHexFlashingYet = false;

	public GameObject OlgaStomach;
	public GameObject JohannStomach;
	public GameObject WernerStomach;

	public Sprite Empty;
	public Sprite oneOf15;
	public Sprite twoOf15;
	public Sprite threeOf15;
	public Sprite fourOf15;
	public Sprite fiveOf15;
	public Sprite sixOf15;
	public Sprite sevenOf15;
	public Sprite eightOf15;
	public Sprite nineOf15;
	public Sprite tenOf15;
	public Sprite elevenOf15;
	public Sprite twelveOf15;
	public Sprite thirteenOf15;
	public Sprite fourteenOf15;
	public Sprite Full;
	
	protected SimplePlayer _OlgaPlayer = null;
	protected SimplePlayer _JohannPlayer = null;
	protected SimplePlayer _WernerPlayer = null;

	private bool johannSelected;
	private bool olgaSelected;
	private bool wernerSelected;

	float OlgaStomachPercent = 0;
	float JohannStomachPercent = 0;
	float WernerStomachPercent = 0;

	private Transform johannTab;
	private Transform olgaTab;
	private Transform wernerTab;

	public float amplitudeX;
	public float omegaX;
	private float index;
	
	private Vector3 olgaTabOrigin;
	private Vector3 johannTabOrigin;
	private Vector3 wernerTabOrigin;

	void Start(){
		_johannBalloon = GameObject.Find("JohannBalloon").GetComponent<SpriteRenderer>();
		_olgaBalloon = GameObject.Find("OlgaBalloon").GetComponent<SpriteRenderer>();
		_wernerBalloon = GameObject.Find("WernerBalloon").GetComponent<SpriteRenderer>();

		_johannStomachText = transform.FindChild("JohannStomachText").GetComponent<ShadowText>();
		_olgaStomachText = transform.FindChild("OlgaStomachText").GetComponent<ShadowText>();
		_wernerStomachText = transform.FindChild("WernerStomachText").GetComponent<ShadowText>();

		johannTab = GameObject.Find("JohannMenuTab").GetComponent<Transform>();
		olgaTab = GameObject.Find("OlgaMenuTab").GetComponent<Transform>();
		wernerTab = GameObject.Find("WernerMenuTab").GetComponent<Transform>();

		olgaTabOrigin = olgaTab.transform.localPosition;
		johannTabOrigin = johannTab.transform.localPosition;
		wernerTabOrigin = wernerTab.transform.localPosition;

		_startAngle = OlgaStomach.transform.localRotation.eulerAngles.z;

		johannFlashHexScript.disappear();
		olgaFlashHexScript.disappear();
		wernerFlashHexScript.disappear();
	}

	void Update(){
		//Saves the info of each character
		if(_JohannPlayer == null)
			_JohannPlayer = Johann.theOneTrueJohann;
		if(_OlgaPlayer == null)
			_OlgaPlayer = Olga.theOneTrueOlga;
		if(_WernerPlayer == null)
			_WernerPlayer = Werner.theOneTrueWerner;

		//Checks who is selected right now and does spiffy effects on the tab for the current selected character
		if(_JohannPlayer.selectedPlayer == _JohannPlayer){
			if(!JohannHexFlashingYet){
				JohannHexFlashingYet = true;
				johannFlashHexScript.startFlash();
			}
			if(!_JohannPlayer.amMoving && _JohannPlayer != null)
				johannSelectedHex.transform.position = _JohannPlayer.transform.position;
			index += Time.deltaTime;
			float x = Mathf.Abs (amplitudeX * Mathf.Sin(omegaX * index));
			johannTab.transform.localPosition = new Vector3(x + 2, johannTab.transform.localPosition.y, 0);
		} else {
			johannTab.transform.localPosition = johannTabOrigin;
			JohannHexFlashingYet = false;
			johannFlashHexScript.stopFlash();
			johannFlashHexScript.disappear();
		}
		if(_OlgaPlayer.selectedPlayer == _OlgaPlayer){
			if(!OlgaHexFlashingYet){
				OlgaHexFlashingYet = true;
				olgaFlashHexScript.startFlash();
			}
			if(!_OlgaPlayer.amMoving && _OlgaPlayer != null)
				olgaSelectedHex.transform.position = _OlgaPlayer.transform.position;
			index += Time.deltaTime;
			float x = Mathf.Abs (amplitudeX * Mathf.Sin(omegaX * index));
			olgaTab.transform.localPosition = new Vector3(x + 2, olgaTab.transform.localPosition.y, 0);
		} else {
			olgaTab.transform.localPosition = olgaTabOrigin;
			OlgaHexFlashingYet = false;
			olgaFlashHexScript.stopFlash();
			olgaFlashHexScript.disappear();
		}
		if(_WernerPlayer.selectedPlayer == _WernerPlayer){
			if(!WernerHexFlashingYet){
				WernerHexFlashingYet = true;
				wernerFlashHexScript.startFlash();
			}
			if(!_WernerPlayer.amMoving && _WernerPlayer != null)
				wernerSelectedHex.transform.position = _WernerPlayer.transform.position;
			index += Time.deltaTime;
			float x = Mathf.Abs (amplitudeX * Mathf.Sin(omegaX * index));
			wernerTab.transform.localPosition = new Vector3(x + 2, wernerTab.transform.localPosition.y, 0);
		} else {
			wernerTab.transform.localPosition = wernerTabOrigin;
			WernerHexFlashingYet = false;
			wernerFlashHexScript.stopFlash();
			wernerFlashHexScript.disappear();
		}


		//Updates the stomach numbers UI
		_johannStomachText.setText(string.Format("{0}/{1}", _JohannPlayer.currentStomach, _JohannPlayer.maxStomach));
		_olgaStomachText.setText(string.Format("{0}/{1}", _OlgaPlayer.currentStomach, _OlgaPlayer.maxStomach));
		_wernerStomachText.setText(string.Format("{0}/{1}", _WernerPlayer.currentStomach, _WernerPlayer.maxStomach));

		//Decide stomach sprite for OLGA
		OlgaCurrentStomach = _OlgaPlayer.currentStomach;
		OlgaMaxStomach = _OlgaPlayer.maxStomach;
		decideSprite(OlgaCurrentStomach, OlgaMaxStomach, OlgaStomach);

		//Decide stomach sprite for JOHANN
		JohannCurrentStomach = _JohannPlayer.currentStomach;
		JohannMaxStomach = _JohannPlayer.maxStomach;
		decideSprite(JohannCurrentStomach, JohannMaxStomach, JohannStomach);

		//Decide stomach sprite for WERNER
		WernerCurrentStomach = _WernerPlayer.currentStomach;
		WernerMaxStomach = _WernerPlayer.maxStomach;
		decideSprite(WernerCurrentStomach, WernerMaxStomach, WernerStomach);

		//Visual cues!!
		OlgaStomachPercent = (OlgaCurrentStomach * 100) / OlgaMaxStomach;
		JohannStomachPercent = (JohannCurrentStomach * 100) / JohannMaxStomach;
		WernerStomachPercent = (WernerCurrentStomach * 100) / WernerMaxStomach;

			//Shake me!
		if(!_blockShakeTest){
			if(OlgaStomachPercent > 100)
				_shakeOlga = true;
			else if(OlgaStomachPercent < 100)
				_shakeOlga = false;
			if(JohannStomachPercent > 100)
				_shakeJohann = true;
			else if(JohannStomachPercent < 100)
				_shakeJohann = false;
			if(WernerStomachPercent > 100)
				_shakeWerner = true;
			else if(WernerStomachPercent < 100)
				_shakeWerner = false;
			testShake();
		}

			//Pulse me red!
		if(!_pulseJohann && JohannStomachPercent > 100){
			pulseJohannRed();
			_pulseJohann = true;
		}
		if(!_pulseOlga && OlgaStomachPercent > 100){
			pulseOlgaRed();
			_pulseOlga = true;
		}
		if(!_pulseWerner && WernerStomachPercent > 100){
			pulseWernerRed();
			_pulseWerner = true;
		}

			//Change my balloon!
		if(OlgaStomachPercent > 100)
			_olgaBalloon.sprite = sadFace;
		else if(OlgaStomachPercent <= 100 && !_OlgaPlayer.standingOnExit())
			_olgaBalloon.sprite = null;
		else if(OlgaStomachPercent <= 100 && _OlgaPlayer.standingOnExit())
			_olgaBalloon.sprite = happyFace;

		if(JohannStomachPercent > 100)
			_johannBalloon.sprite = sadFace;
		else if(JohannStomachPercent <= 100 && !_JohannPlayer.standingOnExit())
			_johannBalloon.sprite = null;
		else if(JohannStomachPercent <= 100 && _JohannPlayer.standingOnExit())
			_johannBalloon.sprite = happyFace;

		if(WernerStomachPercent > 100)
			_wernerBalloon.sprite = sadFace;
		else if(WernerStomachPercent <= 100 && !_WernerPlayer.standingOnExit())
			_wernerBalloon.sprite = null;
		else if(WernerStomachPercent <= 100 && _WernerPlayer.standingOnExit())
			_wernerBalloon.sprite = happyFace;

	}
	
	void decideSprite(int currentStomach, int maxStomach, GameObject whoToChange){
		float StomachPercent = (currentStomach * 100) / maxStomach;
		if(StomachPercent <= 0)
			whoToChange.GetComponent<SpriteRenderer>().sprite = Empty;
		else if(StomachPercent > 0 && StomachPercent <= 7)
			whoToChange.GetComponent<SpriteRenderer>().sprite = oneOf15;
		else if(StomachPercent > 7 && StomachPercent <= 13)
			whoToChange.GetComponent<SpriteRenderer>().sprite = twoOf15;
		else if(StomachPercent > 13 && StomachPercent <= 20)
			whoToChange.GetComponent<SpriteRenderer>().sprite = threeOf15;
		else if(StomachPercent > 20 && StomachPercent <= 27)
			whoToChange.GetComponent<SpriteRenderer>().sprite = fourOf15;
		else if(StomachPercent > 27 && StomachPercent <= 33)
			whoToChange.GetComponent<SpriteRenderer>().sprite = fiveOf15;
		else if(StomachPercent > 33 && StomachPercent <= 40)
			whoToChange.GetComponent<SpriteRenderer>().sprite = sixOf15;
		else if(StomachPercent > 40 && StomachPercent <= 47)
			whoToChange.GetComponent<SpriteRenderer>().sprite = sevenOf15;
		else if(StomachPercent > 47 && StomachPercent <= 53)
			whoToChange.GetComponent<SpriteRenderer>().sprite = eightOf15;
		else if(StomachPercent > 53 && StomachPercent <= 60)
			whoToChange.GetComponent<SpriteRenderer>().sprite = nineOf15;
		else if(StomachPercent > 60 && StomachPercent <= 67)
			whoToChange.GetComponent<SpriteRenderer>().sprite = tenOf15;
		else if(StomachPercent > 67 && StomachPercent <= 73)
			whoToChange.GetComponent<SpriteRenderer>().sprite = elevenOf15;
		else if(StomachPercent > 73 && StomachPercent <= 80)
			whoToChange.GetComponent<SpriteRenderer>().sprite = twelveOf15;
		else if(StomachPercent > 80 && StomachPercent <= 87)
			whoToChange.GetComponent<SpriteRenderer>().sprite = thirteenOf15;
		else if(StomachPercent > 87 && StomachPercent <= 93)
			whoToChange.GetComponent<SpriteRenderer>().sprite = fourteenOf15;
		else if(StomachPercent > 93)
			whoToChange.GetComponent<SpriteRenderer>().sprite = Full;
	}

	void testShake(){
		if(_shakeOlga || _shakeJohann || _shakeWerner)
			StartCoroutine(doShake());
	}
	
	IEnumerator doShake(){
		_blockShakeTest = true;
		float t = 0;
		float shakeCounter = 0;
		float shakeMultiplier = 1;
		while (t < shakeTime) {
			yield return 0;
			t += Time.deltaTime;
			shakeCounter += Time.deltaTime;
			if (shakeCounter >= shakeInterval) {
				shakeMultiplier = -shakeMultiplier;
				if(_shakeOlga)
					OlgaStomach.transform.localRotation = Quaternion.Euler(0, 0, _startAngle+shakeAngle*shakeMultiplier);
				if(_shakeJohann)
					JohannStomach.transform.localRotation = Quaternion.Euler(0, 0, _startAngle+shakeAngle*shakeMultiplier);
				if(_shakeWerner)
					WernerStomach.transform.localRotation = Quaternion.Euler(0, 0, _startAngle+shakeAngle*shakeMultiplier);
				shakeCounter = 0;
			}
		}
		OlgaStomach.transform.localRotation = Quaternion.Euler(0, 0, _startAngle);
		JohannStomach.transform.localRotation = Quaternion.Euler(0, 0, _startAngle);
		WernerStomach.transform.localRotation = Quaternion.Euler(0, 0, _startAngle);
		_blockShakeTest = false;
		_shakeOlga = false;
		_shakeJohann = false;
		_shakeWerner = false;
	}

	void pulseJohannRed(){
		iTween.ValueTo(JohannStomach, iTween.Hash("from", Color.red, "to", Color.white, "onupdatetarget", gameObject, "oncompletetarget", gameObject, "onupdate", "johannColorUpdate", "oncomplete", "repulseJohann", "time", 1.0f));
	}
	
	void johannColorUpdate(Color newColor){
		JohannStomach.GetComponent<SpriteRenderer>().color = newColor;
	}
	
	public void repulseJohann(){
		_pulseJohann = false;
	}
	
	void pulseOlgaRed(){
		iTween.ValueTo(OlgaStomach, iTween.Hash("from", Color.red, "to", Color.white, "onupdatetarget", gameObject, "oncompletetarget", gameObject, "onupdate", "olgaColorUpdate", "oncomplete", "repulseOlga", "time", 1.0f));
	}
	
	void olgaColorUpdate(Color newColor){
		OlgaStomach.GetComponent<SpriteRenderer>().color = newColor;
	}
	
	public void repulseOlga(){
		_pulseOlga = false;
	}
	
	void pulseWernerRed(){
		iTween.ValueTo(WernerStomach, iTween.Hash("from", Color.red, "to", Color.white, "onupdatetarget", gameObject, "oncompletetarget", gameObject, "onupdate", "wernerColorUpdate", "oncomplete", "repulseWerner", "time", 1.0f));
	}
	
	void wernerColorUpdate(Color newColor){
		WernerStomach.GetComponent<SpriteRenderer>().color = newColor;
	}
	
	public void repulseWerner(){
		_pulseWerner = false;
	}
}
