using UnityEngine;
using System.Collections;

public class PotionItem : Item {


	public ShadowText _quantityText;

	protected int _quantity = 1;
	public int quantity {
		get { return _quantity; }
		set { _quantity = value; }
	}

	void Update() {
		_quantityText.setText(string.Format("x{0}", _quantity));
	}

	public override void pickedUpByPlayer (SimplePlayer player)
	{
		base.pickedUpByPlayer (player);

		// If the player has a potion item already, instead of attaching to the player, increase its quantity.

		PotionItem[] maybePotions = player.menuObj.GetComponentsInChildren<PotionItem>(true);
		if (maybePotions.Length > 0) {
			maybePotions[0].quantity++;
			Destroy (gameObject);

		}
		else {
			player.menuObj.GetComponent<PlayerMenu>().insertNewMenuItem(transform);
		}
	}

	public override void itemPressed ()
	{
		if (_player.currentHealth < _player.maxHealth && _quantity > 0) {
			_player.currentHealth = _player.maxHealth;
			_quantity--;
			_player.waitPressed();
		}
	}

}
