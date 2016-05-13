using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/*
 * A chunk is a square mesh that's part of the larger terrain object.
 * Chunks dynamically load and unload as the player gets closer and farther.
 * This un/loading will be taken care of by the DynamicTerrain class. 
 */

public class Chunk: MonoBehaviour {

	#region Chunk Vars

	DynamicTerrain terrain;
	VertexMap vmap;
	int chunkRes;
	float chunkSize;
	public GameObject chunk; // Chunk object/mesh
	public GameObject grassEmitter;
	public int x; //x position in chunk grid
	public int y; //y position in chunk grid
	public float priority = 0f;

	public bool hasCheckedForRoad = false;
	public bool hasRoad = false;  // Chunk has road on it
	public bool nearRoad = false; // Chunk is within one chunk distance of a road

	Mesh colliderMesh;
	public Mesh mesh;
	Vector3[] verts;
	IntVector2[] coords;
	//bool[] constrained;
	int numVerts;
	Vector3[] normals;
	Vector2[] uvs;
	int[] triangles;
	Color[] colors;

	bool isUpdatingVerts = false;
	public bool needsColliderUpdate = false;
	public bool needsColorUpdate = false;

	public List<GameObject> decorations;

	#endregion
	#region Chunk Methods

	public void Initialize (int x, int y) {
		this.x = x;
		this.y = y;

		// Init vars
		terrain = DynamicTerrain.instance;
		vmap = DynamicTerrain.instance.vertexmap;
		chunkRes = WorldManager.instance.chunkResolution;
		chunkSize = WorldManager.instance.chunkSize;

		// Generate vertices
		verts = CreateUniformVertexArray (chunkRes);
		numVerts = verts.Length;

		//
		coords = new IntVector2[numVerts];

		// Init normals
		normals = new Vector3[numVerts];

		// Generate UVs
		uvs = CreateUniformUVArray (chunkRes);

		// Generate triangles
		triangles = CreateSquareArrayTriangles (chunkRes);

		// Init colors
		colors = new Color[numVerts];

		mesh = CreateChunkMesh();

		// Create GameObject
		//chunk = CreateChunk (verts, normals, uvs, triangles);
		//chunk.name += "Position:"+chunk.transform.position.ToString();

		// Move GameObject
		transform.position = new Vector3 (x * chunkSize - chunkSize/2f, 0f, y * chunkSize - chunkSize/2f);

		gameObject.name = "Chunk ("+x+","+y+") Position:"+transform.position.ToString();

		// Register all vertices with vertex map
		// Move vertices, generate normals/colors
		for (int i=0; i<numVerts; i++) {

			// Init normal/color
			normals [i] = Vector3.up;
			colors[i] = new Color (1f, 1f, 1f, 0.5f);

			// Get VMap coords
			IntVector2 coord = IntToV2 (i);
			coords[i] = coord;

			// Get corresponding vertex
			Vertex vert = vmap.VertexAt(coord.x, coord.y);

			// Get height from vertex
			if (vert != null) UpdateVertex (i, vert.height);
			else {
				Vertex v = vmap.AddVertex(coord);
				v.SetHeight(0f);
				UpdateVertex (i, 0f);
			}

			// Register vertex
			//vmap.RegisterChunkVertex (coord, new IntVector2(x, y), i);
		}

		needsColliderUpdate = true;

		decorations = new List<GameObject>();
	}

	public void Reuse (int x, int y) {
		this.x = x;
		this.y = y;

		transform.position = new Vector3 (x * chunkSize - chunkSize/2f, 0f, y * chunkSize - chunkSize/2f);

		gameObject.name = "Chunk ("+x+","+y+") Position:"+transform.position.ToString();

		// Register all vertices with vertex map
		// Move vertices, generate normals/colors
		for (int i=0; i<numVerts; i++) {

			// Init normal/color
			normals [i] = Vector3.up;
			colors[i] = new Color (1f, 1f, 1f, 0.5f);

			// Get VMap coords
			IntVector2 coord = IntToV2 (i);
			coords[i] = coord;

			// Get corresponding vertex
			Vertex vert = vmap.VertexAt(coord.x, coord.y);

			// Get height from vertex
			if (vert != null) UpdateVertex (i, vert.height);
			else {
				Vertex v = vmap.AddVertex(coord);
				v.SetHeight(0f);
				UpdateVertex (i, 0f);
			}

			// Register vertex
			//vmap.RegisterChunkVertex (coord, new IntVector2(x, y), i);
		}

		needsColliderUpdate = true;

		decorations.Clear();
	}

	/// <summary>
	/// Creates a uniform vertex array.
	/// </summary>
	/// <returns>The uniform vertex array.</returns>
	/// <param name="vertexSize">Vertex size.</param>
	Vector3[] CreateUniformVertexArray (int vertexSize) { 
		float chunkSize = WorldManager.instance.chunkSize;
		int chunkRes = WorldManager.instance.chunkResolution;
		float scale = chunkSize / (chunkRes-1);
		int numVertices = vertexSize * vertexSize;
		Vector3[] uniformArray = new Vector3 [numVertices];
		for (int vert = 0; vert < numVertices; vert++) {
			uniformArray [vert] = new Vector3 (vert % vertexSize, 0, vert / vertexSize); //create vertex
			uniformArray [vert] = uniformArray[vert] * scale; //scale vector appropriately
		}
		return uniformArray;
	}
		
	//takes the number of vertices per side, returns a uniform array of UV coords for a square vertex array
	Vector2[] CreateUniformUVArray(int vertexSize) {
		int numVertices = vertexSize * vertexSize;
		Vector2[] uniformUVArray = new Vector2[numVertices];
		for (int vert = 0; vert < numVertices; vert++) {
			int x = vert % vertexSize; //get x position of vert
			int y = vert / vertexSize; //get y position of vert
			float u = x / (float)(vertexSize - 1); // normalize x into u between 0 and 1
			float v = ((vertexSize - 1) - y) / (float)(vertexSize - 1); //normalize y into v between 0 and 1 and flip direction
			uniformUVArray[vert] = new Vector2(u , v);
		}
		return uniformUVArray;
	}

	//takes the number of vertices per side, returns indices (each group of three defines a triangle) into a square vertex array to form mesh 
	int[] CreateSquareArrayTriangles (int vertexSize){ 
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
				triangleArray [i] = vert + vertexSize; //vertex 1
				triangleArray [i + 1] = vert + 1; //vertex 2
				triangleArray [i + 2] = vert; //vertex 3
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
				triangleArray [i] = vert + vertexSize - 1; //vertex 1
				triangleArray [i + 1] = vert + vertexSize; //vertex 2
				triangleArray [i + 2] = vert; //vertex 3
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
	//GameObject CreateChunk (Vector3[] vertices, Vector3[] normals, Vector2[] UVcoords, int[] triangles) {
	Mesh CreateChunkMesh() {

		// Create mesh
		Mesh chunkMesh = new Mesh();
		chunkMesh.MarkDynamic ();
		chunkMesh.vertices = verts;
		chunkMesh.normals = normals;
		chunkMesh.uv = uvs;
		chunkMesh.triangles = triangles;
		chunkMesh.colors = colors;

		// Assign mesh
		GetComponent<MeshFilter>().mesh = chunkMesh;

		// Assign material
		MeshRenderer renderer = GetComponent<MeshRenderer> ();
		renderer.material = WorldManager.instance.terrainMaterial;
		renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
		renderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

		// Assign collision mesh
		MeshCollider collider = GetComponent<MeshCollider>();
		collider.sharedMesh = mesh;
		collider.convex = false;

		// Init rigidbody
		Rigidbody rigidbody = GetComponent<Rigidbody>();
		rigidbody.freezeRotation = true;
		rigidbody.isKinematic = true;
		rigidbody.useGravity = false;
		rigidbody.constraints = RigidbodyConstraints.FreezeAll;

		// Add grass system
		grassEmitter = GameObject.Instantiate (WorldManager.instance.grassEmitterTemplate);
		grassEmitter.transform.parent = transform;
		grassEmitter.transform.position += new Vector3 (-chunkSize/2f, 0f, -chunkSize/2f);

		// Randomize grass density
		ParticleSystem sys = grassEmitter.GetComponent<ParticleSystem>();
		sys.maxParticles = UnityEngine.Random.Range(0,WorldManager.instance.grassPerChunk);
		sys.playOnAwake = true;

		// Assign particle system emission shape
		ParticleSystem.ShapeModule shape = sys.shape;
		shape.mesh = mesh;
	
		// Assign particle system emission rate
		ParticleSystem.EmissionModule emit = sys.emission;
		emit.rate = new ParticleSystem.MinMaxCurve(WorldManager.instance.decorationsPerStep);

		return chunkMesh;
	}

	/// <summary>
	/// Updates the physics collider.
	/// </summary>
	public void UpdateCollider () {
		needsColliderUpdate = false;

		//Debug.Log("UpdateCollider", gameObject);

		// Clear current grass
		grassEmitter.GetComponent<ParticleSystem>().Clear();

		// Reassign mesh vertices/normals
		mesh.vertices = verts;
		mesh.normals = normals;

		// Recalculate bounding box
		mesh.RecalculateBounds();

		// Reassign collider mesh
		GetComponent<MeshCollider> ().sharedMesh = mesh;

		// Replace decorations
		ReplaceDecorations();

		// Replace grass
		grassEmitter.GetComponent<ParticleSystem>().Play();

	}

	public void UpdateColors () {
		mesh.colors = colors;
		needsColorUpdate = false;
	}

	/// <summary>
	/// Updates a vertex.
	/// </summary>
	/// <param name="index">Index.</param>
	/// <param name="height">Height.</param>
	/// <param name="normal">Normal.</param>
	public void UpdateVertex (int index, float height) {
		//if (UnityEngine.Random.Range(0,100) == 1) Debug.Log("update "+index+" "+height, gameObject);
		try {
			// Check if height update is needed
			if (verts[index].y != height) {
				priority++;
				needsColliderUpdate = true;
				verts[index].y = height;
			}
		} catch (IndexOutOfRangeException e) {
			Debug.LogError ("Chunk.UpdateVertex(): invalid index "+index+"! " + e.Message);
			return;
		}
	}

	/// <summary>
	/// Updates the color.
	/// </summary>
	/// <param name="index">Index.</param>
	/// <param name="blendValue">Blend value.</param>
	public void UpdateColor (int index, float blendValue) {

		// Check if color update is needed
		if (colors[index].a != blendValue) {
			colors[index].a = blendValue;
			needsColorUpdate = true;
		}
	}

	private bool CheckDist (float dist, float updateDist, float margin) {
		return ((dist < (updateDist + margin)) && (dist > (updateDist - margin)));
	}

	private IEnumerator UpdateVerts() {
		
		isUpdatingVerts = true;
		float margin = WorldManager.instance.chunkSize / 2;
		VertexMap vmap = DynamicTerrain.instance.vertexmap;
		float startTime = Time.realtimeSinceStartup;
		Vector3 playerPos = PlayerMovement.instance.transform.position;
		Vector3 chunkPos = transform.position;

		int v = 0;
		for (; v < numVerts; v++) {
	
			// Get VMap coordinates
			IntVector2 coord = coords[v];

			// Get coresponding vertex
			Vertex vert = vmap.VertexAt(coord.x,coord.y);

			// Update vertex height
			UpdateVertex (v, vert.height);

			// If vertex is not locked and there is frequency data to use
			if (!vert.locked && DynamicTerrain.instance.freqData != null) { 

				// Distance between player and vertex
				Vector3 vertPos = chunkPos + verts [v];
				float distance = Vector3.Distance (vertPos, playerPos);

				// If vertex is close enough
				if (CheckDist (distance, WorldManager.instance.vertexUpdateDistance, margin)) {

					// Calculate new height
					Vector3 angleVector = vertPos - playerPos;
					float angle = Vector3.Angle (Vector3.right, angleVector);
					float linIntInput = angle / 360f;
					float newY = DynamicTerrain.instance.freqData.GetDataPoint (linIntInput) *
					              WorldManager.instance.heightScale;

					// If new height, set it
					if (newY != vmap.VertexAt(coord).height) vmap.SetHeight (coord, newY);
				}
			}

			if (v == numVerts-1) {
				isUpdatingVerts = false;
				yield break;
			} else if (Time.realtimeSinceStartup - startTime > 1f / Application.targetFrameRate) {
				yield return null;
				startTime = Time.realtimeSinceStartup;
			}

		}


	}

	public void StopUpdatingVerts () {
		StopCoroutine("UpdateVerts");
	}

	public void ChunkUpdate (){
		if (!hasCheckedForRoad) {
			CheckForRoad(PlayerMovement.instance.moving ? PlayerMovement.instance.progress : 0f);
		}
		//float chunkSize = WorldManager.instance.chunkSize;
		//Vector3 centerOfChunk = chunk.transform.position + new Vector3 (chunkSize / 2, 0f, chunkSize / 2);
//		float distance = Vector3.Distance (PlayerMovement.instance.transform.position, centerOfChunk);
		//roadNearby = NearbyRoad ();
		//if (CheckDist(distance, updateDist, chunkSize) && !isUpdatingVerts) {
			//UpdateVerts (updateDist, freqData);
		if (!isUpdatingVerts) StartCoroutine("UpdateVerts");
		//}
	}
		
	public bool Constrained (Vector3 vertex) {
		if (!nearRoad) {
			return false;
		}
		// check if vertex is within distance to road
		float resolution = 4f;
		Bezier road = WorldManager.instance.road.GetComponent<Bezier> ();
		float progress = PlayerMovement.instance.progress;
		float diff = 1f - progress;
		while (progress <= 1f) {
			Vector3 sample = road.GetPoint (progress);
			Vector3 chunk = vertex - new Vector3 (0f, vertex.y, 0f);
			if (Mathf.Abs (Vector3.Distance (sample, chunk)) < 50f) {
				return true;
			}
			progress += diff / resolution;
		}
		return false;
	}

	public void CheckForRoad (float startProgress) {
		hasCheckedForRoad = true;
		Road road = WorldManager.instance.road;
		Vector3 chunkPos = transform.position;
		float chunkSize = WorldManager.instance.chunkSize;
		float checkResolution = (1f - startProgress) * WorldManager.instance.roadPathCheckResolution;

		// Set boundaries for "near road" consideration
		Vector2 nearMin = new Vector2 (chunkPos.x - chunkSize, chunkPos.z - chunkSize);
		Vector2 nearMax = new Vector2 (chunkPos.x + chunkSize * 2f, chunkPos.z + chunkSize * 2f);

		// Set boundaries for "has road" consideration
		Vector2 hasMin = new Vector2 (chunkPos.x, chunkPos.z);
		Vector2 hasMax = new Vector2 (chunkPos.x + chunkSize, chunkPos.z + chunkSize);

		float progress = startProgress;
		while (progress <= 1f) {

			// Sample road and check distance to chunk
			Vector3 sample = road.GetPoint(progress);
			if (sample.x >= nearMin.x && sample.x <= nearMax.x &&
				sample.z >= nearMin.y && sample.z <= nearMax.y) {
				if (!nearRoad) gameObject.name += "|nearRoad";
				nearRoad = true;

				// If near road, check if has road
				if (sample.x >= hasMin.x && sample.x <= hasMax.x &&
					sample.z >= hasMin.y && sample.z <= hasMax.y) {
					hasRoad = true;
					gameObject.name += "|hasRoad";
					return;
				}
			}
			progress += 1f / checkResolution;
		}

		// Update registries
		if (nearRoad) terrain.RegisterChunk(this);
	}

	public static IntVector2 ToNearestVMapCoords (float x, float y) {
		float chunkSize = WorldManager.instance.chunkSize;
		int chunkRes = WorldManager.instance.chunkResolution;

		return new IntVector2 (
			Mathf.RoundToInt((x-chunkSize/2f) * (float)chunkRes / chunkSize),
			Mathf.RoundToInt((y-chunkSize/2f) * (float)chunkRes / chunkSize)
		);
	}

	IntVector2 IntToV2 (int i) {
		int chunkRes = WorldManager.instance.chunkResolution;

		int xi = x * (chunkRes-1) + i % chunkRes;
		int yi = y * (chunkRes-1) + i / chunkRes;
		return new IntVector2 (xi, yi);
	}

	/// <summary>
	/// Resets height of all decorations on a chunk
	/// </summary>
	public void ReplaceDecorations () {
		foreach (Transform tr in GetComponentsInChildren<Transform>()) {

			// Skip chunk itself
			if (tr == transform || tr == grassEmitter.transform) continue;

			// Raycast down
			RaycastHit hit;
			Vector3 rayOrigin = new Vector3 (tr.position.x, WorldManager.instance.heightScale, tr.position.z);
			if (Physics.Raycast(rayOrigin, Vector3.down,out hit, Mathf.Infinity))
				tr.position = new Vector3 (tr.position.x, hit.point.y, tr.position.z);
		}
	}

	/// <summary>
	/// Removes and pools all decorations on the chunk.
	/// </summary>
	public void RemoveDecorations () {
		foreach (GameObject decoration in decorations)
			WorldManager.instance.RemoveDecoration(decoration);
	}

	#endregion

}