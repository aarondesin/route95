using UnityEngine;
using System.Collections;

public class AudioTremoloFilter : MonoBehaviour {

	[Range(0.01f, Mathf.PI/8f)]
	public float rate;

	[Range(0f, 1f)]
	public float depth;

	[SerializeField]
	[Range(0f, Mathf.PI*2f)]
	float r;

	void FixedUpdate () {
		r += rate;
		if (r > Mathf.PI * 2f) r -= (Mathf.PI * 2f);

		//float axis = 1f - (1f - depth) / 2f;
		//float amplitude = depth / 2f;

		//GetComponent<AudioSource>().volume = amplitude * Mathf.Cos(r) + axis;
	}

	void OnEnable() {
		r = 0f;
	}

	void OnDisable () {
		GetComponent<AudioSource>().volume = 1f;
	}

	void OnAudioFilterRead (float[] data, int channels) {

		float axis = 1f - (1f - depth) / 2f;
		float amplitude = depth / 2f;

		for (int i=0; i< data.Length; i++) {
			data[i] = data[i] * (amplitude + Mathf.Cos(r) + axis);
		}
	}
}
