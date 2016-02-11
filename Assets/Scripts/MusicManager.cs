using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;// need for using lists
using System.IO; // need for path operations

// All percussion instruments
public enum PercussionInstrument {
	RockDrums
};

// All melodic instruments
public enum MelodicInstrument {
	ElectricGuitar
};

// All instruments (melodic and percussion) for use by MusicManager
public enum Instrument {
	RockDrums,
	ElectricGuitar
};


// All keys available in the game
public enum Key{
	DFlat,
	DMajor,
	EFlat,
	EMajor,
	FMajor
};


public class MusicManager : MonoBehaviour {
	public static MusicManager instance; // access this MusicManager from anywhere using MusicManager.instance

	// --Global Music Properties-- //
	public static Key currentKey = Key.EMajor; // value will be passed from key button
	public static Instrument currentInstrument = Instrument.RockDrums;
	public static Song currentSong = new Song();

	// --Game Data Storage --//
	public static Dictionary<string, AudioClip> Sounds = new Dictionary<string, AudioClip>(); // holds all loaded sounds
	//public List<Riff> riffs = new List<Riff>(); // all riffs
	public List<Riff> riffs = new List<Riff> () {
		new Riff () { name = "Guitar Riff", currentInstrument = Instrument.ElectricGuitar },
		new Riff () { name = "Drum Beat", currentInstrument = Instrument.RockDrums }
	};

	// List of all sound paths to load
	List<string> soundsToLoad = new List<string>() {
		// Melodic.ElectricGuitar
		"Audio/Instruments/Melodic/ElectricGuitar_E2",
		"Audio/Instruments/Melodic/ElectricGuitar_F#2",
		"Audio/Instruments/Melodic/ElectricGuitar_G#2",
		"Audio/Instruments/Melodic/ElectricGuitar_A2",
		"Audio/Instruments/Melodic/ElectricGuitar_B2",
		"Audio/Instruments/Melodic/ElectricGuitar_C#3",
		"Audio/Instruments/Melodic/ElectricGuitar_D#3",

		// Percussion.RockDrums
		"Audio/Instruments/Percussion/RockDrums_Kick",
		"Audio/Instruments/Percussion/RockDrums_Snare",
		"Audio/Instruments/Percussion/RockDrums_Tom",
		"Audio/Instruments/Percussion/RockDrums_Hat"
	};

	public static Dictionary<Instrument, string> instToString = new Dictionary<Instrument, string> () {
		{ Instrument.ElectricGuitar, "Electric Guitar" },
		{ Instrument.RockDrums, "Rock Drums" },
	};
		
	public AudioSource OneShot; // used for playing one-shot sound effects (UI, etc.)
	public AudioSource LoopRiff;

	public static float tempo = 120f; // tempo in BPM
	private float BeatTimer;
	private int beat;
	public static bool loop = false;



	void Start () {
		if (instance) Debug.LogError("More than one MusicManager exists!");
		else instance = this;

		OneShot = gameObject.AddComponent<AudioSource>();
		LoadAllAudioClips (soundsToLoad);
	}

	public void riffloop(){
		if (loop) {
			loop = false;
			beat = 0;
		}
		else
			loop = true;
	}

	void Update(){
		if (loop) {
			if (BeatTimer <= 0f) {
				InstrumentSetup.currentRiff.PlayRiff (beat++);
				//if (beat >= (int)Mathf.Pow(2f,(drumRiffs[drumRiffIndex].subdivs+1))) beat = 0;
				//BeatTimer = 3600f/tempo/drumRiffs[drumRiffIndex].subdivs;
				if (beat >= 4)
					beat = 0;
				BeatTimer = 3600f / tempo;// 3600f = 60 fps * 60 seconds 

			} else {
				//BeatTimer--;
				BeatTimer -= Time.deltaTime * 100f;
				Debug.Log (BeatTimer);
			}
		} 

	}


	// Adds a new riff
	public void AddRiff () {
		Riff temp = new Riff ();
		InstrumentSetup.currentRiff = temp;
		riffs.Add (temp);
	}

	// Plays a single sound effect through OneShot AudioSource
	public void PlayOneShot (AudioClip clip) {
		OneShot.Stop();
		OneShot.clip = clip;
		OneShot.Play();
	}


	// Loads all audio clip paths in soundsToLoad
	void LoadAllAudioClips (List<string> paths) {
		foreach (string path in paths) {
			LoadAudioClip  (path);
		}
	}
	


	// Loads a single audio clip
	void LoadAudioClip (string path) {
		AudioClip sound = (AudioClip) Resources.Load (path);
		if (sound == null) {
			Debug.LogError("Failed to load AudioClip at "+path);
		} else {
			Debug.Log("Loaded "+path);
			Sounds.Add (Path.GetFileNameWithoutExtension (path), sound);
		}
	}

}
	