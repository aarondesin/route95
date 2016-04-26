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

	public static Bezier instance;

	// 
	// Bezier properties
	//
	[SerializeField]
	private Vector3[] points;

	[SerializeField]
	private BezierControlPointMode[] modes;

	public const float CHECK_RESOLUTION = 25f;

	//
	// Road pathing params
	//
	public float generateRoadRadius; // Distance from player to generate road
	public float placementDistance = 30f; // Marginal distance to add new road
	public float placementRange = 0.4f; // Radius within to place new road node

	public bool generated = false;

	//
	// Mesh generation params
	//
	public int stepsPerCurve = 30;
	private int steps = 0; // Current steps

	public float width; // Width of generated road
	public float height;
	public float slope = 0.8f; // Ratio of top plane to bottom width

	List<Vector3> verts;
	List<Vector2> uvs;
	List<int> tris;

	//
	// Decoration params
	//
	public float decorationRemoveThreshold = 0.3f; // Decorations behind this percent will be removed
	Dictionary<GameObject, float> decorations; // Decorations, and their position along the curve

	#region Unity Callbacks

	void Start () {
		instance = this;
		points = new Vector3[0];
		decorations = new Dictionary<GameObject, float>();

		// Set default points and build mesh
		Reset ();
		Build();
	}

	public void Update () {

		// Remove far away points behind the player
		if (Vector3.Distance (points [3], WorldManager.instance.player.transform.position) > generateRoadRadius) {

			float progress = WorldManager.instance.player.GetComponent<PlayerMovement>().progress;
			float numerator = progress * this.CurveCount;
			Vector3[] newPoints = new Vector3[points.Length - 3];
			for (int i = 0; i < newPoints.Length; i++)
				newPoints [i] = points [i + 3];

			WorldManager.instance.player.GetComponent<PlayerMovement> ().progress = numerator / this.CurveCount;

			// Check for decorations to remove
			List<GameObject> deletes = new List<GameObject>();
			foreach (GameObject decoration in decorations.Keys) {
				if (decorations[decoration] < decorationRemoveThreshold) {
					GameObject temp = decoration;
					deletes.Add(temp);
				}
			}
			foreach (GameObject delete in deletes) {
				decorations.Remove(delete);
				Destroy(delete);
			}
		}
			
		if (Vector3.Distance (points [0], WorldManager.instance.player.transform.position) < generateRoadRadius ||
			Vector3.Distance (points [points.Length - 4], WorldManager.instance.player.transform.position) > generateRoadRadius) {
			//Debug.Log ("PlayerBackingUpException!");
		}
			
		// Create new points in front of player
		while (Vector3.Distance (points [points.Length - 1], WorldManager.instance.player.transform.position) < generateRoadRadius) {
			
			float progress = WorldManager.instance.player.GetComponent<PlayerMovement>().progress;
			float numerator = progress * this.CurveCount;

			AddCurve ();
			WorldManager.instance.player.GetComponent<PlayerMovement> ().progress = numerator / this.CurveCount;

			generated = true;
		}

		//if (generated && DynamicTerrain.instance.full && !DynamicTerrain.instance.randomized) {
			//DynamicTerrain.instance.vertexmap.Randomize(0.5f);
			//DynamicTerrain.instance.randomized = true;
		//}
	}
		
	#endregion
	#region Road Functions

	// Resets curve to default values
	public void Reset () {
		points = new Vector3[4];
		points [0] = PlayerMovement.instance.transform.position;
		points [0].y = 0f; //2.227f;
		points [1].z = points [0].z;
		points [1].y = points [0].y;
		points [1].x = points [0].x - 60f;
		points [2].z = points [0].z + 130f;
		points [2].y = points [0].y;
		points [2].x = points [0].x + 60f;
		points [3].z = points [0].z + 150f;
		points [3].y = points [0].y;
		points [3].x = points [0].x;
		modes = new BezierControlPointMode[points.Length / 2];
		for (int i=0; i<modes.Length; i++)
			modes [i] = BezierControlPointMode.Mirrored;
	}

	// Adds a new curve to the road bezier
	public void AddCurve () {
		float displacedDirection = placementDistance * placementRange;
		if (points == null) points = new Vector3[0];
		Vector3 point;
		if (points.Length > 0) point = points [points.Length - 1];
		else point = WorldManager.instance.player.transform.position;

		Vector3 direction = GetDirection (1f) * placementDistance;
		Array.Resize (ref points, points.Length + 3);

		RaycastHit hit;
		for (int i=3; i>0; i--) {
			Vector3 rayStart = point + new Vector3 (0f, WorldManager.instance.VERT_HEIGHT_SCALE, 0f);
			if (Physics.Raycast(rayStart, Vector3.down, out hit, Mathf.Infinity))
				point.y = hit.point.y;
			//else Debug.LogError ("Bezier.AddCurve(): attempted to place road off map!");
			
			point += direction + new Vector3(UnityEngine.Random.Range(-displacedDirection, displacedDirection), 0f, UnityEngine.Random.Range(-displacedDirection, displacedDirection));
			points [points.Length - i] = point;
		}

		Array.Resize (ref modes, modes.Length + 1);
		modes [modes.Length - 1] = modes [modes.Length - 2];
		EnforceMode (points.Length - 4);
		steps += stepsPerCurve;
		Build();
		Bulldoze();
	}

	// Marks all points between player and newly created points for leveling
	public void Bulldoze () {
		Bulldoze (PlayerMovement.instance.progress);
	}

	public void Bulldoze (float startProgress) {
		float progress = startProgress;
		float diff = 1f - progress;
		while (progress < 1f) {
			DynamicTerrain.instance.vertexmap.CheckRoads (GetPoint(progress));
			progress += diff / CHECK_RESOLUTION;
		}
	}

	// Sets the road mesh
	public void Build () {
		GetComponent<MeshFilter> ().mesh.Clear ();

		Mesh mesh = new Mesh();

		// Populate vertex, UV, and triangles lists
		BuildRoadMesh ();

		// Apply lists
		mesh.SetVertices (verts);
		mesh.SetUVs (0, uvs);
		mesh.SetTriangles (tris, 0);

		// Recalculate properties
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		mesh.Optimize();

		// Set mesh
		GetComponent<MeshFilter> ().mesh = mesh;
		GetComponent<MeshFilter> ().sharedMesh = mesh;
	}

	// Calculates vertices, UVs, and tris for road mesh
	void BuildRoadMesh() {

		List<Vector3> newVertices = new List<Vector3> ();
		List<int> newTriangles = new List<int> ();
		List<Vector2> newUVs = new List<Vector2>();

		float progressI = 0f;
		Vector3 pointI = GetPoint (progressI);

		newVertices.Add(pointI + width * -BezRight (GetDirection(progressI)));
		newUVs.Add(new Vector2(-0.25f, 0f));
		int leftDownI = 0;

		newVertices.Add(pointI + width * BezRight (GetDirection(progressI)));
		newUVs.Add(new Vector2(1.25f, 0f));
		int rightDownI = 1;

		newVertices.Add(pointI + slope * width * -BezRight (GetDirection(progressI)) + height * -BezDown(GetDirection(progressI)));
		newUVs.Add(new Vector2(1-slope-0.25f, 0f));
		int leftUpI = 2;

		newVertices.Add(pointI + slope * width * BezRight (GetDirection(progressI)) + height * -BezDown(GetDirection(progressI)));
		newUVs.Add(new Vector2(slope+0.25f, 1f));
		int rightUpI = 3;

		bool flipUVs = true;

		for (int i = 1; i<steps; i++) {
			int num = i;

			float progressF = (float)(num) / (float)steps;
			Vector3 pointF = GetPoint (progressF);

			newVertices.Add(pointF + width * -BezRight (GetDirection(progressF)));
			newUVs.Add(
				(flipUVs ? new Vector2(-0.25f, 1f) : new Vector2 (-0.25f, 0f))
			);
			int leftDownF = num * 4;

			newVertices.Add(pointF + width * BezRight (GetDirection(progressF)));
			newUVs.Add(
				(flipUVs ? new Vector2(1.25f, 1f) : new Vector2 (1.25f, 0f))
			);
			int rightDownF = num * 4 + 1;

			newVertices.Add(pointF + slope * width * -BezRight (GetDirection(progressF)) + height * -BezDown(GetDirection(progressI)));
			newUVs.Add(
				(flipUVs ? new Vector2(1-slope-0.25f, 1f) : new Vector2 (1-slope-0.25f, 0f))
			);
			int leftUpF = num * 4 + 2;

			newVertices.Add(pointF + slope * width * BezRight (GetDirection(progressF)) + height * -BezDown(GetDirection(progressI)));
			newUVs.Add(
				(flipUVs ? new Vector2(slope+0.25f, 1f) : new Vector2 (slope+0.25f, 0f))
			);
			int rightUpF = num * 4 + 3;


			// Left slope
			newTriangles.Add (leftDownI);
			newTriangles.Add (leftUpI);
			newTriangles.Add (leftDownF);

			newTriangles.Add (leftDownF);
			newTriangles.Add (leftUpI);
			newTriangles.Add (leftUpF);


			// Right slope
			newTriangles.Add (rightUpI);
			newTriangles.Add (rightDownI);
			newTriangles.Add (rightDownF);

			newTriangles.Add (rightUpF);
			newTriangles.Add (rightUpI);
			newTriangles.Add (rightDownF);


			// Road surface plane
			newTriangles.Add (leftUpF);
			newTriangles.Add (rightUpI);
			newTriangles.Add (rightUpF);

			newTriangles.Add (leftUpI);
			newTriangles.Add (rightUpI);
			newTriangles.Add (leftUpF);


			progressI = progressF;
			pointI = pointF;
			leftDownI = leftDownF;
			rightDownI = rightDownF;
			leftUpI = leftUpF;
			rightUpI = rightUpF;

			flipUVs = !flipUVs;
		}

		verts = newVertices;
		uvs = newUVs;
		tris = newTriangles;
	}

	// Returns the number of curves in the bezier
	public int CurveCount {
		get {
			return (points.Length - 1) / 3;
		}
	}

	// Returns the total number of points in the bezier
	public int ControlPointCount {
		get {
			return points.Length;
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

	#endregion
	#region Decoration Functions

	// Registers a decoration and its progress
	public void AddDecoration (GameObject decoration, float prog) {
		decorations.Add (decoration, prog);
	}

	#endregion
	#region Bezier Math

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
		return Bez.GetPoint (points [i ], points [i + 1], points [i + 2], points [i + 3], t);
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
		return GetComponent<Transform> ().TransformPoint (Bez.GetFirstDerivative (points [i], points [i + 1], points [i + 2], points [i + 3], t)) - GetComponent<Transform> ().position;
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

	// Enforces the control point
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
