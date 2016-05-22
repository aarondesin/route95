using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Class to handle instrument display on the back of the car.
/// </summary>
public class InstrumentDisplay : MonoBehaviour {

	#region InstrumentDisplay Vars

	public static InstrumentDisplay instance; // Quick reference to this instance

	public Image glow;                        // Sprite to change for glow
	public float fadeSpeed;                   // Speed of fade

	#endregion
	#region Unity Callbacks

	void Start () {
		instance = this;
	}

	void FixedUpdate () {

		if (GameManager.instance.currentState != GameManager.State.Live) return;
		if (GameManager.instance.paused) return;

		Color color = glow.GetComponent<Image>().color;
		color.a -= fadeSpeed;
		glow.GetComponent<Image>().color = color;

	}

	#endregion
	#region InstrumentDisplay Methods

	/// <summary>
	/// Refreshes the display, changing art if necessary.
	/// </summary>
	public void Refresh () {
		gameObject.Image().sprite = MusicManager.instance.currentInstrument.icon;
		glow.sprite = MusicManager.instance.currentInstrument.glow;
	}

	/// <summary>
	/// Sets glow to full.
	/// </summary>
	public void WakeGlow () {
		Color color = glow.GetComponent<Image>().color;
		color.a = 1f;
		glow.GetComponent<Image>().color = color;
	}

	#endregion
}
