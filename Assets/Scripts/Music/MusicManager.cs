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

	public struct Sound {
		public AudioClip clip;
		public AudioSource source;
		public float volume;
		public float delay;
	}

	#region MusicManager Vars

	public static MusicManager instance; // access this MusicManager from anywhere using MusicManager.instance

	public AudioMixer mixer;
	public Project currentProject;

	// --Global Music Properties-- //
	public Instrument currentInstrument = MelodicInstrument.ElectricGuitar;
	[NonSerialized]
	public Song currentSong = null;
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
	public bool loopPlaylist = false;
	public Image loopPlaylistButton;

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

	void Awake () {
		if (instance) Debug.LogError("More than one MusicManager exists!");
		else instance = this;

		maxBeats = (int)Mathf.Pow(2f, (float)Riff.MAX_SUBDIVS+2);

		tempo = Tempo.Medium;
		Instrument.LoadInstruments ();


		loadsToDo = Sounds.soundsToLoad.Count + Instrument.AllInstruments.Count +
			(Enum.GetValues(typeof(Key)).Length-1) * ScaleInfo.AllScales.Count;

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
					if (currentSong == null) return;
					if (currentSong.Beats == 0) return;

					if (beat >= currentSong.Beats) {
						beat = 0;

						if (currentPlayingSong < currentProject.songs.Count-1) {
							DisableAllAudioSources();
							currentPlayingSong++;
						} else {
							if (loopPlaylist) currentPlayingSong = 0;
							else GameManager.instance.SwitchToPostplay();
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

				if (Time.realtimeSinceStartup - startTime > 1f/Application.targetFrameRate) {
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
		AudioClip sound = (AudioClip) Resources.Load (path);

		if (sound == null) Debug.LogError("Failed to load AudioClip at "+path);
		else SoundClips.Add (path, sound);
	}

	IEnumerator LoadInstruments () {
		GameManager.instance.ChangeLoadingMessage("Loading instruments...");
		float startTime = Time.realtimeSinceStartup;
		int numLoaded = 0;

		instrumentAudioSources = new Dictionary<Instrument, AudioSource>();
		for (int i=0; i<Instrument.AllInstruments.Count; i++) {

			Instrument.AllInstruments[i].Load();

			GameObject obj = new GameObject (Instrument.AllInstruments[i].name);

			// Group instrument under MusicManager
			obj.transform.parent = transform.parent;

			AudioSource source = obj.AddComponent<AudioSource>();
			source.outputAudioMixerGroup = mixer.FindMatchingGroups (obj.name) [0];
			instrumentAudioSources.Add(Instrument.AllInstruments[i], source);

			// Add distortion filter
			AudioDistortionFilter distortion = obj.AddComponent<AudioDistortionFilter> ();
			distortion.enabled = false;

			// Add tremolo filter
			AudioTremoloFilter tremolo = obj.AddComponent<AudioTremoloFilter>();
			tremolo.enabled = false;

			// Add chorus filter
			AudioChorusFilter chorus = obj.AddComponent<AudioChorusFilter> ();
			chorus.enabled = false;

			// Add flanger filter
			AudioFlangerFilter flanger = obj.AddComponent<AudioFlangerFilter>();
			flanger.enabled = false;

			// Add echo filter
			AudioEchoFilter echo = obj.AddComponent<AudioEchoFilter> ();
			echo.enabled = false;

			// Add reverb filter based on MusicManager's reverb filter
			AudioReverbFilter reverb = obj.AddComponent<AudioReverbFilter>();
			AudioReverbFilter masterReverb = GetComponent<AudioReverbFilter>();
			reverb.dryLevel = masterReverb.dryLevel;
			reverb.room = masterReverb.room;
			reverb.roomHF = masterReverb.roomHF;
			reverb.roomLF = masterReverb.roomLF;
			reverb.decayTime = masterReverb.decayTime;
			reverb.decayHFRatio = masterReverb.decayHFRatio;
			reverb.reflectionsLevel = masterReverb.reflectionsLevel;
			reverb.reflectionsDelay = masterReverb.reflectionsDelay;
			reverb.reverbLevel = masterReverb.reverbLevel;
			reverb.hfReference = masterReverb.hfReference;
			reverb.lfReference = masterReverb.lfReference;
			reverb.diffusion = masterReverb.diffusion;
			reverb.density = masterReverb.density;
			reverb.enabled = false;



			numLoaded++;

			if (Time.realtimeSinceStartup - startTime > 1f/Application.targetFrameRate) {
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

	/// <summary>
	/// Creates a new, blank project.
	/// </summary>
	public void NewProject () {
		currentProject = new Project();
	}

	/// <summary>
	/// Saves the current project.
	/// </summary>
	public void SaveCurrentProject () {
		SaveLoad.SaveCurrentProject();
	}

	/// <summary>
	/// Creates a new blank song and adds
	/// it to the current project.
	/// </summary>
	public void NewSong () {
		currentSong = new Song();
		currentProject.songs.Add(currentSong);
	}

	/// <summary>
	/// Saves the current song.
	/// </summary>
	public void SaveCurrentSong () {
		SaveLoad.SaveCurrentSong();
	}

	/// <summary>
	/// Sets the key of the current song.
	/// </summary>
	/// <param name="key">New key (int)</param>
	public void SetKey (int key) {
		currentSong.key = (Key)key;
	}

	/// <summary>
	/// Sets the key of the current song.
	/// </summary>
	/// <param name="key">New key.</param>
	public void SetKey (Key key) {
		SetKey ((int)key);
	}

	/// <summary>
	/// Toggles whether to loop playlist.
	/// </summary>
	public void ToggleLoopPlaylist () {
		loopPlaylist = !loopPlaylist;
		loopPlaylistButton.sprite = loopPlaylist ? InstrumentSetup.instance.percussionFilled : InstrumentSetup.instance.percussionEmpty;
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

	public bool IsKick (Note note) {
		return note.filename == "Audio/Instruments/Percussion/RockDrums/RockDrums_Kick";
	}
	
	#endregion
}