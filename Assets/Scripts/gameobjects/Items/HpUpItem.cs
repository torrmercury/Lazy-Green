using UnityEngine;
using System.Collections;

public class HpUpItem : Item {

	public override void pickedUpByPlayer (SimplePlayer player)
	{
		base.pickedUpByPlayer (player);

		// Just increase the player's max hp and current hp and then leave
		player.maxHealth++;
		player.currentHealth++;

		Destroy(gameObject);

	}
}
