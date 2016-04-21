using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class KeyManager : MonoBehaviour {

	public static KeyManager instance;

	public Dictionary<Key, Dictionary<ScaleInfo, Dictionary<Instrument, Scale>>> scales;
	public Dictionary<Instrument, List<string>> percussionSets;

	void Start () {
		instance = this;

	}

	public void BuildScales () {
		percussionSets = new Dictionary<Instrument, List<string>>() {
			{ Instrument.RockDrums, Sounds.soundsToLoad["RockDrums"] },
			{ Instrument.ExoticPercussion, Sounds.soundsToLoad["ExoticPercussion"] }
		};

		scales = new Dictionary<Key, Dictionary<ScaleInfo, Dictionary<Instrument, Scale>>>();
		foreach (Key key in Enum.GetValues(typeof(Key))) {
			//Debug.Log(key.ToString());

			if (key != Key.None) {
				scales.Add (key, new Dictionary<ScaleInfo, Dictionary<Instrument, Scale>>());
				foreach (ScaleInfo scale in ScaleInfo.AllScales) {
					//Debug.Log(scale.scaleIndex);
					
					scales[key].Add (scale, new Dictionary<Instrument, Scale>());
					foreach (Instrument instrument in Instrument.AllInstruments) {
						if (instrument.type == InstrumentType.Melodic) {
							//Debug.Log(instrument.codeName);
							scales[key][scale].Add (instrument, BuildScale (Sounds.soundsToLoad[instrument.codeName], scale, instrument.startingNote[key]));
						}
					}

				}
			}

		}

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
	  	  
}
	