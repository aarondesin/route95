// Road.cs
// ©2016 Team 95

using Route95.Core;
using Route95.UI;

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Route95.World {

	/// <summary>
	/// Class to handle the road object.
	/// </summary>
    public class Road : Bezier {


        #region Road Vars

		/// <summary>
		/// Is the road currently loading?
		/// </summary>
        bool _loading = false;

		/// <summary>
		/// Is the road fully loaded?
		/// </summary>
        bool _loaded = false;

		/// <summary>
		/// List of road points to check.
		/// </summary>
        List<Vector3> _toCheck;

		/// <summary>
		/// Current number of subdivision steps.
		/// </summary>
        int _steps = 0;

		/// <summary>
		/// Road mesh.
		/// </summary>
        Mesh _mesh;

		/// <summary>
		/// Vertices in road mesh.
		/// </summary>
        List<Vector3> _verts;

		/// <summary>
		/// Road mesh UVs.
		/// </summary>
        List<Vector2> _uvs;

		/// <summary>
		/// Road mesh triangles.
		/// </summary>
        List<int> _tris;

		/// <summary>
		/// UV step progress.
		/// </summary>
        float _UVProgress = 0f;

        #endregion
        #region Unity Callbacks

        void Awake() {

			// Init vars
            _toCheck = new List<Vector3>();

            _verts = new List<Vector3>();
            _uvs = new List<Vector2>();
            _tris = new List<int>();

            // Init mesh
            _mesh = new Mesh();

            // Init road points
            _points = new List<Vector3>();
        }

        public void Update() {
            if (!_loading) return;

            if (!_loaded) {
                List<string> loadMessages = new List<string>() {
					"Blazing a trail...",
					"Rerouting...",
					"Following star maps..."
				};
                LoadingScreen.Instance.SetLoadingMessage(loadMessages.Random());
            }

            float progress = PlayerMovement.Instance.Progress;

            Vector3 playerPosition = PlayerMovement.Instance.transform.position;

            bool changesMade = false;

			float generateRoadRadius = WorldManager.Instance.RoadExtendRadius;
			float cleanupRoadRadius = WorldManager.Instance.RoadCleanupRadius;

            // Create new points in front of player
            if (Vector3.Distance(_points.Tail(), playerPosition) < generateRoadRadius) {

                float numerator = progress * CurveCount;

                // Create curve
                AddCurve();

                // Update player progress
                PlayerMovement.Instance.Progress = numerator / CurveCount;

                changesMade = true;
            }
            else

            // If road beginning is too close
            if (Vector3.Distance(_points.Head(), playerPosition) < generateRoadRadius) {

                float numerator = progress * CurveCount * 2f;
                float denominatorOld = CurveCount * 2f;

                // Generate backwards
                Backtrack();

                // Update player progress
                PlayerMovement.Instance.Progress = (numerator + 2f) / (denominatorOld + 2f);

                changesMade = true;

                // If road beginning is too far away
            }
            else if (Vector3.Distance(_points.Head(), playerPosition) > cleanupRoadRadius) {

                float numerator = progress * CurveCount * 2f;
                float denominator = CurveCount * 2f;

                // Remove first curve
                RemoveCurve();

                // Update player progress
                PlayerMovement.Instance.Progress = (numerator - 2f) / (denominator - 2f);

                changesMade = true;


            }
            else if (!_loaded) {
                _loaded = true;
				float height = WorldManager.Instance.RoadHeight;
                PlayerMovement.Instance.transform.position = GetPoint(0.6f) + new Vector3(0f, 2.27f + height, 0f);
                PlayerMovement.Instance.transform.LookAt(GetPoint(0.6f) + GetVelocity(0.6f), Vector3.up);
                if (WorldManager.Instance.DoDecorate)
                    WorldManager.Instance.DoLoadDecorations();
                else WorldManager.Instance.FinishLoading();
            }

            if (changesMade) Build();
            else if (_toCheck.Count > 0) Check(_toCheck.PopFront());

        }

		#endregion
		#region Properties

		public bool IsLoaded { get { return _loaded; } }

		#endregion
		#region Road Methods

		/// <summary>
		/// Generates initial road points.
		/// </summary>
		public void Reset() {
			float heightScale = WorldManager.Instance.HeightScale;

            // Get initial point
            Vector3 point = new Vector3(0f, heightScale, 0f);

            // Raycast down to terrain
            RaycastHit hit;
            if (Physics.Raycast(point, Vector3.down, out hit, Mathf.Infinity))
                point.y = hit.point.y;

            // Init points list
            _points = new List<Vector3>() { point };

            // Init modes list
            _modes = new List<BezierControlPointMode>() {
            BezierControlPointMode.Mirrored
        };

            AddCurve(true);
        }

        public void DoLoad() {
            // Build mesh
            Reset();
            _loading = true;
        }

        // Adds a new curve to the road bezier
        void AddCurve(bool ignoreMaxSlope = false) {
			float placementDistance = WorldManager.Instance.RoadPlacementDistance;
			float variance = WorldManager.Instance.RoadVariance;
            float displacedDirection = placementDistance * variance; //placementRange;
			float worldHeightScale = WorldManager.Instance.HeightScale;
			float maxSlope = WorldManager.Instance.RoadMaxSlope;
			int stepsPerCurve = WorldManager.Instance.RoadStepsPerCurve;

            Vector3 point = _points.Tail();
            Vector3 old = point;

            Vector3 direction;
            if (_points.Count == 1) {
                direction = UnityEngine.Random.insideUnitSphere;
                direction.y = 0f;
                direction.Normalize();
                direction *= placementDistance;
            }
            else direction = GetDirection(1f) * placementDistance;

            RaycastHit hit;

            for (int i = 3; i > 0; i--) {
                float a = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
                float d = UnityEngine.Random.Range(displacedDirection * 0.75f, displacedDirection);
                //float d = UnityEngine.Random.Range (0f, variance);

                //point += direction * (1f - d) * placementDistance + new Vector3 (Mathf.Cos(a), 0f, Mathf.Sin(a)) * d * placementDistance;
                point += direction + new Vector3(Mathf.Cos(a), 0f, Mathf.Sin(a)) * d;

                Vector3 rayStart = point + new Vector3(0f, worldHeightScale, 0f);

                float dist = Vector2.Distance(new Vector2(old.x, point.x), new Vector2(old.z, point.z));
                if (Physics.Raycast(rayStart, Vector3.down, out hit, Mathf.Infinity)) {
                    if (ignoreMaxSlope) point.y = hit.point.y;
                    else point.y += Mathf.Clamp(hit.point.y - point.y, -dist * maxSlope, dist * maxSlope);
                }
                else {
                    throw new InvalidOperationException("Failed to place road point!");
                }
                _points.Add(point);
            }

            DynamicTerrain.Instance.OnExtendRoad();

            _modes.Add(_modes.Tail());
            EnforceMode(_points.Count - 4);
            _steps += stepsPerCurve;
            DoBulldoze(PlayerMovement.Instance.Moving ? PlayerMovement.Instance.Progress : 0f);
        }

        public void Backtrack() {
			float placementDistance = WorldManager.Instance.RoadPlacementDistance;
			float variance = WorldManager.Instance.RoadVariance;
            float displacedDirection = placementDistance * variance; //placementRange;
			float maxSlope = WorldManager.Instance.RoadMaxSlope;
			float heightScale = WorldManager.Instance.HeightScale;
			int stepsPerCurve = WorldManager.Instance.RoadStepsPerCurve;

            Vector3 point = _points.Head();
            Vector3 old = point;

            Vector3 direction = -GetDirection(0f) * placementDistance;

            RaycastHit hit;

            for (int i = 3; i > 0; i--) {
                float a = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
                float d = UnityEngine.Random.Range(displacedDirection * 0.75f, displacedDirection);
                //float d = UnityEngine.Random.Range (0f, variance);

                //point += direction * (1f - d) * placementDistance + new Vector3 (Mathf.Cos(a), 0f, Mathf.Sin(a)) * d * placementDistance;
                point += direction + new Vector3(Mathf.Cos(a), 0f, Mathf.Sin(a)) * d;

                Vector3 rayStart = point + new Vector3(0f, heightScale, 0f);

                float dist = Vector2.Distance(new Vector2(old.x, point.x), new Vector2(old.z, point.z));

                if (Physics.Raycast(rayStart, Vector3.down, out hit, Mathf.Infinity))
                    point.y += Mathf.Clamp(hit.point.y - point.y, -dist * maxSlope, dist * maxSlope);

                _points.Insert(0, point);
            }

            DynamicTerrain.Instance.OnExtendRoad();

            _modes.Add(_modes.Tail());
            EnforceMode(4);
            _steps += stepsPerCurve;
            DoBulldoze(0f, 1f / (float)CurveCount);

        }

        /// <summary>
        /// Removes a curve behind the player.
        /// </summary>
        void RemoveCurve() {

            for (int i = 0; i < 3; i++) _points.RemoveAt(0);
            _modes.RemoveAt(0);

			int stepsPerCurve = WorldManager.Instance.RoadStepsPerCurve;
            _steps -= stepsPerCurve;

        }

        // Marks all points between player and newly created points for leveling
        public void DoBulldoze(float startProgress, float endProgress = 1f) {
            //StopCoroutine ("Bulldoze");
            StartCoroutine(Bulldoze(startProgress, endProgress));
        }

        IEnumerator Bulldoze(float startProgress, float endProgress) {
            float startTime = Time.realtimeSinceStartup;
            float progress = startProgress;
            float diff = endProgress - startProgress;
            if (diff < 0f) yield break;
            float resolution = WorldManager.Instance.RoadPathCheckResolution * diff;

            while (progress < endProgress) {
                Vector3 point = GetPoint(progress);
                _toCheck.Add(point);
                progress += diff / resolution;

                if (Time.realtimeSinceStartup - startTime > GameManager.TargetDeltaTime) {
                    yield return null;
                    startTime = Time.realtimeSinceStartup;
                }
            }
            _toCheck.Add(GetPoint(endProgress));

            yield return null;
        }

        void Check(Vector3 point) {
            DynamicTerrain.Instance.VertexMap.DoCheckRoads(point);
        }

        // Sets the road mesh
        public void Build() {
            _mesh.Clear();

            // Populate vertex, UV, and triangles lists
            BuildRoadMesh();

            // Apply lists
            _mesh.vertices = _verts.ToArray();
            _mesh.uv = _uvs.ToArray();
            _mesh.triangles = _tris.ToArray();

            // Recalculate properties
            _mesh.RecalculateNormals();
            _mesh.RecalculateBounds();
            _mesh.Optimize();

            // Set mesh
            GetComponent<MeshFilter>().mesh = _mesh;
            GetComponent<MeshFilter>().sharedMesh = _mesh;
        }

        // Calculates vertices, UVs, and tris for road mesh
        void BuildRoadMesh() {

			float height = WorldManager.Instance.RoadHeight;
			float width = WorldManager.Instance.RoadWidth;
			float slope = WorldManager.Instance.RoadSlope;

            _verts.Clear();
            _uvs.Clear();
            _tris.Clear();

            float UVoffset = 0f;
            //float UVslope = slope;
			float UVslope = 0f;

            float progressI = 0f;
            Vector3 pointI = GetPoint(progressI);
            Vector3 dirI = GetDirection(progressI);
            Vector3 rightI = BezRight(dirI);
            Vector3 downI = BezDown(dirI);

            // Left down
            _verts.Add(pointI + width * -rightI);
            _uvs.Add(new Vector2(-UVoffset, 0f));
            int leftDownI = 0;

            // Right down
            _verts.Add(pointI + width * rightI);
            _uvs.Add(new Vector2(1f + UVoffset, 0f));
            int rightDownI = 1;

            // Left up
            _verts.Add(pointI + slope * width * -rightI + height * -downI);
            _uvs.Add(new Vector2(-UVoffset + UVslope, 0f));
            int leftUpI = 2;

            // Right up
            _verts.Add(pointI + slope * width * rightI + height * -downI);
            _uvs.Add(new Vector2(1f + UVoffset - UVslope, 1f));
            int rightUpI = 3;

            bool flipUVs = true;

            for (int i = 1; i < _steps; i++) {
                int num = i;

                float progressF = (float)(num) / (float)_steps;
                Vector3 pointF = GetPoint(progressF);
                Vector3 dirF = GetDirection(progressF);
                Vector3 rightF = BezRight(dirF);
                Vector3 downF = BezDown(dirF);

                _UVProgress += Vector3.Distance(pointF, pointI) / 20f;

                // Left down
                _verts.Add(pointF + width * -rightF);
                _uvs.Add(new Vector2(-UVoffset, _UVProgress));
                int leftDownF = num * 4;

                // Right down
                _verts.Add(pointF + width * rightF);
                _uvs.Add(new Vector2(1f + UVoffset, _UVProgress));
                int rightDownF = num * 4 + 1;

                // Left up
                _verts.Add(pointF + slope * width * -rightF + height * -downF);
                _uvs.Add(new Vector2(-UVoffset + UVslope, _UVProgress));
                int leftUpF = num * 4 + 2;

                // Right up
                _verts.Add(pointF + slope * width * rightF + height * -downF);
                _uvs.Add(new Vector2(1f + UVoffset - UVslope, _UVProgress));
                int rightUpF = num * 4 + 3;


                // Left slope
                _tris.Add(leftDownI);
                _tris.Add(leftUpI);
                _tris.Add(leftDownF);

                _tris.Add(leftDownF);
                _tris.Add(leftUpI);
                _tris.Add(leftUpF);


                // Right slope
                _tris.Add(rightUpI);
                _tris.Add(rightDownI);
                _tris.Add(rightDownF);

                _tris.Add(rightUpF);
                _tris.Add(rightUpI);
                _tris.Add(rightDownF);


                // Road surface plane
                _tris.Add(leftUpF);
                _tris.Add(rightUpI);
                _tris.Add(rightUpF);

                _tris.Add(leftUpI);
                _tris.Add(rightUpI);
                _tris.Add(leftUpF);


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
}
