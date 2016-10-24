// LoadingBar.cs
// ©2016 Team 95

using UnityEngine.UI;

namespace Route95.UI {

    /// <summary>
    /// Class to handle the loading screen progress bar.
    /// </summary>
    public class LoadingBar : SingletonMonoBehaviour<LoadingBar> {

        #region LoadingBar Vars

        /// <summary>
        /// Slider component of this loading bar.
        /// </summary>
        Slider _slider;

        #endregion
        #region Unity Callbacks

        new void Awake () {
            base.Awake();

            // Init vars
            _slider = GetComponent<Slider>();
        }

        #endregion
        #region Methods

        /// <summary>
        /// Updates the loading bar progress value.
        /// </summary>
        /// <param name="p">Progress.</param>
        public void UpdateProgress (float p) {
            _slider.value = p;
        }

        #endregion
    }
}
