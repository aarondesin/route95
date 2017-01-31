// ReverbDecaySlider.cs
// ©2016 Team 95

namespace Route95.UI {

    /// <summary>
    /// Class to handle the reverb effect decay slider.
    /// </summary>
    public class ReverbDecaySlider : EffectSlider {

        public override float Max { get { return 10f; } }

        public override float Min { get { return 0.1f; } }

        public override void OnSliderChanged(float value) {
            RiffEditor.CurrentRiff.ReverbDecay = _slider.value;
        }

        public override void UpdateSlider() {
            _slider.value = RiffEditor.CurrentRiff.ReverbDecay;
        }
    }
}
