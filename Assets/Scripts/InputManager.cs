using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputManager : MonoBehaviour {

	public static InputManager instance;

	public static Dictionary<KeyCode, Instrument> keyToInstrument = new Dictionary<KeyCode, Instrument>() {
		{ KeyCode.Alpha1, Instrument.RockDrums },
		{ KeyCode.Alpha2, Instrument.ElectricGuitar },
		{ KeyCode.Alpha3, Instrument.ElectricBass }
	};

	public Dictionary<Instrument, AudioClip> instrumentSwitchSounds = new Dictionary<Instrument, AudioClip>();

	// List of mapped keys for instruments, so that the program knows what to check for
	public static List<KeyCode> mappedInstruments = new List<KeyCode>() {
		KeyCode.Alpha1,
		KeyCode.Alpha2,
		KeyCode.Alpha3
	};

	public static Dictionary<KeyCode, int> keyToLick;

	public static List<KeyCode> mappedLicks = new List<KeyCode>() {
		KeyCode.Q,
		KeyCode.W
		//KeyCode.E
	};

	void Start () {
		instance = this;
		instrumentSwitchSounds = new Dictionary<Instrument, AudioClip>() {
			{Instrument.RockDrums, Resources.Load<AudioClip>("Audio/Gameplay/Instruments/RockDrums")},
			{Instrument.ElectricGuitar, Resources.Load<AudioClip>("Audio/Gameplay/Instruments/ElectricGuitar")},
			{Instrument.ElectricBass, Resources.Load<AudioClip>("Audio/Gameplay/Instruments/ElectricBass")}
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
				foreach (KeyCode key in mappedInstruments) {
					if (Input.GetKeyDown(key)) {
						SwitchInstrument(keyToInstrument[key]);
						GameManager.instance.WakeLiveUI();
					}
				}
				// Check for playing lick
				foreach (KeyCode key2 in mappedLicks) {
					if (Input.GetKey(key2)) {
						if (MusicManager.instance.licks [MusicManager.instance.currentInstrument] [keyToLick [key2]] != null)
							PlayLick (MusicManager.instance.licks [MusicManager.instance.currentInstrument] [keyToLick [key2]]);
						else {
							Debug.Log ("none available");
						}
					}

				}
				if (keyToLick == null) {
					
					keyToLick = new Dictionary<KeyCode, int>() {
						
						{ KeyCode.Q, 0 }
						//{ KeyCode.W, 1 },
						//{ KeyCode.E, 2 }


					};
					Debug.Log(keyToLick.Count);
					Debug.Log (MusicManager.instance.licks.Count);
				}
				if (keyToLick == null) {
					
					keyToLick = new Dictionary<KeyCode, int>() {
						
						{ KeyCode.Q, 0 },
						{ KeyCode.W, 1 }
						//{ KeyCode.E, 2 }


					};
					Debug.Log(keyToLick.Count);
					Debug.Log (MusicManager.instance.licks.Count);
				}
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
