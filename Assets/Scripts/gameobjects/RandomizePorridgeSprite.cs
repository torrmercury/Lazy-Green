using UnityEngine;
using System.Collections;

public class RandomizePorridgeSprite : MonoBehaviour {

	public Sprite[] porridgeSprites = new Sprite[6];
	private Sprite chosenSprite;

	private SimplePorridge porridgeSprite;

	void Start(){
		chosenSprite = porridgeSprites[Random.Range(0, porridgeSprites.Length)];
		gameObject.GetComponent<SpriteRenderer>().sprite = chosenSprite;
	}
}
