using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System;
using System.Collections;
using System.Collections.Generic;// need for using lists
using System.IO; // need for path operations
using System.Linq;

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
	Slower,
	Slow,
	Medium,
	Fast,
	Faster,
	Fastest,
	NUM_TEMPOS
};

public class MusicManager : MonoBehaviour {

	#region MusicManager Vars

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

	public Tempo tempo = Tempo.Medium;
	private float BeatTimer;
	private int beat;
	public static bool playing = false;
	public static bool loop = false;

	public static Dictionary<Tempo, float> tempoToFloat = new Dictionary<Tempo, float> () {
		{ Tempo.Slowest, 50f },
		{ Tempo.Slower, 70f },
		{ Tempo.Slow, 90f },
		{ Tempo.Medium, 110f },
		{ Tempo.Fast, 130f },
		{ Tempo.Faster, 150f },
		{ Tempo.Fastest, 170f }
	};

	public int maxBeats; // max beats in riff, after subdivisions

	float startLoadTime;

	public bool loadedAudioSources = false;

	int loadProgress;
	public int loadsToDo;

	#endregion
	#region Unity Callbacks

	void Start () {
		if (instance) Debug.LogError("More than one MusicManager exists!");
		else instance = this;

		currentSong = new Song ();

		maxBeats = (int)Mathf.Pow(2f, (float)Riff.MAX_SUBDIVS+2);

		tempo = Tempo.Medium;
		Instrument.LoadInstruments ();


		loadsToDo = Sounds.soundsToLoad.Count + Instrument.AllInstruments.Count +
			(Enum.GetValues(typeof(Key)).Length-1) * ScaleInfo.AllScales.Count;
	}



	public void FinishLoading() {
		Debug.Log("MusicManager.Load(): finished in "+(Time.realtimeSinceStartup-startLoadTime).ToString("0.0000")+" seconds.");
		WorldManager.instance.Load();
	}

	#endregion
	#region Loading Callbacks

	//
	// LoadSounds (Coroutine)
	// LoadInstruments (Coroutine)
	// 

	public void Load() {
		startLoadTime = Time.realtimeSinceStartup;
		StartCoroutine ("LoadSounds");
	}

	IEnumerator LoadSounds () {
		GameManager.instance.ChangeLoadingMessage("Loading sounds...");
		float startTime = Time.realtimeSinceStartup;
		int numLoaded = 0;

		foreach (KeyValuePair<string, List<string>> list in Sounds.soundsToLoad) {
			foreach (string path in list.Value) {
				LoadAudioClip(path);
				numLoaded++;
				//Debug.Log("loaded "+path);

				if (Time.realtimeSinceStartup - startTime > 1f/GameManager.instance.targetFrameRate) {
					yield return null;
					startTime = Time.realtimeSinceStartup;
					GameManager.instance.ReportLoaded (numLoaded);
					numLoaded = 0;
				}
			}
		}

		yield return StartCoroutine("LoadInstruments");
	}

	// Loads a single audio clip
	void LoadAudioClip (string path) {
		if (SoundClips.ContainsKey(path)) return;
		AudioClip sound = (AudioClip) Resources.Load (path);
		if (sound == null) {
			Debug.LogError("Failed to load AudioClip at "+path);
		} else {
			SoundClips.Add (path, sound);
		}
	}

	IEnumerator LoadInstruments () {
		GameManager.instance.ChangeLoadingMessage("Loading instruments...");
		float startTime = Time.realtimeSinceStartup;
		int numLoaded = 0;

		instrumentAudioSources = new Dictionary<Instrument, AudioSource>();
		for (int i=0; i<Instrument.AllInstruments.Count; i++) {

			Instrument.AllInstruments[i].Load();

			GameObject obj = new GameObject (Instrument.AllInstruments[i].name);
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
			obj.GetComponent<AudioReverbFilter>().enabled = false;

			obj.AddComponent<AudioDistortionFilter> ();
			obj.GetComponent<AudioDistortionFilter>().enabled = false;

			obj.AddComponent<AudioTremoloFilter>();
			obj.GetComponent<AudioTremoloFilter>().enabled = false;

			obj.AddComponent<AudioEchoFilter> ();
			obj.GetComponent<AudioEchoFilter>().enabled = false;

			obj.AddComponent<AudioChorusFilter> ();
			obj.GetComponent<AudioChorusFilter>().enabled = false;

			numLoaded++;

			if (Time.realtimeSinceStartup - startTime > 1f/GameManager.instance.targetFrameRate) {
				yield return null;
				startTime = Time.realtimeSinceStartup;
				GameManager.instance.ReportLoaded (numLoaded);
				numLoaded = 0;
			}
		}

		if (instrumentAudioSources.Count == Instrument.AllInstruments.Count) KeyManager.instance.DoBuildScales();
		yield return null;
	}

	#endregion
	#region MusicManager Callbacks


	/*void GetAudioEffect () {
		//currentInstrument = InstrumentSetup.currentRiff.instrument;
		currentInstrument = Instrument.AllInstruments[InstrumentSetup.currentRiff.instrumentIndex];
		//Debug.Log ("Calling getAudioEffect " + currentInstrument);
		instrumentAudioSources[currentInstrument].gameObject.GetComponent<AudioDistortionFilter>().distortionLevel = InstrumentSetup.currentRiff.distortionLevel;
		//instrumentAudioSources[currentInstrument].gameObject.GetComponent<AudioEchoFilter>().decayRatio = InstrumentSetup.currentRiff.echoDecayRatio;
		//instrumentAudioSources[currentInstrument].gameObject.GetComponent<AudioEchoFilter>().delay = InstrumentSetup.currentRiff.echoDelay;
	}*/


	public void NewProject () {
		currentProject = new Project();
	}

	public void SaveCurrentProject () {
		SaveLoad.SaveCurrentProject();
	}

	public void NewSong () {
		currentSong = new Song();
		currentProject.songs.Add(currentSong);
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
	}

	void FixedUpdate(){
		if (playing && !GameManager.instance.paused) {
			if (BeatTimer <= 0f) {
				switch (GameManager.instance.currentMode) {
				case GameManager.Mode.Setup:
					InstrumentSetup.currentRiff.PlayRiff (beat++);
					if (beat >= InstrumentSetup.currentRiff.beatsShown*(int)Mathf.Pow(2f,Riff.MAX_SUBDIVS) && loop)
						beat = 0;
					break;
				case GameManager.Mode.Live:
					
					if (beat >= currentSong.Beats) {
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
	
					float songTotalTime = currentSong.Beats*7200f/tempoToFloat[tempo]/4f;
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
		Riff temp = new Riff ();
		InstrumentSetup.currentRiff = temp;
		currentSong.RegisterRiff(temp);
		SongArrangeSetup.instance.selectedRiffIndex = temp.index;
		SongArrangeSetup.instance.Refresh();
		return temp;
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