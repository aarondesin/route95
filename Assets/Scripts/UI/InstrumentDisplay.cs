// InstrumentDisplay.cs
// ©2016 Team 95

using Route95.Core;
using Route95.Music;

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Route95.UI {

    /// <summary>
    /// Class to handle instrument display on the back of the car.
    /// </summary>
    public class InstrumentDisplay : SingletonMonoBehaviour<InstrumentDisplay> {

        #region InstrumentDisplay Vars

        /// <summary>
        /// Reference to icon.
        /// </summary>
        Image _icon;

        /// <summary>
        /// Sprite to change for glow.
        /// </summary>
        Image _glow;

        /// <summary>
        /// Speed of fade.
        /// </summary>
        [SerializeField]
        float _fadeSpeed;

        /// <summary>
        /// Instrument icons glows.
        /// </summary>
        List<Fadeable> _glows;

        #endregion
        #region Unity Callbacks

        new void Awake () {
            base.Awake();

            // Init vars
            _icon = GetComponent<Image>();
        }

        void FixedUpdate() {

            if (GameManager.Instance.CurrentState != GameManager.State.Live) return;
            if (GameManager.Instance.Paused) return;

            Color color = _glow.color;
            color.a -= _fadeSpeed;
            _glow.color = color;

        }

        #endregion
        #region InstrumentDisplay Methods

        /// <summary>
        /// Refreshes the display, changing art if necessary.
        /// </summary>
        public void Refresh() {
            _icon.sprite = MusicManager.Instance.CurrentInstrument.Icon;
            _glow.sprite = MusicManager.Instance.CurrentInstrument.Glow;
        }

        /// <summary>
        /// Sets glow to full.
        /// </summary>
        public void WakeGlow() {
            Color color = _glow.color;
            color.a = 1f;
            _glow.color = color;
        }

        /// <summary>
        /// Wakes a particular glow.
        /// </summary>
        public void WakeGlow(int index) {
            _glows[index].UnFade();
        }

        /// <summary>
        /// Fades a particular glow.
        /// </summary>
        public void FadeGlow(int index) {
            _glows[index].Fade();
        }

        #endregion
    }
}
