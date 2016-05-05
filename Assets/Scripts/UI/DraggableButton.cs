using UnityEngine;
using System.Collections;

public class DraggableButton : MonoBehaviour {

	#region DraggableButton Vars
	public float maxDragDistanceUp;
	public float maxDragDistanceDown;
	public float maxDragDistanceLeft;
	public float maxDragDistanceRight;

	//public float dragThreshold; // threshold for action to be recognized as a drag instead of a click
	//public bool isDrag = false;

	const float dragBias = 1.25f; // threshold for drag to recognize one direction over another

	#endregion
	#region DraggableButton Methods

	// Calls appropriate functions based on drag direction
	public void Drag (Vector3 dragVector) {
		float hDrag = Mathf.Abs(dragVector.x);
		float yDrag = Mathf.Abs(dragVector.y);

		//if (dragVector.magnitude >= dragThreshold) isDrag = true;
		//else isDrag = false;

		if (dragVector.x < 0f) DragLeft (Mathf.Clamp01(hDrag/maxDragDistanceLeft));
		else DragRight (Mathf.Clamp01(hDrag/maxDragDistanceRight));
		if (dragVector.y < 0f) DragDown (Mathf.Clamp01(yDrag/maxDragDistanceDown));
		else DragUp (Mathf.Clamp01(yDrag/maxDragDistanceUp));

		//if (isDrag) Debug.Log("isDrag");
	}

	public virtual void OnMouseDown () {}
	public virtual void OnMouseUp () {}

	public virtual void DragLeft (float actionRatio) {}
	public virtual void DragRight (float actionRatio) {}
	public virtual void DragDown (float actionRatio) {}
	public virtual void DragUp (float actionRation) {}

	#endregion
}
