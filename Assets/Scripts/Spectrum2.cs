using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spectrum2 : MonoBehaviour {
	public static Spectrum2 instance;

	// Use this for initialization
	public int numberOfObjects = 20;
	public float height;
	public float radius;
	public Vector3[] points;
	public float scale = 20f;
	public float opacity;

	void Start () {
		instance = this;

		points = new Vector3[numberOfObjects];
		for (int i=0; i<numberOfObjects; i++) {
			float angle = i*Mathf.PI*2f/numberOfObjects;
			Vector3 pos = new Vector3 (Mathf.Cos (angle), 0f, Mathf.Sin(angle))*radius;
			GameObject point = new GameObject();
			point.GetComponent<Transform>().position = pos;
			point.GetComponent<Transform>().SetParent(GetComponent<Transform>());
			points[i] = point.transform.position;
		}
		GetComponent<LineRenderer>().SetVertexCount(numberOfObjects);
	}

	// Update is called once per frame
	void Update () {
		float[] freqDataArray = new float[256];
		//AudioListener.GetSpectrumData (freqDataArray, 0, FFTWindow.Rectangular);
		AudioListener.GetSpectrumData (freqDataArray, 0, FFTWindow.BlackmanHarris);
		LinInt spectrum = new LinInt (freqDataArray);
		for (int i = freqDataArray.Length - 2; i >= 0; i--) {
			spectrum.addKeyPoint (freqDataArray [i]);
		}
		for (int i=0; i<numberOfObjects; i++) {
			//Vector3 previousScale = points[i].GetComponent<Transform>().localScale;
			//previousScale.y = Mathf.Log(spectrum.getDataPoint ((float)i / numberOfObjects)) * scale;
			//points [i].transform.localScale = previousScale;
			//points[i] = GetComponent<Transform>().parent.position + new Vector3 (points[i].x, height + Mathf.Log(spectrum.getDataPoint ((float)i / numberOfObjects)) * scale, points[i].z);
			float linIntInput = (float)i / numberOfObjects;
			//linIntInput = Mathf.Log(linIntInput+1f)/Mathf.Log(2f);
			points[i].y = height + Mathf.Log(spectrum.getDataPoint (linIntInput)) * scale;
			//Debug.DrawLine (points[i].transform.position, (i == 0 ? points[points.Count-1].transform.position : points[i-1].transform.position), Color.green, 0.02f, true);
		}
		GetComponent<LineRenderer>().SetPositions(points);
	}
}
