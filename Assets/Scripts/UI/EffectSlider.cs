// EffectSlider.cs
// ©2016 Team 95

using UnityEngine;
using UnityEngine.UI;

namespace Route95.UI {

    /// <summary>
    /// Base class for all effect sliders.
    /// </summary>
    public abstract class EffectSlider : MonoBehaviour {

        #region Vars

        /// <summary>
        /// Slider component on this object.
        /// </summary>
        protected Slider _slider;

        #endregion
        #region Unity Callbacks

        void Awake () {
            // Init vars
            _slider = GetComponent<Slider>();
            _slider.maxValue = Max;
            _slider.minValue = Min;
            _slider.value = Min;
            _slider.onValueChanged.AddListener(OnSliderChanged);
        }

        #endregion
        #region Methods

        /// <summary>
        /// Returns the maximum value of the slider/effect.
        /// </summary>
        public abstract float Max { get; }

        /// <summary>
        /// Returns the minimum value of the slider/effect.
        /// </summary>
        public abstract float Min { get; }

        /// <summary>
        /// Called when the slider is changed.
        /// </summary>
        public abstract void OnSliderChanged(float value);

        /// <summary>
        /// Changes the slider.
        /// </summary>
        public abstract void UpdateSlider();

        #endregion
    }
}
