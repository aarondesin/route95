using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// A special use of DraggableButton for the riff editor not buttons.
/// </summary>
public class NoteButton : DraggableButton {

	#region NoteButton Vars

	const int notesVisible = 10;
	const float vDragDistance = 128f;
	const float hDragDistance = 128f;

	public Note targetNote;
	public Image volumeImage;

	float oldVolume;
	#endregion
	#region Unity Callbacks

	void Awake () {
		maxDragDistanceUp = vDragDistance;
		maxDragDistanceDown = vDragDistance;
		maxDragDistanceLeft = hDragDistance;
		maxDragDistanceRight = hDragDistance;

		//dragThreshold = 25f;
	}

	#endregion
	#region DraggableButton Overrides

	public override void OnMouseDown() {
		oldVolume = targetNote.volume;
	}


	public override void DragDown (float actionRatio) {
		targetNote.volume = Mathf.Clamp01 (oldVolume - actionRatio);
		volumeImage.fillAmount = targetNote.volume;
	}

	public override void DragUp (float actionRatio) {
		targetNote.volume = Mathf.Clamp01 (oldVolume + actionRatio);
		volumeImage.fillAmount = targetNote.volume;
	}

	public override void DragLeft (float actionRatio) {
		targetNote.duration = 1f;
	}

	public override void DragRight (float actionRatio) {
		targetNote.duration = 1 + (float)(notesVisible-1) * actionRatio;
	}

	#endregion
}
