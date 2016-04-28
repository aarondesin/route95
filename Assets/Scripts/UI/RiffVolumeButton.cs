using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RiffVolumeButton : DraggableButton {

	#region RiffVolumeButton Vars

	const float vDragDistance = 128f;
	const float hDragDistance = 128f;

	float oldVolume;

	Image img;
	Riff targetRiff;

	#endregion
	#region Unity Callbacks

	void Start () {
		maxDragDistanceUp = vDragDistance;
		maxDragDistanceDown = vDragDistance;
		maxDragDistanceLeft = hDragDistance;
		maxDragDistanceRight = hDragDistance;

		img = GetComponent<Image>();
	}

	#endregion
	#region RiffVolumeButton Methods

	public override void OnMouseDown() {
		targetRiff = InstrumentSetup.currentRiff;
		oldVolume = targetRiff.volume;
	}

	public override void OnMouseUp() {
		//Debug.Log(targetNote.volume);
	}

	public override void DragDown (float actionRatio) {
		//targetNote.volume += 0.5f - (actionRatio / 2f);
		targetRiff.volume = Mathf.Clamp01 (oldVolume - actionRatio);
		img.fillAmount = targetRiff.volume;//0.5f - (actionRatio / 2f);
		//Debug.Log("DragDown("+actionRatio+")");
	}

	public override void DragUp (float actionRatio) {
		//targetNote.volume = 0.5f + (actionRatio / 2f);
		targetRiff.volume = Mathf.Clamp01 (oldVolume + actionRatio);
		img.fillAmount = targetRiff.volume;
	}

	public override void DragLeft (float actionRatio) {
		//targetNote.duration = 1f;
	}

	public override void DragRight (float actionRatio) {
		//targetNote.duration = 1 + (float)(notesVisible-1) * actionRatio;
	}

	#endregion
}
