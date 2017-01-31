// FlangerRateSlider.cs
// ©2016 Team 95

namespace Route95.UI {

    using UnityEngine;

    /// <summary>
    /// Class to handle the flanger effect rate slider.
    /// </summary>
    public class FlangerRateSlider : EffectSlider {

        float _min = Mathf.PI / 32f;
        float _max = Mathf.PI / 16f;

        public override float Max { get { return _max; } }

        public override float Min { get { return _min; } }

        public override void OnSliderChanged(float value) {
            RiffEditor.CurrentRiff.FlangerRate = _slider.value;
        }

        public override void UpdateSlider() {
            _slider.value = RiffEditor.CurrentRiff.FlangerRate;
        }
    }
}
