// EffectSlider.cs
// ©2016 Team 95

using UnityEngine;
using UnityEngine.UI;

namespace Route95.UI {

    public abstract class EffectSlider : MonoBehaviour {

        protected Slider _slider;

        void Awake () {
            _slider = GetComponent<Slider>();

            _slider.maxValue = Max;
            _slider.minValue = Min;
            _slider.value = Min;

            _slider.onValueChanged.AddListener(OnSliderChanged);
        }

        public abstract float Max { get; }

        public abstract float Min { get; }

        public abstract void OnSliderChanged(float value);

        public abstract void UpdateSlider();
    }
}
