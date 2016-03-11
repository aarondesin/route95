using UnityEngine;
using System.Collections;

public class Road : MonoBehaviour {
	private Bezier roadBezier;


	// Use this for initialization
	void Start () {
		roadBezier = new Bezier ();
	}
	
	// Update is called once per frame
	void Update () {
		roadBezier.AddCurve ();
		roadBezier.Build ();
	}
}
