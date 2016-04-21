using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VertexMap {
	public Dictionary<int, Dictionary<int, Vertex>> vertices; 

	const float NEARBY_ROAD_DISTANCE = 50f; // max dist from a road for a vert to be considered nearby a road

	public VertexMap () {
		vertices = new Dictionary<int, Dictionary<int, Vertex>>();
	}

	//
	// Functions to check if the vertex map contains a vertex
	//
	public bool ContainsVertex (IntVector2 i) {
		return ContainsVertex (i.x, i.y);
	}

	public bool ContainsVertex (int x, int y) {
		if (vertices == null) return false;
		if (!vertices.ContainsKey(x)) return false;
		if (!vertices[x].ContainsKey(y)) return false;
		return true;
	}

	//
	// Check the height of a vertex
	//
	public float GetHeight (IntVector2 i) {
		return GetHeight (i.x, i.y);
	}

	public float GetHeight (int x, int y) {
		if (!ContainsVertex (x, y)) return float.NaN;
		return vertices[x][y].height;
	}

	//
	// Set the height of a vertex
	//
	public void SetHeight (IntVector2 i, float h) {
		SetHeight (i.x, i.y, h);
	}

	public void SetHeight (int x, int y, float h) {
		vertices[x][y].SetHeight (h);
	}

	//
	//
	//
	public Vector3 GetNormal (IntVector2 i) {
		return GetNormal (i.x, i.y);
	}

	public Vector3 GetNormal (int x, int y) {
		return vertices[x][y].normal;
	}

	//
	// Lock a vertex
	//
	public void Lock (IntVector2 i) {
		Lock (i.x, i.y);
	}

	public void Lock (int x, int y) {
		vertices[x][y].locked = true;
	}
		
	//
	// Functions to check if a vertex is locked
	//
	public bool IsLocked (IntVector2 i) {
		return IsLocked (i.x, i.y);
	}

	public bool IsLocked (int x, int y) {
		return (ContainsVertex(x,y) ? vertices[x][y].locked : true);
	}

	//
	// Functions to check if a vertex is constrained (too close to road)
	//
	public bool IsConstrained (IntVector2 i) {
		return IsConstrained (i.x, i.y);
	}

	public bool IsConstrained (int x, int y) {
		return vertices[x][y].distToRoad < NEARBY_ROAD_DISTANCE;
	}

	//
	// Add a chunk vertex <-> Vertex relationship
	//
	public void RegisterChunkVertex (IntVector2 i, Mesh chunkMesh, int vertIndex) {
		RegisterChunkVertex (i.x, i.y, chunkMesh, vertIndex);
	}

	public void RegisterChunkVertex (int x, int y, Mesh chunkMesh, int vertIndex) {
		if (!ContainsVertex (x, y)) AddVertex (x, y);

		vertices[x][y].chunkVertices.Add(new KeyValuePair<Mesh, int> (chunkMesh, vertIndex));
	}

	//

	void AddVertex (int x, int y) {
		AddVertex (new Vertex (x, y));
	}

	void AddVertex (Vertex vert) {
		if (!vertices.ContainsKey(vert.x))
			vertices.Add (vert.x, new Dictionary <int, Vertex>() );

		if (!vertices[vert.x].ContainsKey(vert.y))
			vertices[vert.x].Add (vert.y, vert);
		else
			Debug.LogError ("VertexMap.Add(): attempted to add a vertex to a filled position!");
	}
			
}

public class Vertex {
	public List<KeyValuePair<Mesh, int>> chunkVertices;
	public bool locked = false;
	public int x;
	public int y;
	public float height = 0f;
	public float distToRoad = Mathf.Infinity;
	public Vector3 normal = Vector3.up;

	public Vertex (int x, int y) {
		this.x = x;
		this.y = y;
		chunkVertices = new List<KeyValuePair<Mesh, int>>();
	}

	public void SetHeight (float h) {
		bool debug = Random.Range (0, 100) == 0;
		height = h;

		//if (debug) Debug.Log("SetHeight");
		foreach (KeyValuePair<Mesh, int> chunkVert in chunkVertices) {
			//if (debug) Debug.Log(chunkVert.Key);
			Vector3[] verts = new Vector3[chunkVert.Key.vertices.Length];
			for (int i=0; i<verts.Length; i++) {
				verts[i] = chunkVert.Key.vertices[i];
				if (i == chunkVert.Value) verts[i].y = height;
			}
			chunkVert.Key.vertices = verts;
		}
	}
		
}
