using UnityEngine;
using System.Collections;

public class StoUpItem : Item {

	public override void pickedUpByPlayer (SimplePlayer player)
	{
		base.pickedUpByPlayer (player);

		player.maxStomach++;
		Destroy (gameObject);

	}

}
