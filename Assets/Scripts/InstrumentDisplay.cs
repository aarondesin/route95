using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InstrumentDisplay : MonoBehaviour {
	public static InstrumentDisplay instance;

	public Image glow;
	public float fadeSpeed;

	public static Dictionary<Instrument, Sprite> instrumentIcons;
	public static Dictionary<Instrument, Sprite> instrumentIconGlows;

	void Start () {
		instance = this;
		instrumentIcons = new Dictionary<Instrument, Sprite>() {
			{ Instrument.ElectricGuitar, Resources.Load<Sprite>("UI/Instrument_ElectricGuitar") },
			{ Instrument.ElectricBass, Resources.Load<Sprite>("UI/Instrument_ElectricBass") },
			{ Instrument.AcousticGuitar, Resources.Load<Sprite>("UI/Instrument_ElectricGuitar") },
			{ Instrument.ClassicalGuitar, Resources.Load<Sprite>("UI/Instrument_ElectricGuitar") },
			{ Instrument.RockDrums, Resources.Load<Sprite> ("UI/Instrument_RockDrums") }
		};
		instrumentIconGlows = new Dictionary<Instrument, Sprite>() {
			{ Instrument.ElectricGuitar, Resources.Load<Sprite>("UI/Instrument_ElectricGuitar_Glow") },
			{ Instrument.ElectricBass, Resources.Load<Sprite>("UI/Instrument_ElectricBass_Glow") },
			{ Instrument.AcousticGuitar, Resources.Load<Sprite>("UI/Instrument_ElectricGuitar_Glow") },
			{ Instrument.ClassicalGuitar, Resources.Load<Sprite>("UI/Instrument_ElectricGuitar_Glow") },
			{ Instrument.RockDrums, Resources.Load<Sprite> ("UI/Instrument_RockDrums_Glow") }
		};
	}

	void FixedUpdate () {
		if (GameManager.instance.currentMode == Mode.Live && !GameManager.instance.paused) {
			Color color = glow.GetComponent<Image>().color;
			color.a -= fadeSpeed;
			glow.GetComponent<Image>().color = color;
		}
	}

	public void Refresh () {
		GetComponent<Image>().sprite = instrumentIcons[MusicManager.instance.currentInstrument];
		glow.GetComponent<Image>().sprite = instrumentIconGlows[MusicManager.instance.currentInstrument];
	}

	public void WakeGlow () {
		Color color = glow.GetComponent<Image>().color;
		color.a = 1f;
		glow.GetComponent<Image>().color = color;
	}
	
}
