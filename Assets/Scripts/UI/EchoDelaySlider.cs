// EchoDelaySlider.cs
// ©2016 Team 95

namespace Route95.UI {

    /// <summary>
    /// Class to handle the echo effect delay slider.
    /// </summary>
    public class EchoDelaySlider : EffectSlider {

        public override float Max { get { return 2500f; } }

        public override float Min { get { return 10f; } }

        public override void OnSliderChanged(float value) {
            RiffEditor.CurrentRiff.EchoDelay = _slider.value;
        }

        public override void UpdateSlider() {
            _slider.value = RiffEditor.CurrentRiff.EchoDelay;
        }
    }
}
