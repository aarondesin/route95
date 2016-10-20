using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DistortionLevelSlider : EffectSlider {

	public override void Initialize () {
		UpdateSlider (RiffEditor.currentRiff.distortionLevel);
	}

	public override void ChangeValue () {
		RiffEditor.currentRiff.distortionLevel = slider.value;
	}

}