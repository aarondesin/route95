using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TremoloRateSlider : EffectSlider {

	public override void Initialize () {
		UpdateSlider (RiffEditor.currentRiff.tremoloRate - (Mathf.PI/32f) / (Mathf.PI/32f));
	}

	public override void ChangeValue () {
		RiffEditor.currentRiff.tremoloRate = (Mathf.PI/32f) + slider.value * (Mathf.PI/32f);
	}

}