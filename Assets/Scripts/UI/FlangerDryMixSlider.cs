// FlangerDryMixSlider.cs
// ©2016 Team 95

namespace Route95.UI {

    using UnityEngine;

    /// <summary>
    /// Class to handle the flanger effect dry mix slider.
    /// </summary>
    public class FlangerDryMixSlider : EffectSlider {

        public override float Max { get { return 1f; } }

        public override float Min { get { return 0f; } }

        public override void OnSliderChanged(float value) {
            RiffEditor.CurrentRiff.FlangerDryMix = _slider.value;
        }

        public override void UpdateSlider() {
            _slider.value = RiffEditor.CurrentRiff.FlangerDryMix;
        }
    }
}
