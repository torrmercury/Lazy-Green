using UnityEngine;
using System.Collections;

/// <summary>
/// This script handles tweening in and out the player menus. 
/// </summary>
public class BottomMenu : MonoBehaviour {

	public GameObject bottomBar, bottomLeftEnd, bottomRightEnd;

	// How much we scale the bottom bar (along the x axis) for each menu in the item. 
	public float scalePerMenuItem = 51;

	// the y coordinate where we consider ourselves closed. 
	protected float _closedThreshold;
	protected float _closedY;
	protected float _openY;

	protected GameObject _currentMenu, _menuToOpen;


	protected bool isClosed {
		get { return transform.localPosition.y < _closedThreshold; }
	}

	public void openPlayerMenu(GameObject playerMenu) {
		if (isClosed) {
			switchToPlayerMenu(playerMenu);
			iTween.MoveTo(gameObject, iTween.Hash("islocal", true, "y", _openY, "time", 0.2f));
		}
		else {
			_menuToOpen = playerMenu;
			iTween.MoveTo(gameObject, iTween.Hash("isLocal", true, "y", _closedY, "time", 0.2f, "oncompletetarget", gameObject, "oncomplete", "finishOpenPlayerMenu"));
		}

	}

	public void finishOpenPlayerMenu() {
		switchToPlayerMenu(_menuToOpen);
		iTween.MoveTo(gameObject, iTween.Hash("islocal", true, "y", _openY, "time", 0.2f));
	}

	protected void switchToPlayerMenu(GameObject menuToOpen) {
		if (_currentMenu != null)
			_currentMenu.SetActive(false);
		_currentMenu = menuToOpen;

		// TODO: Resize our bar by the number of items in the menu
		int numActiveButtons = 0;
		foreach (Transform child in _currentMenu.transform) {
			if (child.gameObject.activeSelf) {
				numActiveButtons++;
			}
		}
		float bottomScaleX = scalePerMenuItem*numActiveButtons;
		bottomBar.transform.localScale = new Vector3(bottomScaleX, bottomBar.transform.localScale.y, bottomBar.transform.localScale.z);
		bottomLeftEnd.transform.localPosition = new Vector3(-bottomScaleX/16, bottomLeftEnd.transform.localPosition.y, bottomLeftEnd.transform.localPosition.z);
		bottomRightEnd.transform.localPosition = new Vector3(bottomScaleX/16, bottomRightEnd.transform.localPosition.y, bottomRightEnd.transform.localPosition.z);

		_currentMenu.SetActive(true);
	}

	public void closeMenu() {
		iTween.MoveTo(gameObject, iTween.Hash("islocal", true, "y", _closedY, "time", 0.2f));
	}


	// Use this for initialization
	void Start () {
		_closedThreshold = transform.localPosition.y+0.5f;
		_closedY = transform.localPosition.y;
		_openY = _closedY+11;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
