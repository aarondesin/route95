// Created using the fantastic tutorial at:
// http://catlikecoding.com/unity/tutorials/curves-and-splines/

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;

public class Bezier : MonoBehaviour{

	public enum BezierControlPointMode {
		Free,
		Aligned,
		Mirrored
	}

	#region Hidden Vars

	// 
	// Bezier properties
	//
	[SerializeField]
	private Vector3[] points;

	[SerializeField]
	private BezierControlPointMode[] modes;

	#endregion
	#region Bezier Operations

	// Manually set points
	public void SetPoints (Vector3[] newPoints) {
		points = newPoints;
	}

	// Return points
	public Vector3[] GetPoints () {
		return points;
	}

	// Manually set control point modes
	public void SetModes (BezierControlPointMode[] newModes) {
		modes = newModes;
	}

	public BezierControlPointMode[] GetModes () {
		return modes;
	}

	public void SetPoint (int index, Vector3 value) {
		if (index >= points.Length) return;
		points[index] = value;
	}

	// Returns the coordinates of a point
	public static Vector3 GetPoint (Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
		t = Mathf.Clamp01 (t);
		float oneMinusT = 1f - t;
		return
			oneMinusT * oneMinusT * oneMinusT * p0 +
			3f * oneMinusT * oneMinusT * t * p1 +
			3f * oneMinusT * t * t * p2 +
			t * t * t * p3;
	}

	// Returns the local direction of a point
	public static Vector3 GetFirstDerivative (Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
		t = Mathf.Clamp01 (t);
		float oneMinusT = 1f - t;
		return
			3f * oneMinusT * oneMinusT * (p1 - p0) +
			6f * oneMinusT * t * (p2 - p1) +
			3f * t * t * (p3 - p2);
	}

	// Returns the position of a point at t percent
	public Vector3 GetPoint (float t) {
		int i;
		if (t >= 1f) {
			t = 1f;
			i = points.Length - 4;
		} else {
			t = Mathf.Clamp01 (t) * CurveCount;
			i = (int)t;
			t -= i;
			i *= 3;
		}
		return GetPoint (points [i ], points [i + 1], points [i + 2], points [i + 3], t);
	}

	// Returns the velocity vector at t percent
	public Vector3 GetVelocity (float t) {
		int i;
		if (t >= 1f) {
			t = 1f;
			i = points.Length - 4;
		} else {
			t = Mathf.Clamp01 (t) * CurveCount;
			i = (int)t;
			t -= i;
			i *= 3;
		}
		return 
			transform.TransformPoint (GetFirstDerivative (points [i], points [i + 1], points [i + 2], points [i + 3], t)) - 
			transform.position;
	}

	// Returns the direction vector at t percent
	public Vector3 GetDirection (float t) {
		return GetVelocity (t).normalized;
	}

	// Returns the normalized relative down direction for a bezier
	public Vector3 BezDown (Vector3 direction) {
		Vector3 planed = Vector3.ProjectOnPlane (direction, Vector3.up).normalized; // Project direction on X/Z plane
		planed = Quaternion.Euler(0, -90, 0) * planed;
		return Vector3.Cross (direction, planed);
	}

	// Returns the normalized relative right direction for a bezier
	public Vector3 BezRight (Vector3 direction) {
		Vector3 planed = Vector3.ProjectOnPlane (direction, Vector3.up).normalized;
		planed = Quaternion.Euler (0, -90, 0) * planed;
		return planed;
	}

	// Return total number of points in bezier
	public int PointsCount {
		get {
			return (points.Length);
		}
	}

	// Returns the number of curves in the bezier
	public int CurveCount {
		get {
			return (points.Length - 1) / 3;
		}
	}

	// Returns the total number of points in the bezier
	public int ModeCount {
		get {
			return modes.Length;
		}
	}

	// Returns the point at index
	public Vector3 GetControlPoint (int index) {
		return points [index];
	}

	// Sets a control point
	public void SetControlPoint (int index, Vector3 point) {
		if (index % 3 == 0) {
			Vector3 delta = point - points [index];
			if (index > 0) {
				points [index - 1] += delta;
			}
			if (index + 1 < points.Length) {
				points [index + 1] += delta;
			}
		}
		points [index] = point;
		EnforceMode (index);
	}

	// Returns the control point mode at index
	public BezierControlPointMode GetControlPointMode (int index) {
		return modes [(index + 1) / 3];
	}

	// Sets the control point mode at an index
	public void SetControlPointMode (int index, BezierControlPointMode mode) {
		modes [(index + 1) / 3] = mode;
		EnforceMode (index);
	}

	// Enforces the control point
	public void EnforceMode (int index) {
		int modeIndex = (index + 1) / 3;
		BezierControlPointMode mode = modes [modeIndex];
		if (mode == BezierControlPointMode.Free || modeIndex == 0 || modeIndex == modes.Length - 1) {
			return;
		}

		int middleIndex = modeIndex * 3;
		int fixedIndex, enforcedIndex;
		if (index <= middleIndex) {
			fixedIndex = middleIndex - 1;
			enforcedIndex = middleIndex + 1;
		} else {
			fixedIndex = middleIndex + 1;
			enforcedIndex = middleIndex - 1;
		}

		Vector3 middle = points [middleIndex];
		Vector3 enforcedTangent = middle - points [fixedIndex];
		points [enforcedIndex] = middle + enforcedTangent;

		if (mode == BezierControlPointMode.Aligned) {
			enforcedTangent = enforcedTangent.normalized * Vector3.Distance (middle, points [enforcedIndex]);
		}
		points [enforcedIndex] = middle + enforcedTangent;
	}

	#endregion

	void OnDrawGizmosSelected () {
		Gizmos.color = Color.blue;

		Vector3[] verts = GetComponent<MeshFilter>().mesh.vertices;
		int[] tris = GetComponent<MeshFilter>().mesh.triangles;
		for (int i=0; i<verts.Length-4 && i<GetComponent<MeshFilter>().mesh.normals.Length; i++)
			Gizmos.DrawLine (verts[i], verts[i+4]);

		for (int i=0; i<tris.Length; i+=3) {
			Gizmos.DrawLine (verts[tris[i]], verts[tris[i+1]]);
			Gizmos.DrawLine (verts[tris[i+1]], verts[tris[i+2]]);
			Gizmos.DrawLine (verts[tris[i+2]], verts[tris[i]]);
		}
	}
}
