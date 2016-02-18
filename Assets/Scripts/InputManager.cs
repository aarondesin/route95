using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputManager : MonoBehaviour {

	public static InputManager instance;

	public static Dictionary<string, Instrument> keyToInstrument = new Dictionary<string, Instrument>() {
		{ "Alpha1", Instrument.RockDrums },
		{ "Alpha2", Instrument.ElectricGuitar },
		{ "Alpha3", Instrument.ElectricBass }
	};

	void Start () {
		instance = this;
	}

	void Update () {
		if (Input.GetKeyDown(KeyCode.Alpha1)) {
			SwitchInstrument(keyToInstrument["Alpha1"]);
		} else if (Input.GetKeyDown(KeyCode.Alpha2)) {
			SwitchInstrument(keyToInstrument["Alpha2"]);
		} else if (Input.GetKeyDown(KeyCode.Alpha3)) {
			SwitchInstrument(keyToInstrument["Alpha3"]);
		}
	}

	void SwitchInstrument (Instrument instrument) {
		MusicManager.instance.currentInstrument = instrument;
		InstrumentDisplay.instance.Refresh();
	}
}
