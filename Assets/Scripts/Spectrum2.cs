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
		//Debug.Log ("dikkudes");
		//if (DynamicTerrain.instance == null) Debug.Log ("weehee");
		if (GameManager.instance.currentMode == Mode.Live &&
			DynamicTerrain.instance != null) {

			//float[] freqDataArray = new float[256];
			//LinInt spectrum = new LinInt (freqDataArray);

			//LinInt spectrum = DynamicTerrain.instance.freqData;

			for (int i = 0; i < numberOfObjects; i++) {
				//Debug.Log("dick");
				float y = height + Mathf.Log (DynamicTerrain.instance.freqData.getDataPoint ((float)i / numberOfObjects)) * scale;
				//Debug.Log (y);
				if (y != float.NaN) {
				//Vector3 previousScale = points[i].GetComponent<Transform>().localScale;
				//previousScale.y = Mathf.Log(spectrum.getDataPoint ((float)i / numberOfObjects)) * scale;
				//points [i].transform.localScale = previousScale;
				//points[i] = GetComponent<Transform>().parent.position + new Vector3 (points[i].x, height + Mathf.Log(spectrum.getDataPoint ((float)i / numberOfObjects)) * scale, points[i].z);
					points [i].y = y;
				//Debug.DrawLine (points[i].transform.position, (i == 0 ? points[points.Count-1].transform.position : points[i-1].transform.position), Color.green, 0.02f, true);
				}
			}
			GetComponent<LineRenderer> ().SetPositions (points);
		}
	}

}
