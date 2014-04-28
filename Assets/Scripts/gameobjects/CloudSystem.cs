using UnityEngine;
using System.Collections;

public class CloudSystem : MonoBehaviour {

	private bool startedCoroutine = true;

	public float maxWait = 15.0f;
	public float minWait = 12.0f;

	public Sprite[] cloudSprites = new Sprite[4];
	private Sprite chosenSprite;

	void Start(){
		startedCoroutine = true;
		StartCoroutine(spawnClouds());
	}

	IEnumerator spawnClouds(){
		while(startedCoroutine){

			yield return new WaitForSeconds(1f);
		}
	}
	
}
