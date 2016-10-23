// DraggableButton.cs
// ©2016 Team 95

using UnityEngine;

namespace Route95.UI {

    /// <summary>
    /// Enables a button to be draggable.
    /// </summary>
    public abstract class DraggableButton : MonoBehaviour {

        #region DraggableButton Vars

        /// <summary>
        /// Maximum drag distance upwards.
        /// </summary>
        [SerializeField]
        protected float _maxDragDistanceUp;

        /// <summary>
        /// Maximum drag distance downwards.
        /// </summary>
        [SerializeField]
        protected float _maxDragDistanceDown;

        /// <summary>
        /// Maximum drag distance to the left.
        /// </summary>
        [SerializeField]
        protected float _maxDragDistanceLeft;

        /// <summary>
        /// Maximum drag distance to the right.
        /// </summary>
        [SerializeField]
        protected float _maxDragDistanceRight;

        /// <summary>
        /// Threshold for drag to recognize one direction over another.
        /// </summary>
        const float _DRAG_BIAS = 1.25f;

        #endregion
        #region DraggableButton Methods

        /// <summary>
        /// Calls the appropriate function based on drag direction.
        /// </summary>
        /// <param name="dragVector">Vector of mouse dragging.</param>
        public void Drag(Vector3 dragVector) {
            float hDrag = Mathf.Abs(dragVector.x);
            float yDrag = Mathf.Abs(dragVector.y);

            if (dragVector.x < 0f) DragLeft(Mathf.Clamp01(hDrag / _maxDragDistanceLeft));
            else DragRight(Mathf.Clamp01(hDrag / _maxDragDistanceRight));
            if (dragVector.y < 0f) DragDown(Mathf.Clamp01(yDrag / _maxDragDistanceDown));
            else DragUp(Mathf.Clamp01(yDrag / _maxDragDistanceUp));
        }

        /// <summary>
        /// Called when the mouse button is pressed on this object.
        /// </summary>
        public abstract void OnMouseDown();

        /// <summary>
        /// Called when the mouse button is released on this object.
        /// </summary>
        public abstract void OnMouseUp();

        /// <summary>
        /// Called when dragged left.
        /// </summary>
        public abstract void DragLeft(float actionRatio);

        /// <summary>
        /// Called when dragged right.
        /// </summary>
        public abstract void DragRight(float actionRatio);

        /// <summary>
        /// Called when dragged down.
        /// </summary>
        public abstract void DragDown(float actionRatio);

        /// <summary>
        /// Called when dragged up.
        /// </summary>
        public abstract void DragUp(float actionRation);

        #endregion
    }
}
