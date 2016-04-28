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

	#region MusicManager Vars

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

	public Tempo tempo = Tempo.Medium;
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

	public bool loadedAudioSources = false;

	#endregion
	#region Unity Callbacks

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
		ScaleInfo.BuildScaleInfo();
		LoadInstruments();
		LoadScales();
		Debug.Log("MusicManager.Load(): finished in "+(Time.realtimeSinceStartup-startLoadTime).ToString("0.0000")+" seconds.");
		GameManager.instance.LoadNext();
	}

	#endregion
	#region MusicManager Callbacks

	// Loads all audio clip paths in soundsToLoad
	void LoadSounds() {
		GameManager.instance.ChangeLoadingMessage("Loading sounds...");
		foreach (KeyValuePair<string, List<string>> list in Sounds.soundsToLoad) {
			foreach (string path in list.Value) {
				LoadAudioClip(path);
			}
		}
	}

	void GetAudioEffect () {
		//currentInstrument = InstrumentSetup.currentRiff.instrument;
		currentInstrument = Instrument.AllInstruments[InstrumentSetup.instance.currentRiff.instrumentIndex];
		//Debug.Log ("Calling getAudioEffect " + currentInstrument);
		instrumentAudioSources[currentInstrument].gameObject.GetComponent<AudioDistortionFilter>().distortionLevel = InstrumentSetup.instance.currentRiff.distortionLevel;
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

		loadedAudioSources = true;
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
		Song newSong = new Song();
		currentSong = newSong;
		currentProject.AddSong(newSong);
	}

	public void SaveCurrentSong () {
		SaveLoad.SaveCurrentSong();
	}

	public void SetKey (int key) {
		currentSong.key = (Key)key;
	}

	public void SetKey (Key key) {
		SetKey ((int)key);
	}

	public void PlayRiffLoop(){
		if (loop) {
			StopLooping();
			instrumentAudioSources[Instrument.AllInstruments[InstrumentSetup.instance.currentRiff.instrumentIndex]].Stop();
		} else {
			playing = true;
			loop = true;
		}
	}

	public void StopLooping () {
		playing = false;
		loop = false;
		beat = 0;
		instrumentAudioSources[Instrument.AllInstruments[InstrumentSetup.instance.currentRiff.instrumentIndex]].Stop();
	}

	void FixedUpdate(){
		if (playing && !GameManager.instance.paused) {
			if (BeatTimer <= 0f) {
				switch (GameManager.instance.currentMode) {
				case GameManager.Mode.Setup:
					InstrumentSetup.instance.currentRiff.PlayRiff (beat++);
					if (beat >= InstrumentSetup.instance.currentRiff.beatsShown*(int)Mathf.Pow(2f,Riff.MAX_SUBDIVS) && loop)
						beat = 0;
					break;
				case GameManager.Mode.Live:
					
					if (beat >= currentSong.beats) {
						//Debug.Log("shit");
						beat = 0;

							if (currentPlayingSong < currentProject.songs.Count-1) {
								beat = 0;
								DisableAllAudioSources();
								currentPlayingSong++;
							} else {
								GameManager.instance.SwitchToPostplay();
							}
					}
					currentSong.PlaySong(beat);
	
					float songTotalTime = currentSong.beats*7200f/tempoToFloat[tempo]/4f;
					float songCurrentTime = (beat*7200f/tempoToFloat[tempo]/4f) + (7200f/tempoToFloat[tempo]/4f)-BeatTimer;
					GameManager.instance.songProgressBar.GetComponent<SongProgressBar>().SetValue(songCurrentTime/songTotalTime);
					beat++;
					break;
				}
				BeatTimer = 7200f / tempoToFloat[tempo] /4f;// 3600f = 60 fps * 60 seconds

			} else {
	
				BeatTimer -= 1.667f;
			}
		} 

	}

	void DisableAllAudioSources () {
		foreach (Instrument inst in Instrument.AllInstruments) instrumentAudioSources[inst].enabled = false;
	}

	public void IncreaseTempo () {
		if ((int)tempo < (int)Tempo.NUM_TEMPOS-1) {
			tempo = (Tempo)((int)tempo+1);
			if (InstrumentSetup.instance != null)
				InstrumentSetup.instance.UpdateTempoText();
		}
	}

	public void DecreaseTempo () {
		if ((int)tempo > 0) {
			tempo = (Tempo)((int)tempo-1);
			if (InstrumentSetup.instance != null)
				InstrumentSetup.instance.UpdateTempoText();
		}
	}


	// Adds a new riff
	public Riff AddRiff () {
		//Debug.Log("added");
		Riff temp = new Riff ();
		InstrumentSetup.instance.currentRiff = temp;
		SongArrangeSetup.instance.selectedRiffIndex = currentProject.riffs.Count;
		currentProject.riffs.Add (temp);
		SongArrangeSetup.instance.Refresh();
		GetAudioEffect ();
		return temp;
	}

	public void AddRiff (Riff riff) {
		currentProject.riffs.Add (riff);
	}

	public Riff RiffByString (string riffName) {
		foreach (Riff riff in currentProject.riffs) {
			if (riffName == riff.name) return riff;
		}
		return null;
	}

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
	}
	
	#endregion
}