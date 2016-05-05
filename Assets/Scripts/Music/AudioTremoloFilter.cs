using UnityEngine;
using System.Collections;

public class AudioTremoloFilter : MonoBehaviour {

	[Range(Mathf.PI/32f, Mathf.PI/16f)]
	public float rate = Mathf.PI/32f;

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

		for (int i=0; i<data.Length; i++) {
			data[i] = data[i] *( 1f - (1f-depth)/2f + 0.5f*Mathf.Cos(r));
			//data[i] = data[i] * Mathf.Cos(r);
		}
	}
}
