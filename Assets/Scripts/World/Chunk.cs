﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * A chunk is a square mesh that's part of the larger terrain object.
 * Chunks dynamically load and unload as the player gets closer and farther.
 * This un/loading will be taken care of by the DynamicTerrain class. 
 */

public class Chunk {

	#region Chunk Vars

	public GameObject chunk; // Chunk object/mesh
	public GameObject grassEmitter;
	public int x; //x position in chunk grid
	public int y; //y position in chunk grid
	public float priority = 0f;

	bool hasCheckedForRoad = false;
	public bool hasRoad = false;  // Chunk has road on it
	public bool nearRoad = false; // Chunk is within one chunk distance of a road

	Mesh colliderMesh;
	public Mesh mesh;
	Vector3[] verts;
	//bool[] constrained;
	int numVerts;
	Vector3[] normals;
	Vector2[] uvs;
	int[] triangles;
	Color[] colors;

	bool isUpdatingVerts = false;
	public bool needsColliderUpdate = false;

	#endregion
	#region Chunk Methods

	public Chunk (int x, int y) {
		this.x = x;
		this.y = y;

		VertexMap vmap = DynamicTerrain.instance.vertexmap;
		int chunkRes = WorldManager.instance.chunkResolution;
		float chunkSize = WorldManager.instance.chunkSize;

		// Generate vertices
		verts = CreateUniformVertexArray (chunkRes);
		numVerts = verts.Length;

		// Init normals
		normals = new Vector3[numVerts];

		// Generate UVs
		uvs = CreateUniformUVArray (chunkRes);

		// Generate triangles
		triangles = CreateSquareArrayTriangles (chunkRes);

		// Init colors
		colors = new Color[numVerts];

		// Create GameObject
		chunk = CreateChunk (verts, normals, uvs, triangles);
		chunk.name += "Position:"+chunk.transform.position.ToString();

		// Move GameObject
		chunk.transform.position += new Vector3 (x * chunkSize, 0f, y * chunkSize);

		// Register all vertices with vertex map
		// Move vertices, generate normals/colors
		for (int i=0; i<numVerts; i++) {

			// Init normal/color
			normals [i] = Vector3.up;
			colors[i] = new Color (0f, 0f, 0f, 0.5f);

			// Get VMap coords
			IntVector2 coords = IntToV2 (i);

			// Get corresponding vertex
			Vertex vert = vmap.VertexAt(coords.x, coords.y);

			// Get height from vertex
			if (vert != null) UpdateVertex (i, vert.height);

			// Register vertex
			vmap.RegisterChunkVertex (coords, this, i);
		}
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
	GameObject CreateChunk (Vector3[] vertices, Vector3[] normals, Vector2[] UVcoords, int[] triangles) {
		float chunkSize = WorldManager.instance.chunkSize;

		// Create GameObject
		GameObject chunk = new GameObject ("Chunk ("+x+","+y+")", 
			typeof(MeshFilter), 
			typeof(MeshRenderer),
			typeof(MeshCollider),
			typeof(Rigidbody)
		);

		// Move GameObject
		chunk.transform.position = new Vector3 (-chunkSize/2, 0, -chunkSize/2);

		// Create mesh
		mesh = new Mesh();
		mesh.MarkDynamic ();
		mesh.vertices = vertices;
		mesh.normals = normals;
		mesh.uv = UVcoords;
		mesh.triangles = triangles;
		mesh.colors = colors;

		// Assign mesh
		chunk.GetComponent<MeshFilter>().mesh = mesh;

		// Assign material
		MeshRenderer renderer = chunk.GetComponent<MeshRenderer> ();
		renderer.material = WorldManager.instance.terrainMaterial;
		renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
		renderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

		// Assign collision mesh
		MeshCollider collider = chunk.GetComponent<MeshCollider>();
		collider.sharedMesh = mesh;
		collider.convex = false;

		// Init rigidbody
		Rigidbody rigidbody = chunk.GetComponent<Rigidbody>();
		rigidbody.freezeRotation = true;
		rigidbody.isKinematic = true;
		rigidbody.useGravity = false;
		rigidbody.constraints = RigidbodyConstraints.FreezeAll;

		// Add grass system
		grassEmitter = GameObject.Instantiate (WorldManager.instance.grassEmitterTemplate);
		grassEmitter.transform.parent = chunk.transform;
		grassEmitter.transform.position += new Vector3 (-chunkSize/2f, 0f, -chunkSize/2f);

		// Randomize grass density
		ParticleSystem sys = grassEmitter.GetComponent<ParticleSystem>();
		sys.maxParticles = Random.Range(0,WorldManager.instance.grassPerChunk);
		sys.playOnAwake = true;

		// Assign particle system emission shape
		ParticleSystem.ShapeModule shape = sys.shape;
		shape.mesh = mesh;
	
		// Assign particle system emission rate
		ParticleSystem.EmissionModule emit = sys.emission;
		emit.rate = new ParticleSystem.MinMaxCurve(WorldManager.instance.decorationsPerStep);

		return chunk;
	}

	/// <summary>
	/// Updates the physics collider.
	/// </summary>
	public void UpdateCollider () {
		needsColliderUpdate = false;

		// Clear current grass
		grassEmitter.GetComponent<ParticleSystem>().Clear();

		// Reassign mesh vertices/normals
		mesh.vertices = verts;
		mesh.normals = normals;

		// Recalculate bounding box
		mesh.RecalculateBounds();

		// Reassign collider mesh
		chunk.GetComponent<MeshCollider> ().sharedMesh = mesh;

		// Replace decorations
		ReplaceDecorations();

		// Replace grass
		grassEmitter.GetComponent<ParticleSystem>().Play();

	}

	/// <summary>
	/// Updates a vertex.
	/// </summary>
	/// <param name="index">Index.</param>
	/// <param name="height">Height.</param>
	/// <param name="normal">Normal.</param>
	public void UpdateVertex (int index, float height) {

		// Check if height update is needed
		if (verts[index].y != height) {
			needsColliderUpdate = true;
			verts[index].y = height;
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
			mesh.colors = colors;
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

		int v = 0;
		for (; v < numVerts; v++) {
	
			// Get VMap coordinates
			IntVector2 coords = IntToV2 (v);

			// Get cooresponding vertex
			Vertex vert = vmap.VertexAt(coords.x,coords.y);

			// Update vertex height
			UpdateVertex (v, vert.height);

			// If vertex is not locked and there is frequency data to use
			if (!vmap.VertexAt (coords).locked && DynamicTerrain.instance.freqData != null) { 

				// Distance between player and vertex
				Vector3 playerPos = PlayerMovement.instance.transform.position;
				Vector3 vertPos = chunk.transform.position + verts [v];
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
					if (newY != verts [v].y) vmap.SetHeight (coords, newY);
				}
			}

			if (v == numVerts) {
				isUpdatingVerts = false;
				yield break;
			} else if (Time.realtimeSinceStartup - startTime > 1f / Application.targetFrameRate) {
				yield return null;
				startTime = Time.realtimeSinceStartup;
			}

		}


	}

	public void Update (){
		//if (needsColliderUpdate) UpdateCollider();
		if (!hasCheckedForRoad) {
			CheckForRoad(PlayerMovement.instance.moving ? PlayerMovement.instance.progress : 0f);
		}
		//float chunkSize = WorldManager.instance.chunkSize;
		//Vector3 centerOfChunk = chunk.transform.position + new Vector3 (chunkSize / 2, 0f, chunkSize / 2);
//		float distance = Vector3.Distance (PlayerMovement.instance.transform.position, centerOfChunk);
		//roadNearby = NearbyRoad ();
		//if (CheckDist(distance, updateDist, chunkSize) && !isUpdatingVerts) {
			//UpdateVerts (updateDist, freqData);
		if (!isUpdatingVerts) WorldManager.instance.StartCoroutine(UpdateVerts());
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
		Vector3 chunkPos = chunk.transform.position;
		float chunkSize = WorldManager.instance.chunkSize;
		float checkResolution = (1f - startProgress) * WorldManager.instance.roadPathCheckResolution;

		Vector2 nearMin = new Vector2 (
			chunkPos.x - chunkSize,
			chunkPos.z - chunkSize
		);

		Vector2 nearMax = new Vector2 (
			chunkPos.x + chunkSize * 2f,
			chunkPos.z + chunkSize * 2f
		);

		Vector2 hasMin = new Vector2 (
			chunkPos.x,
			chunkPos.z
		);

		Vector2 hasMax = new Vector2 (
			chunkPos.x + chunkSize,
			chunkPos.z + chunkSize
		);

		Road road = WorldManager.instance.road;

		float progress = startProgress;
		while (progress <= 1f) {
			
			Vector3 sample = road.GetPoint(progress);
			if (sample.x >= nearMin.x && sample.x <= nearMax.x &&
				sample.z >= nearMin.y && sample.z <= nearMax.y) {
				if (!nearRoad) chunk.name += "|nearRoad";
				nearRoad = true;
				if (sample.x >= hasMin.x && sample.x <= hasMax.x &&
					sample.z >= hasMin.y && sample.z <= hasMax.y) {
					hasRoad = true;
					chunk.name += "|hasRoad";
					return;
				}
			}
			progress += 1f / checkResolution;
		}
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

		int xi = x*chunkRes + i%chunkRes - x;
		int yi = y*chunkRes + i/chunkRes - y;
		return new IntVector2 (xi, yi);
	}

	// Shifts all decorations on a chunk
	public void ReplaceDecorations () {
		//chunk.GetComponent<MeshCollider>().sharedMesh = chunk.GetComponent<MeshFilter>().mesh;
		foreach (Transform tr in chunk.GetComponentsInChildren<Transform>()) {
			if (tr == chunk.transform || tr == grassEmitter.transform) continue;
			if (tr.gameObject.GetComponent<Decoration>().dynamic) continue;
			RaycastHit hit;
			if (Physics.Raycast(new Vector3 (tr.position.x, WorldManager.instance.heightScale, tr.position.z), Vector3.down,out hit, Mathf.Infinity)) {
				tr.position = new Vector3 (tr.position.x, hit.point.y, tr.position.z);
			}
		}
	}

	#endregion

}