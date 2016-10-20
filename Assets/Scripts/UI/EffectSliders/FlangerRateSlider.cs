using UnityEngine;
using System.Collections;

public class FlangerRateSlider : EffectSlider {

	public override void Initialize () {
		UpdateSlider (RiffEditor.currentRiff.flangerRate / (Mathf.PI*32f) - Mathf.PI/32f);
	}

	public override void ChangeValue () {
		RiffEditor.currentRiff.flangerDryMix = Mathf.PI/32f + Mathf.PI/32f * slider.value;
	}
}
