using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class InputManager : MonoBehaviour {

	public static InputManager instance;

	public List<AudioSource> audioSources;

	// 0: lick system
	// 1: single-note system
	private int LiveSystem = 1;

	public static Dictionary<KeyCode, Instrument> keyToInstrument = new Dictionary<KeyCode, Instrument>() {
		{ KeyCode.Alpha1, Instrument.RockDrums },
		{ KeyCode.Alpha2, Instrument.ElectricGuitar },
		{ KeyCode.Alpha3, Instrument.ElectricBass },
		{ KeyCode.Alpha4, Instrument.AcousticGuitar },
		{ KeyCode.Alpha5, Instrument.ClassicalGuitar },
		{ KeyCode.Alpha6, Instrument.PipeOrgan },
		{ KeyCode.Alpha7, Instrument.Keyboard }
	};

	public Dictionary<Instrument, AudioClip> instrumentSwitchSounds = new Dictionary<Instrument, AudioClip>();

	public static Dictionary<KeyCode, int> keyToLick = new Dictionary<KeyCode, int>() {
		{ KeyCode.Q, 0 },
		{ KeyCode.W, 1 },
		{ KeyCode.E, 2 },
		{ KeyCode.R, 3 },
		{ KeyCode.T, 4 }
	};

	public static Dictionary<KeyCode, int> keyToNote = new Dictionary<KeyCode, int>() {
		{ KeyCode.P, 0 },
		{ KeyCode.O, 1 },
		{ KeyCode.I, 2 },
		{ KeyCode.U, 3 },
		{ KeyCode.Y, 4 },
		{ KeyCode.T, 5 },
		{ KeyCode.R, 6 },
		{ KeyCode.E, 7 },
		{ KeyCode.W, 8 },
		{ KeyCode.Q, 9 },
		{ KeyCode.L, 10 },
		{ KeyCode.K, 11 },
		{ KeyCode.J, 12 },
		{ KeyCode.H, 13 },
		{ KeyCode.G, 14 },
		{ KeyCode.F, 15 },
		{ KeyCode.D, 16 },
		{ KeyCode.S, 17 },
		{ KeyCode.A, 18 },
		{ KeyCode.M, 19 },
		{ KeyCode.N, 20 },
		{ KeyCode.B, 21 },
		{ KeyCode.V, 22 },
		{ KeyCode.C, 23 },
		{ KeyCode.X, 24 },
		{ KeyCode.Z, 25 }
	};

	void Start () {
		instance = this;
		instrumentSwitchSounds = new Dictionary<Instrument, AudioClip>() {
			{ Instrument.RockDrums, Resources.Load<AudioClip>("Audio/Gameplay/Instruments/RockDrums") },
			{ Instrument.ElectricGuitar, Resources.Load<AudioClip>("Audio/Gameplay/Instruments/ElectricGuitar")},
			{ Instrument.ElectricBass, Resources.Load<AudioClip>("Audio/Gameplay/Instruments/ElectricBass")},
			{ Instrument.AcousticGuitar, Resources.Load<AudioClip>("Audio/Gameplay/Instruments/ElectricBass")},
			{ Instrument.ClassicalGuitar, Resources.Load<AudioClip>("Audio/Gameplay/Instruments/ElectricBass")},
			{ Instrument.PipeOrgan, Resources.Load<AudioClip>("Audio/Gameplay/Instruments/ElectricBass")},
			{ Instrument.Keyboard, Resources.Load<AudioClip>("Audio/Gameplay/Instruments/ElectricBass")}
		};
		audioSources = new List<AudioSource>();
		for (int i=0; i<26; i++) {
			GameObject obj = new GameObject();
			AudioSource temp = obj.AddComponent<AudioSource>();
			temp.volume = 1.0f;
			audioSources.Add(temp);
		}
	}

	void Update () {
		if (GameManager.instance.currentMode == Mode.Live) {
			if (Input.GetKeyDown (KeyCode.Escape)) {
				GameManager.instance.TogglePause ();
			} else if (Input.GetKeyDown (KeyCode.UpArrow)) {
				MusicManager.instance.IncreaseTempo ();
			} else if (Input.GetKeyDown (KeyCode.DownArrow)) {
				MusicManager.instance.DecreaseTempo ();
			} else {
				// Check for instruments switch
				foreach (KeyValuePair<KeyCode, Instrument> key in keyToInstrument) {
					if (Input.GetKeyDown(key.Key)) {
						SwitchInstrument(keyToInstrument[key.Key]);
						GameManager.instance.WakeLiveUI();
					}
				}

				switch (LiveSystem) {
				/*case 0: // licks
					// Check for playing lick
					foreach (KeyCode key2 in keyToLick.Keys.ToList()) {
						if (Input.GetKey(key2)) {
							if (MusicManager.instance.licks[MusicManager.instance.currentInstrument] != null) {
								if (MusicManager.instance.licks [MusicManager.instance.currentInstrument] [keyToLick [key2]] != null)
									PlayLick (MusicManager.instance.licks [MusicManager.instance.currentInstrument] [keyToLick [key2]]);
								else {
									Debug.Log ("none available");
								}
							}
						}

					}
					if (keyToLick == null) {
						

						Debug.Log(keyToLick.Count);
						Debug.Log (MusicManager.instance.licks.Count);
					}
					break;*/

					case 1: // notes
					foreach (KeyCode key in keyToNote.Keys.ToList()) {
						Instrument inst = MusicManager.instance.currentInstrument;
						if (Input.GetKeyDown(key)) {
							int noteIndex;
							if (inst == Instrument.RockDrums) {
								noteIndex = KeyManager.instance.percussionSets[inst].Count-1-keyToNote[key];
								if (noteIndex >= 0) {
									Note note = new Note(KeyManager.instance.percussionSets[inst][noteIndex]);
									note.PlayNote(audioSources[keyToNote[key]], false);
								}
							} else {
								noteIndex = KeyManager.instance.scales[MusicManager.instance.currentSong.key][inst].allNotes.Count-1-keyToNote[key];
								if (noteIndex >= 0) {
									Note note = new Note(KeyManager.instance.scales[MusicManager.instance.currentSong.key][inst].allNotes[noteIndex]);
									//note.PlayNote(MusicManager.instance.instrumentAudioSources[MusicManager.instance.currentInstrument], true);
									if (note != null)
										note.PlayNote(audioSources[keyToNote[key]], true);
								}
							}
							
						} else if (Input.GetKeyUp(key) && MusicManager.instance.currentInstrument != Instrument.RockDrums) {
							audioSources[keyToNote[key]].Stop();
						}
					}
						break;
				}
				/*if (keyToLick == null) {
					
					keyToLick = new Dictionary<KeyCode, int>() {
						
						{ KeyCode.Q, 0 },
						{ KeyCode.W, 1 },
						{ KeyCode.E, 2 }


					};
					Debug.Log(keyToLick.Count);
					Debug.Log (MusicManager.instance.licks.Count);
				}*/
			}
		}
	}

	void SwitchInstrument (Instrument instrument) {
		if (instrument != MusicManager.instance.currentInstrument) {
			MusicManager.instance.currentInstrument = instrument;
			MusicManager.instance.GetComponent<AudioSource>().PlayOneShot(instrumentSwitchSounds[instrument]);
			Debug.Log (MusicManager.instance.currentInstrument);
			InstrumentDisplay.instance.Refresh();
		}
	}

	/*void PlayLick (Riff lick) {
		MusicManager.instance.QueueLick(lick);
	}*/
}
