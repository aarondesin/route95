using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EchoDelaySlider : EffectSlider {

	public override void Initialize () {
		UpdateSlider (InstrumentSetup.currentRiff.echoDelay / 0.2f);
	}

	public override void ChangeValue () {
		InstrumentSetup.currentRiff.echoDelay = slider.value * 0.2f;
	}

}