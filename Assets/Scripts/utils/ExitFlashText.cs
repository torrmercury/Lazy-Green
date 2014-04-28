using UnityEngine;
using System.Collections;

public class ExitFlashText : MonoBehaviour {

	private tk2dTextMesh myColorRenderer;
	private bool alreadyScalling;
		
	void Start(){
		myColorRenderer = gameObject.GetComponent<tk2dTextMesh>();
		alreadyScalling = false;
	}

	void Update(){
		if(HexGrid.instance.allowFlash)
			startEffects();
	}

	void startEffects(){
		if(alreadyScalling == false){
			iTween.ScaleFrom(gameObject, iTween.Hash("scale", new Vector3(2,2,2), "time", 1.5f, "oncomplete", "finishedScalling")); 
			alreadyScalling = true;
		}
		if(HexGrid.instance.lastTurn == false){
			iTween.ValueTo(gameObject, iTween.Hash("from", Color.red, "to", Color.white, "onupdatetarget", gameObject, "onupdate", "colorUpdate", "time", 1.0f));
		} else if(HexGrid.instance.lastTurn == true){
			iTween.ValueTo(gameObject, iTween.Hash("from", Color.red, "to", Color.white, "onupdatetarget", gameObject, "onupdate", "colorUpdate", "oncomplete", "reFlashMe", "time", 1.0f));
		}
	}

	private void reFlashMe(){
		iTween.ValueTo(gameObject, iTween.Hash("from", Color.red, "to", Color.white, "onupdatetarget", gameObject, "onupdate", "colorUpdate", "oncomplete", "reFlashMe", "time", 1.0f));
	}

	private void finishedScalling(){
		alreadyScalling = false;
	}

	private void colorUpdate(Color newColor){
		myColorRenderer.color = newColor;
		myColorRenderer.Commit();
	}
}
