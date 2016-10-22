// ChorusRateSlider.cs
// ©2016 Team 95

namespace Route95.UI {

    /// <summary>
    /// Class to handle the riff editor chorus effect rate slider.
    /// </summary>
    public class ChorusRateSlider : EffectSlider {

        /// <summary>
        /// Inits the slider.
        /// </summary>
        public override void Initialize() {
            UpdateSlider(RiffEditor.CurrentRiff.chorusRate);
        }

        /// <summary>
        /// Changes the current riff's chorus effect rate.
        /// </summary>
        public override void ChangeValue() {
            RiffEditor.CurrentRiff.chorusRate = slider.value;
        }
    }
}