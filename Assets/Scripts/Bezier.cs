// Created using the fantastic tutorial at:
// http://catlikecoding.com/unity/tutorials/curves-and-splines/

using UnityEngine;
using UnityEngine.UI;
//using UnityEditor;
using System.Collections.Generic;
using System;

public class Bezier : MonoBehaviour {

	[SerializeField]
	private Vector3[] points;
	public int steps;

	public enum BezierControlPointMode {
		Free,
		Aligned,
		Mirrored,
	}

	[SerializeField]
	private BezierControlPointMode[] modes;
	
	private List<Vector3> vertices;
	private List<int> triangles;
	private List<Vector3> normals;
	private List<Vector2> uvs;

	public float w;
	public float h;

	public bool halved;
	public bool flipped;

	public void Reset () {
		modes = new BezierControlPointMode[points.Length / 2];
		for (int i=0; i<modes.Length; i++) {
			modes [i] = BezierControlPointMode.Free;
		}
	}

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
		return Bez.GetPoint (points [i ], points [i + 1], points [i + 2], points [i + 3], t);
	}

	public Vector3 GetPoint2 (float t) {
		return Bez.GetPoint (points [0], points [1], points [2], points [3], t);
	}

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
		return GetComponent<Transform> ().TransformPoint (Bez.GetFirstDerivative (points [i], points [i + 1], points [i + 2], points [i + 3], t)) - GetComponent<Transform> ().position;
	}

	public Vector3 GetVelocity2 (float t) {
		return GetComponent<Transform>().TransformPoint (
			Bez.GetFirstDerivative (points [0], points [1], points [2], points[3], t)) - GetComponent<Transform> ().position;
	}

	public Vector3 GetDirection (float t) {
		return GetVelocity (t).normalized;
	}

	public Vector3 ClosestPointOnBezier (Vector3 pt) {
		var npt = ClosestPointOnLine (points [0], points [2], pt);
		var d = Vector3.Distance (points [0], npt) / Vector3.Distance (points [0], points [2]);
		return GetPoint2 (d);
	}

	public Vector3 ClosestPointOnBezier2 (Vector3 pt) {
		float closestVal = Mathf.Infinity;
		int closestInd = 0;
		Vector3 sPt = points [0];
		for (int i=1; i<= steps; i++) {
			Vector3 ePt = GetPoint2(i/(float)steps);
			var segment = (ePt-sPt).normalized;
			var dot = Mathf.Abs(Vector3.Dot (pt-sPt, segment));
			if (dot <= closestVal) {
				closestVal = dot;
				closestInd = i-1;
			} else {
				break;
			}
			sPt = ePt;
		}
		Debug.Log ("On segment " + closestInd + "-" + (closestInd + 1));
		return ClosestPointOnLine(GetPoint2 (closestInd/(float)steps), GetPoint2 ((closestInd+1)/(float)steps), pt);
	}

	
	Vector3 ClosestPointOnLine(Vector3 a, Vector3 b, Vector3 pt)
	{
		Vector3 line = (b - a).normalized;
		var v = pt - a;
		var d = Vector3.Dot (v, line);
		return a + line * d;
	}

	public void AddCurve () {
		Vector3 point = points [points.Length - 1];
		Array.Resize (ref points, points.Length + 3);
		point.x += 1f;
		points [points.Length - 3] = point;
		point.x += 1f;
		points [points.Length - 2] = point;
		point.x += 1f;
		points [points.Length - 1] = point;

		Array.Resize (ref modes, modes.Length + 1);
		modes [modes.Length - 1] = modes [modes.Length - 2];
		EnforceMode (points.Length - 4);
	}

	public int CurveCount {
		get {
			return (points.Length - 1) / 3;
		}
	}


	public int ControlPointCount {
		get {
			return points.Length;
		}
	}

	public Vector3 GetControlPoint (int index) {
		return points [index];
	}

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

	public BezierControlPointMode GetControlPointMode (int index) {
		return modes [(index + 1) / 3];
	}

	public void SetControlPointMode (int index, BezierControlPointMode mode) {
		modes [(index + 1) / 3] = mode;
		EnforceMode (index);
	}

	private void EnforceMode (int index) {
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

	private void Start () {
		//Rebuild ();
	}

	public void Build () {
		//GetComponent<MeshFilter> ().mesh.Clear ();
		if (GetComponent<MeshFilter> ().sharedMesh != null)
			GetComponent<MeshFilter> ().sharedMesh.Clear ();
		Mesh mesh = new Mesh ();

		BuildMeshSharp ();
		
		mesh.vertices = vertices.ToArray ();
		//Debug.Log (mesh.vertices.Length);

		mesh.triangles = triangles.ToArray ();
		//Debug.Log (mesh.triangles.Length);

		mesh.uv = uvs.ToArray ();

		mesh.RecalculateNormals();

		Solve (mesh);

		mesh.RecalculateBounds();


		//mesh.normals = normals.ToArray ();

		//Unwrapping.GenerateSecondaryUVSet (mesh);
		//MeshGen.DoUVs (mesh);


		mesh.Optimize();

		GetComponent<MeshFilter> ().sharedMesh = mesh;
		GetComponent<MeshCollider> ().sharedMesh = mesh;
	}

	private Vector3 pt1 (float t) {
		//return (halved && flipped) ? sPt + Vector3.down * h : sPt + Vector3.down * h + Vector3.right * w;
		//return (halved && flipped) ? GetPoint2(t) + Quaternion.AngleAxis(-45, Vector3.up)  * h : sPt + Vector3.down * h + Vector3.right * w;
		return (halved && flipped) ? GetPoint(t) + BezDown(GetDirection(t)) * h : GetPoint(t) + BezDown(GetDirection(t)) * h + BezRight(GetDirection(t)) * w;
	}

	private Vector3 BezDown (Vector3 direction) {
		Vector3 planed = Vector3.ProjectOnPlane (direction, Vector3.up).normalized; // Project direction on X/Z plane
		planed = Quaternion.Euler(0, -90, 0) * planed;
		return Vector3.Cross (direction, planed);
	}

	private Vector3 BezRight (Vector3 direction) {
		Vector3 planed = Vector3.ProjectOnPlane (direction, Vector3.up).normalized;
		planed = Quaternion.Euler (0, -90, 0) * planed;
		return planed;
	}

	private Vector3 pt2 (float t) {
		//return (!halved || (halved && flipped)) ? sPt + Vector3.down * h + Vector3.left * w : sPt + Vector3.down * h;
		return (!halved || (halved && flipped)) ? GetPoint(t) + BezDown(GetDirection(t)) * h + -BezRight(GetDirection(t)) * w : GetPoint(t) + BezDown(GetDirection(t)) * h;
	}

	private void BuildMeshSharp () {
		vertices = new List<Vector3> ();
		triangles = new List<int> ();
		normals = new List<Vector3> ();
		uvs = new List<Vector2> ();


		//
		// Generate front cap
		//
		vertices.Add (points [0]); // 0
		vertices.Add (pt1 (0f)); // 1
		vertices.Add (pt2 (0f)); // 2

		triangles.Add(2);
		triangles.Add(1);
		triangles.Add (0);

		Vector3 normal = Vector3.Cross (vertices[2]-vertices [0], vertices [1]-vertices[0]);
		normals.Add (normal);
		normals.Add (normal);
		normals.Add (normal);

		uvs.Add (new Vector2 (0.5f, 1.0f));
		uvs.Add (new Vector2 (1.0f, 0.0f));
		uvs.Add (new Vector2 (0.0f, 0.0f));


		//
		// Generate back cap
		//
		vertices.Add (points [points.Length-1]); // 3
		vertices.Add (pt1 (1f)); // 4
		vertices.Add (pt2 (1f)); // 5

		triangles.Add(3);
		triangles.Add(4);
		triangles.Add (5);

		normal = Vector3.Cross (vertices[4]-vertices [3], vertices [5]-vertices[3]);
		normals.Add (normal);
		normals.Add (normal);
		normals.Add (normal);

		uvs.Add (new Vector2 (0.5f, 1.0f));
		uvs.Add (new Vector2 (1.0f, 0.0f));
		uvs.Add (new Vector2 (0.0f, 0.0f));


		//
		// Generate slope one
		//
		var sPt = points [0];
		var sPt1 = pt1 (0f);
		vertices.Add (sPt); // 6
		vertices.Add (sPt1); // 7
		normal = Vector3.Cross (GetPoint (1f / (float)steps) - sPt, sPt1 - sPt);
		normals.Add (normal);
		normals.Add (normal);
		uvs.Add (new Vector2 (0.0f, 1.0f));
		uvs.Add (new Vector2 (0.0f, 0.0f));
		bool uv_flip = true;

		for (int i=1; i<= steps; i++) {
			Vector3 ePt = GetPoint (i / (float)steps);
			Vector3 ePt1 = pt1 (i / (float)steps);

			normal = Vector3.Cross (sPt1 - sPt, ePt - sPt);

			vertices.Add (ePt);
			vertices.Add (ePt1);

			normals.Add(normal);
			normals.Add(normal);

			if (uv_flip) {
				uvs.Add (new Vector2 (1.0f, 1.0f));
				uvs.Add (new Vector2 (1.0f, 0.0f));
			} else {
				uvs.Add (new Vector2 (0.0f, 1.0f));
				uvs.Add (new Vector2 (0.0f, 0.0f));
			}
			uv_flip = !uv_flip;

			int sPt_i = 6+(i-1)*2;
			int sPt1_i = 7+(i-1)*2;

			int ePt_i = 8+(i-1)*2;
			int ePt1_i = 9+(i-1)*2;

			triangles.Add(sPt_i);
			triangles.Add(sPt1_i);
			triangles.Add(ePt_i);

			triangles.Add(ePt1_i);
			triangles.Add(ePt_i);
			triangles.Add(sPt1_i);

			sPt = ePt;
			sPt1 = ePt1;
		}


		//
		// Generate slope two
		//
		sPt = points [0];
		var sPt2 = pt2 (0f);
		vertices.Add (sPt); // 6+2+(steps*2)
		vertices.Add (sPt2); // 7+2+(steps*2)
		normal = Vector3.Cross (sPt2 - sPt, GetPoint (1f / (float)steps) - sPt);
		normals.Add (normal);
		normals.Add (normal);
		uvs.Add (new Vector2 (0.0f, 1.0f));
		uvs.Add (new Vector2 (0.0f, 0.0f));
		uv_flip = true;
		
		for (int i=1; i<= steps; i++) {
			Vector3 ePt = GetPoint (i / (float)steps);
			Vector3 ePt2 = pt2 (i / (float)steps);

			normal = Vector3.Cross (ePt - sPt, sPt2 - sPt);
			
			vertices.Add (ePt);
			vertices.Add (ePt2);

			normals.Add (normal);
			normals.Add (normal);

			if (uv_flip) {
				uvs.Add (new Vector2 (1.0f, 1.0f));
				uvs.Add (new Vector2 (1.0f, 0.0f));
			} else {
				uvs.Add (new Vector2 (0.0f, 1.0f));
				uvs.Add (new Vector2 (0.0f, 0.0f));
			}
			uv_flip = !uv_flip;
			
			int sPt_i = 6+2+(steps*2)+(i-1)*2;
			int sPt2_i = 7+2+(steps*2)+(i-1)*2;
			
			int ePt_i = 8+2+(steps*2)+(i-1)*2;
			int ePt2_i = 9+2+(steps*2)+(i-1)*2;
			

			triangles.Add(sPt_i);
			triangles.Add(ePt_i);
			triangles.Add(sPt2_i);

			triangles.Add(ePt_i);
			triangles.Add(ePt2_i);
			triangles.Add(sPt2_i);
			
			sPt = ePt;
			sPt2 = ePt2;
		}


		//
		// Generate bottom
		//
		sPt1 = pt1 (0f);
		sPt2 = pt2 (0f);
		vertices.Add (sPt1); // 6+2+(steps*2)+2+(steps*2)
		vertices.Add (sPt2); // 7+2+(steps*2)+2+(steps*2)
		normal = Vector3.Cross (sPt2 - sPt1, GetPoint (1f / (float)steps) - sPt1);
		normals.Add (normal);
		normals.Add (normal);
		uvs.Add (new Vector2 (0.0f, 1.0f));
		uvs.Add (new Vector2 (0.0f, 0.0f));
		uv_flip = true;
		
		for (int i=1; i<= steps; i++) {
			Vector3 ePt1 = pt1(i / (float)steps);
			Vector3 ePt2 = pt2 (i / (float)steps);

			normal = Vector3.Cross (sPt2 - sPt1, ePt2 - sPt1);
			
			vertices.Add (ePt1);
			vertices.Add (ePt2);

			normals.Add (normal);
			normals.Add (normal);

			if (uv_flip) {
				uvs.Add (new Vector2 (1.0f, 1.0f));
				uvs.Add (new Vector2 (1.0f, 0.0f));
			} else {
				uvs.Add (new Vector2 (0.0f, 1.0f));
				uvs.Add (new Vector2 (0.0f, 0.0f));
			}
			uv_flip = !uv_flip;
			
			int sPt1_i = 6+2+(steps*2)+2+(steps*2)+(i-1)*2;
			int sPt2_i = 7+2+(steps*2)+2+(steps*2)+(i-1)*2;
			
			int ePt1_i = 8+2+(steps*2)+2+(steps*2)+(i-1)*2;
			int ePt2_i = 9+2+(steps*2)+2+(steps*2)+(i-1)*2;
			
			triangles.Add(sPt2_i);
			triangles.Add(ePt1_i);
			triangles.Add(sPt1_i);
			
			triangles.Add(sPt2_i);
			triangles.Add(ePt2_i);
			triangles.Add(ePt1_i);
			
			sPt1 = ePt1;
			sPt2 = ePt2;
		}
			

	}

	public static void Solve(Mesh mesh)
	{
		int triangleCount = mesh.triangles.Length;
		int vertexCount = mesh.vertices.Length;
		
		
		Vector3[] tan1 = new Vector3[vertexCount];
		Vector3[] tan2 = new Vector3[vertexCount];
		Vector4[] tangents = new Vector4[vertexCount];
		for(long a = 0; a < triangleCount; a+=3)
		{
			long i1 = mesh.triangles[a+0];
			long i2 = mesh.triangles[a+1];
			long i3 = mesh.triangles[a+2];
			Vector3 v1 = mesh.vertices[i1];
			Vector3 v2 = mesh.vertices[i2];
			Vector3 v3 = mesh.vertices[i3];
			Vector2 w1 = mesh.uv[i1];
			Vector2 w2 = mesh.uv[i2];
			Vector2 w3 = mesh.uv[i3];
			float x1 = v2.x - v1.x;
			float x2 = v3.x - v1.x;
			float y1 = v2.y - v1.y;
			float y2 = v3.y - v1.y;
			float z1 = v2.z - v1.z;
			float z2 = v3.z - v1.z;
			float s1 = w2.x - w1.x;
			float s2 = w3.x - w1.x;
			float t1 = w2.y - w1.y;
			float t2 = w3.y - w1.y;
			float r = 1.0f / (s1 * t2 - s2 * t1);
			Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
			Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);
			tan1[i1] += sdir;
			tan1[i2] += sdir;
			tan1[i3] += sdir;
			tan2[i1] += tdir;
			tan2[i2] += tdir;
			tan2[i3] += tdir;
		}
		for (long a = 0; a < vertexCount; ++a)
		{
			Vector3 n = mesh.normals[a];
			Vector3 t = tan1[a];
			Vector3 tmp = (t - n * Vector3.Dot(n, t)).normalized;
			tangents[a] = new Vector4(tmp.x, tmp.y, tmp.z);
			tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
		}
		mesh.tangents = tangents;
	}
	
	void OnValidate() {
		Build();
	}
	
	void OnDrawGizmosSelected () {
		Gizmos.color = Color.blue;
		for (int i=0; i<vertices.Count && i<normals.Count; i++) {
			Gizmos.DrawLine(GetComponent<Transform>().position+vertices[i], GetComponent<Transform>().position+vertices[i]+normals[i]*0.01f);
		}
	}
}
