// DistortionLevelSlider.cs
// ©2016 Team 95

namespace Route95.UI {

    /// <summary>
    /// Class to handle the riff editor distortion effect level slider.
    /// </summary>
    public class DistortionLevelSlider : EffectSlider {

        /// <summary>
        /// Inits the slider.
        /// </summary>
        public override void Initialize() {
            UpdateSlider(RiffEditor.CurrentRiff.distortionLevel);
        }

        /// <summary>
        /// Changes the current riff's distortion effect level.
        /// </summary>
        public override void ChangeValue() {
            RiffEditor.CurrentRiff.distortionLevel = slider.value;
        }
    }
}