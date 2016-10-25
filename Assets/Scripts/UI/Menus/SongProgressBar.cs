using Route95.Core;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Route95.UI {

    /// <summary>
    /// Class to handle the live mode song progress bar.
    /// </summary>
    public class SongProgressBar : MenuBase<SongProgressBar> {

        #region SongProgressBar Vars

        [Tooltip("Reference to background image.")]
        public Image background;

        [Tooltip("Reference to bar image.")]
        public Image bar;

        float value;                            // Current song progress value

        #endregion
        #region Unity Callbacks

        void Update() {
            RectTransform bgtr = background.gameObject.GetComponent<RectTransform>();
            RectTransform batr = bar.gameObject.GetComponent<RectTransform>();
            batr.sizeDelta = new Vector2(value * bgtr.rect.width, bgtr.rect.height);
            batr.anchoredPosition3D = new Vector3(
                    batr.sizeDelta.x / 2f,
                    0f,
                    0f
            );
        }

        #endregion
        #region SongProgressBar Methods

        /// <summary>
        /// Set the song progress bar value.
        /// </summary>
        /// <param name="v">New value.</param>
        public void SetValue(float v) {
            value = Mathf.Clamp01(v);
        }

        #endregion
    }
}
