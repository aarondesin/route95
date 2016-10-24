// ReverbLevelSlider.cs
// ©2016 Team 95

namespace Route95.UI {

    /// <summary>
    /// Class to handle the reverb effect decay slider.
    /// </summary>
    public class ReverbLevelSlider : EffectSlider {

        public override float Max { get { return 1000f; } }

        public override float Min { get { return -1000f; } }

        public override void OnSliderChanged(float value) {
            RiffEditor.CurrentRiff.ReverbLevel = _slider.value;
        }

        public override void UpdateSlider() {
            _slider.value = RiffEditor.CurrentRiff.ReverbLevel;
        }
    }
}
