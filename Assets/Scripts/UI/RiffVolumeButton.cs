// RiffVolumeButton.cs
// ©2016 Team 95

using Route95.Music;

using UnityEngine;
using UnityEngine.UI;

namespace Route95.UI {

    /// <summary>
    /// Special use of DraggableButton for riff volume slider.
    /// </summary>
    public class RiffVolumeButton : DraggableButton {

        #region RiffVolumeButton Vars

        /// <summary>
        /// Vertical drag distance.
        /// </summary>
        const float V_DRAG_DISTANCE = 128f;

        /// <summary>
        /// Horizontal drag distance.
        /// </summary>
        const float H_DRAG_DISTANCE = 128f;

        /// <summary>
        /// Copy of old note volume.
        /// </summary>
        float _oldVolume;

        /// <summary>
        /// Target riff to edit.
        /// </summary>
        Riff _targetRiff;

        #endregion
        #region Unity Callbacks

        void Awake() {
            // Init vars
            _maxDragDistanceUp = V_DRAG_DISTANCE;
            _maxDragDistanceDown = V_DRAG_DISTANCE;
            _maxDragDistanceLeft = H_DRAG_DISTANCE;
            _maxDragDistanceRight = H_DRAG_DISTANCE;
        }

        #endregion
        #region DraggableButton Overrides

        public override void OnMouseDown() {
            _targetRiff = RiffEditor.CurrentRiff;
            _oldVolume = _targetRiff.Volume;
        }

        public override void DragDown(float actionRatio) {
            _targetRiff.Volume = Mathf.Clamp01(_oldVolume - actionRatio);
            gameObject.GetComponent<Image>().fillAmount = _targetRiff.Volume;
        }

        public override void DragUp(float actionRatio) {
            _targetRiff.Volume = Mathf.Clamp01(_oldVolume + actionRatio);
            gameObject.GetComponent<Image>().fillAmount = _targetRiff.Volume;
        }

        #endregion
    }
}
