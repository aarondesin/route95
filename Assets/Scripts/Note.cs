using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Note{
	public Note(AudioSource currnote){
		this.currnote = currnote;

	}
	private AudioSource currnote;

	public static AudioSource Kick;
	public static AudioSource Tom;
	public static AudioSource Snare;
	public static AudioSource Hat;


	public static int numInst = 3;
	public static int numSounds = 4;
	public AudioSource [,] soundList = new AudioSource[numInst , numSounds];


	public void playnote(){
		
	}

	public void loadSounds(){

		// Percussion sounds
		soundList [0 ,0] = Kick;
		soundList [0 ,1] = Tom;
		soundList [0 ,2] = Snare;
		soundList [0 ,3] = Hat;

	}

	/*public AudioClip[] list;
	
	void Start ()
	{
		//Loading the items into the array
		list =  new AudioClip[]{(AudioClip)Resources.Load("Sound/Music/intro.wav"),
			(AudioClip)Resources.Load("Sound/Music/level1.wav"), 
			(AudioClip)Resources.Load("Sound/Music/level2.wav"), 
			(AudioClip)Resources.Load("Sound/Music/level3.wav")};
	}
*/
	

}