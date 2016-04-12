using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class InputManager : MonoBehaviour {

	public static InputManager instance;

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

	void PlayLick (Riff lick) {
		MusicManager.instance.QueueLick(lick);
	}
}
