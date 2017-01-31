// ChorusRateSlider.cs
// ©2016 Team 95

namespace Route95.UI {

    /// <summary>
    /// Class to handle the chorus effect rate slider.
    /// </summary>
    public class ChorusRateSlider : EffectSlider {

        public override float Max { get { return 20f; } }

        public override float Min { get { return 0.1f; } }

        public override void OnSliderChanged(float value) {
            RiffEditor.CurrentRiff.ChorusRate = _slider.value;
        }

        public override void UpdateSlider() {
            _slider.value = RiffEditor.CurrentRiff.ChorusRate;
        }
    }
}
