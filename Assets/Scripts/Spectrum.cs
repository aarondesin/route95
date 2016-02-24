using UnityEngine;
using System.Collections;

public class Spectrum : MonoBehaviour {

	// Use this for initialization
	public GameObject prefab;
	public int numberOfObjects = 20;
	public float radius = 5f;
	public GameObject[]cubes;
	public float scale = 20f;

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
		float[] freqDataArray = new float[256];
		AudioListener.GetSpectrumData (freqDataArray, 0, FFTWindow.Rectangular);
		LinInt spectrum = new LinInt (freqDataArray);
		for (int i = 0; i < numberOfObjects; i++) {
			Vector3 previousScale = cubes [i].transform.localScale;
			previousScale.y = spectrum.getDataPoint ((float)i / numberOfObjects) * scale;
			cubes [i].transform.localScale = previousScale;
		}
	}
}
