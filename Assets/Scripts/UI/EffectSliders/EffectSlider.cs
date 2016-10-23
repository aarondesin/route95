// EffectSlider.cs
// ©2016 Team 95

using UnityEngine;
using UnityEngine.UI;

namespace Route95.UI {

    /// <summary>
    /// Class to handle instrument effect value sliders.
    /// </summary>
    public abstract class EffectSlider : MonoBehaviour {

        #region Vars

        /// <summary>
        /// Slider to use.
        /// </summary>
        protected Slider _slider;

        #endregion
        #region Unity Callbacks

        void Awake () {
            // Init vars
            _slider = GetComponentInChildren<Slider>();
        }

        #endregion
        #region Methods

        /// <summary>
        /// Initializes the effect slider.
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Called when the value of the slider is changed.
        /// </summary>
        public abstract void ChangeValue();

        /// <summary>
        /// Changes the value of the slider.
        /// </summary>
        public void UpdateSlider(float value) {
            _slider.value = value;
        }

        #endregion
    }
}


