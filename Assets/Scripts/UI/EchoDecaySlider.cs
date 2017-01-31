// EchoDecaySlider.cs
// ©2016 Team 95

namespace Route95.UI {

    /// <summary>
    /// Class to handle the echo effect decay slider.
    /// </summary>
    public class EchoDecaySlider : EffectSlider {

        public override float Max { get { return 0.99f; } }

        public override float Min { get { return 0f; } }

        public override void OnSliderChanged(float value) {
            RiffEditor.CurrentRiff.EchoDecay = _slider.value;
        }

        public override void UpdateSlider() {
            _slider.value = RiffEditor.CurrentRiff.EchoDecay;
        }
    }
}
