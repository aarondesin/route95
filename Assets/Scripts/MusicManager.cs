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
	ElectricGuitar,
	NUM_INSTRUMENTS // easy access to number of instruments in game
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
	public Key currentKey = Key.EMajor; // value will be passed from key button
	public Instrument currentInstrument = Instrument.RockDrums;
	public Song currentSong = new Song();
	public bool loopSong = false; // loop song in live mode?

	// --Game Data Storage --//
	public static Dictionary<string, AudioClip> Sounds = new Dictionary<string, AudioClip>(); // holds all loaded sounds
	//public List<Riff> riffs = new List<Riff>(); // all riffs
	public List<Riff> riffs = new List<Riff> ();

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
	public Dictionary<Instrument, AudioSource> instrumentAudioSources;

	public static float tempo = 120f; // tempo in BPM
	private float BeatTimer;
	private int beat;
	public static bool playing = false;
	public static bool loop = false;



	void Start () {
		if (instance) Debug.LogError("More than one MusicManager exists!");
		else instance = this;

		OneShot = gameObject.AddComponent<AudioSource>();
		LoadAllAudioClips (soundsToLoad);
		instrumentAudioSources = new Dictionary<Instrument, AudioSource>();
		for (int i=0; i<(int)Instrument.NUM_INSTRUMENTS;i++) {
			AudioSource source = gameObject.AddComponent<AudioSource>();
			//instrumentAudioSources.Add((Instrument)i, new AudioSource());
			instrumentAudioSources.Add((Instrument)i, source);
		}
		instrumentAudioSources[Instrument.ElectricGuitar].volume = 0.6f;

		SetupExampleRiffs();
	}

	public void PlayRiffLoop(){
		if (loop) {
			StopLooping();
		} else {
			playing = true;
			loop = true;
		}
	}

	public void StopLooping () {
		playing = false;
		loop = false;
		beat = 0;
		OneShot.Stop();
	}

	void Update(){
		if (playing) {
			if (BeatTimer <= 0f) {
				switch (GameManager.instance.currentMode) {
				case Mode.Setup:
					InstrumentSetup.currentRiff.PlayRiff (beat++);
					if (beat >= 4 && loop)
						beat = 0;
					break;
				case Mode.Live:
					//Debug.Log(beat);
					currentSong.PlaySong(beat++);
					if (beat >= currentSong.beats) {
						if (loopSong) {
							beat = 0;
						} else {
							GameManager.instance.SwitchToPostplay();
						}
					}
					break;
				}
				//if (beat >= (int)Mathf.Pow(2f,(drumRiffs[drumRiffIndex].subdivs+1))) beat = 0;
				//BeatTimer = 3600f/tempo/drumRiffs[drumRiffIndex].subdivs;
				BeatTimer = 3600f / tempo;// 3600f = 60 fps * 60 seconds 

			} else {
				//BeatTimer--;
				BeatTimer -= Time.deltaTime * 100f;
			}
		} 

	}


	// Adds a new riff
	public Riff AddRiff () {
		Riff temp = new Riff ();
		InstrumentSetup.currentRiff = temp;
		SongArrangeSetup.instance.selectedRiffIndex = riffs.Count;
		riffs.Add (temp);
		return temp;
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

	// Remotely toggles looping
	public void ToggleLoopSong () {
		loopSong = !loopSong;
	}

	public void StartSong () {
		loop = loopSong;
		playing = true;
	}

	public void StopPlaying () {
		playing = false;
		//loop = false;
	}

	public void SetupExampleRiffs () {
		riffs.Add( new Riff () {
			name = "Example Guitar Riff",
			currentInstrument = Instrument.ElectricGuitar,
			notes = new List<List<Note>>() {
				new List<Note> () {new Note() { sound = Sounds["ElectricGuitar_E2"] }},
				new List<Note> () {new Note() { sound = Sounds["ElectricGuitar_G#2"] }},
				new List<Note> () {new Note() { sound = Sounds["ElectricGuitar_F#2"] }},
				new List<Note> () {new Note() { sound = Sounds["ElectricGuitar_A2"] }}
			}
		});
		riffs.Add( new Riff () {
			name = "Example Drum Beat",
			currentInstrument = Instrument.RockDrums,
			notes = new List<List<Note>>() {
				new List<Note> () { new Note() { sound = Sounds["RockDrums_Kick"] }},
				new List<Note> () { new Note() { sound = Sounds["RockDrums_Hat"] }},
				new List<Note> () { new Note() { sound = Sounds["RockDrums_Snare"] }},
				new List<Note> () { new Note () {sound = Sounds["RockDrums_Hat"] }}
			}
		});
	}
}
	