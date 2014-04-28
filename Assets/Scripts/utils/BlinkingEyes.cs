using UnityEngine;
using System.Collections;

public class BlinkingEyes : MonoBehaviour {

	public Sprite openEyes;
	public Sprite closedEyes;

	private Vector2 originalPos;
	
	void Start(){
		originalPos = gameObject.GetComponent<Transform>().transform.localPosition;
		StartCoroutine(LetsBlink());
	}

	IEnumerator LetsBlink(){
		while(true){
			//Wait between 3 to 6 seconds
			yield return new WaitForSeconds(Random.Range(3.0f, 5.0f));

			//Blink for like a few milliseconds
			gameObject.GetComponent<SpriteRenderer>().sprite = closedEyes;
			gameObject.GetComponent<Transform>().localPosition = originalPos + new Vector2(0.1f, -0.1f);
			yield return new WaitForSeconds(0.2f);

			//Change it back to open eyes
			gameObject.GetComponent<SpriteRenderer>().sprite = openEyes;
			gameObject.GetComponent<Transform>().localPosition = originalPos;
		}
	}

}
