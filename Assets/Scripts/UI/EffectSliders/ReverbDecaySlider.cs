// ReverbDecaySlider.cs
// ©2016 Team 95

namespace Route95.UI {

    /// <summary>
    /// Class to handle the riff editor reverb effect decay slider.
    /// </summary>
    public class ReverbDecaySlider : EffectSlider {

        /// <summary>
        /// Inits the slider.
        /// </summary>
        public override void Initialize() {
            UpdateSlider(RiffEditor.CurrentRiff.reverbDecayTime / 20f);
        }

        /// <summary>
        /// Changes the current riff's reverb effect decay time.
        /// </summary>
        public override void ChangeValue() {
            RiffEditor.CurrentRiff.reverbDecayTime = slider.value * 20f;
        }
    }
}