// Bezier.cs
// ©2016 Team 95

// Created using the fantastic tutorial at:
// http://catlikecoding.com/unity/tutorials/curves-and-splines/

using System.Collections.Generic;

using UnityEngine;

namespace Route95.World {

	/// <summary>
	/// Base bezier class with operations.
	/// </summary>
	public class Bezier : MonoBehaviour {

		#region Bezier Enums

		/// <summary>
		/// Control point mode.
		/// </summary>
		public enum BezierControlPointMode {
			Free,
			Aligned,
			Mirrored
		}

		#endregion
		#region Bezier Vars

		/// <summary>
		/// All points in the bezier.
		/// </summary>
		[SerializeField]
		[Tooltip("All points in the bezier.")]
		protected List<Vector3> _points;

		/// <summary>
		/// All modes in the bezier.
		/// </summary>
		[SerializeField]
		[Tooltip("All modes in the bezier.")]
		protected List<BezierControlPointMode> _modes;

		#endregion
		#region Properties

		/// <summary>
		/// Number of points in the bezier, including control points (read-only).
		/// </summary>
		public int PointsCount { get { return _points.Count; } }

		/// <summary>
		/// Number of curves in the bezier. (read-only).
		/// </summary>
		public int CurveCount { get { return (_points.Count - 1) / 3; } }

		/// <summary>
		/// Number of control modes in the bezier (read-only).
		/// </summary>
		public int ModeCount { get { return _modes.Count; } }

		#endregion
		#region Bezier Methods

		/// <summary>
		/// Sets the points of the bezier.
		/// </summary>
		public void SetPoints(List<Vector3> newPoints) {
			_points = newPoints;
		}

		/// <summary>
		/// Gets the points of the bezier.
		/// </summary>
		public List<Vector3> GetPoints() {
			return _points;
		}

		/// <summary>
		/// Sets the control point modes.
		/// </summary>
		public void SetModes(List<BezierControlPointMode> newModes) {
			_modes = newModes;
		}

		/// <summary>
		/// Gets the control point modes.
		/// </summary>
		public List<BezierControlPointMode> GetModes() {
			return _modes;
		}

		/// <summary>
		/// Sets the point at the specified index.
		/// </summary>
		public void SetPoint(int index, Vector3 value) {
			if (index >= _points.Count) return;
			_points[index] = value;
		}

		/// <summary>
		/// Returns the coordinates of a point.
		/// </summary>
		public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
			t = Mathf.Clamp01(t);
			float oneMinusT = 1f - t;
			return
				oneMinusT * oneMinusT * oneMinusT * p0 +
				3f * oneMinusT * oneMinusT * t * p1 +
				3f * oneMinusT * t * t * p2 +
				t * t * t * p3;
		}

		/// <summary>
		/// Returns the local direction of a point.
		/// </summary>
		public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
			t = Mathf.Clamp01(t);
			float oneMinusT = 1f - t;
			return
				3f * oneMinusT * oneMinusT * (p1 - p0) +
				6f * oneMinusT * t * (p2 - p1) +
				3f * t * t * (p3 - p2);
		}

		/// <summary>
		/// Returns the point at a certain percentage of the bezier.
		/// </summary>
		public Vector3 GetPoint(float t) {
			int i;
			if (t >= 1f) {
				t = 1f;
				i = _points.Count - 4;
			} else {
				t = Mathf.Clamp01(t) * CurveCount;
				i = Mathf.FloorToInt(t);
				t -= i;
				i *= 3;
			}
			return GetPoint(_points[i], _points[i + 1], _points[i + 2], _points[i + 3], t);
		}

		/// <summary>
		/// Returns the velocity at a certain percentage of the bezier.
		/// </summary>
		public Vector3 GetVelocity(float t) {
			int i;
			if (t >= 1f) {
				t = 1f;
				i = _points.Count - 4;
			} else {
				t = Mathf.Clamp01(t) * CurveCount;
				i = (int)t;
				t -= i;
				i *= 3;
			}
			return
				transform.TransformPoint(GetFirstDerivative(_points[i], _points[i + 1], _points[i + 2], _points[i + 3], t)) -
				transform.position;
		}

		/// <summary>
		/// Returns the normalized velocity at a certain percentage of the bezier.
		/// </summary>
		public Vector3 GetDirection(float t) {
			return GetVelocity(t).normalized;
		}

		/// <summary>
		/// Returns the normalized relative downwards vector at a point.
		/// </summary>
		public Vector3 BezDown(Vector3 direction) {
			Vector3 planed = Vector3.ProjectOnPlane(direction, Vector3.up).normalized; // Project direction on X/Z plane
			planed = Quaternion.Euler(0, -90, 0) * planed;
			return Vector3.Cross(direction, planed);
		}

		/// <summary>
		/// Returns the normalized relative right vector a point.
		/// </summary>
		public Vector3 BezRight(Vector3 direction) {
			Vector3 planed = Vector3.ProjectOnPlane(direction, Vector3.up).normalized;
			planed = Quaternion.Euler(0, -90, 0) * planed;
			return planed;
		}

		/// <summary>
		/// Returns the control point at an index.
		/// </summary>
		public Vector3 GetControlPoint(int index) {
			return _points[index];
		}

		/// <summary>
		/// Sets the control point at an index.
		/// </summary>
		public void SetControlPoint(int index, Vector3 point) {
			if (index % 3 == 0) {
				Vector3 delta = point - _points[index];
				if (index > 0) _points[index - 1] += delta;
				if (index + 1 < _points.Count) _points[index + 1] += delta;
			}
			_points[index] = point;
			EnforceMode(index);
		}

		/// <summary>
		/// Returns the control mode at an index.
		/// </summary>
		public BezierControlPointMode GetControlPointMode(int index) {
			return _modes[(index + 1) / 3];
		}

		/// <summary>
		/// Sets the control mode at an index.
		/// </summary>
		public void SetControlPointMode(int index, BezierControlPointMode mode) {
			_modes[(index + 1) / 3] = mode;
			EnforceMode(index);
		}

		/// <summary>
		/// Enforces the control point mode at an index.
		/// </summary>
		public void EnforceMode(int index) {
			int modeIndex = (index + 1) / 3;
			BezierControlPointMode mode = _modes[modeIndex];
			if (mode == BezierControlPointMode.Free || modeIndex == 0 || modeIndex == _modes.Count - 1) {
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

			Vector3 middle = _points[middleIndex];
			Vector3 enforcedTangent = middle - _points[fixedIndex];
			_points[enforcedIndex] = middle + enforcedTangent;

			if (mode == BezierControlPointMode.Aligned) {
				enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, _points[enforcedIndex]);
			}
			_points[enforcedIndex] = middle + enforcedTangent;
		}

		#endregion
	}
}
