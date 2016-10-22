// EchoDryMixSlider.cs
// ©2016 Team 95

namespace Route95.UI {

    /// <summary>
    /// Class to handle the riff editor echo effect dry mix slider.
    /// </summary>
    public class EchoDryMixSlider : EffectSlider {

        /// <summary>
        /// Inits the slider.
        /// </summary>
        public override void Initialize() {
            UpdateSlider(RiffEditor.CurrentRiff.echoDryMix);
        }

        /// <summary>
        /// Changes the current riff's echo effect dry mix.
        /// </summary>
        public override void ChangeValue() {
            RiffEditor.CurrentRiff.echoDryMix = slider.value;
        }
    }
}