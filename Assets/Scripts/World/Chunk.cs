using UnityEngine;
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
	public int x; //x position in chunk grid
	public int y; //y position in chunk grid

	bool hasCheckedForRoad = false;
	public bool hasRoad = false;  // Chunk has road on it
	public bool nearRoad = false; // Chunk is within one chunk distance of a road

	Vector3[] verts;
	Vector2[] uvs;
	int[] triangles;

	#endregion
	#region Chunk Methods

	public Chunk (int x, int y) {
		this.x = x;
		this.y = y;

		// Create chunk mesh
		int chunkRes = WorldManager.instance.chunkResolution;
		float chunkSize = WorldManager.instance.chunkSize;
		verts = CreateUniformVertexArray (chunkRes);
		uvs = CreateUniformUVArray (chunkRes);
		triangles = CreateSquareArrayTriangles (chunkRes);
		chunk = CreateChunk (verts, uvs, triangles);
		chunk.transform.position += new Vector3 (x * chunkSize, 0f, y * chunkSize);
		chunk.name += "Position:"+chunk.transform.position.ToString();

		//CheckForRoad (PlayerMovement.instance.moving ? PlayerMovement.instance.progress : 0f);

		// Register all vertices with vertex map
		for (int i=0; i<verts.Length; i++) {
			IntVector2 coords = IntToV2 (i);
			VertexMap vmap = DynamicTerrain.instance.vertexmap;

			vmap.RegisterChunkVertex (coords, this, i);
			verts[i].y = vmap.GetHeight(coords);
		}
	}

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

	//create terrain gameobject with mesh
	GameObject CreateChunk (Vector3[] vertices, Vector2[] UVcoords, int[] triangles) {
		float chunkSize = WorldManager.instance.chunkSize;

		GameObject chunk = new GameObject ("Chunk ("+x+","+y+")", 
			typeof(MeshFilter), 
			typeof(MeshRenderer),
			typeof(MeshCollider),
			typeof(Rigidbody)
		);
		chunk.transform.position = new Vector3 (-chunkSize/2, 0, -chunkSize/2);

		//mesh filter stuff
		Mesh mesh = new Mesh ();
		mesh.vertices = vertices;
		mesh.uv = UVcoords;
		mesh.triangles = triangles;
		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();
		mesh.MarkDynamic ();
		chunk.GetComponent<MeshFilter> ().mesh = mesh;
		chunk.GetComponent<MeshCollider>().sharedMesh = mesh;

		//mesh renderer stuff
		chunk.GetComponent<MeshRenderer> ().material = WorldManager.instance.terrainMaterial;
		chunk.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
		chunk.GetComponent<MeshRenderer>().reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

		chunk.GetComponent<MeshCollider>().convex = false;

		chunk.GetComponent<Rigidbody>().freezeRotation = true;
		chunk.GetComponent<Rigidbody>().isKinematic = true;
		chunk.GetComponent<Rigidbody>().useGravity = false;
		chunk.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

		return chunk;
	}

	public void UpdateCollider () {
		chunk.GetComponent<MeshFilter>().mesh.RecalculateBounds();
		chunk.GetComponent<MeshCollider>().sharedMesh = chunk.GetComponent<MeshFilter>().mesh;
	}

	public void UpdateVertex (int index, float height) {
		Vector3[] vertices = chunk.GetComponent<MeshFilter> ().mesh.vertices;
		//vertices[index].y += (height - vertices[index].y)/4f;
		vertices[index].y = height;
		chunk.GetComponent<MeshFilter>().mesh.vertices = vertices;

	}



	private bool CheckDist (float dist, float updateDist, float margin) {
		return ((dist < (updateDist + margin)) && (dist > (updateDist - margin)));
	}

	private void UpdateVerts(float updateDist, LinInt freqData) {
		float margin = WorldManager.instance.chunkSize / 2;
		Vector3[] vertices = chunk.GetComponent<MeshFilter> ().mesh.vertices;
		VertexMap vmap = DynamicTerrain.instance.vertexmap;
		bool changesMade = false;
		for (int v = 0; v < vertices.Length; v++) {
			
			IntVector2 coords = IntToV2 (v);

			//if (!vmap.IsLocked (coords)) { //if vert is frozen
				if (!vmap.IsConstrained(coords)) { 
				Vector3 playerPos = PlayerMovement.instance.transform.position;

					Vector3 vertPos = chunk.transform.position + vertices [v];
					float distance = Vector3.Distance (vertPos, playerPos);
					if (CheckDist (distance, updateDist, margin)) {
						Vector3 angleVector = vertPos - playerPos;
						float angle = Vector3.Angle (Vector3.right, angleVector);
						float linIntInput = angle / 360f;
						float newY = freqData.GetDataPoint (linIntInput) * 
						WorldManager.instance.heightScale;
						if (newY != vertices[v].y) {
							changesMade = true;
							vmap.SetHeight (coords, newY);
						}
					}
				}

				if (changesMade) {
					ReplaceDecorations();
				}
			//}
		}

	}

	public void Update (float updateDist, LinInt freqData){
		if (!hasCheckedForRoad) {
			CheckForRoad(PlayerMovement.instance.moving ? PlayerMovement.instance.progress : 0f);
		}
		float chunkSize = WorldManager.instance.chunkSize;
		Vector3 centerOfChunk = chunk.transform.position + new Vector3 (chunkSize / 2, 0f, chunkSize / 2);
		float distance = Vector3.Distance (PlayerMovement.instance.transform.position, centerOfChunk);
		//roadNearby = NearbyRoad ();
		if (CheckDist(distance, updateDist, chunkSize)) {
			UpdateVerts (updateDist, freqData);
		}
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
		float distance = Mathf.Infinity;
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
			Mathf.RoundToInt((x-chunkSize/2f) / chunkSize),
			Mathf.RoundToInt((y-chunkSize/2f) / chunkSize)
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
		chunk.GetComponent<MeshCollider>().sharedMesh = chunk.GetComponent<MeshFilter>().mesh;
		foreach (Transform tr in chunk.GetComponentsInChildren<Transform>()) {
			if (tr != chunk.transform) {
				RaycastHit hit;
				if (Physics.Raycast(new Vector3 (tr.position.x, WorldManager.instance.heightScale, tr.position.y), Vector3.down,out hit, Mathf.Infinity)) {
					tr.position = new Vector3 (tr.position.x, hit.point.y, tr.position.z);
				}
			}
		}
	}

	#endregion

}