using UnityEngine;
using System.Collections;

/*
 * A chunk is a square mesh that's part of the larger terrain object.
 * Chunks dynamically load and unload as the player gets closer and farther.
 * This un/loading will be taken care of by the DynamicTerrain class. 
 */

public class Chunk{
	private float CHUNK_SIZE; //size of side of chunk square
	private int CHUNK_RESOLUTION; //number of vertices per side of chunk
	private int x; //x position in chunk grid
	private int y; //y position in chunk grid

	public GameObject chunk;
	private Vector3[] dMap; //displacement map, displaces each vertex by a Vector3
	private Material terrainMaterial;

	public Chunk (int x, int y, float chunkSize, int chunkResolution, Material terrMat) {
		this.x = x;
		this.y = y;
		CHUNK_SIZE = chunkSize;
		CHUNK_RESOLUTION = chunkResolution;
		terrainMaterial = terrMat;

		Vector3[] verts = createUniformVertexArray (CHUNK_RESOLUTION);
		Vector2[] uvs = createUniformUVArray (CHUNK_RESOLUTION);
		int[] triangles = createSquareArrayTriangles (CHUNK_RESOLUTION);
		chunk = createChunk (verts, uvs, triangles);
		chunk.GetComponent<MeshRenderer> ().material = terrainMaterial;
		chunk.transform.position += new Vector3 (x * CHUNK_SIZE, 0f, y * CHUNK_SIZE);
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

	void setDMap (Vector3[] newDMap){
		for (int v = 0; v < dMap.Length; v++) {
			chunk.GetComponent<Mesh> ().vertices [v] = chunk.GetComponent<Mesh> ().vertices [v] - dMap [v];
			chunk.GetComponent<Mesh> ().vertices [v] = chunk.GetComponent<Mesh> ().vertices [v] + newDMap [v];
		}
		dMap = newDMap;
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
		GameObject chunk = new GameObject ("chunk", typeof(MeshFilter), typeof(MeshRenderer));
		chunk.transform.position = new Vector3 (-CHUNK_SIZE/2, 0, -CHUNK_SIZE/2);

		//mesh filter stuff
		Mesh mesh = new Mesh ();
		mesh.vertices = vertices;
		mesh.uv = UVcoords;
		mesh.triangles = triangles;
		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();
		chunk.GetComponent<MeshFilter> ().mesh = mesh;

		//mesh renderer stuff
		chunk.GetComponent<MeshRenderer> ().material = terrainMaterial;

		return chunk;
	}

}

