// Blinking.cs
// ©2016 Team 95

using UnityEngine;
using UnityEngine.UI;

namespace Route95.UI {

    /// <summary>
    /// Class to blink a sprite.
    /// </summary>
    public class Blinking : MonoBehaviour {

        #region Blinking Vars

        /// <summary>
        /// The color of the image at the peak of its blink cycle.
        /// </summary>
        [Tooltip("The color of the image at the peak of its blink cycle.")]
        [SerializeField]
        Color _peakColor;

        /// <summary>
        /// The rate at which the image blinks.
        /// </summary>
        [Tooltip("The rate at which the image blinks.")]
        [Range(0.01f, 0.5f)]
        [SerializeField]
        float _blinkInterval;

        /// <summary>
        /// The depth of the alpha dip.
        /// </summary>
        [Tooltip("The depth of the alpha dip.")]
        [Range(0f, 1f)]
        [SerializeField]
        float _blinkDepth = 0.5f;

        /// <summary>
        /// Current progress of the blinking.
        /// </summary>
        [Range(0f, 2f * Mathf.PI)]
        float _progress = 0f;

        /// <summary>
        /// Two times pi.
        /// </summary>
        float _TWOPI;

        /// <summary>
        /// Reference to this object's graphic.
        /// </summary>
        MaskableGraphic _graphic;

        #endregion
        #region Unity Callbacks

        void Awake () {
            // Init vars
            _TWOPI = 2f * Mathf.PI;
            _graphic = GetComponent<MaskableGraphic>();
        }

        void Update() {
            if (_progress >= _TWOPI) _progress -= _TWOPI;
            Color color = _peakColor;
            color.a = 1f - _blinkDepth + _blinkDepth * Mathf.Sin(_progress);
            _graphic.color = color;
            _progress += _blinkInterval;
        }

        #endregion
    }
}