using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;// need for using lists
using System.IO; // need for path operations

#if UNITY_EDITOR
using UnityEditor;
#endif

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
	Eminor,
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
	public static Dictionary<string, AudioClip> SoundClips = new Dictionary<string, AudioClip>(); // holds all loaded sounds
	public List<Riff> riffs = new List<Riff> ();
	public List<SongPiece> songPieces = new List<SongPiece>();
	public Dictionary<string, SongPiece> songPiecesByName = new Dictionary<string, SongPiece>();
	public Dictionary<Instrument, List<Riff>> licks = new Dictionary<Instrument, List<Riff>>() {
		{ Instrument.ElectricBass, new List <Riff> () },
		{ Instrument.ElectricGuitar, new List <Riff> () },
		{ Instrument.RockDrums, new List <Riff> () }
	};
	// All lick notes waiting to be played
	List<List<Note>> lickQueue = new List<List<Note>>();
	bool lickPlaying = false;


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

	float startLoadTime;

	bool loadedExamples = false;

	Instrument lickInstrument;

	void Start () {
		if (instance) Debug.LogError("More than one MusicManager exists!");
		else instance = this;

		currentSong = new Song ();

		maxBeats = (int)Mathf.Pow(2f, (float)Riff.MAX_SUBDIVS+2);
	}

	public void Load() {
		startLoadTime = Time.realtimeSinceStartup;
		LoadSounds();
		LoadInstruments();
		LoadScales();
		//LoadExampleRiffs();
		//LoadExampleLicks();
		Debug.Log("MusicManager.Load(): finished in "+(Time.realtimeSinceStartup-startLoadTime).ToString("0.0000")+" seconds.");
		GameManager.instance.LoadNext();
	}

	// Loads all audio clip paths in soundsToLoad
	void LoadSounds() {
		GameManager.instance.ChangeLoadingMessage("Loading sounds...");
		//foreach (string path in paths) {
		foreach (KeyValuePair<string, List<string>> list in Sounds.soundsToLoad) {
			foreach (string path in list.Value) {
				LoadAudioClip(path);
			}
		}
	}

	void LoadInstruments () {
		GameManager.instance.ChangeLoadingMessage("Loading instruments...");

		instrumentAudioSources = new Dictionary<Instrument, AudioSource>();
		for (int i=0; i<(int)Instrument.NUM_INSTRUMENTS; i++) {
			GameObject obj = new GameObject ();
			obj.name = (string)Enum.GetName (typeof(Instrument), (Instrument)i);
			AudioSource source = obj.AddComponent<AudioSource>();
			instrumentAudioSources.Add((Instrument)i, source);
			obj.AddComponent<AudioReverbFilter>();
			obj.GetComponent<AudioReverbFilter>().dryLevel = GetComponent<AudioReverbFilter>().dryLevel;
			obj.GetComponent<AudioReverbFilter>().room = GetComponent<AudioReverbFilter>().room;
			obj.GetComponent<AudioReverbFilter>().roomHF = GetComponent<AudioReverbFilter>().roomHF;
			obj.GetComponent<AudioReverbFilter>().roomLF = GetComponent<AudioReverbFilter>().roomLF;
			obj.GetComponent<AudioReverbFilter>().decayTime = GetComponent<AudioReverbFilter>().decayTime;
			obj.GetComponent<AudioReverbFilter>().decayHFRatio = GetComponent<AudioReverbFilter>().decayHFRatio;
			obj.GetComponent<AudioReverbFilter>().reflectionsLevel = GetComponent<AudioReverbFilter>().reflectionsLevel;
			obj.GetComponent<AudioReverbFilter>().reflectionsDelay = GetComponent<AudioReverbFilter>().reflectionsDelay;
			obj.GetComponent<AudioReverbFilter>().reverbLevel = GetComponent<AudioReverbFilter>().reverbLevel;
			obj.GetComponent<AudioReverbFilter>().hfReference = GetComponent<AudioReverbFilter>().hfReference;
			obj.GetComponent<AudioReverbFilter>().lfReference = GetComponent<AudioReverbFilter>().lfReference;
			obj.GetComponent<AudioReverbFilter>().diffusion = GetComponent<AudioReverbFilter>().diffusion;
			obj.GetComponent<AudioReverbFilter>().density = GetComponent<AudioReverbFilter>().density;
			GameManager.instance.IncrementLoadProgress();
		}
	}

	void LoadScales () {
		GameManager.instance.ChangeLoadingMessage("Loading scales...");
		KeyManager.instance.BuildScales();
	}

	public void SetKey (int key) {
		currentKey = (Key)key;
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
		instrumentAudioSources[InstrumentSetup.currentRiff.instrument].Stop();
		//OneShot.Stop();
	}

	void FixedUpdate(){
		if (playing && !GameManager.instance.paused) {
			if (BeatTimer <= 0f) {
				switch (GameManager.instance.currentMode) {
				case Mode.Setup:
					InstrumentSetup.currentRiff.PlayRiff (beat++);
					if (beat >= InstrumentSetup.currentRiff.beatsShown*(int)Mathf.Pow(2f,Riff.MAX_SUBDIVS) && loop)
						beat = 0;
					break;
				case Mode.Live:
					

					//Debug.Log(lickQueue.Count);


					//Debug.Log("shit");
					if (beat >= currentSong.beats) {
						//Debug.Log("shit");
						beat = 0;
						if (loopSong) {
							//Debug.Log(lickQueue.Count);
						} else {
							GameManager.instance.SwitchToPostplay();
						}
					}
					if (lickQueue.Count > 0){
						if (IsDownBeat(beat)) {
							//Debug.Log("dick");
							lickPlaying = true;
						}
						if (lickPlaying) {
							PopLickQueue();
						}
					} else {
						lickPlaying = false;
					}
					if (!lickPlaying) currentSong.PlaySong(beat);
						else currentSong.PlaySongExceptFor(beat, lickInstrument);
					//Debug.Log(beat);
					float songTotalTime = currentSong.beats*3600f / (float) (maxBeats/4) / tempo;
					float songCurrentTime = (beat*3600f / (float) (maxBeats/4) / tempo) + (3600f / (float) (maxBeats/4) / tempo)-BeatTimer;
					GameManager.instance.songProgressBar.GetComponent<SongProgressBar>().SetValue(songCurrentTime/songTotalTime);
					beat++;
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
		//Debug.Log("added");
		Riff temp = new Riff ();
		InstrumentSetup.currentRiff = temp;
		SongArrangeSetup.instance.selectedRiffIndex = riffs.Count;
		riffs.Add (temp);
		SongArrangeSetup.instance.Refresh();
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
		//Debug.Log("Adding "+songPiece.name);
		songPieces.Add(songPiece);
		songPiecesByName.Add(songPiece.name, songPiece);
		//Debug.Log("Loaded songpiece "+songPiece.name);
		//SongArrangeSetup.instance.Refresh();
	}

	public void AddSongPieceToSong () {
		SongPiece temp = new SongPiece() {
			name = "SongPiece"+songPieces.Count
		};
		songPieces.Add(temp);
		currentSong.songPieces.Add(temp);
		songPiecesByName.Add(temp.name, temp);
	}

	public void AddSongPieceToSong (SongPiece songPiece) {
		AddSongPiece(songPiece);
		currentSong.songPieces.Add(songPiece);
	}

	public void QueueLick (Riff lick) {
		if (lick == null || lickQueue.Count != 0) return;
		lickQueue.Clear();
		lickInstrument = lick.instrument;
		foreach (List<Note> beat in lick.notes) {
			lickQueue.Add(beat);
			//Debug.Log ("test");
		}
		lickPlaying = false;
		//Debug.Log("queued "+lickQueue.Count);
	}

	/*public void QueueLick (Riff lick) {
		if (lick == null) return;
		int nextDownBeat = (beat % 4 == 0 ? beat :
			beat+1 % 4 == 0 ? beat+1 :
			beat+2 % 4 == 0 ? beat+1 :
			beat+3 % 4 == 0 ? beat+3 :
		);
		for (int i=nextDownBeat) 
	}*/

	bool IsDownBeat(int pos) {
		return pos%4 == 0;
	}

	public void PopLickQueue () {
		//Debug.Log("play");
		//MusicManager.instance.currentSong.RemoveAt(beat, currentInstrument);
		foreach (Note note in lickQueue[0]) {
			note.PlayNote(instrumentAudioSources[currentInstrument], true);
			InstrumentDisplay.instance.WakeGlow();
		}
		lickQueue.RemoveAt(0);

	}

	// Plays a single sound effect through OneShot AudioSource
	public void PlayOneShot (AudioClip clip) {
		OneShot.Stop();
		OneShot.clip = clip;
		OneShot.Play();
	}

	// Loads a single audio clip
	void LoadAudioClip (string path) {
		AudioClip sound = (AudioClip) Resources.Load (path);
		if (sound == null) {
			Debug.LogError("Failed to load AudioClip at "+path);
		} else {
			Debug.Log("Loaded "+path);
			//SoundClips.Add (Path.GetFileNameWithoutExtension (path), sound);
			SoundClips.Add (path, sound);
			GameManager.instance.IncrementLoadProgress();
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
		beat = 0;
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
		songPiecesByName.Clear();
	}
		
	public void LoadExampleRiffs() {
		if (!loadedExamples) {
			riffs.Add( new Riff () {
				name = "Example Guitar Riff",
				instrument = Instrument.ElectricGuitar,
				notes = new List<List<Note>>() {
					new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricGuitar].root[0]) },
					new List<Note> (),
					new List<Note> (),
					new List<Note> (),
					new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricGuitar].fifth[0]) },
					new List<Note> (),
					new List<Note> (),
					new List<Note> (),
					new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricGuitar].seventh[0])},
					new List<Note> (),
					new List<Note> (),
					new List<Note> (),
					new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricGuitar].fourth[0])},
					new List<Note> (),
					new List<Note> (),
					new List<Note> ()
				}
			});
			riffs.Add( new Riff () {
				name = "Example Bass Riff",
				instrument = Instrument.ElectricBass,
				notes = new List<List<Note>>() {
				new List<Note> () { new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricBass].root[0]) },
					new List<Note> (),
					new List<Note> (),
					new List<Note> (),
				new List<Note> () { new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricBass].fifth[0]) },
					new List<Note> (),
					new List<Note> (),
					new List<Note> (),
				new List<Note> () { new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricBass].seventh[0]) },
					new List<Note> (),
					new List<Note> (),
					new List<Note> (),
				new List<Note> () { new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricBass].fourth[0]) },
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
					new List<Note> () { new Note("Audio/Instruments/Percussion/RockDrums_Kick") },
					new List<Note> (),
					new List<Note> (),
					new List<Note> (),
					new List<Note> () { new Note("Audio/Instruments/Percussion/RockDrums_Hat") },
					new List<Note> (),
					new List<Note> (),
					new List<Note> (),
					new List<Note> () { new Note("Audio/Instruments/Percussion/RockDrums_Snare") },
					new List<Note> (),
					new List<Note> (),
					new List<Note> (),
					new List<Note> () { new Note ("Audio/Instruments/Percussion/RockDrums_Hat") },
					new List<Note> (),
					new List<Note> (),
					new List<Note> ()
				}
			});
			SongArrangeSetup.instance.Refresh();
		}
	}
		
	public void LoadExampleLicks() {
		if (!loadedExamples) {
			licks[Instrument.ElectricGuitar].Add (new Riff () {
				name = "Example Guitar Lick",
				instrument = Instrument.ElectricGuitar,
				notes = new List<List<Note>>() {
					new List<Note> () { new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricGuitar].root[0]) },
					new List<Note> () ,
					new List<Note> () ,
					new List<Note> () ,
					new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricGuitar].fourth[0]) },
					new List<Note> () ,
					new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricGuitar].fifth[0])},
					new List<Note> ()
				}
			});

			licks[Instrument.ElectricBass].Add (new Riff () {
				name = "Example Bass Lick",
				instrument = Instrument.ElectricBass,
				notes = new List<List<Note>>() {
					new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricBass].root[0]) },
					new List<Note> () ,
					new List<Note> () ,
					new List<Note> () ,
					new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricBass].root[0]) },
					new List<Note> () ,
					new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricBass].third[0]) },
					new List<Note> () ,
					new List<Note> () ,
					new List<Note> () ,
					new List<Note> () ,
					new List<Note> () ,
					new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricBass].fifth[0]) },
					new List<Note> () ,
					new List<Note> () ,
					new List<Note> ()
				}
			});

			licks[Instrument.RockDrums].Add (new Riff () {
				name = "Example Drums Lick",
				instrument = Instrument.RockDrums,
				notes = new List<List<Note>>() {
					new List<Note> () {new Note("Audio/Instruments/Percussion/RockDrums_Snare")},
					new List<Note> () {new Note("Audio/Instruments/Percussion/RockDrums_Snare")},
					new List<Note> () {new Note("Audio/Instruments/Percussion/RockDrums_Tom")},
					new List<Note> () {new Note("Audio/Instruments/Percussion/RockDrums_Snare")},
					new List<Note> () {new Note("Audio/Instruments/Percussion/RockDrums_Snare")},
					new List<Note> () {new Note("Audio/Instruments/Percussion/RockDrums_Tom")},
					new List<Note> () { new Note("Audio/Instruments/Percussion/RockDrums_Kick")},
					new List<Note>()
				}
			});
			loadedExamples = true;
		}
	}
}
	