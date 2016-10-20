using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChorusDryMixSlider : EffectSlider {

	public override void Initialize () {
		UpdateSlider (RiffEditor.currentRiff.chorusDryMix);
	}

	public override void ChangeValue () {
		RiffEditor.currentRiff.chorusDryMix = slider.value;
	}

}