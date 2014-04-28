using UnityEngine;
using System.Collections;

public class ExitHexSideGUI : MonoBehaviour {

	protected ShadowText _countdownText;
	public Sprite littlePotSprite;
	public GameObject countdownText;

	public void Start (){
		_countdownText = GetComponentInChildren<ShadowText>();
	}
	
	public void Update (){
		_countdownText.setText(HexGrid.instance.turnsLeft.ToString());
	}	

	public void changeMeToLittlePot(){
		GetComponent<SpriteRenderer>().sprite = littlePotSprite;
		countdownText.transform.localPosition += new Vector3(0,-0.5f,0);

	}
}
