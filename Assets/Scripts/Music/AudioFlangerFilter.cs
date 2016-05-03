using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioFlangerFilter : MonoBehaviour {

	//[Range(1, 10)]
	//public int delay = 1;
	[Range(Mathf.PI/32f, Mathf.PI/16f)]
	public float rate = Mathf.PI/32f;
	float r = 0f;

	float time;
	[SerializeField]
	[Range(0.005f, 0.025f)]
	float delay = 0.005f;

	[Range(0f,1f)]
	public float dryMix = 0.5f;

	List<float[]> oldDatas;
	float[]mixed;
	int len;

	void FixedUpdate () {
		r += rate;
		if (r > Mathf.PI * 2f) r -= (Mathf.PI * 2f);
		delay = 0.015f + 0.01f * Mathf.Sin(r);

	}

	void OnEnable () {
		r = 0f;
		time = 0.02f * GameManager.instance.targetFrameRate;
	}	

	void MixSignals () {
		len = oldDatas[0].Length;
		if (mixed == null) mixed= new float[len];
		float val = delay / time;
		int hi = Mathf.CeilToInt(val);
		int lo = Mathf.FloorToInt(val);
		if (hi < 0 || lo < 0) return;
		//Debug.Log(hi + " "+lo);
		float mix = (val - (float)lo);
		for (int i=0; i<len; i++) {
			mixed[i] = oldDatas[hi][i] * mix + oldDatas[lo][i] * (1f-mix);
		}
	}

	public void OnAudioFilterRead (float[] data, int channels) {
		if (oldDatas == null) oldDatas = new List<float[]>();
		while (oldDatas.Count < 5) oldDatas.Add (data);
		//float mix = 2f * dryMix - 1f;
		float oneMinusMix = 1f - dryMix;
		MixSignals ();
		float[] copy = new float[len];
		for (int i=0; i<data.Length; i++) {
			//data[i] = mix * data[i] + oldDatas[delay][i];
			copy[i] = data[i] * dryMix + mixed[i] * oneMinusMix * 0.95f;
			data[i] = copy[i];
		}
		oldDatas.RemoveAt(0);
		oldDatas.Add(copy);
	}
}
