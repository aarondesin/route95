// ChorusDryMixSlider.cs
// ©2016 Team 95

namespace Route95.UI {

    /// <summary>
    /// Class to handle the riff editor chorus effect dry mix slider.
    /// </summary>
    public class ChorusDryMixSlider : EffectSlider {

        /// <summary>
        /// Initializes the slider.
        /// </summary>
        public override void Initialize() {
            UpdateSlider(RiffEditor.CurrentRiff.chorusDryMix);
        }

        /// <summary>
        /// Changes the current riff's chorus effect dry mix.
        /// </summary>
        public override void ChangeValue() {
            RiffEditor.CurrentRiff.chorusDryMix = slider.value;
        }
    }
}