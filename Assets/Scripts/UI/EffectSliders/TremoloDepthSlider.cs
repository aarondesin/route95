// TremoloDepthSlider.cs
// ©2016 Team 95

namespace Route95.UI {

    /// <summary>
    /// Class to handle the riff editor tremolo effect depth slider.
    /// </summary>
    public class TremoloDepthSlider : EffectSlider {

        /// <summary>
        /// Inits the slider.
        /// </summary>
        public override void Initialize() {
            UpdateSlider(RiffEditor.CurrentRiff.tremoloDepth);
        }

        /// <summary>
        /// Changes the current riff's tremolo effect depth.
        /// </summary>
        public override void ChangeValue() {
            RiffEditor.CurrentRiff.tremoloDepth = slider.value;
        }
    }
}