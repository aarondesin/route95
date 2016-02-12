using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;// need for using lists


public class Note {
	
	public AudioClip sound;
	float duration;
	float volume;

	public Note () {
		duration = 1f;
		volume = 1f;
	}

	public Note (AudioClip newSound) {

		if (newSound == null) Debug.LogError ("Note(): was passed null sound!");
		sound = newSound;
		//Debug.Log("note exist?" + newSound);
		duration = 1f;
		volume = 1f;

	}

	// FIX ME!!
	public void PlayNote () {
		//Debug.Log ("do nothing");
		MusicManager.instance.OneShot.PlayOneShot(sound,1f);
		//AudioSource.clip = Note.sound;
		// AudioSource.Play();
		// sound.Play();
	}

	// Play note on a specific AudioSource
	public void PlayNote (AudioSource source) {
		source.PlayOneShot(sound, 1f);
	}
}

/*

public static Audiosource Kick;
public static AudioSource Tom;
public static AudioSource Snare;
public static AudioSource Hat;

public static int numInst = 3;
	public static int numSounds = 4;
	//public AudioSource [,] soundList = new AudioSource[numInst , numSounds];
	
	//public AudioSource [,] soundList = new AudioSource[numInst , numSounds];
	public AudioSource [] soundList = new AudioSource[numsound];
	
	
	
	
	public void loadSounds(){
		
		// Percussion sounds
		//soundList [0 ,0] = Kick;
		/*soundList [0 ,0] = Kick;
		soundList [0 ,1] = Tom;
		soundList [0 ,2] = Snare;
		soundList [0 ,3] = Hat;
		soundList [0 ,3] = Hat;
//Percussion
soundList [0] = Kick;
soundList [1] = Tom;
soundList [2] = Snare;
soundList [3] = Hat;

public void playnote(int sound){ // plays a single note when called
	soundList[sound].Play();

}

*/
