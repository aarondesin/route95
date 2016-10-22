// EchoDelaySlider.cs
// ©2016 Team 95

using UnityEngine;

namespace Route95.UI {

    /// <summary>
    /// Class to handle the riff editor echo effect delay slider.
    /// </summary>
    public class EchoDelaySlider : EffectSlider {

        /// <summary>
        /// Minimum echo delay (steps).
        /// </summary>
        [Tooltip("Minimum echo delay (steps).")]
        [SerializeField]
        float _min = 10f;

        /// <summary>
        /// Maximum echo delay (steps).
        /// </summary>
        [Tooltip("Maximum echo delay (steps).")]
        [SerializeField]
        float _max = 1500f;

        /// <summary>
        /// Inits the slider.
        /// </summary>
        public override void Initialize() {
            UpdateSlider((RiffEditor.CurrentRiff.echoDelay - _min) / (_max - _min));
        }

        /// <summary>
        /// Changes the current riff's echo effect delay.
        /// </summary>
        public override void ChangeValue() {
            float val = (_max - _min) * slider.value + _min;
            RiffEditor.CurrentRiff.echoDelay = val;
        }
    }
}