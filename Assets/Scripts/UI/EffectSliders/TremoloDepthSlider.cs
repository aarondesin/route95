using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TremoloDepthSlider : EffectSlider {

	public override void Initialize () {
		UpdateSlider (RiffEditor.currentRiff.tremoloDepth);
	}

	public override void ChangeValue () {
		RiffEditor.currentRiff.tremoloDepth = slider.value;
	}

}