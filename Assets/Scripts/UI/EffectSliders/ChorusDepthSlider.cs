using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChorusDepthSlider : EffectSlider {

	public override void Initialize () {
		UpdateSlider (RiffEditor.currentRiff.chorusDepth);
	}

	public override void ChangeValue () {
		RiffEditor.currentRiff.chorusDepth = slider.value;
	}

}