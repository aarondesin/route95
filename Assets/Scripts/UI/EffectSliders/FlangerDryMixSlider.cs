// FlangerDryMixSlider.cs
// ©2016 Team 95

namespace Route95.UI {

    /// <summary>
    /// Class to handle the riff editor flanger effect dry mix slider.
    /// </summary>
    public class FlangerDryMixSlider : EffectSlider {

        /// <summary>
        /// Inits the slider.
        /// </summary>
        public override void Initialize() {
            UpdateSlider((RiffEditor.CurrentRiff.flangerDryMix + 1f) / 2f);
        }

        /// <summary>
        /// Changes the current riff's flanger effect dry mix.
        /// </summary>
        public override void ChangeValue() {
            RiffEditor.CurrentRiff.flangerDryMix = slider.value * 2f - 1f;
        }
    }
}