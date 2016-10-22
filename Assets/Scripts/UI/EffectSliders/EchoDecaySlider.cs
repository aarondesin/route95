// EchoDecaySlider.cs
// ©2016 Team 95

namespace Route95.UI {

    /// <summary>
    /// Class to handle the riff editor echo effect decay slider.
    /// </summary>
    public class EchoDecaySlider : EffectSlider {

        /// <summary>
        /// Inits the slider.
        /// </summary>
        public override void Initialize() {
            UpdateSlider(RiffEditor.CurrentRiff.echoDecayRatio / 0.99f);
        }

        /// <summary>
        /// Changes the current riff's echo effect decay.
        /// </summary>
        public override void ChangeValue() {
            RiffEditor.CurrentRiff.echoDecayRatio = slider.value * 0.99f;
        }
    }
}