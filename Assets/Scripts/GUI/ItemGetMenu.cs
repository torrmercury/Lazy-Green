using UnityEngine;
using System.Collections;

public class ItemGetMenu : MonoBehaviour {

	public ShadowText itemText;
	public GameObject itemSpriteObj;

	public void updateItemGet(string playerName, string itemName, Sprite itemSprite) {
		itemText.setText(string.Format("{0} got\n{1}", playerName, itemName));
		(itemSpriteObj.renderer as SpriteRenderer).sprite = itemSprite;
	}


}
