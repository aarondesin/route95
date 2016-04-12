﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class KeyManager : MonoBehaviour {

	public static KeyManager instance;

	public Dictionary<Key, Dictionary<Instrument, Scale>> scales;

	void Start () {
		instance = this;

	}

	public void BuildScales () {
		scales = new Dictionary<Key, Dictionary<Instrument, Scale>> () { 
			// All E minor scales
			{ Key.Eminor, new Dictionary<Instrument,Scale> () {
					{ Instrument.ElectricGuitar, BuildScale (Sounds.soundsToLoad["ElectricGuitar"], ScaleInfo.Minor, 0 ) },
					{ Instrument.ElectricBass, BuildScale (Sounds.soundsToLoad["ElectricBass"], ScaleInfo.Minor, 0) },
					{ Instrument.AcousticGuitar, BuildScale (Sounds.soundsToLoad["AcousticGuitar"], ScaleInfo.Minor, 0) },
					{ Instrument.ClassicalGuitar, BuildScale (Sounds.soundsToLoad["ClassicalGuitar"], ScaleInfo.Minor, 0) },
					{ Instrument.PipeOrgan, BuildScale (Sounds.soundsToLoad["PipeOrgan"], ScaleInfo.Minor,0 ) },
					{ Instrument.Keyboard, BuildScale (Sounds.soundsToLoad["Keyboard"], ScaleInfo.Minor, 0) }
				}
			},

			// All C major scales
			{ Key.CMajor, new Dictionary<Instrument,Scale> () {
					{ Instrument.ElectricGuitar, BuildScale (Sounds.soundsToLoad["ElectricGuitar"], ScaleInfo.Major, 8 ) },
					{ Instrument.ElectricBass, BuildScale (Sounds.soundsToLoad["ElectricBass"], ScaleInfo.Major, 8) },
					{ Instrument.AcousticGuitar, BuildScale (Sounds.soundsToLoad["AcousticGuitar"], ScaleInfo.Major, 8) },
					{ Instrument.ClassicalGuitar, BuildScale (Sounds.soundsToLoad["ClassicalGuitar"], ScaleInfo.Major, 8) },
					{ Instrument.PipeOrgan, BuildScale (Sounds.soundsToLoad["PipeOrgan"], ScaleInfo.Major,8 ) },
					{ Instrument.Keyboard, BuildScale (Sounds.soundsToLoad["Keyboard"], ScaleInfo.Major, 8) }

				}
			},

			// All F# minor scales
			{ Key.FSharpMinor, new Dictionary<Instrument,Scale> () {
					{ Instrument.ElectricGuitar, BuildScale (Sounds.soundsToLoad["ElectricGuitar"], ScaleInfo.Minor, 2 ) },
					{ Instrument.ElectricBass, BuildScale (Sounds.soundsToLoad["ElectricBass"], ScaleInfo.Minor, 2) },
					{ Instrument.AcousticGuitar, BuildScale (Sounds.soundsToLoad["AcousticGuitar"], ScaleInfo.Minor, 2) },
					{ Instrument.ClassicalGuitar, BuildScale (Sounds.soundsToLoad["ClassicalGuitar"], ScaleInfo.Minor, 2) },
					{ Instrument.PipeOrgan, BuildScale (Sounds.soundsToLoad["PipeOrgan"], ScaleInfo.Minor,2 ) },
					{ Instrument.Keyboard, BuildScale (Sounds.soundsToLoad["Keyboard"], ScaleInfo.Minor, 2) }
				}
			},

			// All D minor scales
			{ Key.DMinor, new Dictionary<Instrument,Scale> () {
					{ Instrument.ElectricGuitar, BuildScale (Sounds.soundsToLoad["ElectricGuitar"], ScaleInfo.Minor, 10 ) },
					{ Instrument.ElectricBass, BuildScale (Sounds.soundsToLoad["ElectricBass"], ScaleInfo.Minor, 10) },
					{ Instrument.AcousticGuitar, BuildScale (Sounds.soundsToLoad["AcousticGuitar"], ScaleInfo.Minor, 10) },
					{ Instrument.ClassicalGuitar, BuildScale (Sounds.soundsToLoad["ClassicalGuitar"], ScaleInfo.Minor, 10) },
					{ Instrument.PipeOrgan, BuildScale (Sounds.soundsToLoad["PipeOrgan"], ScaleInfo.Minor, 10 ) },
					{ Instrument.Keyboard, BuildScale (Sounds.soundsToLoad["Keyboard"], ScaleInfo.Minor, 10) }
				}
			},

			// All D# minor scales
			{ Key.DSharpMinor, new Dictionary<Instrument,Scale> () {
					{ Instrument.ElectricGuitar, BuildScale (Sounds.soundsToLoad["ElectricGuitar"], ScaleInfo.Minor, 11 ) },
					{ Instrument.ElectricBass, BuildScale (Sounds.soundsToLoad["ElectricBass"], ScaleInfo.Minor, 11) },
					{ Instrument.AcousticGuitar, BuildScale (Sounds.soundsToLoad["AcousticGuitar"], ScaleInfo.Minor, 11) },
					{ Instrument.ClassicalGuitar, BuildScale (Sounds.soundsToLoad["ClassicalGuitar"], ScaleInfo.Minor, 11) },
					{ Instrument.PipeOrgan, BuildScale (Sounds.soundsToLoad["PipeOrgan"], ScaleInfo.Minor,11 ) },
					{ Instrument.Keyboard, BuildScale (Sounds.soundsToLoad["Keyboard"], ScaleInfo.Minor, 11) }
				}
			},
		};
	}

	public static Scale BuildScale (List<string> soundFiles, ScaleInfo scale, int startIndex) {
		Scale result = new Scale ();
		int i = startIndex;
		try {
			while (i < soundFiles.Count) {
				// Add root/octave
				result.root.Add (soundFiles [i]);
				result.allNotes.Add (soundFiles[i]);
				//Debug.Log("Adding "+soundFiles[i]);

				// Add second
				i += scale.secondIndex;
				result.second.Add (soundFiles [i]);
				result.allNotes.Add (soundFiles[i]);
				//Debug.Log("Adding "+soundFiles[i]);

				// Add third
				i += scale.thirdIndex;
				result.third.Add (soundFiles [i]);
				result.allNotes.Add (soundFiles[i]);
				//Debug.Log("Adding "+soundFiles[i]);

				// Add fourth
				i += scale.fourthIndex;
				result.fourth.Add (soundFiles [i]);
				result.allNotes.Add (soundFiles[i]);
				//Debug.Log("Adding "+soundFiles[i]);

				// Add fifth
				i += scale.fifthIndex;
				result.fifth.Add (soundFiles [i]);
				result.allNotes.Add (soundFiles[i]);
				//Debug.Log("Adding "+soundFiles[i]);

				// Add sixth
				i += scale.sixthIndex;
				result.sixth.Add (soundFiles [i]);
				result.allNotes.Add (soundFiles[i]);
				//Debug.Log("Adding "+soundFiles[i]);

				// Add seventh
				i += scale.seventhIndex;
				result.seventh.Add (soundFiles [i]);
				result.allNotes.Add (soundFiles[i]);
				//Debug.Log("Adding "+soundFiles[i]);

				// Go to next octave
				i += scale.rootIndex;
			}
			return result;
		} catch (ArgumentOutOfRangeException) {
			return result;
		} catch (NullReferenceException n) {
			Debug.LogError (n);
			return result;
		}
	}
	  	  
	 //scale = scales[MM.currentKey][riff.currentInstrument];
}

/*public class Scale{
		public Note root1;
		public Note second1;
		public Note third1;
		public Note fourth1;
		public Note fifth1;
		public Note sixth1;
		public Note seventh1;

		public Note root2;
		public Note second2;
		public Note third2;
		public Note fourth2;
		public Note fifth2;
		public Note sixth2;
		public Note seventh2;

		public Note root3;
		public Note second3;
		public Note third3;
		public Note fourth3;
		public Note fifth3;
		public Note sixth3;
		public Note seventh3;


		public Note root4;
		public Note second4;
		public Note third4;
		public Note fourth4;
		public Note fifth4;
		public Note sixth4;
		public Note seventh4;

		public List<Note> notes = new List<Note>();

	}*/

/*
BuildScale (List<string> sounds, Scale.Minor, int startIndex) {
	int i=rootIndex;
	while (i < souds.Count) {
		scale.root.Add(sounds[i]);
		scale.second.Add(sounds[i+secondIndex]);
		scale.third.Add(sounds[i+thirdIndex]);
		scale.fourth.Add(sounds[i+fourthIndex]);
		scale.fifth.Add(sounds[i+fifthIndex]);
		scale.sixth.Add(sounds[i+sixthIndex]);
		i+=seventhIndex;
	}
}

*/
/*public static Scale ElectricGuitarEminor = new Scale () {

root1 = new Note ("ElectricGuitar_E2"),
second1 = new Note ("ElectricGuitar_F#2"),
third1 = new Note("ElectricGuitar_G2"),
fourth1 = new Note ("ElectricGuitar_A2"),
fifth1 = new Note ("ElectricGuitar_B2"),
sixth1 = new Note("ElectricGuitar_C3"),
seventh1 = new Note ("ElectricGuitar_D3"),

root2 = new Note ("ElectricGuitar_E3"),
second2 = new Note ("ElectricGuitar_F#3"),
third2 = new Note("ElectricGuitar_G3"),
fourth2 = new Note ("ElectricGuitar_A3"),
fifth2 = new Note ("ElectricGuitar_B3"),
sixth2 = new Note("ElectricGuitar_C4"),
seventh2 = new Note ("ElectricGuitar_D4"),

root3 = new Note ("ElectricGuitar_E4"),
second3 = new Note ("ElectricGuitar_F#4"),
third3 = new Note("ElectricGuitar_G4"),
fourth3 = new Note ("ElectricGuitar_A4"),
fifth3 = new Note ("ElectricGuitar_B4"),
sixth3 = new Note("ElectricGuitar_C5"),
seventh3 = new Note ("ElectricGuitar_D5"),

root4 = new Note ("ElectricGuitar_E5"),
second4 = new Note ("ElectricGuitar_F#5"),
third4 = new Note("ElectricGuitar_G5"),
fourth4 = new Note ("ElectricGuitar_A5"),
fifth4 = new Note ("ElectricGuitar_B5"),
sixth4 = new Note("ElectricGuitar_C6")


	};*/