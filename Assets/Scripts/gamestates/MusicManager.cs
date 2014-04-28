using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour {

	public AudioClip[] levelMusic = new AudioClip[5];
	private AudioClip gameSong;

	void Update () {
		if(!audio.isPlaying)
			playRandomMusic();
	}

	public void playRandomMusic(){
		gameSong = levelMusic[Random.Range(0, levelMusic.Length)];
		audio.clip = gameSong;
		audio.Play();
	}
}
