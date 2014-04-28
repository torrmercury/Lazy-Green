using UnityEngine;
using System.Collections;

public class RandomizeSprite : MonoBehaviour {

	public Sprite[] raftSprites = new Sprite[6];
	private Sprite chosenSprite;

	void Start () {
		chosenSprite = raftSprites[Random.Range(0, raftSprites.Length)];
		gameObject.GetComponent<SpriteRenderer>().sprite = chosenSprite;
	}
}
