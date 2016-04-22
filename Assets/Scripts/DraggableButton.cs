using UnityEngine;
using System.Collections;

public class DraggableButton : MonoBehaviour {

	public float maxDragDistanceUp;
	public float maxDragDistanceDown;
	public float maxDragDistanceLeft;
	public float maxDragDistanceRight;

	const float dragBias = 1.25f; // threshold for drag to recognize one direction over another

	public void Drag (Vector3 dragVector) {
		float hDrag = Mathf.Abs(dragVector.x);
		float yDrag = Mathf.Abs(dragVector.y);

		if (hDrag > yDrag * dragBias) {
			if (dragVector.x < 0f) DragLeft (Mathf.Clamp01(hDrag/maxDragDistanceLeft));
			else DragRight (Mathf.Clamp01(hDrag/maxDragDistanceRight));
		} else if (yDrag > hDrag * dragBias) {
			if (dragVector.y < 0f) DragDown (Mathf.Clamp01(yDrag/maxDragDistanceDown));
			else DragUp (Mathf.Clamp01(yDrag/maxDragDistanceUp));
		}
	}

	public virtual void DragLeft (float actionRatio) {}
	public virtual void DragRight (float actionRatio) {}
	public virtual void DragDown (float actionRatio) {}
	public virtual void DragUp (float actionRation) {}
}
