using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ReverbLevelSlider : EffectSlider {

	public override void Initialize () {
		UpdateSlider (RiffEditor.currentRiff.reverbLevel / 2000f);
	}

	public override void ChangeValue () {
		RiffEditor.currentRiff.reverbLevel = slider.value * 2000f;
	}

}