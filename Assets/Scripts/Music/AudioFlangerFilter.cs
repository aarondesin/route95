// AudioFlangerFilter.cs
// ©2016 Team 95

using System;
using System.Collections.Generic;

using UnityEngine;

namespace Route95.Music {

    /// <summary>
    /// Custom audio flanger filter.
    /// </summary>
    public class AudioFlangerFilter : MonoBehaviour {

        #region AudioFlangerFilter Vars

        /// <summary>
        /// Rate at which to oscillate flanger.
        /// </summary>
        [Tooltip("Rate at which to oscillate flanger.")]
        [Range(Mathf.PI / 32f, Mathf.PI / 16f)]
        public float rate = Mathf.PI / 32f;

        /// <summary>
        /// Ratio of dry/wet signal.
        /// </summary>
        [Tooltip("Ratio of dry/wet signal.")]
        [Range(0f, 1f)]
        public float dryMix = 0.5f;

        /// <summary>
        /// Current signal delay.
        /// </summary>
        [Tooltip("Current signal delay.")]
        [Range(0.005f, 0.025f)]
        [SerializeField]
        float _delay = 0.005f;

        /// <summary>
        /// Current oscillation theta.
        /// </summary>
        float _r = 0f;

        /// <summary>
        /// Time between frames.
        /// </summary>
        float _time;

        /// <summary>
        /// Old audio data arrays.
        /// </summary>
        List<float[]> _oldDatas;

        /// <summary>
        /// Array of mixed old audio inputs.
        /// </summary>
        float[] _mixed;

        /// <summary>
        /// Length of old data arrays.
        /// </summary>
        int _len;

        #endregion
        #region Unity Callbacks

        void FixedUpdate() {

            // Add rate
            _r += rate;

            // Wrap r if above 2PI
            if (_r > Mathf.PI * 2f) _r -= (Mathf.PI * 2f);

            // Oscillate delay
            _delay = 0.015f + 0.01f * Mathf.Sin(_r);

        }

        void OnEnable() {
            _r = 0f;
            _time = 0.02f * Application.targetFrameRate;
        }

        public void OnAudioFilterRead(float[] data, int channels) {

            // Initllist of old data if necessary
            if (_oldDatas == null) _oldDatas = new List<float[]>();

            // Add up to 5 old audio inputs
            while (_oldDatas.Count < 5) _oldDatas.Add(data);

            // Mix old signals
            MixSignals();

            // Copy raw audio data
            float[] copy = new float[_len];

            // Mix old and new signals
            float oneMinusMix = 1f - dryMix;
            for (int i = 0; i < data.Length; i++) {
                copy[i] = data[i] * dryMix + _mixed[i] * oneMinusMix * 0.95f;
                data[i] = copy[i];
            }

            // Remove oldest data
            _oldDatas.RemoveAt(0);

            // Add current data
            _oldDatas.Add(copy);
        }

        #endregion
        #region AudioFlangerFilter Methods

        /// <summary>
        /// Mixes current audio signal with past signals.
        /// </summary>
        void MixSignals() {

            // Get length of audio signal
            _len = _oldDatas[0].Length;

            // Init mixed array if necessary
            if (_mixed == null) _mixed = new float[_len];

            // Calculate mixing value between old inputs
            float val = _delay / _time;

            // Get indices of old inputs to use
            int hi = Mathf.CeilToInt(val);
            int lo = Mathf.FloorToInt(val);

            // Stop if indices are invalid
            if (hi < 0 || lo < 0 || hi >= 5 || lo >= 5) return;

            // Mix old inputs
            float mix = (val - (float)lo);
            float oneMinusMix = 1f - mix;

            // Populate mixed array
            for (int i = 0; i < _len; i++) {
                float high = _oldDatas[hi][i];
                float low = _oldDatas[lo][i];

                try {
                    _mixed[i] = (high * mix) + (low * oneMinusMix);
                }
                catch (IndexOutOfRangeException e) {
                    Debug.LogError("i: " + i + " len: " + _len + e.Message);
                }
            }

        }

        #endregion
    }
}
