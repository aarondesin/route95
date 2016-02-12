using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InstrumentDisplay : MonoBehaviour {
	public static InstrumentDisplay instance;

	public static Dictionary<Instrument, Sprite> instrumentIcons;

	void Start () {
		instance = this;
		instrumentIcons = new Dictionary<Instrument, Sprite>() {
			{ Instrument.ElectricGuitar, Resources.Load<Sprite>("UI/Instrument_ElectricGuitar") },
			{ Instrument.RockDrums, Resources.Load<Sprite> ("UI/Instrument_RockDrums") }
		};
	}

	public void Refresh () {
		GetComponent<Image>().sprite = instrumentIcons[MusicManager.instance.currentInstrument];
	}
	
}
