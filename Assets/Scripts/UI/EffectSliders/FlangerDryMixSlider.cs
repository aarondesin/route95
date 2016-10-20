using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FlangerDryMixSlider : EffectSlider {

	public override void Initialize () {
		UpdateSlider ((RiffEditor.currentRiff.flangerDryMix + 1f) / 2f);
	}

	public override void ChangeValue () {
		RiffEditor.currentRiff.flangerDryMix = slider.value * 2f - 1f;
	}

}