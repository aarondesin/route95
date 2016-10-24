using Route95.Core;
using Route95.Music;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Route95.UI {

    /// <summary>
    /// Special use of DraggableButton for riff volume slider.
    /// </summary>
    public class RiffVolumeButton : DraggableButton {

        #region RiffVolumeButton Vars

        const float vDragDistance = 128f;
        const float hDragDistance = 128f;

        float oldVolume; // copy of old note volume

        Riff targetRiff; // riff to edit

        #endregion
        #region Unity Callbacks

        void Awake() {
            _maxDragDistanceUp = vDragDistance;
            _maxDragDistanceDown = vDragDistance;
            _maxDragDistanceLeft = hDragDistance;
            _maxDragDistanceRight = hDragDistance;
        }

        #endregion
        #region DraggableButton Overrides

        public override void OnMouseDown() {
            targetRiff = RiffEditor.CurrentRiff;
            oldVolume = targetRiff.Volume;
        }

        public override void DragDown(float actionRatio) {
            targetRiff.Volume = Mathf.Clamp01(oldVolume - actionRatio);
            gameObject.Image().fillAmount = targetRiff.Volume;
        }

        public override void DragUp(float actionRatio) {
            targetRiff.Volume = Mathf.Clamp01(oldVolume + actionRatio);
            gameObject.Image().fillAmount = targetRiff.Volume;
        }

        #endregion
    }
}
