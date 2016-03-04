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
	ElectricGuitar,
	ElectricBass
};

// All instruments (melodic and percussion) for use by MusicManager
public enum Instrument {
	RockDrums,
	ElectricGuitar,
	ElectricBass,
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
	public Instrument currentInstrument = Instrument.ElectricGuitar;
	public Song currentSong;
	public bool loopSong = false; // loop song in live mode?

	// --Game Data Storage --//
	public static Dictionary<string, AudioClip> Sounds = new Dictionary<string, AudioClip>(); // holds all loaded sounds
	public List<Riff> riffs = new List<Riff> ();
	public List<SongPiece> songPieces = new List<SongPiece>();
	public Dictionary<Instrument, List<Riff>> licks = new Dictionary<Instrument, List<Riff>>() {
		{ Instrument.ElectricBass, new List <Riff> () },
		{ Instrument.ElectricGuitar, new List <Riff> () },
		{ Instrument.RockDrums, new List <Riff> () }
	};
	// All lick notes waiting to be played
	List<List<Note>> lickQueue = new List<List<Note>>();

	// List of all sound paths to load
	List<string> soundsToLoad = new List<string>() {// maybe use dict for efficiency
		// Melodic.ElectricGuitar
		"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_E2",
		"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_F2",
		"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_F#2",
		"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_G2",
		"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_G#2",
		"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_A2",
		"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_A#2",
		"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_B2",
		"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_C3",
		"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_C#3",
		"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_D3",
		"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_D#3",
		"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_E3",
		"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_F3",
		"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_F#3",
		"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_G3",
		"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_G#3",
		"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_A3",
		"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_A#3",
		"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_B3",
		"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_C4",
		"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_C#4",
		"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_D4",
		"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_D#4",
		"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_E5",
		"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_F5",
		"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_F#5",
		"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_G5",
		"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_G#5",
		"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_A5",
		"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_A#5",
		"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_B5",
		"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_C6",
		"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_C#6",

		//Melodic.ElectricBass
		"Audio/Instruments/Melodic/ElectricBass/ElectricBass_E1",
		"Audio/Instruments/Melodic/ElectricBass/ElectricBass_F1",
		"Audio/Instruments/Melodic/ElectricBass/ElectricBass_F#1",
		"Audio/Instruments/Melodic/ElectricBass/ElectricBass_G1",
		"Audio/Instruments/Melodic/ElectricBass/ElectricBass_G#1",
		"Audio/Instruments/Melodic/ElectricBass/ElectricBass_A1",
		"Audio/Instruments/Melodic/ElectricBass/ElectricBass_A#1",
		"Audio/Instruments/Melodic/ElectricBass/ElectricBass_B1",
		"Audio/Instruments/Melodic/ElectricBass/ElectricBass_C2",
		"Audio/Instruments/Melodic/ElectricBass/ElectricBass_C#2",
		"Audio/Instruments/Melodic/ElectricBass/ElectricBass_D2",
		"Audio/Instruments/Melodic/ElectricBass/ElectricBass_D#2",
		"Audio/Instruments/Melodic/ElectricBass/ElectricBass_E2",
		"Audio/Instruments/Melodic/ElectricBass/ElectricBass_F2",
		"Audio/Instruments/Melodic/ElectricBass/ElectricBass_F#2",
		"Audio/Instruments/Melodic/ElectricBass/ElectricBass_G2",
		"Audio/Instruments/Melodic/ElectricBass/ElectricBass_G#2",
		"Audio/Instruments/Melodic/ElectricBass/ElectricBass_A2",
		"Audio/Instruments/Melodic/ElectricBass/ElectricBass_A#2",
		"Audio/Instruments/Melodic/ElectricBass/ElectricBass_B2",
		"Audio/Instruments/Melodic/ElectricBass/ElectricBass_C3",
		"Audio/Instruments/Melodic/ElectricBass/ElectricBass_C#3",
		"Audio/Instruments/Melodic/ElectricBass/ElectricBass_D3",
		"Audio/Instruments/Melodic/ElectricBass/ElectricBass_D#3",
		"Audio/Instruments/Melodic/ElectricBass/ElectricBass_E3",
		"Audio/Instruments/Melodic/ElectricBass/ElectricBass_F3",
		"Audio/Instruments/Melodic/ElectricBass/ElectricBass_F#3",
		"Audio/Instruments/Melodic/ElectricBass/ElectricBass_G3",
		"Audio/Instruments/Melodic/ElectricBass/ElectricBass_G#3",
		"Audio/Instruments/Melodic/ElectricBass/ElectricBass_A3",
		"Audio/Instruments/Melodic/ElectricBass/ElectricBass_A#3",
		"Audio/Instruments/Melodic/ElectricBass/ElectricBass_B3",
		"Audio/Instruments/Melodic/ElectricBass/ElectricBass_C4",
		"Audio/Instruments/Melodic/ElectricBass/ElectricBass_C#4",
		"Audio/Instruments/Melodic/ElectricBass/ElectricBass_D4",
		"Audio/Instruments/Melodic/ElectricBass/ElectricBass_D#4",

		// Percussion.RockDrums
		"Audio/Instruments/Percussion/RockDrums_Kick",
		"Audio/Instruments/Percussion/RockDrums_Snare",
		"Audio/Instruments/Percussion/RockDrums_Tom",
		"Audio/Instruments/Percussion/RockDrums_Hat"
	};

	/*
	 * Dictionary<Key, Dictionary<Instrument, Scale>> scales = new Dicto.... () {
	 * 	{ Key.EMinor, new Dictionary<Instrument, Scale> () {
	 * 		{ Instrument.ElectricGuitar, Scale.ElectricGuitarEMinor () }
	 * 
	 * 
	 * 
	 * 
	 * scale = scales[MM.currentKey][riff.currentInstrument];
	 * */

	public static Dictionary<Instrument, string> instToString = new Dictionary<Instrument, string> () {
		{ Instrument.ElectricGuitar, "Electric Guitar" },
		{ Instrument.RockDrums, "Rock Drums" },
		{ Instrument.ElectricBass, "Electric Bass" },
	};
		
	public AudioSource OneShot; // used for playing one-shot sound effects (UI, etc.)
	public AudioSource LoopRiff;
	public Dictionary<Instrument, AudioSource> instrumentAudioSources;

	public static float tempo = 120f; // tempo in BPM
	private float BeatTimer;
	private int beat;
	public static bool playing = false;
	public static bool loop = false;

	public int maxBeats; // max beats in riff, after subdivisions

	void Start () {
		if (instance) Debug.LogError("More than one MusicManager exists!");
		else instance = this;

		currentSong = new Song ();

		OneShot = gameObject.AddComponent<AudioSource>();
		LoadAllAudioClips (soundsToLoad);
		instrumentAudioSources = new Dictionary<Instrument, AudioSource>();
		for (int i=0; i<(int)Instrument.NUM_INSTRUMENTS;i++) {
			GameObject obj = new GameObject ();
			AudioSource source = obj.AddComponent<AudioSource>();
			//instrumentAudioSources.Add((Instrument)i, new AudioSource());
			instrumentAudioSources.Add((Instrument)i, source);
		}
		//instrumentAudioSources[Instrument.ElectricGuitar].volume = 0.6f;
		instrumentAudioSources[Instrument.RockDrums].volume = 0.8f;
		//instrumentAudioSources [Instrument.ElectricBass].gameObject.AddComponent<AudioDistortionFilter> ();
		//instrumentAudioSources[Instrument.ElectricBass].gameObject.GetComponent<AudioDistortionFilter> ().distortionLevel = 0.8f;

		maxBeats = (int)Mathf.Pow(2f, (float)Riff.MAX_SUBDIVS+2);

		SetupExampleRiffs();
		SetupExampleLicks();
		//Debug.Log ("set up done" , licks.Count);


	}

	public void PlayRiffLoop(){
		if (loop) {
			StopLooping();
			instrumentAudioSources[InstrumentSetup.currentRiff.instrument].Stop();
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
					if (beat >= maxBeats && loop)
						beat = 0;
					break;
				case Mode.Live:
					//Debug.Log(beat);
					currentSong.PlaySong(beat++);
					//Debug.Log(lickQueue.Count);
					if (lickQueue.Count > 0) {
						//Debug.Log("dick");
						PopLickQueue();
					}
					if (beat >= currentSong.beats) {
						beat = 0;
						if (loopSong) {
							Debug.Log(lickQueue.Count);
						} else {
							GameManager.instance.SwitchToPostplay();
						}
					}
					break;
				}
				//if (beat >= (int)Mathf.Pow(2f,(drumRiffs[drumRiffIndex].subdivs+1))) beat = 0;
				//BeatTimer = 3600f/tempo/drumRiffs[drumRiffIndex].subdivs;
				BeatTimer = 3600f / (float) (maxBeats/4) / tempo;// 3600f = 60 fps * 60 seconds 

			} else {
				//BeatTimer--;
				//BeatTimer -= Time.deltaTime * 100f;
				BeatTimer -= 1.667f;
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

	public void AddRiff (Riff riff) {
		riff.copy = CopyNumber(riff.name);
		riffs.Add (riff);
		//SongArrangeSetup.instance.Refresh();
	}

	public Riff RiffByString (string riffName) {
		foreach (Riff riff in riffs) {
			if (riffName == riff.name) return riff;
		}
		return null;
	}

	public void AddSongPiece (SongPiece songPiece) {
		songPieces.Add(songPiece);
		//SongArrangeSetup.instance.Refresh();
	}

	public void QueueLick (Riff lick) {
		if (lick == null || lickQueue.Count != 0) return;
		lickQueue.Clear();
		foreach (List<Note> beat in lick.notes) {
			lickQueue.Add(beat);
			Debug.Log ("test");
		}
		Debug.Log("queued "+lickQueue.Count);
	}

	public void PopLickQueue () {
		//Debug.Log("play");
		foreach (Note note in lickQueue[0]) {
			note.PlayNote(instrumentAudioSources[currentInstrument], true);
		}
		lickQueue.RemoveAt(0);
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

	// Returns true if there is a riff of a certain name already
	public bool ContainsRiffNamed (string riffName) {
		foreach (Riff riff in riffs) {
			if (riff.name == riffName) return true;
		}
		return false;
	}
	public int CopyNumber (string riffName) {
		Debug.Log("CopyNumber(): "+riffName);
		int result = 0;
		foreach (Riff riff in riffs) {
			if (riff.name == riffName) {
				result++;
			}
		}
		return result;
	}

	// Clears song, songpiece, and riff data
	public void Clear () {
		riffs.Clear();
		songPieces.Clear();
	}

	public void SetupExampleRiffs () {
		riffs.Add( new Riff () {
			name = "Example Guitar Riff",
			instrument = Instrument.ElectricGuitar,
			notes = new List<List<Note>>() {
				new List<Note> () {new Note("ElectricGuitar_E2") },
				new List<Note> (),
				new List<Note> (),
				new List<Note> (),
				new List<Note> () {new Note("ElectricGuitar_G#2") },
				new List<Note> (),
				new List<Note> (),
				new List<Note> (),
				new List<Note> () {new Note("ElectricGuitar_F#2")},
				new List<Note> (),
				new List<Note> (),
				new List<Note> (),
				new List<Note> () {new Note("ElectricGuitar_A2") },
				new List<Note> (),
				new List<Note> (),
				new List<Note> ()
			}
		});
		riffs.Add( new Riff () {
			name = "Example Bass Riff",
			instrument = Instrument.ElectricBass,
			notes = new List<List<Note>>() {
				new List<Note> () { new Note("ElectricBass_E1") },
				new List<Note> (),
				new List<Note> (),
				new List<Note> (),
				new List<Note> () { new Note("ElectricBass_G#1") },
				new List<Note> (),
				new List<Note> (),
				new List<Note> (),
				new List<Note> () { new Note("ElectricBass_B1") },
				new List<Note> (),
				new List<Note> (),
				new List<Note> (),
				new List<Note> () { new Note ("ElectricBass_C#2") },
				new List<Note> (),
				new List<Note> (),
				new List<Note> ()
			}
		});
		riffs.Add( new Riff () {
			name = "Example Drum Beat",
			instrument = Instrument.RockDrums,
			cutSelf = false,
			notes = new List<List<Note>>() {
				new List<Note> () { new Note("RockDrums_Kick") },
				new List<Note> (),
				new List<Note> (),
				new List<Note> (),
				new List<Note> () { new Note("RockDrums_Hat") },
				new List<Note> (),
				new List<Note> (),
				new List<Note> (),
				new List<Note> () { new Note("RockDrums_Snare") },
				new List<Note> (),
				new List<Note> (),
				new List<Note> (),
				new List<Note> () { new Note ("RockDrums_Hat") },
				new List<Note> (),
				new List<Note> (),
				new List<Note> ()
			}
		});
	}

	public void SetupExampleLicks () {
		licks[Instrument.ElectricGuitar].Add (new Riff () {
			name = "Example Guitar Lick",
			instrument = Instrument.ElectricGuitar,
			notes = new List<List<Note>>() {
				new List<Note> () {new Note("ElectricGuitar_E2") },
				new List<Note> () ,
				new List<Note> () ,
				new List<Note> () ,
				new List<Note> () {new Note("ElectricGuitar_A2") },
				new List<Note> () ,
				new List<Note> () {new Note("ElectricGuitar_B2") },
				new List<Note> ()
			}
		});

		licks[Instrument.ElectricBass].Add (new Riff () {
			name = "Example Bass Lick",
			instrument = Instrument.ElectricBass,
			notes = new List<List<Note>>() {
				new List<Note> () {new Note("ElectricBass_F#1") },
				new List<Note> () ,
				new List<Note> () ,
				new List<Note> () ,
				new List<Note> () {new Note("ElectricBass_F#1") },
				new List<Note> () ,
				new List<Note> () {new Note("ElectricBass_A1") },
				new List<Note> () ,
				new List<Note> () ,
				new List<Note> () ,
				new List<Note> () ,
				new List<Note> () ,
				new List<Note> () {new Note("ElectricBass_C#2") },
				new List<Note> () ,
				new List<Note> () ,
				new List<Note> ()
			}
		});

		licks[Instrument.RockDrums].Add (new Riff () {
			name = "Example Drums Lick",
			instrument = Instrument.RockDrums,
			notes = new List<List<Note>>() {
				new List<Note> () {new Note("RockDrums_Snare")},
				new List<Note> () {new Note("RockDrums_Snare")},
				new List<Note> () ,
				new List<Note> () ,
				new List<Note> () { new Note("RockDrums_Hat")},
			}
		});
	}
}
	