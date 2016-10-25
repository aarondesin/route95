// IconSpinner.cs
// ©2016 Team 95

using System.Collections.Generic;

using UnityEngine;

namespace Route95.UI {

    /// <summary>
    /// Class to handle an animated icon spinner effect.
    /// </summary>
    public class IconSpinner : MonoBehaviour {

        #region IconSpinner Vars

        /// <summary>
        /// List of images to move.
        /// </summary>
        [Tooltip("List of images to move.")]
        [SerializeField]
        List<RectTransform> _images;

        /// <summary>
        /// Radius of spinning area.
        /// </summary>
        [Tooltip("Radius of spinning area.")]
        [SerializeField]
        float _spinRadius;

        /// <summary>
        /// Spinning speed.
        /// </summary>
        [Tooltip("Spinning speed.")]
        [SerializeField]
        float _spinRate;

        /// <summary>
        /// Number of images.
        /// </summary>
        int _imageCount;

        /// <summary>
        /// Current theta.
        /// </summary>
        float _r = 0f;

        /// <summary>
        /// Two times pi.
        /// </summary>
        float _TWOPI = Mathf.PI * 2f;

        /// <summary>
        /// Angle per icon;
        /// </summary>
        float _c;

        #endregion
        #region Unity Callbacks

        void Awake() {
            // Get number of images
            _imageCount = _images.Count;
            _c = _TWOPI / (float)_imageCount;
        }

        void Update() {
            // Update theta
            _r += _spinRate * Time.deltaTime;

            // Move all images
            for (int i = 0; i < _imageCount; i++) {
                float newAngle = _r + (float)i * _c;
                _images[i].anchoredPosition3D =
                    new Vector3(Mathf.Cos(newAngle), Mathf.Sin(newAngle), 0f) * _spinRadius;
            }
        }

        #endregion
    }
}
