using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;// need for using lists


public class Note {
	
	public AudioClip sound;
	float duration;
	float volume;

	public Note (AudioClip newSound) {
		if (Sounds.Kick == null) Debug.LogError("failed to load kick");
		if (newSound == null)
			Debug.LogError ("sound is null");
		sound = newSound;
		duration = 1f;
		volume = 1f;
	}

	// FIX ME!!
	public void PlayNote () {
		// AudioSource.clip = sound;
		// AudioSource.Play();
		// sound.Play();
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
