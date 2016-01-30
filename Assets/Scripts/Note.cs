using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Note{

	public static AudioSource Kick;
	public static AudioSource Tom;
	public static AudioSource Snare;
	public static AudioSource Hat;



	public static int numInst = 3;
	public static int numSounds = 4;
	public AudioSource [,] soundList = new AudioSource[numInst , numSounds];




	public void loadSounds(){

		// Percussion sounds
		soundList [0 ,0] = Kick;
		soundList [0 ,1] = Tom;
		soundList [0 ,2] = Snare;
		soundList [0 ,3] = Hat;
	}



	

}