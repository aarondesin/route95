using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class KeyManager : MonoBehaviour {

	public static KeyManager instance;

	#region KeyManager Vars

	public Dictionary<Key, Dictionary<ScaleInfo, Dictionary<MelodicInstrument, Scale>>> scales;
	public Dictionary<Instrument, List<string>> percussionSets;

	#endregion
	#region Unity Callbacks

	void Start () {
		instance = this;
	}

	#endregion
	#region KeyManager Methods

	public void DoBuildScales () {
		//Debug.Log("buildscales");
		StartCoroutine ("BuildScales");
	}

	//public void BuildScales () {
	IEnumerator BuildScales () {
		GameManager.instance.ChangeLoadingMessage("Loading scales...");
		float startTime = Time.realtimeSinceStartup;
		int numLoaded = 0;

		percussionSets = new Dictionary<Instrument, List<string>>() {
			{ PercussionInstrument.RockDrums, Sounds.soundsToLoad["RockDrums"] },
			{ PercussionInstrument.ExoticPercussion, Sounds.soundsToLoad["ExoticPercussion"] }
		};

		scales = new Dictionary<Key, Dictionary<ScaleInfo, Dictionary<MelodicInstrument, Scale>>>();
		foreach (Key key in Enum.GetValues(typeof(Key))) {

			if (key != Key.None) {
				scales.Add (key, new Dictionary<ScaleInfo, Dictionary<MelodicInstrument, Scale>>());
				foreach (ScaleInfo scale in ScaleInfo.AllScales) {

					scales[key].Add (scale, new Dictionary<MelodicInstrument, Scale>());
					foreach (Instrument instrument in Instrument.AllInstruments) {
						if (instrument.type == Instrument.Type.Melodic) {
							scales[key][scale].Add (
								(MelodicInstrument)instrument, BuildScale (Sounds.soundsToLoad[instrument.codeName], 
									scale, ((MelodicInstrument)instrument).startingNote[key])
							);
							//Debug.Log("Scale["+key.ToString()+"]["+scale.name+"]["+instrument.codeName+"]");
							numLoaded++;

							if (Time.realtimeSinceStartup - startTime > 1f/GameManager.instance.targetFrameRate) {
								yield return null;
								startTime = Time.realtimeSinceStartup;
								GameManager.instance.ReportLoaded(numLoaded);

								numLoaded = 0;
							}
						}
					}

				}
			}

		}
		MusicManager.instance.FinishLoading();
		yield return null;

	}

	public static Scale BuildScale (List<string> soundFiles, ScaleInfo scale, int startIndex) {
		Scale result = new Scale ();
		int i = startIndex;
		try {
			while (i < soundFiles.Count) {
				// Add root/octave
				result.root.Add (soundFiles [i]);
				result.allNotes.Add (soundFiles[i]);

				// Add second
				i += scale.secondIndex;
				result.second.Add (soundFiles [i]);
				result.allNotes.Add (soundFiles[i]);

				// Add third
				i += scale.thirdIndex;
				result.third.Add (soundFiles [i]);
				result.allNotes.Add (soundFiles[i]);

				// Add fourth
				i += scale.fourthIndex;
				result.fourth.Add (soundFiles [i]);
				result.allNotes.Add (soundFiles[i]);

				// Add fifth
				i += scale.fifthIndex;
				result.fifth.Add (soundFiles [i]);
				result.allNotes.Add (soundFiles[i]);

				// Add sixth
				i += scale.sixthIndex;
				result.sixth.Add (soundFiles [i]);
				result.allNotes.Add (soundFiles[i]);

				// Add seventh
				i += scale.seventhIndex;
				result.seventh.Add (soundFiles [i]);
				result.allNotes.Add (soundFiles[i]);

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

	#endregion
	  	  
}
	