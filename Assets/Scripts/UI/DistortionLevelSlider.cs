// DistortionLevelSlider.cs
// ©2016 Team 95

namespace Route95.UI {

    /// <summary>
    /// Class to handle the distortion effect level slider.
    /// </summary>
    public class DistortionLevelSlider : EffectSlider {

        public override float Max { get { return 0.9f; } }

        public override float Min { get { return 0f; } }

        public override void OnSliderChanged(float value) {
            RiffEditor.CurrentRiff.DistortionLevel = _slider.value;
        }

        public override void UpdateSlider() {
            _slider.value = RiffEditor.CurrentRiff.DistortionLevel;
        }
    }
}
