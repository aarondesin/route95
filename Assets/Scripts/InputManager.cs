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

	// List of mapped keys for instruments, so that the program knows what to check for
	public static List<KeyCode> mappedInstruments = new List<KeyCode>() {
		KeyCode.Alpha1,
		KeyCode.Alpha2,
		KeyCode.Alpha3
	};

	public static Dictionary<KeyCode, int> keyToLick;

	public static List<KeyCode> mappedLicks = new List<KeyCode>() {
		KeyCode.Q
		//KeyCode.W,
		//KeyCode.E
	};

	void Start () {
		instance = this;
	}

	void Update () {
		if (GameManager.instance.currentMode == Mode.Live) {
			// Check for instruments switch
			foreach (KeyCode key in mappedInstruments) {
				if (Input.GetKeyDown(key)) SwitchInstrument(keyToInstrument[key]);
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
		}
	}

	void SwitchInstrument (Instrument instrument) {
		MusicManager.instance.currentInstrument = instrument;
		Debug.Log (MusicManager.instance.currentInstrument);
		InstrumentDisplay.instance.Refresh();
	}

	void PlayLick (Riff lick) {
		MusicManager.instance.QueueLick(lick);
	}
}
