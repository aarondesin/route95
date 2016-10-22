// ReverbLevelSlider.cs
// ©2016 Team 95

namespace Route95.UI {

    /// <summary>
    /// Class to handle the riff editor reverb effect level slider.
    /// </summary>
    public class ReverbLevelSlider : EffectSlider {

        /// <summary>
        /// Inits the slider.
        /// </summary>
        public override void Initialize() {
            UpdateSlider(RiffEditor.CurrentRiff.reverbLevel / 2000f);
        }

        /// <summary>
        /// Changes the current riff's reverb effect level.
        /// </summary>
        public override void ChangeValue() {
            RiffEditor.CurrentRiff.reverbLevel = slider.value * 2000f;
        }
    }
}