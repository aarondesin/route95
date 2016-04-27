using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Road : MonoBehaviour {
	public static Road instance;

	//
	// Road pathing params
	//
	Bezier path;

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
		path = new Bezier();
		path.SetPoints(new Vector3[0]);
		decorations = new Dictionary<GameObject, float>();

		generateRoadRadius = WorldManager.instance.roadExtendRadius;

		// Set default points and build mesh
		Reset ();
		Build();
	}

	public void Update () {
		Vector3[] points = path.GetPoints();

		// Remove far away points behind the player
		if (Vector3.Distance (points[3], PlayerMovement.instance.transform.position) > generateRoadRadius) {

			float progress = PlayerMovement.instance.progress;
			float numerator = progress * path.CurveCount;
			Vector3[] newPoints = new Vector3[points.Length - 3];
			for (int i = 0; i < newPoints.Length; i++)
				newPoints [i] = points [i + 3];

			PlayerMovement.instance.progress = numerator / path.CurveCount;

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

		if (Vector3.Distance (points [0], PlayerMovement.instance.transform.position) < generateRoadRadius ||
			Vector3.Distance (points [points.Length - 4], PlayerMovement.instance.transform.position) > generateRoadRadius) {
			//Debug.Log ("PlayerBackingUpException!");
		}

		// Create new points in front of player
		while (Vector3.Distance (points [points.Length - 1], PlayerMovement.instance.transform.position) < generateRoadRadius) {

			float progress = PlayerMovement.instance.progress;
			float numerator = progress * path.CurveCount;

			AddCurve ();
			PlayerMovement.instance.progress = numerator / path.CurveCount;

			generated = true;
		}

	}

	#endregion
	#region Road Functions

	public int CurveCount {
		get {
			return (path.CurveCount);
		}
	}

	public Vector3 GetPoint (float prog) {
		return path.GetPoint (prog);
	}

	public Vector3 GetVelocity (float prog) {
		return path.GetVelocity (prog);
	}

	public Vector3 Right (Vector3 dir) {
		return path.BezRight (dir);
	}

	// Resets curve to default values
	public void Reset () {
		Vector3[] points = new Vector3[4];
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
		path.SetPoints (points);

		Bezier.BezierControlPointMode[] modes = new Bezier.BezierControlPointMode[points.Length / 2];
		for (int i=0; i<modes.Length; i++)
			modes [i] = Bezier.BezierControlPointMode.Mirrored;
		path.SetModes (modes);
	}

	// Adds a new curve to the road bezier
	public void AddCurve () {
		Vector3[] points = path.GetPoints();
		float displacedDirection = placementDistance * placementRange;

		Vector3 point;
		if (points.Length > 0) point = points [points.Length - 1];
		else point = PlayerMovement.instance.transform.position;

		Vector3 direction = path.GetDirection (1f) * placementDistance;
		Array.Resize (ref points, points.Length + 3);

		RaycastHit hit;
		for (int i=3; i>0; i--) {
			Vector3 rayStart = point + new Vector3 (0f, WorldManager.instance.heightScale, 0f);
			if (Physics.Raycast(rayStart, Vector3.down, out hit, Mathf.Infinity))
				point.y = hit.point.y;
			//else Debug.LogError ("Bezier.AddCurve(): attempted to place road off map!");

			point += direction + new Vector3(
				UnityEngine.Random.Range(-displacedDirection, displacedDirection), 
				0f, 
				UnityEngine.Random.Range(-displacedDirection, displacedDirection)
			);
			points[path.PointsCount - i] = point;
		}
		path.SetPoints(points);

		Bezier.BezierControlPointMode[] modes = path.GetModes();
		Array.Resize (ref modes, modes.Length + 1);
		modes[modes.Length - 1] = modes[modes.Length -2];
		path.SetModes (modes);
		path.EnforceMode (points.Length - 4);
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
			DynamicTerrain.instance.vertexmap.CheckRoads (path.GetPoint(progress));
			progress += diff / WorldManager.instance.roadPathCheckResolution;
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
		Vector3 pointI = path.GetPoint (progressI);

		newVertices.Add(pointI + width * -path.BezRight (path.GetDirection(progressI)));
		newUVs.Add(new Vector2(-0.25f, 0f));
		int leftDownI = 0;

		newVertices.Add(pointI + width * path.BezRight (path.GetDirection(progressI)));
		newUVs.Add(new Vector2(1.25f, 0f));
		int rightDownI = 1;

		newVertices.Add(
			pointI + slope * width * 
			-path.BezRight (path.GetDirection(progressI)) + height * -path.BezDown(path.GetDirection(progressI))
		);
		newUVs.Add(new Vector2(1-slope-0.25f, 0f));
		int leftUpI = 2;

		newVertices.Add(
			pointI + slope * width * 
			path.BezRight (path.GetDirection(progressI)) + height * -path.BezDown(path.GetDirection(progressI))
		);
		newUVs.Add(new Vector2(slope+0.25f, 1f));
		int rightUpI = 3;

		bool flipUVs = true;

		for (int i = 1; i<steps; i++) {
			int num = i;

			float progressF = (float)(num) / (float)steps;
			Vector3 pointF = path.GetPoint (progressF);

			newVertices.Add(pointF + width * -path.BezRight (path.GetDirection(progressF)));
			newUVs.Add(
				(flipUVs ? new Vector2(-0.25f, 1f) : new Vector2 (-0.25f, 0f))
			);
			int leftDownF = num * 4;

			newVertices.Add(pointF + width * path.BezRight (path.GetDirection(progressF)));
			newUVs.Add(
				(flipUVs ? new Vector2(1.25f, 1f) : new Vector2 (1.25f, 0f))
			);
			int rightDownF = num * 4 + 1;

			newVertices.Add(
				pointF + slope * width * 
				-path.BezRight (path.GetDirection(progressF)) + height * -path.BezDown(path.GetDirection(progressI))
			);
			newUVs.Add(
				(flipUVs ? new Vector2(1-slope-0.25f, 1f) : new Vector2 (1-slope-0.25f, 0f))
			);
			int leftUpF = num * 4 + 2;

			newVertices.Add(
				pointF + slope * width * 
				path.BezRight (path.GetDirection(progressF)) + height * -path.BezDown(path.GetDirection(progressI))
			);
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

	#endregion
	#region Decoration Functions

	// Registers a decoration and its progress
	public void AddDecoration (GameObject decoration, float prog) {
		decorations.Add (decoration, prog);
	}

	#endregion
}
