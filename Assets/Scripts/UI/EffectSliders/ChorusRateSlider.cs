using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChorusRateSlider : EffectSlider {

	public override void Initialize () {
		UpdateSlider (RiffEditor.currentRiff.chorusRate);
	}

	public override void ChangeValue () {
		RiffEditor.currentRiff.chorusRate = slider.value;
	}

}