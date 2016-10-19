using UnityEngine;
using System.Collections;

public class LiveInstrumentIcons : SingletonMonoBehaviour<LiveInstrumentIcons> {

    /// <summary>
    /// How long to wait before fading instrument icons (live mode).
    /// </summary>
	[Tooltip("(Live Mode) How long to wait before fading the instrument icons.")]
    [SerializeField]
	float _liveInstrumentIconFadeWaitTime;

    /// <summary>
    /// How quickly to fade instrument icons (live mode).
    /// </summary>
	[Tooltip("(Live Mode) How quickly to fade the instrument icons.")]
    [SerializeField]
	float _liveInstrumentIconFadeSpeed;

    /// <summary>
    /// Fade timer (live mode).
    /// </summary>
    [Tooltip("Current live mode instrument icon fade progress.")]
	float _liveInstrumentIconFadeTimer;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
	/// Wakes the live UI.
	/// </summary>
	public void WakeLiveUI () {
		fadeTimer = fadeWaitTime;
		Color color = Color.white;
		color.a = 1f;
		livePlayQuitPrompt.color = color;
		foreach (Image image in liveIcons.GetComponentsInChildren<Image>()) {
			image.color = color;
			if (image.GetComponentInChildren<Text>())
				image.GetComponentInChildren<Text>().color = color;
		}
	}
}
