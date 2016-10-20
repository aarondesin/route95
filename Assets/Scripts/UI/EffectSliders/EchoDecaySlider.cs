using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EchoDecaySlider : EffectSlider {

	public override void Initialize () {
		UpdateSlider (RiffEditor.currentRiff.echoDecayRatio / 0.99f);
	}

	public override void ChangeValue () {
		RiffEditor.currentRiff.echoDecayRatio = slider.value * 0.99f;
	}

}