// TremoloDepthSlider.cs
// ©2016 Team 95

namespace Route95.UI {

    /// <summary>
    /// Class to handle the tremolo effect depth slider.
    /// </summary>
    public class TremoloDepthSlider : EffectSlider {

        public override float Max { get { return 1f; } }

        public override float Min { get { return 0f; } }

        public override void OnSliderChanged(float value) {
            RiffEditor.CurrentRiff.TremoloDepth = _slider.value;
        }

        public override void UpdateSlider() {
            _slider.value = RiffEditor.CurrentRiff.TremoloDepth;
        }
    }
}
