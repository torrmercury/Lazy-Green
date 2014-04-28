using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {
	
	public AudioClip[] fartSounds = new AudioClip[3];
	private AudioClip fartSoundSelected;
	

	public void playFartSound(){
		fartSoundSelected = fartSounds[Random.Range(0, fartSounds.Length)];
		audio.clip = fartSoundSelected;
		audio.Play();
	}
}
