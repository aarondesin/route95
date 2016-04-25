using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * A chunk is a square mesh that's part of the larger terrain object.
 * Chunks dynamically load and unload as the player gets closer and farther.
 * This un/loading will be taken care of by the DynamicTerrain class. 
 */

public class Chunk{
	private float HEIGHT_SCALE; //scales the heights of the vertices from the LinInt data
	private float CHUNK_SIZE; //size of side of chunk square
	private int CHUNK_RESOLUTION; //number of vertices per side of chunk
	private int x; //x position in chunk grid
	private int y; //y position in chunk grid

	public GameObject chunk;
	private bool[] vertLocked; //if vertex in mesh.vertices has had its height set
	private bool roadNearby;
	private Vector3[] dMap; //displacement map, displaces each vertex by a Vector3
	private Material terrainMaterial;

	public Chunk (int x, int y, float chunkSize, int chunkResolution, Material terrMat, float heightScale) {
		this.x = x;
		this.y = y;
		CHUNK_SIZE = chunkSize;
		CHUNK_RESOLUTION = chunkResolution;
		terrainMaterial = terrMat;

		Vector3[] verts = createUniformVertexArray (CHUNK_RESOLUTION);
		vertLocked = new bool[verts.Length];
		Vector2[] uvs = createUniformUVArray (CHUNK_RESOLUTION);
		int[] triangles = createSquareArrayTriangles (CHUNK_RESOLUTION);
		chunk = createChunk (verts, uvs, triangles);
		chunk.GetComponent<MeshRenderer> ().material = terrainMaterial;
		chunk.transform.position += new Vector3 (x * CHUNK_SIZE, 0f, y * CHUNK_SIZE);
		roadNearby = nearbyRoad ();

		//int r = Random.Range (0, verts.Length-1);

		for (int i=0; i<verts.Length; i++) {
			
			Vector2 c = IntToV2 (i);
			if (DynamicTerrain.instance.ReadHeightMap((int)c.x, (int)c.y) == float.NaN) {
				DynamicTerrain.instance.WriteHeightMap ((int)c.x, (int)c.y, 0f);
			} else {
				verts[i].y = DynamicTerrain.instance.ReadHeightMap ((int)c.x, (int)c.y);
			}
			//if (i == r) {
				//Debug.Log(""+i+"on chunk "+x+","+y+" maps to "+(int)c.x+","+(int)c.y);
			//}
		}

		//test for random height map
		if (WorldManager.instance.DO_RANDOM_HEIGHT_MAPS) {
			dMap = new Vector3[verts.Length];
			setDMap (createRandomHMap (verts.Length));
		}
		HEIGHT_SCALE = heightScale;
	}

	public int getX () {
		return x;
	}

	public int getY () {
		return y;
	}

	Vector3[] getDMap(){
		return dMap;
	}

	public Vector2 getCoordinate() {
		return new Vector2 (x, y);
	}

	Vector3[] createRandomDMap(int size) {
		Vector3[] dmap = new Vector3[size];
		for (int i = 0; i < size; i++) {
			float scale = 1f;
			float xDisplacement = Random.Range(0f, scale);
			float yDisplacement = Random.Range(0f, scale);
			float zDisplacement = Random.Range(0f, scale);
			dmap[i] = new Vector3 (xDisplacement, yDisplacement, zDisplacement);
		}
		return dmap;
	}

	Vector3[] createRandomHMap(int size) {
		Vector3[] dmap = new Vector3[size];
		for (int i = 0; i < size; i++) {
			float scale = 2f;
			//float yDisplacement = Random.Range(0f, scale);
			float yDisplacement = Mathf.PerlinNoise (x*CHUNK_SIZE + i%CHUNK_RESOLUTION, y*CHUNK_SIZE + i/CHUNK_RESOLUTION);
			yDisplacement = yDisplacement * scale;
			dmap[i] = new Vector3 (0f, yDisplacement, 0f);
		}
		return dmap;
	}

	void setDMap (Vector3[] newDMap){
		if (newDMap != null) {
			Vector3[] newVertices = new Vector3[newDMap.Length];
			for (int v = 0; v < dMap.Length; v++) {
				chunk.GetComponent<MeshFilter> ().mesh.vertices [v] -= dMap [v];
				newVertices[v] = chunk.GetComponent<MeshFilter> ().mesh.vertices [v] + newDMap [v];
			}
			chunk.GetComponent<MeshFilter> ().mesh.vertices = newVertices;
			chunk.GetComponent<MeshFilter> ().mesh.RecalculateNormals ();
			chunk.GetComponent<MeshFilter> ().mesh.RecalculateBounds ();
			dMap = newDMap;
		}
	}

	Vector3[] createUniformVertexArray(int vertexSize){ 
		float scale = CHUNK_SIZE / (CHUNK_RESOLUTION-1);
		int numVertices = vertexSize * vertexSize;
		Vector3[] uniformArray = new Vector3 [numVertices];
		for (int vert = 0; vert < numVertices; vert++) {
			uniformArray [vert] = new Vector3 (vert % vertexSize, 0, vert / vertexSize); //create vertex
			uniformArray [vert] = uniformArray[vert] * scale; //scale vector appropriately
		}
		return uniformArray;
	}

	//takes the number of vertices per side, returns a uniform array of UV coords for a square vertex array
	Vector2[] createUniformUVArray(int vertexSize) {
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
	int[] createSquareArrayTriangles (int vertexSize){ 
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
	GameObject createChunk (Vector3[] vertices, Vector2[] UVcoords, int[] triangles) {
		GameObject chunk = new GameObject ("chunk", 
			typeof(MeshFilter), 
			typeof(MeshRenderer),
			typeof(MeshCollider)
		);
		chunk.transform.position = new Vector3 (-CHUNK_SIZE/2, 0, -CHUNK_SIZE/2);

		//mesh filter stuff
		Mesh mesh = new Mesh ();
		mesh.vertices = vertices;
		mesh.uv = UVcoords;
		mesh.triangles = triangles;
		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();
		chunk.GetComponent<MeshFilter> ().mesh = mesh;
		chunk.GetComponent<MeshCollider>().sharedMesh = mesh;

		//mesh renderer stuff
		chunk.GetComponent<MeshRenderer> ().material = terrainMaterial;
		chunk.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
		chunk.GetComponent<MeshRenderer>().reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

		chunk.GetComponent<MeshCollider>().convex = false;

		return chunk;
	}

	private bool checkDist (float dist, float updateDist, float margin) {
		return ((dist < (updateDist + margin)) && (dist > (updateDist - margin)));
	}

	private void updateVerts(GameObject player, float updateDist, LinInt freqData) {
		float margin = CHUNK_SIZE / 2;
		Vector3[] vertices = chunk.GetComponent<MeshFilter> ().mesh.vertices;
		for (int v = 0; v < vertices.Length; v++) {
			Vector2 c = IntToV2 (v);
			if (!vertLocked[v]) {
				if (!Constrained (chunk.transform.position + vertices [v])) {
					Vector3 vertPos = chunk.transform.position + vertices [v];
					float distance = Vector3.Distance (vertPos, player.transform.position);
					if (checkDist (distance, updateDist, margin)) {
						Vector3 angleVector = vertPos - player.transform.position;
						float angle = Vector3.Angle (Vector3.right, angleVector);
						float linIntInput = angle / 360f;
						float newY = freqData.getDataPoint (linIntInput) * HEIGHT_SCALE;
						DynamicTerrain.instance.WriteHeightMap ((int)c.x, (int)c.y, newY);
						float diff = newY - vertices [v].y;
						vertices [v].y += diff/3f;
						vertLocked [v] = true;
						if (chunk.GetComponent<MeshFilter> ().mesh.normals [v].y < 0f) {
							chunk.GetComponent<MeshFilter> ().mesh.normals [v] *= -1;
						}
						chunk.GetComponent<MeshFilter> ().mesh.RecalculateBounds ();
						chunk.GetComponent<MeshCollider> ().sharedMesh = chunk.GetComponent<MeshFilter> ().mesh;
						//Debug.DrawRay (vertPos+new Vector3 (0f, newY, 0f), chunk.GetComponent<MeshFilter>().mesh.normals[v], Color.green);
					}
				} else {
					vertices[v].y = 0f;
					DynamicTerrain.instance.WriteHeightMap ((int)c.x, (int)c.y, 0f);
					if (chunk.GetComponent<MeshFilter> ().mesh.normals [v].y < 0f) {
						chunk.GetComponent<MeshFilter> ().mesh.normals [v] *= -1;
					}
					vertLocked [v] = true;
				}
			} else if (vertices [v].y != DynamicTerrain.instance.ReadHeightMap ((int)c.x, (int)c.y)) {
				float diff = DynamicTerrain.instance.ReadHeightMap((int)c.x, (int)c.y) - vertices [v].y;
				vertices [v].y += diff/3f;
			}
		}
		chunk.GetComponent<MeshFilter> ().mesh.vertices = vertices;
		chunk.GetComponent<MeshFilter> ().mesh.RecalculateNormals();
		chunk.GetComponent<MeshFilter> ().mesh.RecalculateBounds();
		ReplaceDecorations();
	}
	public void update (GameObject player, float updateDist, LinInt freqData){
		Vector3 centerOfChunk = chunk.transform.position + new Vector3 (CHUNK_SIZE / 2, 0f, CHUNK_SIZE / 2);
		float distance = Vector3.Distance (player.transform.position, centerOfChunk);
		roadNearby = nearbyRoad ();
		if (checkDist(distance, updateDist, CHUNK_SIZE)) {
			updateVerts (player, updateDist, freqData);
		}
	}
	public bool Constrained (Vector3 vertex) {
		if (!roadNearby) {
			return false;
		}
		// check if vertex is within distance to road
		float resolution = 100f;
		Bezier road = WorldManager.instance.road.GetComponent<Bezier> ();
		float progress = 0f;
		while (progress <= 1f) {
			Vector3 sample = road.GetPoint (progress);
			Vector3 chunk = vertex - new Vector3 (0f, vertex.y, 0f);
			if (Mathf.Abs (Vector3.Distance (sample, chunk)) < 50f) {
				return true;
			}
			progress += 1 / resolution;
		}
		return false;
	}

	public bool nearbyRoad () {
		Vector3 pos = chunk.transform.position;
		float xMin = pos.x-CHUNK_SIZE*0.5f;
		float xMax = pos.x + CHUNK_SIZE*1.5f;
		float zMin = pos.z-CHUNK_SIZE*0.5f;
		float zMax = pos.z + CHUNK_SIZE*1.5f;
		float resolution = 4f;
		float diff = 1f - PlayerMovement.instance.progress;
		Bezier road = WorldManager.instance.road.GetComponent<Bezier> ();
		float progress = PlayerMovement.instance.progress;
		while (progress <= 1f) {
			Vector3 sample = road.GetPoint (progress);
			float x = sample.x;
			float z = sample.z;
			if (x >= xMin && x < xMax && z >= zMin && z < zMax) {
				return true;
			}
			progress += diff / resolution;
		}
		return false;
	}

	public bool containsRoad () {
		Vector3 pos = chunk.transform.position;
		float xMin = pos.x;
		float xMax = xMin + CHUNK_SIZE;
		float zMin = pos.z;
		float zMax = zMin + CHUNK_SIZE;
		float resolution = 4f;
		float diff = 1f - PlayerMovement.instance.progress;
		Bezier road = WorldManager.instance.road.GetComponent<Bezier> ();
		float progress = PlayerMovement.instance.progress;
		while (progress <= 1f) {
			Vector3 sample = road.GetPoint (progress);
			float x = sample.x;
			float z = sample.z;
			if (x >= xMin && x < xMax && z >= zMin && z < zMax) {
				return true;
			}
			progress += diff / resolution;
		}
		return false;
	}

	Vector2 IntToV2 (int i) {
		int xi = x*CHUNK_RESOLUTION + i%CHUNK_RESOLUTION;
		int yi = y*CHUNK_RESOLUTION + i/CHUNK_RESOLUTION;
		return new Vector2 (
			xi - x,
			yi - y
		);
	}

	public void ReplaceDecorations () {
		foreach (Transform tr in chunk.GetComponentsInChildren<Transform>()) {
			if (tr != chunk.transform) {
				RaycastHit hit;
				if (Physics.Raycast(new Vector3 (tr.position.x, WorldManager.instance.MAX_DECORATION_HEIGHT, tr.position.y), Vector3.down,out hit, Mathf.Infinity)) {
					//Debug.Log("bap");
					tr.position = new Vector3 (tr.position.x, hit.point.y, tr.position.z);
				}
			}
		}
	}

}