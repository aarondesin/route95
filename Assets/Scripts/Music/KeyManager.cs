﻿// KeyManager.cs
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
        IEnumerator BuildScales() {

            // Update loading message
            LoadingScreen.Instance.SetLoadingMessage("Loading scales...");

            // Mark start time
            float startTime = Time.realtimeSinceStartup;
            int numLoaded = 0;

            // Build percussion sets
            _percussionSets = new Dictionary<Instrument, List<string>>() {
            { PercussionInstrument.RockDrums, Sounds.SoundsToLoad["RockDrums"] },
            { PercussionInstrument.ExoticPercussion, Sounds.SoundsToLoad["ExoticPercussion"] }
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
                        if (instrument.InstrumentType == Instrument.Type.Percussion) continue;

                        // Add instrument to sscale mapping
                        _scales[key][scale].Add(
                            (MelodicInstrument)instrument, BuildScale(Sounds.SoundsToLoad[instrument.CodeName],
                                scale, ((MelodicInstrument)instrument).StartingNote(key))
                        );

                        numLoaded++;

                        // If over time, skip until next frame
                        if (Time.realtimeSinceStartup - startTime > GameManager.LoadingTargetDeltaTime) {
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
        public static Scale BuildScale(List<string> soundFiles, ScaleInfo scale, int startIndex) {
            Scale result = new Scale();
            int i = startIndex;
            try {
                while (i < soundFiles.Count) {
                    // Add root/octave
                    result.AddRoot(soundFiles[i]);

                    // Add second
                    i += scale.SecondInterval;
                    result.AddSecond(soundFiles[i]);

                    // Add third
                    i += scale.ThirdInterval;
                    result.AddThird(soundFiles[i]);

                    // Add fourth
                    i += scale.FourthInterval;
                    result.AddFourth(soundFiles[i]);

                    // Add fifth
                    i += scale.FifthInterval;
                    result.AddFifth(soundFiles[i]);

                    // Add sixth
                    i += scale.SixthInterval;
                    result.AddSixth(soundFiles[i]);

                    // Add seventh
                    i += scale.SeventhInterval;
                    result.AddSeventh(soundFiles[i]);

                    // Go to next octave
                    i += scale.RootInterval;
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
        public Scale GetScale(Key key, ScaleInfo scale,
            MelodicInstrument inst) {
            return _scales[key][scale][inst];
        }

        /// <summary>
        /// Returns the note set for a percussion instrument.
        /// </summary>
        /// <param name="inst"></param>
        public List<string> GetNoteSet(PercussionInstrument inst) {
            return _percussionSets[inst];
        }

        #endregion

    }
}
