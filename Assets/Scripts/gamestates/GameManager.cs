using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public enum GameState {
		Paused,
		Running
	}
	protected GameState _currentState = GameState.Running;
	public GameState currentState {
		get { return _currentState; }
	}
	public bool isPaused {
		get { return _currentState == GameState.Paused; }
	}

	// Fuzzy Singleton (we don't really check to enforce that there's only one so be careful).
	protected static GameManager _instance;
	public static GameManager instance {
		get { return _instance; }
	}
	

	// Use this for initialization
	public virtual void Start () {
		_instance = this;
	}
	
	// Update is called once per frame
	public virtual void Update () {
	
	}

	// Simple sleep function for maximum game feel
	// This is here instead of in global funcs because it needs to be attached to a game object for the coroutines to work.
	public void sleep(float time) {
		StartCoroutine(doSleep(time));
	}
	
	protected IEnumerator doSleep(float time) {
		Time.timeScale = 0;
		float startTime = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup - startTime < time) {
			yield return 0;
		}
		Time.timeScale = 1;
	}

}
