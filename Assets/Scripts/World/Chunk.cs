// Chunk.cs
// ©2016 Team 95

using Route95.Core;

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Route95.World {

	/// <summary>
	/// Comparer class for chunks.
	/// </summary>
    public class ChunkComparer : IComparer<Chunk> {
        int IComparer<Chunk>.Compare(Chunk x, Chunk y) {
            return x.Priority.CompareTo(y.Priority);
        }
    }

    /// <summary>
    /// A chunk is a square mesh that's part of the larger terrain object.
    /// Chunks dynamically load and unload as the player gets closer and farther.
    /// This un/loading will be taken care of by the DynamicTerrain class. 
    /// </summary>
    public class Chunk : MonoBehaviour, IComparable<Chunk>, IPoolable {

        #region Chunk Vars

		/// <summary>
		/// Coordinates in chunk grid.
		/// </summary>
        int _x, _y;

		/// <summary>
		/// 3D mesh to use for the chunk.
		/// </summary>
		Mesh _mesh;

		/// <summary>
		/// Number of vertices used in the mesh.
		/// </summary>
        int _numVerts;

        /// <summary>
		/// Vertices used in the mesh.
		/// </summary>
        Vector3[] _verts;

		/// <summary>
		/// Triangles used in the mesh.
		/// </summary>
        int[] _triangles;

		/// <summary>
		/// Normals used for each mesh vertex.
		/// </summary>
        Vector3[] _normals;

		/// <summary>
		/// UVs used for each mesh vertex.
		/// </summary>
        Vector2[] _uvs;

		/// <summary>
		/// Colors at each mesh vertex.
		/// </summary>
        Color[] _colors;

		/// <summary>
		/// Vertex map coordinates of each mesh vertex
		/// </summary>
        IntVector2[] _coords;

		/// <summary>
		/// References to representative vertex map vertices for each mesh vertex.
		/// </summary>
        Vertex[] _mapVerts;

		/// <summary>
		/// List of decorations parented to this chunk.
		/// </summary>
        List<GameObject> _decorations;

		/// <summary>
		/// This chunk's priority when considering chunks to update.
		/// </summary>
        float _priority = 0f;

		/// <summary>
		/// This chunk's grass emitter.
		/// </summary>
        GameObject _grassEmitter;

		/// <summary>
		/// Has this chunk checked for roads?
		/// </summary>
        bool _hasCheckedForRoad = false;

		/// <summary>
		/// Chunk has road on it.
		/// </summary>
        bool _hasRoad = false;

		/// <summary>
		/// Chunk is within one chunk distance of a road.
		/// </summary>
        bool _nearRoad = false;

		/// <summary>
		/// Is the chunk currently updating its vertices?
		/// </summary>
        bool _isUpdatingVerts = false;

		/// <summary>
		/// Does the chunk need a collider update?
		/// </summary>
        bool _needsColliderUpdate = false;

		/// <summary>
		/// Does the chunk need a color update?
		/// </summary>
        bool _needsColorUpdate = false;

        #endregion
        #region IComparable Implementations

        int IComparable<Chunk>.CompareTo(Chunk other) {
            if (other == null)
                throw new ArgumentException("Other object not a chunk!");

            if (_priority > other._priority) return 1;
            else if (_priority == other._priority) return 0;
            else return -1;
        }

        #endregion
        #region IPoolable Implementations

        void IPoolable.OnPool() {
            gameObject.SetActive(false);
            _priority = 0;
        }

        void IPoolable.OnDepool() {
            gameObject.SetActive(true);
        }

		#endregion
		#region Properties

		public int X { get { return _x; } }

		public int Y { get { return _y; } }

		public float Priority {
			get { return _priority; }
			set { _priority = value; }
		}

		public bool HasRoad { get { return _hasRoad; } }

		public bool NearRoad { get { return _nearRoad; } }

		public bool HasCheckedForRoad {
			get { return _hasCheckedForRoad; }
			set { _hasCheckedForRoad = value; }
		}

		#endregion
		#region Chunk Methods

		/// <summary>
		/// Initializes a brand new chunk at x and y.
		/// </summary>
		public void Initialize(int x, int y) {

            DynamicTerrain terrain = DynamicTerrain.Instance;

            // Init vars
            _x = x;
            _y = y;

            VertexMap vmap = terrain.VertexMap;
            int chunkRes = WorldManager.Instance.ChunkResolution;
            float chunkSize = WorldManager.Instance.ChunkSize;

            // Generate vertices
            _verts = CreateUniformVertexArray(chunkRes);
            _numVerts = _verts.Length;

            // Generate triangles
            _triangles = CreateSquareArrayTriangles(chunkRes);

            // Init normals
            _normals = new Vector3[_numVerts];

            // Generate UVs
            _uvs = CreateUniformUVArray(chunkRes);

            // Init colors
            _colors = new Color[_numVerts];

            // Init coords and mapVerts
            _coords = new IntVector2[_numVerts];
            _mapVerts = new Vertex[_numVerts];

            // Build initial chunk mesh
            _mesh = CreateChunkMesh();

            // Assign mesh
            GetComponent<MeshFilter>().mesh = _mesh;

            // Move GameObject to appropriate position
            transform.position = new Vector3(x * chunkSize - chunkSize / 2f, 0f, y * chunkSize - chunkSize / 2f);

            // Initialize name
            gameObject.name = "Chunk (" + x + "," + y + ") Position:" + transform.position.ToString();

            // Init decorations list
            _decorations = new List<GameObject>();

            // Register all vertices with vertex map
            // Move vertices, generate normals/colors
            for (int i = 0; i < _numVerts; i++) {

                // Init normal/color
                _normals[i] = Vector3.up;
                _colors[i] = Color.white;

                // Get VMap coords
                IntVector2 coord = IntToV2(i);
                _coords[i] = coord;

                // Get corresponding vertex
                _mapVerts[i] = vmap.VertexAt(coord, true);

                // If vertex exists, get height
                UpdateVertex(i, _mapVerts[i].Height);
                UpdateColor(i, _mapVerts[i].Color);
            }

            // Assign material
            MeshRenderer renderer = GetComponent<MeshRenderer>();
            renderer.sharedMaterial = WorldManager.Instance.TerrainMaterial;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            renderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

            // Assign collision mesh
            MeshCollider collider = GetComponent<MeshCollider>();
            collider.sharedMesh = _mesh;
            collider.convex = false;

            // Init rigidbody
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            rigidbody.freezeRotation = true;
            rigidbody.isKinematic = true;
            rigidbody.useGravity = false;
            rigidbody.constraints = RigidbodyConstraints.FreezeAll;

            // Add grass system
            _grassEmitter = GameObject.Instantiate(WorldManager.Instance.GrassEmitterPrefab);
            _grassEmitter.transform.parent = transform;
            _grassEmitter.transform.localPosition = Vector3.zero;

            // Randomize grass density
            ParticleSystem sys = _grassEmitter.GetComponent<ParticleSystem>();
            sys.maxParticles = UnityEngine.Random.Range(0, WorldManager.Instance.GrassPerChunk);
            sys.playOnAwake = true;

            // Assign particle system emission shape
            ParticleSystem.ShapeModule shape = sys.shape;
            shape.mesh = _mesh;

            // Assign particle system emission rate
            ParticleSystem.EmissionModule emit = sys.emission;
            emit.rate = new ParticleSystem.MinMaxCurve(WorldManager.Instance.DecorationsPerStep);

            UpdateCollider();
        }

        /// <summary>
        /// Resets necessary variables after de-pooling a chunk.
        /// </summary>
        public void Reuse(int x, int y) {
            DynamicTerrain terrain = DynamicTerrain.Instance;
			VertexMap vmap = terrain.VertexMap;
			float chunkSize = WorldManager.Instance.ChunkSize;

            // Update vars
            _x = x;
            _y = y;

            // Move chunk to appropriate position
            transform.position = new Vector3(x * chunkSize - chunkSize / 2f, 0f, y * chunkSize - chunkSize / 2f);

            // Update chunk name
            gameObject.name = "Chunk (" + x + "," + y + ") Position:" + transform.position.ToString();

            _priority = 0f;

            // Clear decoration list
            _decorations.Clear();

            // Register all vertices with vertex map
            // Move vertices, generate normals/colors
            for (int i = 0; i < _numVerts; i++) {

                // Get VMap coords
                IntVector2 coord = IntToV2(i);
                _coords[i] = coord;

                // Get corresponding vertex
                _mapVerts[i] = vmap.VertexAt(coord, true);

                // Get height from vertex
                UpdateVertex(i, _mapVerts[i].Height);

            }

            _hasCheckedForRoad = false;

            UpdateCollider();
        }

        /// <summary>
        /// Creates a uniform vertex array.
        /// </summary>
        /// <returns>The uniform vertex array.</returns>
        /// <param name="vertexSize">Vertex size.</param>
        Vector3[] CreateUniformVertexArray(int vertexSize) {
            float chunkSize = WorldManager.Instance.ChunkSize;
            int chunkRes = WorldManager.Instance.ChunkResolution;
            float scale = chunkSize / (chunkRes - 1);
            int numVertices = vertexSize * vertexSize;
            Vector3[] uniformArray = new Vector3[numVertices];
            for (int vert = 0; vert < numVertices; vert++) {
                uniformArray[vert] = new Vector3(vert % vertexSize, 0, vert / vertexSize); //create vertex
                uniformArray[vert] = uniformArray[vert] * scale; //scale vector appropriately
            }
            return uniformArray;
        }

        /// <summary>
        /// Takes the number of vertices per side, returns a uniform array of UV coords for a square vertex array
        /// </summary>
        /// <param name="vertexSize"></param>
        /// <returns></returns>
        Vector2[] CreateUniformUVArray(int vertexSize) {
            int numVertices = vertexSize * vertexSize;
            Vector2[] uniformUVArray = new Vector2[numVertices];
            for (int vert = 0; vert < numVertices; vert++) {
                int x = vert % vertexSize; //get x position of vert
                int y = vert / vertexSize; //get y position of vert
                float u = x / (float)(vertexSize - 1); // normalize x into u between 0 and 1
                float v = ((vertexSize - 1) - y) / (float)(vertexSize - 1); //normalize y into v between 0 and 1 and flip direction
                uniformUVArray[vert] = new Vector2(u, v);
            }
            return uniformUVArray;
        }

        /// <summary>
        /// Takes the number of vertices per side, returns indices (each group of three defines a triangle) into a square vertex array to form mesh 
        /// </summary>
        /// <param name="vertexSize"></param>
        /// <returns></returns>
        int[] CreateSquareArrayTriangles(int vertexSize) {
            int numTriangles = 2 * vertexSize * (vertexSize - 1);//a mesh with n^2 vertices has 2n(n-1) triangles
            int[] triangleArray = new int[numTriangles * 3]; //three points per triangle 
            int numVertices = vertexSize * vertexSize;
            int i = 0; //index into triangleArray (next two are the sibling vertices for its triangle, add 3 to jump to next triangle)
            for (int vert = 0; vert < numVertices - vertexSize; vert++) {
                /* Make these types of triangles
                 * 3---2
                 * *\**|
                 * **\*|
                 * ***\|
                 * ****1
                 */
                if (((vert + 1) % vertexSize) != 0) { //if vertex is not on the right edge
                    triangleArray[i] = vert + vertexSize; //vertex 1
                    triangleArray[i + 1] = vert + 1; //vertex 2
                    triangleArray[i + 2] = vert; //vertex 3
                    i = i + 3; //jump to next triangle
                }

                /* Make these types of triangles
                 * ****3 
                 * ***7|
                 * **7*|
                 * *7**|
                 * 1---2
                 */
                if ((vert % vertexSize) != 0) { //if vertex is not on the left edge
                    triangleArray[i] = vert + vertexSize - 1; //vertex 1
                    triangleArray[i + 1] = vert + vertexSize; //vertex 2
                    triangleArray[i + 2] = vert; //vertex 3
                    i = i + 3; //jump to next triangle
                }
            }
            return triangleArray;
        }

        /// <summary>
        /// Creates the chunk GameObject.
        /// </summary>
        /// <returns>The chunk.</returns>
        /// <param name="vertices">Vertices.</param>
        /// <param name="normals">Normals.</param>
        /// <param name="UVcoords">U vcoords.</param>
        /// <param name="triangles">Triangles.</param>
        Mesh CreateChunkMesh() {

            // Create mesh
            Mesh chunkMesh = new Mesh();

            // Optimize mesh for frequent updates
            chunkMesh.MarkDynamic();

            // Assign vertices/normals/UVs/tris/colors
            chunkMesh.vertices = _verts;
            chunkMesh.normals = _normals;
            chunkMesh.uv = _uvs;
            chunkMesh.triangles = _triangles;
            chunkMesh.colors = _colors;

            return chunkMesh;
        }

        /// <summary>
        /// Updates the physics collider.
        /// </summary>
        public void UpdateCollider() {
            _needsColliderUpdate = false;

            ParticleSystem grass = _grassEmitter.GetComponent<ParticleSystem>();

            // Clear current grass
            grass.Clear();

            // Reassign mesh vertices/normals
            _mesh.vertices = _verts;
            _mesh.normals = _normals; // NEEDED FOR PROPER LIGHTING

            // Recalculate bounding box
            _mesh.RecalculateBounds();

            // Reassign collider mesh
            GetComponent<MeshCollider>().sharedMesh = _mesh;

            // Assign particle system emission shape
            ParticleSystem.ShapeModule shape = grass.shape;
            shape.mesh = _mesh;

            // Replace decorations
            ReplaceDecorations();

            // Replace grass
            _grassEmitter.GetComponent<ParticleSystem>().Play();

        }

        /// <summary>
        /// Updates the verrex colors.
        /// </summary>
        public void UpdateColors() {
            _mesh.colors = _colors;
            _needsColorUpdate = false;
        }

        /// <summary>
        /// Updates a vertex.
        /// </summary>
        /// <param name="index">Index.</param>
        /// <param name="height">Height.</param>
        /// <param name="normal">Normal.</param>
        public void UpdateVertex(int index, float height, bool forceUpdate = false) {
            try {

                // Check if height update is needed
                if (_verts[index].y != height) {
                    _priority++;
                    _verts[index].y = height;
                    if (forceUpdate) _mesh.vertices = _verts;
                    _needsColliderUpdate = true;
                }

            }
            catch (IndexOutOfRangeException e) {
                Debug.LogError("Chunk.UpdateVertex(): invalid index " + index + "! " + e.Message);
                return;
            }
        }

        /// <summary>
        /// Updates the color.
        /// </summary>
        /// <param name="index">Index.</param>
        /// <param name="blendValue">Blend value.</param>
        public void UpdateColor(int index, Color color) {

            // Check if color update is needed
            if (_colors[index] != color) {
                _colors[index] = color;
                _needsColorUpdate = true;
            }
        }

        /// <summary>
        /// Checks if a vertex is within range to be updated.
        /// </summary>
        /// <returns><c>true</c>, if dist was checked, <c>false</c> otherwise.</returns>
        /// <param name="dist">Dist.</param>
        /// <param name="updateDist">Update dist.</param>
        /// <param name="margin">Margin.</param>
        private bool CheckDist(float dist, float updateDist, float margin) {
            return ((dist < (updateDist + margin)) && (dist > (updateDist - margin)));
        }

        /// <summary>
        /// Coroutine to update the vertices of a chunk.
        /// </summary>
        /// <returns>The verts.</returns>
        private IEnumerator UpdateVerts() {

            if (!GameManager.Instance.IsLoaded) yield break;

            _isUpdatingVerts = true;
            float margin = WorldManager.Instance.ChunkSize / 2;
            float startTime = Time.realtimeSinceStartup;
            Vector3 playerPos = PlayerMovement.Instance.transform.position;
            Vector3 chunkPos = transform.position;
			DynamicTerrain terrain = DynamicTerrain.Instance;
			VertexMap vmap = terrain.VertexMap;

            int v = 0;
            for (; v < _numVerts; v++) {

                // Get VMap coordinates
                IntVector2 coord = _coords[v];

                // Get coresponding vertex
                Vertex vert = _mapVerts[v];

                // Update vertex height
                UpdateVertex(v, vert.Height);

                if (terrain.FreqData == null) yield break;

                // If vertex is not locked and there is frequency data to use
                if (!vert.Locked) {

                    // Distance between player and vertex
                    Vector3 vertPos = chunkPos + _verts[v];
                    float distance = Vector3.Distance(vertPos, playerPos);

                    // If vertex is close enough
                    if (CheckDist(distance, WorldManager.Instance.VertexUpdateDistance, margin)) {

                        // Calculate new height
                        Vector3 angleVector = vertPos - playerPos;
                        float angle = Vector3.Angle(Vector3.right, angleVector);
                        float linIntInput = angle / 360f;
                        float newY = terrain.FreqData.GetDataPoint(linIntInput) *
                                      WorldManager.Instance.HeightScale;

                        // If new height, set it
                        //if (newY != vmap.VertexAt(coord, false).height) vmap.SetHeight (coord, newY);
                        if (newY != 0f) vmap.AddHeight(coord, newY);
                    }
                }

                if (v == _numVerts - 1) {
                    _isUpdatingVerts = false;
                    yield break;
                }
                else if (Time.realtimeSinceStartup - startTime > GameManager.TargetDeltaTime) {
                    yield return null;
                    startTime = Time.realtimeSinceStartup;
                }
            }
        }

        /// <summary>
        /// Stops the vertex update coroutine.
        /// </summary>
        public void StopUpdatingVerts() {
            StopCoroutine("UpdateVerts");
        }

        /// <summary>
        /// Updates all necessary properties of a chunk.
        /// </summary>
        public void ChunkUpdate() {

            // Update collider if necessary
            if (_needsColliderUpdate) UpdateCollider();

            // Update vertex colors if necessary
            if (_needsColorUpdate) UpdateColors();

            // Check for road if necessary
            if (!_hasCheckedForRoad && WorldManager.Instance.Road.IsLoaded)
                CheckForRoad(PlayerMovement.Instance.Moving ? PlayerMovement.Instance.Progress : 0f);

            // Update verts if possible
            if (!_isUpdatingVerts) StartCoroutine("UpdateVerts");

            _priority = 0f;
        }

        /// <summary>
        /// Checks if a chunk is nearby to or has a road.
        /// </summary>
        /// <param name="startProgress">Start progress.</param>
        public void CheckForRoad(float startProgress) {
            _hasCheckedForRoad = true;
            Road road = WorldManager.Instance.Road;
			DynamicTerrain terrain = DynamicTerrain.Instance;
            Vector3 chunkPos = transform.position;
			float chunkSize = WorldManager.Instance.ChunkSize;
            float checkResolution = (1f - startProgress) * WorldManager.Instance.RoadPathCheckResolution;

            // Set boundaries for "near road" consideration
            Vector2 nearMin = new Vector2(chunkPos.x - chunkSize, chunkPos.z - chunkSize);
            Vector2 nearMax = new Vector2(chunkPos.x + chunkSize * 2f, chunkPos.z + chunkSize * 2f);

            // Set boundaries for "has road" consideration
            Vector2 hasMin = new Vector2(chunkPos.x, chunkPos.z);
            Vector2 hasMax = new Vector2(chunkPos.x + chunkSize, chunkPos.z + chunkSize);

            float progress = startProgress;
            while (progress <= 1f) {
                // Sample road and check distance to chunk
                Vector3 sample = road.GetPoint(progress);
                if (sample.x >= nearMin.x && sample.x <= nearMax.x &&
                    sample.z >= nearMin.y && sample.z <= nearMax.y) {

                    if (!_nearRoad) {
                        gameObject.name += "|nearRoad";
                        terrain.AddCloseToRoadChunk(this);
                    }
                    _nearRoad = true;

                    // If near road, check if has road
                    if (sample.x >= hasMin.x && sample.x <= hasMax.x &&
                        sample.z >= hasMin.y && sample.z <= hasMax.y) {

                        terrain.AddRoadChunk(this);
                        gameObject.name += "|hasRoad";
                        _grassEmitter.SetActive(false);
                        _hasRoad = true;
                        return;
                    }
                }
                progress += 1f / checkResolution;
            }
        }

        /// <summary>
        /// Converts a world position to the nearest vertex map coords.
        /// </summary>
        /// <returns>The nearest V map coords.</returns>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        public static IntVector2 ToNearestVMapCoords(float x, float y) {
			//if (x < 0) Debug.Log(x);
			float chunkSize = WorldManager.Instance.ChunkSize;
			int chunkRes = WorldManager.Instance.ChunkResolution;
            IntVector2 result = new IntVector2(
                Mathf.FloorToInt((x + chunkSize / 2f) * (chunkRes - 1) / chunkSize),
                Mathf.FloorToInt((y + chunkSize / 2f) * (chunkRes - 1) / chunkSize)
            );
            //if (x < 0) Debug.Log(result.x);
            //Debug.Log (x+","+y +" mapped to "+result.ToString());
            return result;
        }

        /// <summary>
        /// Converts a vertex index to a vertex map coordinate.
        /// </summary>
        /// <returns>The to v2.</returns>
        /// <param name="i">The index.</param>
        IntVector2 IntToV2(int i) {
            int chunkRes = WorldManager.Instance.ChunkResolution;

            int xi = _x * (chunkRes - 1) + i % chunkRes;
            int yi = _y * (chunkRes - 1) + i / chunkRes;
            return new IntVector2(xi, yi);
        }

        /// <summary>
        /// Resets height of all decorations on a chunk
        /// </summary>
        public void ReplaceDecorations() {
            foreach (Transform tr in GetComponentsInChildren<Transform>()) {

                // Skip chunk itself
                if (tr == transform || tr == _grassEmitter.transform) continue;

                // Raycast down
                RaycastHit hit;
                Vector3 rayOrigin = new Vector3(tr.position.x, WorldManager.Instance.HeightScale, tr.position.z);
                if (Physics.Raycast(rayOrigin, Vector3.down, out hit, Mathf.Infinity))
                    tr.position = new Vector3(tr.position.x, hit.point.y +
                        tr.gameObject.GetComponent<Decoration>().PositionOffset.y, tr.position.z);
            }
        }

        /// <summary>
        /// Removes and pools all decorations on the chunk.
        /// </summary>
        public void RemoveDecorations() {
            foreach (GameObject decoration in _decorations)
                WorldManager.Instance.RemoveDecoration(decoration);
        }

        /// <summary>
        /// Sets terrain debug colors.
        /// </summary>
        /// <param name="color"></param>
        public void SetDebugColors(DynamicTerrain.DebugColors color) {
            switch (color) {
                case DynamicTerrain.DebugColors.Constrained:
                    for (int v = 0; v < _numVerts; v++) {
                        _colors[v] = _mapVerts[v].NoDecorations ? Color.black : Color.white;
                    }
                    _mesh.colors = _colors;
                    break;
            }
        }

		public void AddDecoration (GameObject deco) {
			_decorations.Add(deco);
		}

        #endregion

    }
}