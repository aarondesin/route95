// TremoloRateSlider.cs
// ©2016 Team 95

using UnityEngine;

namespace Route95.UI {

    /// <summary>
    /// Class to handle the riff editor tremolo effect rate slider.
    /// </summary>
    public class TremoloRateSlider : EffectSlider {

        /// <summary>
        /// Inits the slider.
        /// </summary>
        public override void Initialize() {
            UpdateSlider(RiffEditor.CurrentRiff.tremoloRate - (Mathf.PI / 32f) / (Mathf.PI / 32f));
        }

        /// <summary>
        /// Changes the current riff's tremolo effect rate.
        /// </summary>
        public override void ChangeValue() {
            RiffEditor.CurrentRiff.tremoloRate = (Mathf.PI / 32f) + slider.value * (Mathf.PI / 32f);
        }
    }
}