// NoteButton.cs
// ©2016 Team 95

using System;
using Route95.Music;

using UnityEngine;
using UnityEngine.UI;

namespace Route95.UI {

    /// <summary>
    /// A special use of DraggableButton for the riff editor not buttons.
    /// </summary>
    public class NoteButton : DraggableButton {

        #region NoteButton Vars

        const int VISIBLE_NOTES = 10;
        const float V_DRAG_DISTANCE = 128f;
        const float H_DRAG_DISTANCE = 128f;

        public Note targetNote;
        public Image volumeImage;

        float oldVolume;
        #endregion
        #region Unity Callbacks

        void Awake() {
            _maxDragDistanceUp = V_DRAG_DISTANCE;
            _maxDragDistanceDown = V_DRAG_DISTANCE;
            _maxDragDistanceLeft = H_DRAG_DISTANCE;
            _maxDragDistanceRight = H_DRAG_DISTANCE;
        }

        #endregion
        #region DraggableButton Overrides

        public override void OnMouseDown() {
            oldVolume = targetNote.Volume;
        }

        public override void DragDown(float actionRatio) {
            targetNote.Volume = Mathf.Clamp01(oldVolume - actionRatio);
            UpdateButtonArt();
        }

        public override void DragUp(float actionRatio) {
            targetNote.Volume = Mathf.Clamp01(oldVolume + actionRatio);
            UpdateButtonArt();
        }

        public override void DragLeft(float actionRatio) {
            targetNote.Duration = 1f;
        }

        public override void DragRight(float actionRatio) {
            targetNote.Duration = 1 + (float)(VISIBLE_NOTES - 1) * actionRatio;
        }

        #endregion
        #region Methods

        /// <summary>
        /// Updates the button's fill amount and color.
        /// </summary>
        public void UpdateButtonArt() {
            volumeImage.fillAmount = targetNote.Volume;
            volumeImage.color = new Color(0.75f * targetNote.Volume + 0.25f, 0.25f, 0.25f, 1f);
        }

        #endregion
    }
}
