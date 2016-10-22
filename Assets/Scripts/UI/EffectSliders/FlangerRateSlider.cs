// FlangerRateSlider.cs
// ©2016 Team 95

using UnityEngine;

namespace Route95.UI {

    /// <summary>
    /// Class to handle the riff editor flanger effect rate slider.
    /// </summary>
    public class FlangerRateSlider : EffectSlider {

        /// <summary>
        /// Inits the slider.
        /// </summary>
        public override void Initialize() {
            UpdateSlider(RiffEditor.CurrentRiff.flangerRate / (Mathf.PI * 32f) - Mathf.PI / 32f);
        }

        /// <summary>
        /// Changes the current riff's flanger effect rate.
        /// </summary>
        public override void ChangeValue() {
            RiffEditor.CurrentRiff.flangerRate = Mathf.PI / 32f + Mathf.PI / 32f * slider.value;
        }
    }
}
