﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sounds : MonoBehaviour {

	// List of all sound paths to load
	public static Dictionary<string, List<string>> soundsToLoad = new Dictionary<string, List<string>>() {// maybe use dict for efficiency

		// Melodic.ElectricGuitar
		{ "ElectricGuitar", new List<string> {
			"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_E2",
			"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_F2",
			"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_F#2",
			"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_G2",
			"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_G#2",
			"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_A2",
			"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_A#2",
			"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_B2",
			"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_C3",
			"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_C#3",
			"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_D3",
			"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_D#3",
			"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_E3",
			"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_F3",
			"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_F#3",
			"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_G3",
			"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_G#3",
			"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_A3",
			"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_A#3",
			"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_B3",
			"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_C4",
			"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_C#4",
			"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_D4",
			"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_D#4",
			"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_E5",
			"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_F5",
			"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_F#5",
			"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_G5",
			"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_G#5",
			"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_A5",
			"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_A#5",
			"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_B5",
			"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_C6",
			"Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_C#6"
			} },

		//Melodic.ElectricBass
		{ "ElectricBass", new List<string> () {
			"Audio/Instruments/Melodic/ElectricBass/ElectricBass_E1",
			"Audio/Instruments/Melodic/ElectricBass/ElectricBass_F1",
			"Audio/Instruments/Melodic/ElectricBass/ElectricBass_F#1",
			"Audio/Instruments/Melodic/ElectricBass/ElectricBass_G1",
			"Audio/Instruments/Melodic/ElectricBass/ElectricBass_G#1",
			"Audio/Instruments/Melodic/ElectricBass/ElectricBass_A1",
			"Audio/Instruments/Melodic/ElectricBass/ElectricBass_A#1",
			"Audio/Instruments/Melodic/ElectricBass/ElectricBass_B1",
			"Audio/Instruments/Melodic/ElectricBass/ElectricBass_C2",
			"Audio/Instruments/Melodic/ElectricBass/ElectricBass_C#2",
			"Audio/Instruments/Melodic/ElectricBass/ElectricBass_D2",
			"Audio/Instruments/Melodic/ElectricBass/ElectricBass_D#2",
			"Audio/Instruments/Melodic/ElectricBass/ElectricBass_E2",
			"Audio/Instruments/Melodic/ElectricBass/ElectricBass_F2",
			"Audio/Instruments/Melodic/ElectricBass/ElectricBass_F#2",
			"Audio/Instruments/Melodic/ElectricBass/ElectricBass_G2",
			"Audio/Instruments/Melodic/ElectricBass/ElectricBass_G#2",
			"Audio/Instruments/Melodic/ElectricBass/ElectricBass_A2",
			"Audio/Instruments/Melodic/ElectricBass/ElectricBass_A#2",
			"Audio/Instruments/Melodic/ElectricBass/ElectricBass_B2",
			"Audio/Instruments/Melodic/ElectricBass/ElectricBass_C3",
			"Audio/Instruments/Melodic/ElectricBass/ElectricBass_C#3",
			"Audio/Instruments/Melodic/ElectricBass/ElectricBass_D3",
			"Audio/Instruments/Melodic/ElectricBass/ElectricBass_D#3",
			"Audio/Instruments/Melodic/ElectricBass/ElectricBass_E3",
			"Audio/Instruments/Melodic/ElectricBass/ElectricBass_F3",
			"Audio/Instruments/Melodic/ElectricBass/ElectricBass_F#3",
			"Audio/Instruments/Melodic/ElectricBass/ElectricBass_G3",
			"Audio/Instruments/Melodic/ElectricBass/ElectricBass_G#3",
			"Audio/Instruments/Melodic/ElectricBass/ElectricBass_A3",
			"Audio/Instruments/Melodic/ElectricBass/ElectricBass_A#3",
			"Audio/Instruments/Melodic/ElectricBass/ElectricBass_B3",
			"Audio/Instruments/Melodic/ElectricBass/ElectricBass_C4",
			"Audio/Instruments/Melodic/ElectricBass/ElectricBass_C#4",
			"Audio/Instruments/Melodic/ElectricBass/ElectricBass_D4",
			"Audio/Instruments/Melodic/ElectricBass/ElectricBass_D#4"
			} },

		// Percussion.RockDrums
		{ "RockDrums", new List<string> () {
			"Audio/Instruments/Percussion/RockDrums_Kick",
			"Audio/Instruments/Percussion/RockDrums_Snare",
			"Audio/Instruments/Percussion/RockDrums_Tom",
			"Audio/Instruments/Percussion/RockDrums_Hat",
			"Audio/Instruments/Percussion/RockDrums_Crash",
			} }
	};
}
