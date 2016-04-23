using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System;
using System.Collections;
using System.Collections.Generic;// need for using lists
using System.IO; // need for path operations

#if UNITY_EDITOR
using UnityEditor;
#endif

// All keys available in the game
public enum Key {
	None,
	C,
	CSharp,
	D,
	DSharp,
	E,
	F,
	FSharp,
	G,
	GSharp,
	A,
	ASharp,
	B
};

public enum Tempo {
	Slowest,
	Slow,
	Medium,
	Fast,
	Fastest,
	NUM_TEMPOS
};

public class MusicManager : MonoBehaviour {
	public static MusicManager instance; // access this MusicManager from anywhere using MusicManager.instance
	public AudioMixer mixer;

	public Project currentProject = new Project();

	// --Global Music Properties-- //
	public Instrument currentInstrument = MelodicInstrument.ElectricGuitar;
	public Song currentSong;
	public int currentPlayingSong;
	public bool loopSong = false; // loop song in live mode?

	// --Game Data Storage --//
	public static Dictionary<string, AudioClip> SoundClips = new Dictionary<string, AudioClip>(); // holds all loaded sounds
	public AudioSource OneShot; // used for playing one-shot sound effects (UI, etc.)
	public AudioSource LoopRiff;
	public Dictionary<Instrument, AudioSource> instrumentAudioSources;

	//public static float tempo = 120f; // tempo in BPM
	public Tempo tempo;
	private float BeatTimer;
	private int beat;
	public static bool playing = false;
	public static bool loop = false;

	public static Dictionary<Tempo, float> tempoToFloat = new Dictionary<Tempo, float> () {
		{ Tempo.Slowest, 70f },
		{ Tempo.Slow, 90f },
		{ Tempo.Medium, 110f },
		{ Tempo.Fast, 130f },
		{ Tempo.Fastest, 150f }
	};

	public int maxBeats; // max beats in riff, after subdivisions

	float startLoadTime;

	//bool loadedExamples = false;

	//Instrument lickInstrument;

	void Start () {
		if (instance) Debug.LogError("More than one MusicManager exists!");
		else instance = this;

		currentSong = new Song ();

		maxBeats = (int)Mathf.Pow(2f, (float)Riff.MAX_SUBDIVS+2);

		tempo = Tempo.Medium;
		Instrument.LoadInstruments ();

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

	void getAudioEffect () {
		//currentInstrument = InstrumentSetup.currentRiff.instrument;
		currentInstrument = Instrument.AllInstruments[InstrumentSetup.currentRiff.instrumentIndex];
		Debug.Log ("Calling getAudioEffect " + currentInstrument);
		instrumentAudioSources[currentInstrument].gameObject.GetComponent<AudioDistortionFilter>().distortionLevel = InstrumentSetup.currentRiff.distortionLevel;
		//instrumentAudioSources[currentInstrument].gameObject.GetComponent<AudioEchoFilter>().decayRatio = InstrumentSetup.currentRiff.echoDecayRatio;
		//instrumentAudioSources[currentInstrument].gameObject.GetComponent<AudioEchoFilter>().delay = InstrumentSetup.currentRiff.echoDelay;
	}

	void LoadInstruments () {
		GameManager.instance.ChangeLoadingMessage("Loading instruments...");



		instrumentAudioSources = new Dictionary<Instrument, AudioSource>();
		for (int i=0; i<Instrument.AllInstruments.Count; i++) {
			GameObject obj = new GameObject ();
			obj.name = Instrument.AllInstruments[i].name;
			AudioSource source = obj.AddComponent<AudioSource>();
			source.outputAudioMixerGroup = mixer.FindMatchingGroups (obj.name) [0];
			instrumentAudioSources.Add(Instrument.AllInstruments[i], source);
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
			obj.AddComponent<AudioDistortionFilter> ();
			obj.AddComponent<AudioEchoFilter> ();
			obj.AddComponent<AudioChorusFilter> ();

			GameManager.instance.IncrementLoadProgress();


		}
		//instrumentAudioSources[MelodicInstrument.ElectricGuitar].volume = 0.25f;
		instrumentAudioSources[MelodicInstrument.ElectricGuitar].gameObject.AddComponent<AudioDistortionFilter>();
		//instrumentAudioSources[MelodicInstrument.ElectricGuitar].gameObject.GetComponent<AudioDistortionFilter>().distortionLevel = 0.9f;
	}

	void LoadScales () {
		GameManager.instance.ChangeLoadingMessage("Loading scales...");
		KeyManager.instance.BuildScales();
	}

	public void NewProject () {
		currentProject = new Project();
	}

	public void SaveCurrentProject () {
		SaveLoad.SaveCurrentProject();
	}

	public void NewSong () {
		/*if (currentProject.Full()) {
			Prompt.instance.PromptMessage ("Project full", "The current project has reached the max number of songs.", "Bummer.");
		} else {*/
		Song newSong = new Song();
		currentSong = newSong;
		currentProject.AddSong(newSong);
		//}
	}

	public void SaveCurrentSong () {
		SaveLoad.SaveCurrentSong();
	}

	public void SetKey (int key) {
		currentSong.key = (Key)key;
		//LoadExampleLicks();
	}

	public void SetKey (Key key) {
		SetKey ((int)key);
		//LoadExampleLicks();
	}

	public void PlayRiffLoop(){
		if (loop) {
			StopLooping();
			instrumentAudioSources[Instrument.AllInstruments[InstrumentSetup.currentRiff.instrumentIndex]].Stop();
		} else {
			playing = true;
			loop = true;
		}
	}

	public void StopLooping () {
		playing = false;
		loop = false;
		beat = 0;
		instrumentAudioSources[Instrument.AllInstruments[InstrumentSetup.currentRiff.instrumentIndex]].Stop();
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
							if (currentPlayingSong < currentProject.songs.Count-1) {
								beat = 0;
								currentPlayingSong++;
							} else {
								GameManager.instance.SwitchToPostplay();
							}
						}
					}
					/*if (lickQueue.Count > 0){
						if (IsDownBeat(beat)) {
							//Debug.Log("dick");
							lickPlaying = true;
						}
						if (lickPlaying) {
							PopLickQueue();
						}
					} else {
						lickPlaying = false;
					}*/
					//if (!lickPlaying) 
					currentSong.PlaySong(beat);
					//	else currentSong.PlaySongExceptFor(beat, lickInstrument);
					//Debug.Log(beat);
					float songTotalTime = currentSong.beats*7200f/tempoToFloat[tempo]/4f;
					float songCurrentTime = (beat*7200f/tempoToFloat[tempo]/4f) + (7200f/tempoToFloat[tempo]/4f)-BeatTimer;
					GameManager.instance.songProgressBar.GetComponent<SongProgressBar>().SetValue(songCurrentTime/songTotalTime);
					beat++;
					break;
				}
				BeatTimer = 7200f / tempoToFloat[tempo] /4f;// 3600f = 60 fps * 60 seconds 

			} else {
				//BeatTimer--;
				//BeatTimer -= Time.deltaTime * 100f;
				BeatTimer -= 1.667f;
			}
		} 

	}

	public void IncreaseTempo () {
		if ((int)tempo < (int)Tempo.NUM_TEMPOS-1) {
			tempo = (Tempo)((int)tempo+1);
			InstrumentSetup.instance.UpdateTempoText();
		}
	}

	public void DecreaseTempo () {
		if ((int)tempo > 0) {
			tempo = (Tempo)((int)tempo-1);
			InstrumentSetup.instance.UpdateTempoText();
		}
	}


	// Adds a new riff
	public Riff AddRiff () {
		//Debug.Log("added");
		Riff temp = new Riff ();
		InstrumentSetup.currentRiff = temp;
		SongArrangeSetup.instance.selectedRiffIndex = currentProject.riffs.Count;
		currentProject.riffs.Add (temp);
		SongArrangeSetup.instance.Refresh();
		getAudioEffect ();
		return temp;
	}

	public void AddRiff (Riff riff) {
		riff.copy = CopyNumber(riff.name);
		currentProject.riffs.Add (riff);
		//SongArrangeSetup.instance.Refresh();
	}

	public Riff RiffByString (string riffName) {
		foreach (Riff riff in currentProject.riffs) {
			if (riffName == riff.name) return riff;
		}
		return null;
	}

	/*public void AddSongPiece (SongPiece songPiece) {
		//Debug.Log("Adding "+songPiece.name);
		songPieces.Add(songPiece);
		songPiecesByName.Add(songPiece.name, songPiece);
		//Debug.Log("Loaded songpiece "+songPiece.name);
		//SongArrangeSetup.instance.Refresh();
	}*/

	/*public void AddSongPieceToSong () {
	SongPiece temp = new SongPiece() {
		name = "SongPiece"+songPieces.Count
	};
	songPieces.Add(temp);
	currentSong.songPieces.Add(temp);
	songPiecesByName.Add(temp.name, temp);
}*/

/*public void AddSongPieceToSong (SongPiece songPiece) {
AddSongPiece(songPiece);
currentSong.songPieces.Add(songPiece);
}*/

/*public void QueueLick (Riff lick) {
if (lick == null || lickQueue.Count != 0) return;
lickQueue.Clear();
lickInstrument = lick.instrument;
foreach (Beat beat in lick.beats) {
	lickQueue.Add(beat.notes);
}
lickPlaying = false;
}*/

/*bool IsDownBeat(int pos) {
return pos%4 == 0;
}*/

/*public void PopLickQueue () {
//Debug.Log("play");
//MusicManager.instance.currentSong.RemoveAt(beat, currentInstrument);
foreach (Note note in lickQueue[0]) {
	note.volume = 1.25f;
	note.PlayNote(instrumentAudioSources[currentInstrument], true);
	InstrumentDisplay.instance.WakeGlow();
}
lickQueue.RemoveAt(0);
}*/

// Loads a single audio clip
void LoadAudioClip (string path) {
	AudioClip sound = (AudioClip) Resources.Load (path);
	if (sound == null) {
		Debug.LogError("Failed to load AudioClip at "+path);
	} else {
		//Debug.Log("Loaded "+path);
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
	foreach (Riff riff in currentProject.riffs) {
		if (riff.name == riffName) return true;
	}
	return false;
}
public int CopyNumber (string riffName) {
	Debug.Log("CopyNumber(): "+riffName);
	int result = 0;
	foreach (Riff riff in currentProject.riffs) {
		if (riff.name == riffName) {
			result++;
		}
	}
	return result;
}

// Clears song, songpiece, and riff data
public void Clear () {
	//currentProject.riffs.Clear();
	//songPieces.Clear();
	//songPiecesByName.Clear();
}

/*public void LoadExampleRiffs() {
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
				name = "Test",
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
					new List<Note> (),
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
			loadedExamples = true;
			SongArrangeSetup.instance.Refresh();
		}
	}
		
	public void LoadExampleLicks() {
		foreach (Instrument inst in licks.Keys) {
			licks[inst].Clear();
		}
			licks[Instrument.ElectricGuitar].Add (new Riff () {
				name = "Example Guitar Lick",
				instrument = Instrument.ElectricGuitar,
				notes = new List<List<Note>>() {
					new List<Note> () { new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricGuitar].root[0]) },
					new List<Note> () ,
					//new List<Note> () ,
					//new List<Note> () ,
					new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricGuitar].fourth[0]) },
					//new List<Note> () ,
					new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricGuitar].fifth[0])},
					//new List<Note> ()
				}
			});
			licks[Instrument.ElectricGuitar].Add (new Riff () {
				name = "Example Guitar Lick2",
				instrument = Instrument.ElectricGuitar,
				notes = new List<List<Note>>() {
					new List<Note> () { new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricGuitar].third[0]) },
					new List<Note> () ,
					new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricGuitar].third[0]) },
					new List<Note> () ,
					new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricGuitar].fifth[0]) },
					new List<Note> () ,
					new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricGuitar].fifth[0]) },
					new List<Note> () { new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricGuitar].root[0]) },
					new List<Note> ()
				}
			});
		licks[Instrument.ElectricGuitar].Add (new Riff () {
			name = "Example Guitar Lick3",
			instrument = Instrument.ElectricGuitar,
			notes = new List<List<Note>>() {
				new List<Note> () { new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricGuitar].seventh[0]) },
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricGuitar].root[1]) },
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricGuitar].seventh[0]) },
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricGuitar].root[1]) },
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricGuitar].root[0]) },
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricGuitar].root[0]) },
				new List<Note> () { new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricGuitar].seventh[0]) },
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricGuitar].root[1]) }
			}
		});
		licks[Instrument.ElectricGuitar].Add (new Riff () {
			name = "Example Guitar Lick4",
			instrument = Instrument.ElectricGuitar,
			notes = new List<List<Note>>() {
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricGuitar].root[0]) },
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricGuitar].third[0]) },
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricGuitar].fourth[0]) },
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricGuitar].sixth[0]) },
				new List<Note>(),
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricGuitar].sixth[0]) },
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricGuitar].sixth[0]) },
				new List<Note> () { new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricGuitar].sixth[0]) },
				new List<Note> (),
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricGuitar].sixth[0]) },
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricGuitar].sixth[0]) },
				new List<Note> () { new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricGuitar].sixth[0]) },
				new List<Note> (),
				new List<Note> () { new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricGuitar].sixth[0]) },
				new List<Note> () { new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricGuitar].third[0]) },
				new List<Note> () { new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricGuitar].root[0]) }
			}
		});
		licks[Instrument.ElectricGuitar].Add (new Riff () {
			name = "Example Guitar Lick5",
			instrument = Instrument.ElectricGuitar,
			notes = new List<List<Note>>() {
				new List<Note> () { new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricGuitar].second[0]) },
				new List<Note> (),
				new List<Note> (){ new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricGuitar].second[0]) },
				new List<Note> () { new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricGuitar].fourth[0]) },
				new List<Note> () ,
				new List<Note> () ,
				new List<Note> () { new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricGuitar].root[0]) },
				new List<Note> () 
			}
		});
			licks[Instrument.ElectricBass].Add (new Riff () {
				name = "Example Bass Lick",
				instrument = Instrument.ElectricBass,
				notes = new List<List<Note>>() {
					new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricBass].root[0]) },
					new List<Note> () ,
					new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricBass].root[0]) },
					new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricBass].third[0]) },
					new List<Note> () ,
					new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricBass].third[0]) },
					new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricBass].root[0]) },
					new List<Note> (),
					new List<Note> () ,
					new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricBass].fifth[0]) },
					new List<Note> () 
				}
			});
			licks[Instrument.ElectricBass].Add (new Riff () {
				name = "Example Bass Lick2",
				instrument = Instrument.ElectricBass,
				notes = new List<List<Note>>() {
					new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricBass].root[1]) },
					new List<Note> () ,
					new List<Note> () ,
					new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricBass].sixth[0]) },
					new List<Note> () ,
					new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricBass].sixth[0]) },
					new List<Note> () ,
					new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricBass].sixth[0]) },
					new List<Note> () ,
					new List<Note> () ,
					new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricBass].sixth[0]) },
					new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricBass].third[0]) }
				}
			});
		licks[Instrument.ElectricBass].Add (new Riff () {
			name = "Example Bass Lick3",
			instrument = Instrument.ElectricBass,
			notes = new List<List<Note>>() {
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricBass].sixth[0]) },
				new List<Note> () ,
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricBass].sixth[0]) },
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricBass].sixth[0]) },
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricBass].fourth[0]) },
				new List<Note> () ,
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricBass].fourth[0]) },
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricBass].fourth[0]) },
			}
		});
		licks[Instrument.ElectricBass].Add (new Riff () {
			name = "Example Bass Lick4",
			instrument = Instrument.ElectricBass,
			notes = new List<List<Note>>() {
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricBass].fourth[0]) },
				new List<Note> () ,
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricBass].fourth[0]) },
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricBass].sixth[0]) },
				new List<Note> () ,
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricBass].seventh[0]) },
				new List<Note> () ,
				new List<Note> () 
			}
		});
		licks[Instrument.ElectricBass].Add (new Riff () {
			name = "Example Bass Lick5",
			instrument = Instrument.ElectricBass,
			notes = new List<List<Note>>() {
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricBass].root[0]) },
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricBass].root[0]) },
				new List<Note> () ,
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricBass].fifth[0]) },
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricBass].fifth[0]) },
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricBass].fifth[0]) },
				new List<Note> () ,
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricBass].third[0]) },
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricBass].third[0]) },
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricBass].third[0]) },
				new List<Note> (),
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ElectricBass].root[0]) }
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
			licks[Instrument.RockDrums].Add (new Riff () {
				name = "Example Drums Lick2",
				instrument = Instrument.RockDrums,
				notes = new List<List<Note>>() {
					new List<Note> () {new Note("Audio/Instruments/Percussion/RockDrums_Snare")},
					new List<Note> (){new Note("Audio/Instruments/Percussion/RockDrums_Snare")},
					new List<Note> ()  ,
					new List<Note> () {new Note("Audio/Instruments/Percussion/RockDrums_Tom")},
					new List<Note> () {new Note("Audio/Instruments/Percussion/RockDrums_Tom")},
					new List<Note> () ,
					new List<Note> () ,
					new List<Note>(){ new Note("Audio/Instruments/Percussion/RockDrums_Kick")},
					new List<Note>(){ new Note("Audio/Instruments/Percussion/RockDrums_Hat")},
					new List<Note> () 
				}
			});
		licks[Instrument.RockDrums].Add (new Riff () {
			name = "Example Drums Lick3",
			instrument = Instrument.RockDrums,
			notes = new List<List<Note>>() {
				new List<Note> () {new Note("Audio/Instruments/Percussion/RockDrums_Kick")},
				new List<Note> (){new Note("Audio/Instruments/Percussion/RockDrums_Hat")},
				new List<Note> () {new Note("Audio/Instruments/Percussion/RockDrums_Hat")} ,
				new List<Note> () {new Note("Audio/Instruments/Percussion/RockDrums_Hat")},
				new List<Note> () {new Note("Audio/Instruments/Percussion/RockDrums_Snare")},
				new List<Note> () {new Note("Audio/Instruments/Percussion/RockDrums_Hat")},
				new List<Note> () {new Note("Audio/Instruments/Percussion/RockDrums_Kick")},
				new List<Note>(){ new Note("Audio/Instruments/Percussion/RockDrums_Hat")}
			}
		});
		licks[Instrument.RockDrums].Add (new Riff () {
			name = "Example Drums Lick 4",
			instrument = Instrument.RockDrums,
			notes = new List<List<Note>>() {
				new List<Note> () {new Note("Audio/Instruments/Percussion/RockDrums_Kick")},
				new List<Note> (){new Note("Audio/Instruments/Percussion/RockDrums_Kick")},
				new List<Note> (),
				new List<Note> () {new Note("Audio/Instruments/Percussion/RockDrums_Kick")},
				new List<Note> () {new Note("Audio/Instruments/Percussion/RockDrums_Snare")},
				new List<Note> () {new Note("Audio/Instruments/Percussion/RockDrums_Kick")},
				new List<Note> () {new Note("Audio/Instruments/Percussion/RockDrums_Kick")},
				new List<Note>(){ new Note("Audio/Instruments/Percussion/RockDrums_Hat")}
			}
		});
		licks[Instrument.RockDrums].Add (new Riff () {
			name = "Example Drums Lick 5",
			instrument = Instrument.RockDrums,
			notes = new List<List<Note>>() {
				new List<Note> () {new Note("Audio/Instruments/Percussion/RockDrums_Kick")},
				new List<Note> (){new Note("Audio/Instruments/Percussion/RockDrums_Hat")},
				new List<Note> (){new Note("Audio/Instruments/Percussion/RockDrums_Hat")},
				new List<Note> () {new Note("Audio/Instruments/Percussion/RockDrums_Snare")},
				new List<Note> () {new Note("Audio/Instruments/Percussion/RockDrums_Hat")},
				new List<Note> () {new Note("Audio/Instruments/Percussion/RockDrums_Hat")},
				new List<Note> () {new Note("Audio/Instruments/Percussion/RockDrums_Hat")},
				new List<Note>(){ new Note("Audio/Instruments/Percussion/RockDrums_Kick")}
			}
		});
		licks[Instrument.AcousticGuitar].Add (new Riff () {
			name = "Example Guitar Lick",
			instrument = Instrument.AcousticGuitar,
			notes = new List<List<Note>>() {
				new List<Note> () { new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.AcousticGuitar].root[0]) },
				new List<Note> () ,
				//new List<Note> () ,
				//new List<Note> () ,
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.AcousticGuitar].fourth[0]) },
				//new List<Note> () ,
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.AcousticGuitar].fifth[0])},
				//new List<Note> ()
			}
		});
		licks[Instrument.AcousticGuitar].Add (new Riff () {
			name = "Example Guitar Lick2",
			instrument = Instrument.AcousticGuitar,
			notes = new List<List<Note>>() {
				new List<Note> () { new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.AcousticGuitar].third[0]) },
				new List<Note> () ,
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.AcousticGuitar].third[0]) },
				new List<Note> () ,
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.AcousticGuitar].fifth[0]) },
				new List<Note> () ,
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.AcousticGuitar].fifth[0]) },
				new List<Note> () { new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.AcousticGuitar].root[0]) },
				new List<Note> ()
			}
		});
		licks[Instrument.AcousticGuitar].Add (new Riff () {
			name = "Example Guitar Lick3",
			instrument = Instrument.AcousticGuitar,
			notes = new List<List<Note>>() {
				new List<Note> () { new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.AcousticGuitar].seventh[0]) },
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.AcousticGuitar].root[1]) },
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.AcousticGuitar].seventh[0]) },
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.AcousticGuitar].root[1]) },
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.AcousticGuitar].root[0]) },
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.AcousticGuitar].root[0]) },
				new List<Note> () { new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.AcousticGuitar].seventh[0]) },
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.AcousticGuitar].root[1]) }
			}
		});
		licks[Instrument.ClassicalGuitar].Add (new Riff () {
			name = "Example Guitar Lick",
			instrument = Instrument.ClassicalGuitar,
			notes = new List<List<Note>>() {
				new List<Note> () { new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ClassicalGuitar].root[0]) },
				new List<Note> () ,
				//new List<Note> () ,
				//new List<Note> () ,
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ClassicalGuitar].fourth[0]) },
				//new List<Note> () ,
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ClassicalGuitar].fifth[0])},
				//new List<Note> ()
			}
		});
		licks[Instrument.ClassicalGuitar].Add (new Riff () {
			name = "Example Guitar Lick2",
			instrument = Instrument.ClassicalGuitar,
			notes = new List<List<Note>>() {
				new List<Note> () { new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ClassicalGuitar].third[0]) },
				new List<Note> () ,
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ClassicalGuitar].third[0]) },
				new List<Note> () ,
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ClassicalGuitar].fifth[0]) },
				new List<Note> () ,
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ClassicalGuitar].fifth[0]) },
				new List<Note> () { new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ClassicalGuitar].root[0]) },
				new List<Note> ()
			}
		});
		licks[Instrument.ClassicalGuitar].Add (new Riff () {
			name = "Example Guitar Lick3",
			instrument = Instrument.ClassicalGuitar,
			notes = new List<List<Note>>() {
				new List<Note> () { new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ClassicalGuitar].seventh[0]) },
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ClassicalGuitar].root[1]) },
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ClassicalGuitar].seventh[0]) },
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ClassicalGuitar].root[1]) },
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ClassicalGuitar].root[0]) },
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ClassicalGuitar].root[0]) },
				new List<Note> () { new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ClassicalGuitar].seventh[0]) },
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.ClassicalGuitar].root[1]) }
			}
		});
		licks[Instrument.PipeOrgan].Add (new Riff () {
			name = "Example Guitar Lick",
			instrument = Instrument.PipeOrgan,
			notes = new List<List<Note>>() {
				new List<Note> () { new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.PipeOrgan].root[0]) },
				new List<Note> () ,
				//new List<Note> () ,
				//new List<Note> () ,
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.PipeOrgan].fourth[0]) },
				//new List<Note> () ,
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.PipeOrgan].fifth[0])},
				//new List<Note> ()
			}
		});
		licks[Instrument.PipeOrgan].Add (new Riff () {
			name = "Example Guitar Lick2",
			instrument = Instrument.PipeOrgan,
			notes = new List<List<Note>>() {
				new List<Note> () { new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.PipeOrgan].third[0]) },
				new List<Note> () ,
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.PipeOrgan].third[0]) },
				new List<Note> () ,
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.PipeOrgan].fifth[0]) },
				new List<Note> () ,
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.PipeOrgan].fifth[0]) },
				new List<Note> () { new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.PipeOrgan].root[0]) },
				new List<Note> ()
			}
		});
		licks[Instrument.PipeOrgan].Add (new Riff () {
			name = "Example Guitar Lick3",
			instrument = Instrument.PipeOrgan,
			notes = new List<List<Note>>() {
				new List<Note> () { new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.PipeOrgan].seventh[0]) },
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.PipeOrgan].root[1]) },
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.PipeOrgan].seventh[0]) },
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.PipeOrgan].root[1]) },
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.PipeOrgan].root[0]) },
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.PipeOrgan].root[0]) },
				new List<Note> () { new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.PipeOrgan].seventh[0]) },
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.PipeOrgan].root[1]) }
			}
		});
		licks[Instrument.Keyboard].Add (new Riff () {
			name = "Example Guitar Lick",
			instrument = Instrument.Keyboard,
			notes = new List<List<Note>>() {
				new List<Note> () { new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.Keyboard].root[0]) },
				new List<Note> () ,
				//new List<Note> () ,
				//new List<Note> () ,
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.Keyboard].fourth[0]) },
				//new List<Note> () ,
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.Keyboard].fifth[0])},
				//new List<Note> ()
			}
		});
		licks[Instrument.Keyboard].Add (new Riff () {
			name = "Example Guitar Lick2",
			instrument = Instrument.Keyboard,
			notes = new List<List<Note>>() {
				new List<Note> () { new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.Keyboard].third[0]) },
				new List<Note> () ,
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.Keyboard].third[0]) },
				new List<Note> () ,
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.Keyboard].fifth[0]) },
				new List<Note> () ,
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.Keyboard].fifth[0]) },
				new List<Note> () { new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.Keyboard].root[0]) },
				new List<Note> ()
			}
		});
		licks[Instrument.Keyboard].Add (new Riff () {
			name = "Example Guitar Lick3",
			instrument = Instrument.Keyboard,
			notes = new List<List<Note>>() {
				new List<Note> () { new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.Keyboard].seventh[0]) },
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.Keyboard].root[1]) },
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.Keyboard].seventh[0]) },
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.Keyboard].root[1]) },
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.Keyboard].root[0]) },
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.Keyboard].root[0]) },
				new List<Note> () { new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.Keyboard].seventh[0]) },
				new List<Note> () {new Note(KeyManager.instance.scales[MusicManager.instance.currentKey][Instrument.Keyboard].root[1]) }
			}
		});
	}*/
}