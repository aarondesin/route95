using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EchoDryMixSlider : EffectSlider {

	public override void Initialize () {
		UpdateSlider (RiffEditor.currentRiff.echoDryMix);
	}

	public override void ChangeValue () {
		RiffEditor.currentRiff.echoDryMix = slider.value;
	}

}