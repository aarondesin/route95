// LiveInstrumentIcons.cs
// ©2016 Team 95

using UnityEngine;

namespace Route95.UI {

    /// <summary>
    /// Class to handle instrument icon panel in live mode.
    /// </summary>
    public class LiveInstrumentIcons : MenuBase<LiveInstrumentIcons> {
        #region Vars

        /// <summary>
        /// How long to wait before fading instrument icons.
        /// </summary>
        [Tooltip("How long to wait before fading the instrument icons.")]
        [SerializeField]
        float _fadeWaitTime;

        /// <summary>
        /// How quickly to fade instrument icons.
        /// </summary>
        [Tooltip("How quickly to fade the instrument icons.")]
        [SerializeField]
        float _fadeSpeed;

        /// <summary>
        /// Time until fade starts.
        /// </summary>
        float _fadeTimer;

        /// <summary>
        /// Live instrument icon canvas group.
        /// </summary>
        CanvasGroup _canvasGroup;

        #endregion
        #region Unity Callbacks

        new void Awake () {
			base.Awake();

            // Init vars
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        void Update () {
            if (_fadeTimer > 0f) {
                _fadeTimer -= Time.deltaTime;
            } else if (_canvasGroup.alpha > 0f) {
                float alpha = _canvasGroup.alpha - _fadeSpeed * Time.deltaTime;
                _canvasGroup.alpha = Mathf.Clamp01 (alpha);
            }
        }

        #endregion
        #region Methods

        /// <summary>
        /// Wakes the live UI.
        /// </summary>
        public void WakeLiveUI() {
            _fadeTimer = _fadeWaitTime;
            _canvasGroup.alpha = 1f;
        }

        #endregion
    }
}
