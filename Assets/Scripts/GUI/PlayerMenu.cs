using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Script for organizing buttons in the player's menu. Especially important because of menu items.
/// </summary>
public class PlayerMenu : MonoBehaviour {

	public float spacing = 2.5f;

	public Transform cancelButton;



	public Transform confirmButton;


	public void insertNewMenuItem(Transform newItem) {
		newItem.parent = transform;

		float N = transform.childCount;
		float width = N*spacing;

		List<Transform> nonEdgeButtons = new List<Transform>();
		foreach (Transform child in transform) {
			if (child != cancelButton && child != confirmButton && child != newItem) {
				nonEdgeButtons.Add(child);
			}
		}
		nonEdgeButtons.Sort(delegate(Transform t1, Transform t2) {
			if (t1.localPosition.x < t2.localPosition.x)
				return -1;
			else if (t1.localPosition.x > t2.localPosition.x)
				return 1;
			else
				return 0;
		});

		for (int i = 0; i < N; i++) {
			Transform button = null;
			if (i == 0)
				button = cancelButton;
			else if (i == N-2)
				button = newItem;
			else if (i == N-1)
				button = confirmButton;
			else
				button = nonEdgeButtons[i-1];
			button.transform.localPosition = new Vector3(-width/2 + spacing*i + spacing/2, 0.2f, -0.2f);
		}

	}
	


}

