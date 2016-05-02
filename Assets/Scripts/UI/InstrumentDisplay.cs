using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InstrumentDisplay : MonoBehaviour {
	public static InstrumentDisplay instance;

	#region InstrumentDisplay Vars

	public Image glow;
	public float fadeSpeed;

	#endregion
	#region Unity Callbacks

	void Start () {
		instance = this;
	}

	void FixedUpdate () {
		if (GameManager.instance.currentMode == GameManager.Mode.Live && !GameManager.instance.paused) {
			Color color = glow.GetComponent<Image>().color;
			color.a -= fadeSpeed;
			glow.GetComponent<Image>().color = color;
		}
	}

	#endregion
	#region InstrumentDisplay Methods

	public void Refresh () {
		GetComponent<Image>().sprite = MusicManager.instance.currentInstrument.icon;
		glow.GetComponent<Image>().sprite = MusicManager.instance.currentInstrument.glow;
	}

	public void WakeGlow () {
		Color color = glow.GetComponent<Image>().color;
		color.a = 1f;
		glow.GetComponent<Image>().color = color;
	}

	#endregion
	
}
