using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Class to handle instrument display on the back of the car.
/// </summary>
public class InstrumentDisplay : InstancedMonoBehaviour {

	#region InstrumentDisplay Vars

	GameManager Game;
	MusicManager Music;

	public Image glow;                        // Sprite to change for glow
	public float fadeSpeed;                   // Speed of fade

	#endregion
	#region Unity Callbacks

	void Start () {
		Game = GameManager.instance as GameManager;
		Music = MusicManager.instance as MusicManager;
	}

	void FixedUpdate () {

		if (Game.currentState != GameManager.State.Live) return;
		if (Game.paused) return;

		Color color = glow.Image().color;
		color.a -= fadeSpeed;
		glow.Image().color = color;

	}

	#endregion
	#region InstrumentDisplay Methods

	/// <summary>
	/// Refreshes the display, changing art if necessary.
	/// </summary>
	public void Refresh () {
		gameObject.Image().sprite = Music.currentInstrument.icon;
		glow.sprite = Music.currentInstrument.glow;
	}

	/// <summary>
	/// Sets glow to full.
	/// </summary>
	public void WakeGlow () {
		Color color = glow.Image().color;
		color.a = 1f;
		glow.Image().color = color;
	}

	#endregion
}
