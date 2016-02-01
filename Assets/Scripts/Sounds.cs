using UnityEngine;
using System.Collections;

public class Sounds : MonoBehaviour {

	public static AudioClip Kick;
	public static AudioClip Snare;
	public static AudioClip Tom;
	public static AudioClip Hat;


	public static void Load () {

		Kick = Resources.Load ("Audio/Instruments/Percussion/Kick.wav") as AudioClip;
		Snare = Resources.Load ("Audio/Instruments/Percussion/Snare.wav") as AudioClip;
		Tom = Resources.Load ("Audio/Instruments/Percussion/Tom.wav") as AudioClip;
		Hat = Resources.Load ("Audio/Instruments/Percussion/Hat.wav") as AudioClip;

		//public static AudioClip = Resources.Load("Audio/Instruments/Melodic/Electric Guitar/Kick.wav") as AudioClip;

	}

}

