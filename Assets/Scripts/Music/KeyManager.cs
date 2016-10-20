// KeyManager.cs
// ©2016 Team 95

using Route95.Core;
using Route95.UI;

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Route95.Music {

    /// <summary>
    /// Instanced MonoBehaviour to handle all key to scale to 
    /// note operations.
    /// </summary>
    public class KeyManager : SingletonMonoBehaviour<KeyManager> {

        #region KeyManager Vars

        // Mappings of keys to scales to instruments
        public Dictionary<Key, 
            Dictionary<ScaleInfo, 
                Dictionary<MelodicInstrument, Scale>>> _scales;

        // Mapping of percussion instruments to notes
        Dictionary<Instrument, List<string>> _percussionSets;

        #endregion
        #region KeyManager Methods

        /// <summary>
        /// Begins building scales.
        /// </summary>
        public void DoBuildScales() {
            StartCoroutine("BuildScales");
        }

        /// <summary>
        /// Coroutine to build scales.
        /// </summary>
        /// <returns></returns>
        IEnumerator BuildScales() {

            // Update loading message
            LoadingScreen.Instance.SetLoadingMessage("Loading scales...");

            // Mark start time
            float startTime = Time.realtimeSinceStartup;
            int numLoaded = 0;

            // Build percussion sets
            _percussionSets = new Dictionary<Instrument, List<string>>() {
            { PercussionInstrument.RockDrums, Sounds.soundsToLoad["RockDrums"] },
            { PercussionInstrument.ExoticPercussion, Sounds.soundsToLoad["ExoticPercussion"] }
        };

            // Init scales dict
            _scales = new Dictionary<Key, Dictionary<ScaleInfo, Dictionary<MelodicInstrument, Scale>>>();

            // For reach key
            foreach (Key key in Enum.GetValues(typeof(Key))) {

                // Skip Key.None
                if (key == Key.None) continue;

                // Add key to dictionary mapping
                _scales.Add(key, new Dictionary<ScaleInfo, Dictionary<MelodicInstrument, Scale>>());

                // For reach scale type
                foreach (ScaleInfo scale in ScaleInfo.AllScales) {

                    // Add scale to melodic instrument mapping
                    _scales[key].Add(scale, new Dictionary<MelodicInstrument, Scale>());

                    // For each instrument
                    foreach (Instrument instrument in Instrument.AllInstruments) {

                        // Skip percussion instruments
                        if (instrument.type == Instrument.Type.Percussion) continue;

                        // Add instrument to sscale mapping
                        _scales[key][scale].Add(
                            (MelodicInstrument)instrument, BuildScale(Sounds.soundsToLoad[instrument.codeName],
                                scale, ((MelodicInstrument)instrument).startingNote[key])
                        );

                        numLoaded++;

                        // If over time, skip until next frame
                        if (Time.realtimeSinceStartup - startTime > GameManager.TargetDeltaTime) {
                            yield return null;
                            startTime = Time.realtimeSinceStartup;
                            GameManager.Instance.ReportLoaded(numLoaded);

                            numLoaded = 0;
                        }

                    }
                }
            }

            // Finish loading
            MusicManager.Instance.FinishLoading();
            yield return null;

        }

        /// <summary>
        /// Builds a scale.
        /// </summary>
        /// <param name="soundFiles">List of sounds to draw from.</param>
        /// <param name="scale">Scale type.</param>
        /// <param name="startIndex">Index of sound to start with.</param>
        /// <returns>A scale.</returns>
        public static Scale BuildScale(List<string> soundFiles, ScaleInfo scale, int startIndex) {
            Scale result = new Scale();
            int i = startIndex;
            try {
                while (i < soundFiles.Count) {
                    // Add root/octave
                    result.AddRoot(soundFiles[i]);

                    // Add second
                    i += scale.secondIndex;
                    result.AddSecond(soundFiles[i]);

                    // Add third
                    i += scale.thirdIndex;
                    result.AddThird(soundFiles[i]);

                    // Add fourth
                    i += scale.fourthIndex;
                    result.AddFourth(soundFiles[i]);

                    // Add fifth
                    i += scale.fifthIndex;
                    result.AddFifth(soundFiles[i]);

                    // Add sixth
                    i += scale.sixthIndex;
                    result.AddSixth(soundFiles[i]);

                    // Add seventh
                    i += scale.seventhIndex;
                    result.AddSeventh(soundFiles[i]);

                    // Go to next octave
                    i += scale.rootIndex;
                }
                return result;
            }
            catch (ArgumentOutOfRangeException) {
                return result;
            }
            catch (NullReferenceException n) {
                Debug.LogError(n);
                return result;
            }
        }

        /// <summary>
        /// Returns the scale with the given criteria.
        /// </summary>
        /// <param name="key">Musical key.</param>
        /// <param name="scale">Musical scale.</param>
        /// <param name="inst">Melodic instrument.</param>
        public Scale GetScale (Key key, ScaleInfo scale, 
            MelodicInstrument inst) 
        {
            return _scales[key][scale][inst];
        }

        public List<string> GetNoteSet(PercussionInstrument inst) {
            return _percussionSets[inst];
        }

        #endregion

    }
}
