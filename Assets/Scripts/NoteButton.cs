using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NoteButton : DraggableButton {
	const int notesVisible = 10;
	const float vDragDistance = 128f;
	const float hDragDistance = 128f;

	public Note targetNote;
	public Image volumeImage;

	float oldVolume;

	void Start () {
		maxDragDistanceUp = vDragDistance;
		maxDragDistanceDown = vDragDistance;
		maxDragDistanceLeft = hDragDistance;
		maxDragDistanceRight = hDragDistance;
	}

	public override void OnMouseDown() {
		oldVolume = targetNote.volume;
	}

	public override void DragDown (float actionRatio) {
		//targetNote.volume += 0.5f - (actionRatio / 2f);
		targetNote.volume = Mathf.Clamp01 (oldVolume - actionRatio);
		volumeImage.fillAmount = targetNote.volume;//0.5f - (actionRatio / 2f);
		Debug.Log("DragDown("+actionRatio+")");
	}

	public override void DragUp (float actionRatio) {
		//targetNote.volume = 0.5f + (actionRatio / 2f);
		targetNote.volume = Mathf.Clamp01 (oldVolume + actionRatio);
		volumeImage.fillAmount = targetNote.volume;
	}

	public override void DragLeft (float actionRatio) {
		targetNote.duration = 1f;
	}

	public override void DragRight (float actionRatio) {
		targetNote.duration = 1 + (float)(notesVisible-1) * actionRatio;
	}
}
