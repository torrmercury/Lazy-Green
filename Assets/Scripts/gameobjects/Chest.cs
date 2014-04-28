using UnityEngine;
using System.Collections;

public class Chest : HexGridPiece {

	public Item myItem;

	public AudioClip openChestSound;
	public AudioClip itemGetSound;

	public override void Start ()
	{
		_type = HexGridPiece.CHEST_TYPE;
		base.Start ();
	}

	public void openChest(SimplePlayer player) {
		audio.PlayOneShot(openChestSound);
		audio.PlayOneShot(itemGetSound);
		Camera.main.audio.volume = 0.2f;
		HexGrid.instance.itemGetMenu.GetComponent<ItemGetMenu>().updateItemGet(player.name, myItem.name, myItem.sprite);
		HexGrid.instance.revealItemGet();
		myItem.pickedUpByPlayer(player);
		myItem.gameObject.SetActive(true);

		_shrinking = true;
	}


}
