﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ReverbDecaySlider : EffectSlider {

	public override void Initialize () {
		UpdateSlider (RiffEditor.currentRiff.reverbDecayTime / 20f);
	}

	public override void ChangeValue () {
		RiffEditor.currentRiff.reverbDecayTime = slider.value * 20f;
	}

}