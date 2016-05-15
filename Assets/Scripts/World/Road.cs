	using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Road : Bezier {

	public bool loaded = false;

	#region Road Placement Vars

	DynamicTerrain terrain;   // reference to terrain

	float generateRoadRadius; // distance from player to generate road (copied from WM)
	float variance;
	float placementDistance;  // marginal distance to add new road (copied from WM)
	float maxSlope;           // maximum slope per world unit (copied from WM)

	#endregion
	#region Road Mesh Vars

	int stepsPerCurve;        // number of steps per road curve (copied from WM)
	int steps = 0;            // current number of steps

	public float width;              // width of generated road (copied from WM)
	public float height;             // height of generated road (copied from WM)
	public float slope;              // ratio of top plane to bottom width (copied from WM)

	Mesh mesh;                // road mesh
	List<Vector3> verts;      // vertices in road mesh
	List<Vector2> uvs;        // road mesh UVs
	List<int> tris;           // road mesh triangles

	float UVProgress = 0f;    // current UV value to use when generating mesh

	#endregion
	#region Unity Callbacks

	void Awake () {
		loaded = false;

		verts = new List<Vector3> ();
		uvs = new List<Vector2> ();
		tris = new List<int> ();

		// Init mesh
		mesh = new Mesh();

		// Init road points
		points = new Vector3[0];

	}

	public void Start () {

		// Copy vars from WM
		generateRoadRadius = WorldManager.instance.roadExtendRadius;
		variance = WorldManager.instance.roadVariance;
		placementDistance = WorldManager.instance.roadPlacementDistance;
		maxSlope = WorldManager.instance.roadSlope;

		stepsPerCurve = WorldManager.instance.roadStepsPerCurve;

		terrain = WorldManager.instance.terrain;

		// Build mesh
		Reset ();
		
	}

	public void Update () {

		if (!loaded) return;

		// Remove far away points behind the player
		if (Vector3.Distance (points[3], PlayerMovement.instance.transform.position) > generateRoadRadius) {

			// Update player progress
			float progress = PlayerMovement.instance.progress;
			float numerator = progress * CurveCount;


			// Update points array
			Vector3[] newPoints = new Vector3[points.Length - 3];
			for (int i = 0; i < newPoints.Length; i++)
				newPoints [i] = points [i + 3];

			PlayerMovement.instance.progress = numerator / CurveCount;
		}

		// Create new points in front of player
		while (Vector3.Distance (points [points.Length - 1], PlayerMovement.instance.transform.position) < generateRoadRadius) {



			float progress = PlayerMovement.instance.progress;
			float numerator = progress * CurveCount;

			AddCurve ();

			PlayerMovement.instance.progress = numerator / CurveCount;
		}

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


		modes = new Bezier.BezierControlPointMode[points.Length / 2];
		for (int i=0; i<modes.Length; i++)
			modes [i] = Bezier.BezierControlPointMode.Mirrored;
	}

	public float Width {
		get {
			return width;
		}
	}

	public void DoLoad () {
		StartCoroutine("Load");
	}

	IEnumerator Load () {
		GameManager.instance.ChangeLoadingMessage("Loading road...");
		float startTime = Time.realtimeSinceStartup;

		while (true) {
			if (Vector3.Distance (points [points.Length - 1], PlayerMovement.instance.transform.position) < generateRoadRadius) {

				float progress = PlayerMovement.instance.progress;
				float numerator = progress * CurveCount;

				AddCurve();
				PlayerMovement.instance.progress = numerator / CurveCount;

				if (Time.realtimeSinceStartup - startTime > 1f / Application.targetFrameRate) {
					yield return null;
					startTime = Time.realtimeSinceStartup;
				}
				loaded = false;

			} else {
				loaded = true;
				terrain.CheckAllChunksForRoad();
				if (WorldManager.instance.doDecorate)
					WorldManager.instance.DoLoadDecorations();
				else WorldManager.instance.FinishLoading();
				yield break;
			}
		}
	}

	// Adds a new curve to the road bezier
	void AddCurve () {
		//float startTime = Time.realtimeSinceStartup;
		float displacedDirection = placementDistance * variance; //placementRange;
		maxSlope = WorldManager.instance.roadSlope;

		Vector3 point;
		if (points.Length > 0) point = points [points.Length - 1];
		else point = PlayerMovement.instance.transform.position;

		Vector3 direction = GetDirection (1f) * placementDistance;
		Array.Resize (ref points, points.Length + 3);

		RaycastHit hit;

		for (int i=3; i>0; i--) {
			point += direction + new Vector3(
				UnityEngine.Random.Range(-displacedDirection, displacedDirection), 
				0f, 
				UnityEngine.Random.Range(-displacedDirection, displacedDirection)
			);
				
			Vector3 rayStart = point + new Vector3 (0f, WorldManager.instance.heightScale, 0f);

			float dist = Vector2.Distance (new Vector2 (points [points.Length-4].x, point.x), new Vector2 (points [points.Length-4].z, point.z));
			if (Physics.Raycast(rayStart, Vector3.down, out hit, Mathf.Infinity)) {
				point.y += Mathf.Clamp(hit.point.y-point.y, -dist*maxSlope, dist*maxSlope);
			}

			points[PointsCount - i] = point;
		}

		terrain.OnExtendRoad ();

		Array.Resize (ref modes, modes.Length + 1);
		modes[modes.Length - 1] = modes[modes.Length -2];
		EnforceMode (points.Length - 4);
		steps += stepsPerCurve;
		Build();
		DoBulldoze(PlayerMovement.instance.moving ? PlayerMovement.instance.progress : 0f);
	
	}
			
		

	// Marks all points between player and newly created points for leveling
	public void DoBulldoze (float startProgress) {
		StopCoroutine ("Bulldoze");
		StartCoroutine("Bulldoze", startProgress);
	}

	IEnumerator Bulldoze (float startProgress) {
		float startTime = Time.realtimeSinceStartup;
		float progress = startProgress;
		float diff = 1f - progress;
		float resolution = WorldManager.instance.roadPathCheckResolution * (1f - startProgress);
		while (progress < 1f) {
		//	Debug.Log("Bulldoze|Progress: "+progress+"|Point: "+GetPoint(progress));
			Vector3 point = GetPoint(progress);
			List<Vector3> points = new List<Vector3> () {
				point,
				//point+BezRight(point)*width/3f,
				//point-BezRight(point)*width/3f,
				//point+BezRight(point)*width,
				//point-BezRight(point)*width
			};
			//DynamicTerrain.instance.vertexmap.DoCheckRoads (GetPoint(progress));
			terrain.vertexmap.DoCheckRoads (points);
			progress += diff / resolution;

			if (Time.realtimeSinceStartup - startTime > 1f / Application.targetFrameRate) {
				yield return null;
				startTime = Time.realtimeSinceStartup;
			}
		}
		yield return null;
	}

	// Sets the road mesh
	public void Build () {

		// Populate vertex, UV, and triangles lists
		BuildRoadMesh ();

		// Apply lists
		mesh.vertices = verts.ToArray();
		mesh.uv = uvs.ToArray();
		mesh.triangles = tris.ToArray();

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

		verts.Clear ();
		uvs.Clear ();
		tris.Clear ();

		float progressI = 0f;
		Vector3 pointI = GetPoint (progressI);
		Vector3 dirI = GetDirection(progressI);
		Vector3 rightI = BezRight (dirI);
		Vector3 downI = BezDown (dirI);

		verts.Add(pointI + width * -rightI);
		uvs.Add(new Vector2(-0.25f, 0f));
		int leftDownI = 0;

		verts.Add(pointI + width *rightI);
		uvs.Add(new Vector2(1.25f, 0f));
		int rightDownI = 1;

		verts.Add(
			pointI + slope * width * -rightI + height * -downI
		);
		uvs.Add(new Vector2(1-slope-0.25f, 0f));
		int leftUpI = 2;

		verts.Add(
			pointI + slope * width * rightI + height * -downI
		);
		uvs.Add(new Vector2(slope+0.25f, 1f));
		int rightUpI = 3;

		bool flipUVs = true;

		for (int i = 1; i<steps; i++) {
			int num = i;

			float progressF = (float)(num) / (float)steps;
			Vector3 pointF = GetPoint (progressF);
			Vector3 dirF = GetDirection(progressF);
			Vector3 rightF = BezRight(dirF);
			Vector3 downF = BezDown(dirF);

			UVProgress += Vector3.Distance (pointF, pointI);
			float UVValue = 0.5f + 0.5f * Mathf.Sin(UVProgress);

			verts.Add(pointF + width * -rightF);
			uvs.Add(new Vector2(-0.25f, UVValue));
			int leftDownF = num * 4;

			verts.Add(pointF + width * rightF);
			uvs.Add(new Vector2(1.25f, UVValue));
			int rightDownF = num * 4 + 1;

			verts.Add(
				pointF + slope * width * -rightF + height * -downF
			);
			uvs.Add(new Vector2(1-slope-0.25f, UVValue));
			int leftUpF = num * 4 + 2;

			verts.Add(
				pointF + slope * width * 
				rightF + height * -downF
			);
			uvs.Add(new Vector2(slope+0.25f, UVValue));
			int rightUpF = num * 4 + 3;


			// Left slope
			tris.Add (leftDownI);
			tris.Add (leftUpI);
			tris.Add (leftDownF);

			tris.Add (leftDownF);
			tris.Add (leftUpI);
			tris.Add (leftUpF);


			// Right slope
			tris.Add (rightUpI);
			tris.Add (rightDownI);
			tris.Add (rightDownF);

			tris.Add (rightUpF);
			tris.Add (rightUpI);
			tris.Add (rightDownF);


			// Road surface plane
			tris.Add (leftUpF);
			tris.Add (rightUpI);
			tris.Add (rightUpF);

			tris.Add (leftUpI);
			tris.Add (rightUpI);
			tris.Add (leftUpF);


			progressI = progressF;
			pointI = pointF;
			leftDownI = leftDownF;
			rightDownI = rightDownF;
			leftUpI = leftUpF;
			rightUpI = rightUpF;

			flipUVs = !flipUVs;
		}
	}

	#endregion
}
