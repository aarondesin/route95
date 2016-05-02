using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TremoloRateSlider : EffectSlider {

	public override void Initialize () {
		UpdateSlider (InstrumentSetup.currentRiff.tremoloRate - 0.01f / (Mathf.PI/8f - 0.01f));
	}

	public override void ChangeValue () {
		InstrumentSetup.currentRiff.tremoloRate = slider.value * (Mathf.PI/8f - 0.01f) + 0.01f;
	}

}