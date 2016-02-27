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
	public Song currentSong = new Song();
	public bool loopSong = false; // loop song in live mode?

	// --Game Data Storage --//
	public static Dictionary<string, AudioClip> Sounds = new Dictionary<string, AudioClip>(); // holds all loaded sounds
	//public List<Riff> riffs = new List<Riff>(); // all riffs
	public List<Riff> riffs = new List<Riff> ();
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
		"Audio/Instruments/Melodic/ElectricGuitar_E2",
		"Audio/Instruments/Melodic/ElectricGuitar_F#2",
		"Audio/Instruments/Melodic/ElectricGuitar_G#2",
		"Audio/Instruments/Melodic/ElectricGuitar_A2",
		"Audio/Instruments/Melodic/ElectricGuitar_B2",
		"Audio/Instruments/Melodic/ElectricGuitar_C#3",
		"Audio/Instruments/Melodic/ElectricGuitar_D#3",

		//Melodic.ElectricBass
		"Audio/Instruments/Melodic/Bass_guitar/bassguitarE3",
		"Audio/Instruments/Melodic/Bass_guitar/bassguitarF#3",
		"Audio/Instruments/Melodic/Bass_guitar/bassguitarG#3",
		"Audio/Instruments/Melodic/Bass_guitar/bassguitarA3",
		"Audio/Instruments/Melodic/Bass_guitar/bassguitarB3",
		"Audio/Instruments/Melodic/Bass_guitar/bassguitarC#4",
		"Audio/Instruments/Melodic/Bass_guitar/bassguitarD#4",


		// Percussion.RockDrums
		"Audio/Instruments/Percussion/RockDrums_Kick",
		"Audio/Instruments/Percussion/RockDrums_Snare",
		"Audio/Instruments/Percussion/RockDrums_Tom",
		"Audio/Instruments/Percussion/RockDrums_Hat"
	};

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

		OneShot = gameObject.AddComponent<AudioSource>();
		LoadAllAudioClips (soundsToLoad);
		instrumentAudioSources = new Dictionary<Instrument, AudioSource>();
		for (int i=0; i<(int)Instrument.NUM_INSTRUMENTS;i++) {
			AudioSource source = gameObject.AddComponent<AudioSource>();
			//instrumentAudioSources.Add((Instrument)i, new AudioSource());
			instrumentAudioSources.Add((Instrument)i, source);
		}
		instrumentAudioSources[Instrument.ElectricGuitar].volume = 0.6f;

		maxBeats = (int)Mathf.Pow(2f, (float)Riff.MAX_SUBDIVS+2);

		SetupExampleRiffs();
		SetupExampleLicks();
		//Debug.Log ("set up done" , licks.Count);


	}

	public void PlayRiffLoop(){
		if (loop) {
			StopLooping();
			instrumentAudioSources[InstrumentSetup.currentRiff.currentInstrument].Stop();
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
						if (loopSong) {
							Debug.Log(lickQueue.Count);
							beat = 0;
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

	public void AddRiff (Riff riff) {
		riffs.Add (riff);
		SongArrangeSetup.instance.Refresh();
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
			//Debug.Log("Loaded "+path);
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
			currentInstrument = Instrument.ElectricBass,
			notes = new List<List<Note>>() {
				new List<Note> () { new Note("bassguitarE3") },
				new List<Note> (),
				new List<Note> (),
				new List<Note> (),
				new List<Note> () { new Note("bassguitarG#3") },
				new List<Note> (),
				new List<Note> (),
				new List<Note> (),
				new List<Note> () { new Note("bassguitarB3") },
				new List<Note> (),
				new List<Note> (),
				new List<Note> (),
				new List<Note> () { new Note ("bassguitarC#4") },
				new List<Note> (),
				new List<Note> (),
				new List<Note> ()
			}
		});
		riffs.Add( new Riff () {
			name = "Example Drum Beat",
			currentInstrument = Instrument.RockDrums,
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
			currentInstrument = Instrument.ElectricGuitar,
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
			currentInstrument = Instrument.ElectricBass,
			notes = new List<List<Note>>() {
				new List<Note> () {new Note("bassguitarF#3") },
				new List<Note> () ,
				new List<Note> () ,
				new List<Note> () ,
				new List<Note> () {new Note("bassguitarF#3") },
				new List<Note> () ,
				new List<Note> () {new Note("bassguitarA3") },
				new List<Note> () ,
				new List<Note> () ,
				new List<Note> () ,
				new List<Note> () ,
				new List<Note> () ,
				new List<Note> () {new Note("bassguitarC#4") },
				new List<Note> () ,
				new List<Note> () ,
				new List<Note> ()
			}
		});

		licks[Instrument.RockDrums].Add (new Riff () {
			name = "Example Drums Lick",
			currentInstrument = Instrument.RockDrums,
			notes = new List<List<Note>>() {
				new List<Note> () {new Note() { sound = Sounds["RockDrums_Snare"] }},
				new List<Note> () {new Note() { sound = Sounds["RockDrums_Snare"] }},
				new List<Note> () ,
				new List<Note> () ,
				new List<Note> () { new Note() { sound = Sounds["RockDrums_Hat"] }},
			}
		});
	}
}
	