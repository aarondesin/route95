using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spectrum2 : MonoBehaviour {
	public static Spectrum2 instance;

	DynamicTerrain terrain;

	// Use this for initialization
	public int numberOfObjects = 20;
	public float height;
	public float radius;
	public Vector3[] points;
	public float scale = 20f;
	public float opacity;
	public float fallRate;

	float minHeight;
	float maxHeight;

	void Awake () {
		instance = this;

		minHeight = height - scale;
		maxHeight = height + scale;

		// Initialize visualizer points
		points = new Vector3[numberOfObjects+1];
		for (int i=0; i<numberOfObjects + 1; i++) {
			float angle = i*Mathf.PI*2f/numberOfObjects;
			Vector3 pos = new Vector3 (Mathf.Cos (angle), 0f, Mathf.Sin(angle))*radius;
			GameObject point = new GameObject("Point"+i);
			point.transform.position = pos;
			point.transform.SetParent(transform);
			points[i] = point.transform.position;
		}
		GetComponent<LineRenderer>().SetVertexCount(numberOfObjects+1);
		GetComponent<LineRenderer> ().SetPositions (points);
	}

	// Update is called once per frame
	void Update () {
		if (terrain == null) terrain = WorldManager.instance.terrain;
		if (terrain.freqData == null) return;

		for (int i = 0; i < numberOfObjects + 1; i++) {
			//float y = height + Mathf.Log (terrain.freqData.GetDataPoint ((float)(i==0 ? (numberOfObjects-1) : i) / numberOfObjects)) * scale;
			float y = Mathf.Clamp (height + scale * 10f * terrain.freqData.GetDataPoint ((float)(i == numberOfObjects ? 0 : i) / numberOfObjects), minHeight, maxHeight);
			if (y != float.NaN) {
				if (y < points[i].y && points[i].y >= minHeight+fallRate)
					points[i].y -= fallRate;
				else points [i].y = y;
			}
			
		}
		GetComponent<LineRenderer> ().SetPositions (points);
	}

}
