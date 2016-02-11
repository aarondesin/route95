using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputManager : MonoBehaviour {

	public static InputManager instance;

	public static Dictionary<string, Instrument> keyToInstrument = new Dictionary<string, Instrument>() {
		{ "Alpha1", Instrument.RockDrums },
		{ "Alpha2", Instrument.ElectricGuitar }//,
		//{ "3", null },
		//{ "4", null },
		//{ "5", null },
		//{ "6", null },
		//{ "7", null },
		//{ "8", null },
		//{ "9", null },
		//{ "0", null },
		//{ "-", null },
		//{ "=", null }
	};

	void Start () {
		instance = this;
	}

	void Update () {
		if (Input.GetKeyDown(KeyCode.Alpha1)) {
			SwitchInstrument(keyToInstrument["Alpha1"]);
		} else if (Input.GetKeyDown(KeyCode.Alpha2)) {
			SwitchInstrument(keyToInstrument["Alpha2"]);
		} //else if (Input.GetKeyDown("3")) {
		//	MusicManager.currentInstrument = keyToInstrument["3"];
		//} else if (Input.GetKeyDown("4")) {
		//	MusicManager.currentInstrument = keyToInstrument["4"];
		//} else if (Input.GetKeyDown("5")) {
		//	MusicManager.currentInstrument = keyToInstrument["5"];
		//} else if (Input.GetKeyDown("6")) {
		//	MusicManager.currentInstrument = keyToInstrument["6"];
		//} else if (Input.GetKeyDown("7")) {
		//	MusicManager.currentInstrument = keyToInstrument["7"];
		//} else if (Input.GetKeyDown("8")) {
		//	MusicManager.currentInstrument = keyToInstrument["8"];
		//} else if (Input.GetKeyDown("9")) {
		//	MusicManager.currentInstrument = keyToInstrument["9"];
		//} else if (Input.GetKeyDown("0")) {
		//	MusicManager.currentInstrument = keyToInstrument["0"];
		//} else if (Input.GetKeyDown("-")) {
		//	MusicManager.currentInstrument = keyToInstrument["-"];
		//} else if (Input.GetKeyDown("=")) {
		//	MusicManager.currentInstrument = keyToInstrument["="];
		//}
	}

	void SwitchInstrument (Instrument instrument) {
		MusicManager.currentInstrument = instrument;
		InstrumentDisplay.instance.Refresh();
	}
}
