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

	// --Game Data Storage --//
	public static Dictionary<string, AudioClip> Sounds = new Dictionary<string, AudioClip>(); // holds all loaded sounds
	List<Riff> riffs = new List<Riff>(); // all riffs

	// List of all sound paths to load
	List<string> soundsToLoad = new List<string>() {
		"Audio/Instruments/Percussion/Kick",
		"Audio/Instruments/Percussion/Snare",
		"Audio/Instruments/Percussion/Tom",
		"Audio/Instruments/Percussion/Hat",
	};

	public AudioSource OneShot; // used for playing one-shot sound effects (UI, etc.)

	void Start () {
		if (instance) Debug.LogError("More than one MusicManager exists!");
		else instance = this;

		OneShot = gameObject.AddComponent<AudioSource>();
		LoadAllAudioClips (soundsToLoad);
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
	