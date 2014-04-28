using UnityEngine;
using System.Collections;

public class GlobalAudio {

	protected static GlobalAudio _instance;
	public static GlobalAudio instance {
		get {
			if (_instance == null) {
				_instance = new GlobalAudio();
			}
			return _instance;
		}
	}


	protected AudioSource _audio;
	public AudioSource audio {
		get { return _audio; }
	}

	public GlobalAudio() {
		GameObject audioObj = new GameObject("globalaudio");
		GameObject.DontDestroyOnLoad(audioObj);
		_audio = audioObj.AddComponent<AudioSource>();
		_audio.playOnAwake = false;
	}




}
