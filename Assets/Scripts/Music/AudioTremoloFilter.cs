// AudioTremoloFilter.cs
// ©2016 Team 95

using UnityEngine;

namespace Route95.Music {

    /// <summary>
    /// Custom audio tremolo filter.
    /// </summary>
    public class AudioTremoloFilter : MonoBehaviour {

        #region AudioTremoloFilter Vars

        /// <summary>
        /// Tremolo oscillation rate.
        /// </summary>
        [Tooltip("Tremolo oscillation rate.")]
        [Range(Mathf.PI / 32f, Mathf.PI / 16f)]
        public float rate = Mathf.PI / 32f;

        /// <summary>
        /// Tremolo oscillation depth.
        /// </summary>
        [Tooltip("Tremolo oscillation depth.")]
        [Range(0f, 1f)]
        public float depth;

        /// <summary>
        /// Current oscillation theta.
        /// </summary>
        [Tooltip("Current oscillation theta.")]
        [SerializeField]
        [Range(0f, Mathf.PI * 2f)]
        float _r;

        #endregion
        #region Unity Callbacks

        void FixedUpdate() {

            // Add rate to r
            _r += rate;

            // Wrap r if past 2PI
            if (_r > Mathf.PI * 2f) _r -= (Mathf.PI * 2f);
        }

        void OnEnable() {

            // Start r at 0
            _r = 0f;
        }

        void OnDisable() {

            // Reset volume while disabled
            GetComponent<AudioSource>().volume = 1f;
        }

        void OnAudioFilterRead(float[] data, int channels) {

            // Multiply data amplitude by current cos value
            for (int i = 0; i < data.Length; i++)
                data[i] = data[i] * (1f - (1f - depth) / 2f + 0.5f * Mathf.Cos(_r));
        }

        #endregion
    }
}
