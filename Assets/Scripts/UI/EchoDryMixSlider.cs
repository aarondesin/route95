// EchoDryMixSlider.cs
// ©2016 Team 95

namespace Route95.UI {

    /// <summary>
    /// Class to handle the echo effect dry mix slider.
    /// </summary>
    public class EchoDryMixSlider : EffectSlider {

        public override float Max { get { return 1f; } }

        public override float Min { get { return 0f; } }

        public override void OnSliderChanged(float value) {
            RiffEditor.CurrentRiff.EchoDryMix = _slider.value;
        }

        public override void UpdateSlider() {
            _slider.value = RiffEditor.CurrentRiff.EchoDryMix;
        }
    }
}
