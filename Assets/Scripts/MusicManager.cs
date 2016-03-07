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
	//public Dictionary<string, SongPiece> songPiecesByName = new Dictionary<string, SongPiece>();
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
	bool loadedSounds = false;
	bool loadedInstruments = false;

	void Start () {
		if (instance) Debug.LogError("More than one MusicManager exists!");
		else instance = this;

		currentSong = new Song ();

		OneShot = gameObject.AddComponent<AudioSource>();


		//instrumentAudioSources[Instrument.ElectricGuitar].volume = 0.6f;
		//instrumentAudioSources[Instrument.RockDrums].volume = 0.8f;
		//instrumentAudioSources [Instrument.ElectricBass].gameObject.AddComponent<AudioDistortionFilter> ();
		//instrumentAudioSources[Instrument.ElectricBass].gameObject.GetComponent<AudioDistortionFilter> ().distortionLevel = 0.8f;

		maxBeats = (int)Mathf.Pow(2f, (float)Riff.MAX_SUBDIVS+2);



		//Debug.Log ("set up done" , licks.Count);

	}

	public void Load() {
		startLoadTime = Time.realtimeSinceStartup;
		StartCoroutine("LoadAllAudioClips", Sounds.soundsToLoad);
		StartCoroutine("DoLoad");
	}

	public IEnumerator DoLoad() {
		
		GameManager.instance.ChangeLoadingMessage("Loading instruments...");
		//LoadAllAudioClips (Sounds.soundsToLoad);

		instrumentAudioSources = new Dictionary<Instrument, AudioSource>();
		int i=0;
		//while (i<(int)Instrument.NUM_INSTRUMENTS) {
		while (true) {
			if (i>=(int)Instrument.NUM_INSTRUMENTS) {
				loadedInstruments = true;
				yield return NotifyDone();
			}
			GameObject obj = new GameObject ();
			AudioSource source = obj.AddComponent<AudioSource>();
			//instrumentAudioSources.Add((Instrument)i, new AudioSource());
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
			i++;
			//yield return new WaitForEndOfFrame();

		}
		//yield return NotifyDone();
		//SetupExampleRiffs();

		//SetupExampleLicks();

		//if (SoundClips.Count == Sounds.soundsToLoad.Count) yield return NotifyDone();
		/*else*/ //yield return NotifyDone();
		//yield return new WaitForSeconds(2.0f);
	}

	IEnumerator NotifyDone() {
		if (loadedInstruments && loadedSounds) {
			Debug.Log("MusicManager.Load(): finished in "+(Time.realtimeSinceStartup-startLoadTime).ToString("0.0000")+" seconds.");
			GameManager.instance.LoadNext();
			StopCoroutine("LoadAllAudioClips");
			StopCoroutine("DoLoad");
			StopCoroutine("LoadExampleRiffs");
			StopCoroutine("LoadExampleLicks");
			yield return null;
		}
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
		OneShot.Stop();
	}

	void Update(){
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
					currentSong.PlaySong(beat);
					//Debug.Log(beat);
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
		MusicManager.instance.currentSong.RemoveAt(beat, currentInstrument);
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
	IEnumerator LoadAllAudioClips(Dictionary<string, List<string>> paths) {
	//void LoadAllAudioClips (List<string> paths) {
		//foreach (string path in paths) {
		foreach (KeyValuePair<string, List<string>> list in Sounds.soundsToLoad) {
			int i=0;
			while (i<list.Value.Count) {
				for (int j=0; j<GameManager.instance.loadingSpeed && i+j<list.Value.Count; j++) {
					LoadAudioClip(list.Value[i+j]);
				}
				i+=GameManager.instance.loadingSpeed ;
				yield return null;
			}
			//foreach (string path in Sounds.soundsToLoad) {
				//LoadAudioClip  (path);
		}
		yield return StartCoroutine("SetupExampleRiffs");
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
	}

	//public void SetupExampleRiffs () {
	IEnumerator SetupExampleRiffs() {
		riffs.Add( new Riff () {
			name = "Example Guitar Riff",
			instrument = Instrument.ElectricGuitar,
			notes = new List<List<Note>>() {
				new List<Note> () {new Note("Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_E2") },
				new List<Note> (),
				new List<Note> (),
				new List<Note> (),
				new List<Note> () {new Note("Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_G#2") },
				new List<Note> (),
				new List<Note> (),
				new List<Note> (),
				new List<Note> () {new Note("Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_F#2")},
				new List<Note> (),
				new List<Note> (),
				new List<Note> (),
				new List<Note> () {new Note("Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_A2") },
				new List<Note> (),
				new List<Note> (),
				new List<Note> ()
			}
		});
		riffs.Add( new Riff () {
			name = "Example Bass Riff",
			instrument = Instrument.ElectricBass,
			notes = new List<List<Note>>() {
				new List<Note> () { new Note("Audio/Instruments/Melodic/ElectricBass/ElectricBass_E1") },
				new List<Note> (),
				new List<Note> (),
				new List<Note> (),
				new List<Note> () { new Note("Audio/Instruments/Melodic/ElectricBass/ElectricBass_G#1") },
				new List<Note> (),
				new List<Note> (),
				new List<Note> (),
				new List<Note> () { new Note("Audio/Instruments/Melodic/ElectricBass/ElectricBass_B1") },
				new List<Note> (),
				new List<Note> (),
				new List<Note> (),
				new List<Note> () { new Note ("Audio/Instruments/Melodic/ElectricBass/ElectricBass_C#2") },
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
		yield return StartCoroutine("SetupExampleLicks");
	}

	//public void SetupExampleLicks () {
	IEnumerator SetupExampleLicks() {
		licks[Instrument.ElectricGuitar].Add (new Riff () {
			name = "Example Guitar Lick",
			instrument = Instrument.ElectricGuitar,
			notes = new List<List<Note>>() {
				new List<Note> () {new Note("Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_E2") },
				new List<Note> () ,
				new List<Note> () ,
				new List<Note> () ,
				new List<Note> () {new Note("Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_A2") },
				new List<Note> () ,
				new List<Note> () {new Note("Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_B2") },
				new List<Note> ()
			}
		});

		licks[Instrument.ElectricBass].Add (new Riff () {
			name = "Example Bass Lick",
			instrument = Instrument.ElectricBass,
			notes = new List<List<Note>>() {
				new List<Note> () {new Note("Audio/Instruments/Melodic/ElectricBass/ElectricBass_F#1") },
				new List<Note> () ,
				new List<Note> () ,
				new List<Note> () ,
				new List<Note> () {new Note("Audio/Instruments/Melodic/ElectricBass/ElectricBass_F#1") },
				new List<Note> () ,
				new List<Note> () {new Note("Audio/Instruments/Melodic/ElectricBass/ElectricBass_A1") },
				new List<Note> () ,
				new List<Note> () ,
				new List<Note> () ,
				new List<Note> () ,
				new List<Note> () ,
				new List<Note> () {new Note("Audio/Instruments/Melodic/ElectricBass/ElectricBass_C#2") },
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
				new List<Note> () ,
				new List<Note> () ,
				new List<Note> () { new Note("Audio/Instruments/Percussion/RockDrums_Hat")},
			}
		});
		loadedSounds = true;
		yield return NotifyDone();
	}
}
	