using UnityEngine;
using System.Collections;

public class SimpleBarricade : HexGridPiece {

	public int maxHealth = 3;
	public int currentHealth;

	public AudioClip takeDamageSound;
	public AudioClip barricadeDieSound;

	public Sprite crackOne, crackTwo, crackThree;

	protected Sprite _startSprite;

	public override void Start () {
		currentHealth = maxHealth;
		_type = HexGridPiece.BARRICADE_TYPE;
		_startSprite = (renderer as SpriteRenderer).sprite;
		base.Start();
	}


	public void fixBarricade() {
		currentHealth = maxHealth;
		_spriteRender.sprite = _startSprite;
	}

	public void updateBarricadeSprite() {
		if (currentHealth == 2)
			_spriteRender.sprite = crackOne;
		else if (currentHealth == 1)
			_spriteRender.sprite = crackTwo;
		else if (currentHealth == 0)
			_spriteRender.sprite = crackThree;
	}

	private void colorUpdate(Color newColor){
		_spriteRender.color = newColor;
	}

	public void takeDamage(){
		iTween.ValueTo(gameObject, iTween.Hash("from", Color.red, "to", Color.white, "onupdatetarget", gameObject, "onupdate", "colorUpdate", "time", 1.0f));
		currentHealth--;
		shake ();
		if(currentHealth > 0)
			audio.PlayOneShot(takeDamageSound);
		updateBarricadeSprite();

		if(currentHealth <= 0) {
			GlobalAudio.instance.audio.PlayOneShot (barricadeDieSound);
			_shrinking = true;
			_dieAfterShrink = true;
		}


	}

}
