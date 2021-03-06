﻿// Sounds.cs
// ©2016 Team 95

using System.Collections.Generic;

using UnityEngine;

namespace Route95.Music {

    /// <summary>
    /// Class to hold all sound data to be loaded at runtime.
    /// </summary>
    public class Sounds : MonoBehaviour {

        /// <summary>
        /// List of all sound paths to load.
        /// </summary>
        public static Dictionary<string, List<string>> SoundsToLoad = new Dictionary<string, List<string>>() {

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
            "Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_E4",
            "Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_F4",
            "Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_F#4",
            "Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_G4",
            "Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_G#4",
            "Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_A4",
            "Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_A#4",
            "Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_B4",
            "Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_C5",
            "Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_C#5",
            "Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_D5",
            "Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_D#5",
            "Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_E5",
            "Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_F5",
            "Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_F#5",
            "Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_G5",
            "Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_G#5",
            "Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_A5",
            "Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_A#5",
            "Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_B5",
            "Audio/Instruments/Melodic/ElectricGuitar/ElectricGuitar_C6"
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

		//Melodic.AcousticGuitar
		{ "AcousticGuitar", new List<string> () {
                "Audio/Instruments/Melodic/AcousticGuitar/AcousticGuitar_E2",
                "Audio/Instruments/Melodic/AcousticGuitar/AcousticGuitar_F2",
                "Audio/Instruments/Melodic/AcousticGuitar/AcousticGuitar_F#2",
                "Audio/Instruments/Melodic/AcousticGuitar/AcousticGuitar_G2",
                "Audio/Instruments/Melodic/AcousticGuitar/AcousticGuitar_G#2",
                "Audio/Instruments/Melodic/AcousticGuitar/AcousticGuitar_A2",
                "Audio/Instruments/Melodic/AcousticGuitar/AcousticGuitar_A#2",
                "Audio/Instruments/Melodic/AcousticGuitar/AcousticGuitar_B2",
                "Audio/Instruments/Melodic/AcousticGuitar/AcousticGuitar_C3",
                "Audio/Instruments/Melodic/AcousticGuitar/AcousticGuitar_C#3",
                "Audio/Instruments/Melodic/AcousticGuitar/AcousticGuitar_D3",
                "Audio/Instruments/Melodic/AcousticGuitar/AcousticGuitar_D#3",
                "Audio/Instruments/Melodic/AcousticGuitar/AcousticGuitar_E3",
                "Audio/Instruments/Melodic/AcousticGuitar/AcousticGuitar_F3",
                "Audio/Instruments/Melodic/AcousticGuitar/AcousticGuitar_F#3",
                "Audio/Instruments/Melodic/AcousticGuitar/AcousticGuitar_G3",
                "Audio/Instruments/Melodic/AcousticGuitar/AcousticGuitar_G#3",
                "Audio/Instruments/Melodic/AcousticGuitar/AcousticGuitar_A3",
                "Audio/Instruments/Melodic/AcousticGuitar/AcousticGuitar_A#3",
                "Audio/Instruments/Melodic/AcousticGuitar/AcousticGuitar_B3",
                "Audio/Instruments/Melodic/AcousticGuitar/AcousticGuitar_C4",
                "Audio/Instruments/Melodic/AcousticGuitar/AcousticGuitar_C#4",
                "Audio/Instruments/Melodic/AcousticGuitar/AcousticGuitar_D4",
                "Audio/Instruments/Melodic/AcousticGuitar/AcousticGuitar_D#4",
                "Audio/Instruments/Melodic/AcousticGuitar/AcousticGuitar_E5"
            } },

		//Melodic.ClassicalGuitar
		{ "ClassicalGuitar", new List<string> () {
                "Audio/Instruments/Melodic/ClassicalGuitar/ClassicalGuitar_E2",
                "Audio/Instruments/Melodic/ClassicalGuitar/ClassicalGuitar_F2",
                "Audio/Instruments/Melodic/ClassicalGuitar/ClassicalGuitar_F#2",
                "Audio/Instruments/Melodic/ClassicalGuitar/ClassicalGuitar_G2",
                "Audio/Instruments/Melodic/ClassicalGuitar/ClassicalGuitar_G#2",
                "Audio/Instruments/Melodic/ClassicalGuitar/ClassicalGuitar_A2",
                "Audio/Instruments/Melodic/ClassicalGuitar/ClassicalGuitar_A#2",
                "Audio/Instruments/Melodic/ClassicalGuitar/ClassicalGuitar_B2",
                "Audio/Instruments/Melodic/ClassicalGuitar/ClassicalGuitar_C3",
                "Audio/Instruments/Melodic/ClassicalGuitar/ClassicalGuitar_C#3",
                "Audio/Instruments/Melodic/ClassicalGuitar/ClassicalGuitar_D3",
                "Audio/Instruments/Melodic/ClassicalGuitar/ClassicalGuitar_D#3",
                "Audio/Instruments/Melodic/ClassicalGuitar/ClassicalGuitar_E3",
                "Audio/Instruments/Melodic/ClassicalGuitar/ClassicalGuitar_F3",
                "Audio/Instruments/Melodic/ClassicalGuitar/ClassicalGuitar_F#3",
                "Audio/Instruments/Melodic/ClassicalGuitar/ClassicalGuitar_G3",
                "Audio/Instruments/Melodic/ClassicalGuitar/ClassicalGuitar_G#3",
                "Audio/Instruments/Melodic/ClassicalGuitar/ClassicalGuitar_A3",
                "Audio/Instruments/Melodic/ClassicalGuitar/ClassicalGuitar_A#3",
                "Audio/Instruments/Melodic/ClassicalGuitar/ClassicalGuitar_B3",
                "Audio/Instruments/Melodic/ClassicalGuitar/ClassicalGuitar_C4",
                "Audio/Instruments/Melodic/ClassicalGuitar/ClassicalGuitar_C#4",
                "Audio/Instruments/Melodic/ClassicalGuitar/ClassicalGuitar_D4",
                "Audio/Instruments/Melodic/ClassicalGuitar/ClassicalGuitar_D#4",
                "Audio/Instruments/Melodic/ClassicalGuitar/ClassicalGuitar_E5"
            } },

		//Melodic.PipeOrgan
		{ "PipeOrgan", new List<string> () {
                "Audio/Instruments/Melodic/PipeOrgan/PipeOrgan_A2",
                "Audio/Instruments/Melodic/PipeOrgan/PipeOrgan_A#2",
                "Audio/Instruments/Melodic/PipeOrgan/PipeOrgan_B2",
                "Audio/Instruments/Melodic/PipeOrgan/PipeOrgan_C3",
                "Audio/Instruments/Melodic/PipeOrgan/PipeOrgan_C#3",
                "Audio/Instruments/Melodic/PipeOrgan/PipeOrgan_D3",
                "Audio/Instruments/Melodic/PipeOrgan/PipeOrgan_D#3",
                "Audio/Instruments/Melodic/PipeOrgan/PipeOrgan_E3",
                "Audio/Instruments/Melodic/PipeOrgan/PipeOrgan_F3",
                "Audio/Instruments/Melodic/PipeOrgan/PipeOrgan_F#3",
                "Audio/Instruments/Melodic/PipeOrgan/PipeOrgan_G3",
                "Audio/Instruments/Melodic/PipeOrgan/PipeOrgan_G#3",
                "Audio/Instruments/Melodic/PipeOrgan/PipeOrgan_A3",
                "Audio/Instruments/Melodic/PipeOrgan/PipeOrgan_A#3",
                "Audio/Instruments/Melodic/PipeOrgan/PipeOrgan_B3",
                "Audio/Instruments/Melodic/PipeOrgan/PipeOrgan_C4",
                "Audio/Instruments/Melodic/PipeOrgan/PipeOrgan_C#4",
                "Audio/Instruments/Melodic/PipeOrgan/PipeOrgan_D4",
                "Audio/Instruments/Melodic/PipeOrgan/PipeOrgan_D#4",
                "Audio/Instruments/Melodic/PipeOrgan/PipeOrgan_E4",
                "Audio/Instruments/Melodic/PipeOrgan/PipeOrgan_F4",
                "Audio/Instruments/Melodic/PipeOrgan/PipeOrgan_F#4",
                "Audio/Instruments/Melodic/PipeOrgan/PipeOrgan_G4",
                "Audio/Instruments/Melodic/PipeOrgan/PipeOrgan_G#4",
                "Audio/Instruments/Melodic/PipeOrgan/PipeOrgan_A4",
                "Audio/Instruments/Melodic/PipeOrgan/PipeOrgan_A#4",
                "Audio/Instruments/Melodic/PipeOrgan/PipeOrgan_B4",
                "Audio/Instruments/Melodic/PipeOrgan/PipeOrgan_C5",
                "Audio/Instruments/Melodic/PipeOrgan/PipeOrgan_C#5",
                "Audio/Instruments/Melodic/PipeOrgan/PipeOrgan_D5",
                "Audio/Instruments/Melodic/PipeOrgan/PipeOrgan_D#5",
                "Audio/Instruments/Melodic/PipeOrgan/PipeOrgan_E5",
                "Audio/Instruments/Melodic/PipeOrgan/PipeOrgan_F5",
                "Audio/Instruments/Melodic/PipeOrgan/PipeOrgan_F#5",
                "Audio/Instruments/Melodic/PipeOrgan/PipeOrgan_G5",
                "Audio/Instruments/Melodic/PipeOrgan/PipeOrgan_G#5",
                "Audio/Instruments/Melodic/PipeOrgan/PipeOrgan_A5",
                "Audio/Instruments/Melodic/PipeOrgan/PipeOrgan_A#5",
                "Audio/Instruments/Melodic/PipeOrgan/PipeOrgan_B5",
                "Audio/Instruments/Melodic/PipeOrgan/PipeOrgan_C6",
                "Audio/Instruments/Melodic/PipeOrgan/PipeOrgan_C#6"
            } },

		//Melodic.Keyboard
		{ "Keyboard", new List<string> () {
                "Audio/Instruments/Melodic/Keyboard/Keyboard_A3",
                "Audio/Instruments/Melodic/Keyboard/Keyboard_A#3",
                "Audio/Instruments/Melodic/Keyboard/Keyboard_B3",
                "Audio/Instruments/Melodic/Keyboard/Keyboard_C4",
                "Audio/Instruments/Melodic/Keyboard/Keyboard_C#4",
                "Audio/Instruments/Melodic/Keyboard/Keyboard_D4",
                "Audio/Instruments/Melodic/Keyboard/Keyboard_D#4",
                "Audio/Instruments/Melodic/Keyboard/Keyboard_E4",
                "Audio/Instruments/Melodic/Keyboard/Keyboard_F4",
                "Audio/Instruments/Melodic/Keyboard/Keyboard_F#4",
                "Audio/Instruments/Melodic/Keyboard/Keyboard_G4",
                "Audio/Instruments/Melodic/Keyboard/Keyboard_G#4",
                "Audio/Instruments/Melodic/Keyboard/Keyboard_A4",
                "Audio/Instruments/Melodic/Keyboard/Keyboard_A#4",
                "Audio/Instruments/Melodic/Keyboard/Keyboard_B4",
                "Audio/Instruments/Melodic/Keyboard/Keyboard_C5",
                "Audio/Instruments/Melodic/Keyboard/Keyboard_C#5",
                "Audio/Instruments/Melodic/Keyboard/Keyboard_D5",
                "Audio/Instruments/Melodic/Keyboard/Keyboard_D#5",
                "Audio/Instruments/Melodic/Keyboard/Keyboard_E5",
                "Audio/Instruments/Melodic/Keyboard/Keyboard_F5",
                "Audio/Instruments/Melodic/Keyboard/Keyboard_F#5",
                "Audio/Instruments/Melodic/Keyboard/Keyboard_G5",
                "Audio/Instruments/Melodic/Keyboard/Keyboard_G#5",
                "Audio/Instruments/Melodic/Keyboard/Keyboard_A5",
                "Audio/Instruments/Melodic/Keyboard/Keyboard_A#5",
                "Audio/Instruments/Melodic/Keyboard/Keyboard_B5",
                "Audio/Instruments/Melodic/Keyboard/Keyboard_C6",
                "Audio/Instruments/Melodic/Keyboard/Keyboard_C#6",
                "Audio/Instruments/Melodic/Keyboard/Keyboard_D6",
                "Audio/Instruments/Melodic/Keyboard/Keyboard_D#6",
                "Audio/Instruments/Melodic/Keyboard/Keyboard_E6",
                "Audio/Instruments/Melodic/Keyboard/Keyboard_F6",
                "Audio/Instruments/Melodic/Keyboard/Keyboard_F#6",
                "Audio/Instruments/Melodic/Keyboard/Keyboard_G6",
                "Audio/Instruments/Melodic/Keyboard/Keyboard_A6",
                "Audio/Instruments/Melodic/Keyboard/Keyboard_A#6",
                "Audio/Instruments/Melodic/Keyboard/Keyboard_B6",
                "Audio/Instruments/Melodic/Keyboard/Keyboard_C7",
                "Audio/Instruments/Melodic/Keyboard/Keyboard_C#7"

            } },

		//Melodic.Trumpet
		{ "Trumpet", new List<string> () {
                "Audio/Instruments/Melodic/Trumpet/Trumpet_A3",
                "Audio/Instruments/Melodic/Trumpet/Trumpet_A#3",
                "Audio/Instruments/Melodic/Trumpet/Trumpet_B3",
                "Audio/Instruments/Melodic/Trumpet/Trumpet_C4",
                "Audio/Instruments/Melodic/Trumpet/Trumpet_C#4",
                "Audio/Instruments/Melodic/Trumpet/Trumpet_D4",
                "Audio/Instruments/Melodic/Trumpet/Trumpet_D#4",
                "Audio/Instruments/Melodic/Trumpet/Trumpet_E4",
                "Audio/Instruments/Melodic/Trumpet/Trumpet_F4",
                "Audio/Instruments/Melodic/Trumpet/Trumpet_F#4",
                "Audio/Instruments/Melodic/Trumpet/Trumpet_G4",
                "Audio/Instruments/Melodic/Trumpet/Trumpet_G#4",
                "Audio/Instruments/Melodic/Trumpet/Trumpet_A4",
                "Audio/Instruments/Melodic/Trumpet/Trumpet_A#4",
                "Audio/Instruments/Melodic/Trumpet/Trumpet_B4",
                "Audio/Instruments/Melodic/Trumpet/Trumpet_C5",
                "Audio/Instruments/Melodic/Trumpet/Trumpet_C#5",
                "Audio/Instruments/Melodic/Trumpet/Trumpet_D5",
                "Audio/Instruments/Melodic/Trumpet/Trumpet_D#5",
                "Audio/Instruments/Melodic/Trumpet/Trumpet_E5",
                "Audio/Instruments/Melodic/Trumpet/Trumpet_F5",
                "Audio/Instruments/Melodic/Trumpet/Trumpet_F#5",
                "Audio/Instruments/Melodic/Trumpet/Trumpet_G5",
                "Audio/Instruments/Melodic/Trumpet/Trumpet_G#5",
                "Audio/Instruments/Melodic/Trumpet/Trumpet_A5",
                "Audio/Instruments/Melodic/Trumpet/Trumpet_A#5",
                "Audio/Instruments/Melodic/Trumpet/Trumpet_B5",
                "Audio/Instruments/Melodic/Trumpet/Trumpet_C6"

            } },
		

		// Percussion.RockDrums
		{ "RockDrums", new List<string> () {
            "Audio/Instruments/Percussion/RockDrums/RockDrums_Kick",
            "Audio/Instruments/Percussion/RockDrums/RockDrums_Snare",
            "Audio/Instruments/Percussion/RockDrums/RockDrums_LowTom",
            "Audio/Instruments/Percussion/RockDrums/RockDrums_MidTom",
            "Audio/Instruments/Percussion/RockDrums/RockDrums_HiTom",
            "Audio/Instruments/Percussion/RockDrums/RockDrums_Hat",
            "Audio/Instruments/Percussion/RockDrums/RockDrums_Crash"
            }
        },

		// Percussion.ExoticPercussion
		{ "ExoticPercussion", new List<string> () {
                "Audio/Instruments/Percussion/ExoticPercussion/ExoticPercussion_Castinets",
                "Audio/Instruments/Percussion/ExoticPercussion/ExoticPercussion_Claves",
                "Audio/Instruments/Percussion/ExoticPercussion/ExoticPercussion_Cowbell",
                "Audio/Instruments/Percussion/ExoticPercussion/ExoticPercussion_Cowbell2",
                "Audio/Instruments/Percussion/ExoticPercussion/ExoticPercussion_JamBlock",
                "Audio/Instruments/Percussion/ExoticPercussion/ExoticPercussion_Maracas1",
                "Audio/Instruments/Percussion/ExoticPercussion/ExoticPercussion_Maracas2",
                "Audio/Instruments/Percussion/ExoticPercussion/ExoticPercussion_Tambourine"
            }
        }
    };
    }
}
