using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using Route95.Core;
using Route95.Music;

namespace Route95.UI {

/// <summary>
/// Class to handle instrument display on the back of the car.
/// </summary>
public class InstrumentDisplay : SingletonMonoBehaviour<InstrumentDisplay> {

	#region InstrumentDisplay Vars

	public static InstrumentDisplay instance;

	public Image glow;                        // Sprite to change for glow
	public float fadeSpeed;                   // Speed of fade
	public List<Fadeable> glows;                 // Instrument icons glows

	#endregion
	#region Unity Callbacks

	void FixedUpdate () {

		if (GameManager.Instance.CurrentState != GameManager.State.Live) return;
		if (GameManager.Instance.Paused) return;

		Color color = glow.color;
		color.a -= fadeSpeed;
		glow.color = color;

	}

	#endregion
	#region InstrumentDisplay Methods

	/// <summary>
	/// Refreshes the display, changing art if necessary.
	/// </summary>
	public void Refresh () {
		gameObject.Image().sprite = MusicManager.Instance.CurrentInstrument.icon;
		glow.sprite = MusicManager.Instance.CurrentInstrument.glow;
	}

	/// <summary>
	/// Sets glow to full.
	/// </summary>
	public void WakeGlow () {
		Color color = glow.color;
		color.a = 1f;
		glow.color = color;
	}

	public void WakeGlow (int index) {
		glows[index].UnFade();
	}

	public void FadeGlow (int index) {
		glows[index].Fade();
	}

	#endregion
}
}
