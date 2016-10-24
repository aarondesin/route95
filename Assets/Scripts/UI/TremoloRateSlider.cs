// TremoloRateSlider.cs
// ©2016 Team 95

using UnityEngine;

namespace Route95.UI {

    /// <summary>
    /// Class to handle the tremolo effect rate slider.
    /// </summary>
    public class TremoloRateSlider : EffectSlider {

        float _min = Mathf.PI / 32f;
        float _max = Mathf.PI / 16f;

        public override float Max { get { return _max; } }

        public override float Min { get { return _min; } }

        public override void OnSliderChanged(float value) {
            RiffEditor.CurrentRiff.TremoloRate = _slider.value;
        }

        public override void UpdateSlider() {
            _slider.value = RiffEditor.CurrentRiff.TremoloRate;
        }
    }
}
