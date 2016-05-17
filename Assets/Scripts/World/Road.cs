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
		points = new List<Vector3>();

	}

	public void Start () {

		// Copy vars from WM
		width = WorldManager.instance.roadWidth;
		height = WorldManager.instance.roadHeight;
		slope = WorldManager.instance.roadSlope;

		generateRoadRadius = WorldManager.instance.roadExtendRadius;
		variance = WorldManager.instance.roadVariance;
		placementDistance = WorldManager.instance.roadPlacementDistance;
		maxSlope = WorldManager.instance.roadMaxSlope;

		stepsPerCurve = WorldManager.instance.roadStepsPerCurve;

		terrain = WorldManager.instance.terrain;

		// Build mesh
		Reset ();
		
	}

	public void Update () {

		variance = WorldManager.instance.roadVariance;
		maxSlope = WorldManager.instance.roadMaxSlope;

		if (!loaded) return;

		// Remove far away points behind the player
		if (Vector3.Distance (points.Head(), PlayerMovement.instance.transform.position) > generateRoadRadius) {

			RemoveCurve();

			PlayerMovement.instance.progress -= 0.5f / (float)CurveCount;

		}

		// Create new points in front of player
		while (Vector3.Distance (points.Tail(), PlayerMovement.instance.transform.position) < generateRoadRadius) {



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
		points = new List<Vector3>();
		Vector3 p = PlayerMovement.instance.transform.position;
		points.Add(new Vector3 (p.x, 0f, p.y));
		points.Add(new Vector3 (p.x - 60f, p.y, p.z));
		points.Add(new Vector3 (p.x + 60f, p.y, p.z + 130f));
		points.Add(new Vector3 (p.x, p.y, p.z + 150f));

		modes = new List<Bezier.BezierControlPointMode>();
		for (int i=0; i<points.Count / 2; i++)
			modes.Add(Bezier.BezierControlPointMode.Mirrored);
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
			if (Vector3.Distance (points.Tail(), PlayerMovement.instance.transform.position) < generateRoadRadius) {

				float progress = PlayerMovement.instance.progress;
				float numerator = progress * CurveCount;

				AddCurve();
				PlayerMovement.instance.progress = numerator / CurveCount;

				if (Time.realtimeSinceStartup - startTime > GameManager.instance.targetDeltaTime) {
					yield return null;
					startTime = Time.realtimeSinceStartup;
				}
				loaded = false;

			//} else if (Vector3.Distance (points.Head(), PlayerMovement.instance.transform.position) < generateRoadRadius) {

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
		float displacedDirection = placementDistance * variance; //placementRange;

		Vector3 point;
		if (points.Count > 0) point = points.Tail();
		else point = PlayerMovement.instance.transform.position;
		Vector3 old = point;

		Vector3 direction = GetDirection (1f) * placementDistance;

		RaycastHit hit;

		for (int i=3; i>0; i--) {
			float a = UnityEngine.Random.Range (0f, Mathf.PI * 2f);
			float d = UnityEngine.Random.Range (displacedDirection * 0.75f, displacedDirection);

			point += direction + new Vector3 (d*Mathf.Cos(a), 0f, d*Mathf.Sin(a));
				
			Vector3 rayStart = point + new Vector3 (0f, WorldManager.instance.heightScale, 0f);

			float dist = Vector2.Distance (new Vector2 (old.x, point.x), new Vector2 (old.z, point.z));
			if (Physics.Raycast(rayStart, Vector3.down, out hit, Mathf.Infinity)) {
				point.y += Mathf.Clamp(hit.point.y-point.y, -dist*maxSlope, dist*maxSlope);
			}

			//points[PointsCount - i] = point;
			points.Add(point);
		}

		terrain.OnExtendRoad ();

		modes.Add (modes.Tail());
		EnforceMode (points.Count - 4);
		steps += stepsPerCurve;
		Build();
		DoBulldoze(PlayerMovement.instance.moving ? PlayerMovement.instance.progress : 0f);
	
	}

	public void Backtrack () {
		float displacedDirection = placementDistance * variance; //placementRange;

		Vector3 point;
		if (points.Count > 0) point = points [0];
		else point = PlayerMovement.instance.transform.position;

		Vector3 direction = -GetDirection (0f) * placementDistance;



		RaycastHit hit;

		for (int i=3; i>0; i--) {
			float a = UnityEngine.Random.Range (0f, Mathf.PI * 2f);
			float d = UnityEngine.Random.Range (displacedDirection * 0.75f, displacedDirection);

			point += direction + new Vector3 (d*Mathf.Cos(a), 0f, d*Mathf.Sin(a));

			Vector3 rayStart = point + new Vector3 (0f, WorldManager.instance.heightScale, 0f);

			float dist = Vector2.Distance (new Vector2 (points [points.Count-4].x, point.x), new Vector2 (points [points.Count-4].z, point.z));
			if (Physics.Raycast(rayStart, Vector3.down, out hit, Mathf.Infinity)) {
				point.y += Mathf.Clamp(hit.point.y-point.y, -dist*maxSlope, dist*maxSlope);
			}

			points[PointsCount - i] = point;
		}

		terrain.OnExtendRoad ();

		modes.Add (modes[modes.Count-1]);
		EnforceMode (points.Count - 4);
		steps += stepsPerCurve;
		Build();
		DoBulldoze(PlayerMovement.instance.moving ? PlayerMovement.instance.progress : 0f);

	}

	void RemoveCurve () {

		// Update points array
		/*Vector3[] newPoints = new Vector3[points.Length - 3];
		BezierControlPointMode[] newModes = new BezierControlPointMode[modes.Length-1];
		for (int i = 0; i < newPoints.Length; i++) {
			newPoints [i] = points [i + 3];
			if (i % 3 == 0) newModes[i/3] = modes[i/3 + 1];
		}

		points = newPoints;
		modes = newModes;*/

		for (int i=0; i<3; i++) points.RemoveAt(0);
		modes.RemoveAt(0);

		steps -= stepsPerCurve;
		Build();

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

			if (Time.realtimeSinceStartup - startTime > GameManager.instance.targetDeltaTime) {
				yield return null;
				startTime = Time.realtimeSinceStartup;
			}
		}
		yield return null;
	}

	// Sets the road mesh
	public void Build () {
		mesh.Clear();

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

		float UVoffset = 0.4f;
		float UVslope = slope;

		float progressI = 0f;
		Vector3 pointI = GetPoint (progressI);
		Vector3 dirI = GetDirection(progressI);
		Vector3 rightI = BezRight (dirI);
		Vector3 downI = BezDown (dirI);

		// Left down
		verts.Add(pointI + width * -rightI);
		uvs.Add(new Vector2(-UVoffset, 0f));
		int leftDownI = 0;

		// Right down
		verts.Add(pointI + width *rightI);
		uvs.Add(new Vector2(1f + UVoffset, 0f));
		int rightDownI = 1;

		// Left up
		verts.Add(pointI + slope * width * -rightI + height * -downI);
		uvs.Add(new Vector2(-UVoffset + UVslope, 0f));
		int leftUpI = 2;

		// Right up
		verts.Add(pointI + slope * width * rightI + height * -downI);
		uvs.Add(new Vector2(1f + UVoffset - UVslope, 1f));
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

			// Left down
			verts.Add(pointF + width * -rightF);
			uvs.Add(new Vector2(-UVoffset, UVValue));
			int leftDownF = num * 4;

			// Right down
			verts.Add(pointF + width * rightF);
			uvs.Add(new Vector2(1f + UVoffset, UVValue));
			int rightDownF = num * 4 + 1;

			// Left up
			verts.Add(pointF + slope * width * -rightF + height * -downF);
			uvs.Add(new Vector2(-UVoffset + UVslope, UVValue));
			int leftUpF = num * 4 + 2;

			// Right up
			verts.Add(pointF + slope * width * rightF + height * -downF);
			uvs.Add(new Vector2(1f + UVoffset - UVslope, UVValue));
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
