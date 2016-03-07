using UnityEngine;
using System.Collections;

public class Spectrum : MonoBehaviour {

	// Use this for initialization
	public GameObject prefab;
	public int numberOfObjects = 20;
	public float radius = 5f;
	public GameObject[]cubes;
	public float scale = 20000f;

	void Start () {
		for (int i = 0; i < numberOfObjects; i++) {
			float angle = i * Mathf.PI * 2 / numberOfObjects;
			Vector3 pos = new Vector3 (Mathf.Cos (angle), 0, Mathf.Sin (angle)) * radius;
			Instantiate (prefab, pos, Quaternion.identity);
		}
		cubes = GameObject.FindGameObjectsWithTag ("cubes");
		foreach (GameObject obj in cubes) {
			obj.transform.parent = this.transform;
		}
	}
	
	// Update is called once per frame
	void Update () {
		float[] freqDataArray = new float[64];
		AudioListener.GetSpectrumData(freqDataArray, 0, FFTWindow.Hanning);
		LinInt spectrum = new LinInt (freqDataArray);
		//add the keypoint again, but in reverse order to mirror
		for (int i = freqDataArray.Length - 1; i >= 0; i--) {
			spectrum.addKeyPoint (freqDataArray [i]);
		}
		for (int i = 0; i < numberOfObjects; i++) {
			Vector3 previousScale = cubes [i].transform.localScale;
			//previousScale.y = Mathf.Log(Mathf.Abs(spectrum.getDataPoint (i / numberOfObjects)) + 1f) * scale;
			previousScale.y = spectrum.getDataPoint((float)i/numberOfObjects) * scale;
			cubes [i].transform.localScale = previousScale;
		}
	}
}
