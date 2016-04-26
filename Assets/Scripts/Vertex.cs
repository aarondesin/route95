using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IntVector2 {
	public int x;
	public int y;

	public bool IsCorner () {
		return 
			(x == 0 || x == WorldManager.instance.CHUNK_RESOLUTION-1) &&
			(y == 0 || y == WorldManager.instance.CHUNK_RESOLUTION-1);
	}

	public override string ToString () {
		return "("+x+","+y+")";
	}
}

public class VertexMap {
	public Dictionary<int, Dictionary<int, Vertex>> vertices; 

	const float NEARBY_ROAD_DISTANCE = 125f; // max dist from a road for a vert to be considered nearby a road

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

	//to debug
	public bool ContainsVertex (int x, int y, bool debug) {
		Debug.Log("Do I contain (" + x + ", " + y + ")");
		if (vertices == null) {
			if (debug) {
				Debug.Log ("No vertices list.");
			}
			return false;
		}
		if (!vertices.ContainsKey (x)) {
			if (debug) {
				Debug.Log ("No x.");
			}
			return false;
		}
		if (!vertices [x].ContainsKey (y)) {
			if (debug) {
				Debug.Log ("No y.");
			}
			return false;
		}
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
		if (vertices[x][y].nearRoad) return;
		vertices[x][y].setHeight (h);
	}

	public void AddHeight (IntVector2 i, float h) {
		vertices[i.x][i.y].AddHeight(h);
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
		foreach (KeyValuePair<Chunk, int> entry in vertices[x][y].chunkVertices)
			entry.Key.LockVertex();
	}

	public void Unlock (int x, int y) {
		vertices[x][y].locked = false;
		foreach (KeyValuePair<Chunk, int> entry in vertices[x][y].chunkVertices)
			entry.Key.UnlockVertex();
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
		return vertices[x][y].nearRoad;
	}

	// 
	public void CheckRoads (Vector3 roadPoint) {
		foreach (int x in vertices.Keys) {
			foreach (int y in vertices[x].Keys) {
				if (vertices[x][y].nearRoad) continue;
				Vector3 worldPos = vertices[x][y].WorldPos();
				vertices[x][y].nearRoad = 
					Vector2.Distance (new Vector2 (worldPos.x, worldPos.z), new Vector2 (roadPoint.x, roadPoint.z)) <= NEARBY_ROAD_DISTANCE;
					//Vector3.Distance (, roadPoint) <= NEARBY_ROAD_DISTANCE;
				if (vertices[x][y].nearRoad) {
					Unlock (x, y);
					vertices[x][y].setHeight(roadPoint.y);
					Lock (x, y);
					//Debug.Log(PlayerMovement.instance.progress);
					//Debug.Log (vertices[x][y].ToString());
				}
			}
		}
	}

	public void Randomize (float noise) {
		int x = vertices.Keys.Count-1;
		Debug.Log(x);
		//return;
		for (int i=0; i<x; i++) {
			for (int j=0; j<x; j++) {
				IntVector2 coords = new IntVector2 () {
					x=i,
					y=j
				};
				if (!ContainsVertex(coords)) AddVertex(i, j);
			}
		}
		while (!Mathf.IsPowerOfTwo(x-1)) x--;

		vertices[0][0].setHeight(Random.Range (-noise, noise));
		vertices[x][0].setHeight(Random.Range (-noise, noise));
		vertices[0][x].setHeight(Random.Range (-noise, noise));
		vertices[x][x].setHeight(Random.Range (-noise, noise));
		int currRes = x-1;
		var currNoise = noise;
		while (currRes%1 == 0 && currRes > 1) {
			Debug.Log(currRes);
			for (int i=0; i<x-1; i+=currRes) {
				for (int j=0; j<x-1; j+=currRes) {
					int midptX = i+currRes/2;
					int midptY = j+currRes/2;
					float avg = (vertices[i][j].height + vertices[i+currRes][j].height +
						vertices[i][j+currRes].height + vertices[i+currRes][j+currRes].height)/4f;
					vertices[midptX][midptY].setHeight (avg + Random.Range(-currNoise, currNoise));

					vertices[midptX][j].setHeight ((vertices[i][j].height + vertices[i+currRes][j].height)/2f + Random.Range(0f, currNoise));
					vertices[midptX][j+currRes].setHeight ((vertices[i][j+currRes].height + vertices[i+currRes][j+currRes].height)/2f+ Random.Range(0f, currNoise));
					vertices[i][midptY].setHeight ((vertices[i][j].height + vertices[i][j+currRes].height)/2f+ Random.Range(0f, currNoise));
					vertices[i+currRes][midptY].setHeight ((vertices[i+currRes][j].height + vertices[i+currRes][j+currRes].height)/2f+ Random.Range(0f, currNoise));
				}
			}
			currRes /= 2;
			currNoise /= 2f;
		}
	}

	//
	// Add a chunk vertex <-> Vertex relationship
	//
	public void RegisterChunkVertex (IntVector2 i, Chunk chunk, int vertIndex) {
		RegisterChunkVertex (i.x, i.y, chunk, vertIndex);
	}

	public void RegisterChunkVertex (int x, int y, Chunk chunk, int vertIndex) {
		if (!ContainsVertex (x, y)) AddVertex (x, y);

		vertices[x][y].chunkVertices.Add(new KeyValuePair<Chunk, int> (chunk, vertIndex));
		//vertices [x][y].updateNormal();
	}

	//

	void AddVertex (int x, int y) {
		//AddVertex (new Vertex (x, y));
		if (!vertices.ContainsKey(x))
			vertices.Add (x, new Dictionary <int, Vertex>() );

		if (!vertices[x].ContainsKey(y))
			vertices[x].Add (y, new Vertex (x, y));

		else
			Debug.LogError ("VertexMap.Add(): attempted to add a vertex to a filled position!");

		float avgH = 0f;
		avgH += (ContainsVertex(x-1, y) ? vertices[x-1][y].height/4f : 0f);
		avgH += (ContainsVertex(x+1, y) ? vertices[x+1][y].height/4f : 0f);
		avgH += (ContainsVertex(x, y +1) ? vertices[x][y+1].height/4f : 0f);
		avgH += (ContainsVertex(x, y-1) ? vertices[x][y-1].height/4f : 0f);
		avgH += Random.Range (-WorldManager.instance.VERT_HEIGHT_SCALE/4f, WorldManager.instance.VERT_HEIGHT_SCALE/4f);
		//if (Random.Range (0,100) == 0) Debug.Log(avgH);
		SetHeight (new IntVector2 { x=x, y=y}, avgH);
	}

	//void AddVertex (Vertex vert) {
		

	//}
			
}

public class Vertex {
	public List<KeyValuePair<Chunk, int>> chunkVertices;
	public bool locked = false;
	public int x;
	public int y;
	public float height = 0f;
	public float currHeight = 0f;
	public bool nearRoad = false;
	public Vector3 normal = Vector3.up;
	public float blendValue = Random.Range (0f, 1.0f);

	public Vertex (int x, int y) {
		this.x = x;
		this.y = y;
		chunkVertices = new List<KeyValuePair<Chunk, int>>();
	}

	public void updateNormal () {
		normal = Vector3.zero;
		List<KeyValuePair<Chunk, int>> deletes = new List<KeyValuePair<Chunk, int>>();
		foreach (KeyValuePair<Chunk, int> chunkVert in chunkVertices) {
			if (chunkVert.Key.chunk == null) {
				deletes.Add (chunkVert);
				continue;
			}
			normal += chunkVert.Key.chunk.GetComponent<MeshFilter>().mesh.normals[chunkVert.Value];
		}
		normal.Normalize ();
		foreach (KeyValuePair<Chunk, int> delete in deletes)
			chunkVertices.Remove (delete);
	}

	public void setHeight (float h) {
		if (locked || nearRoad) return;
		List<KeyValuePair<Chunk, int>> deletes = new List<KeyValuePair<Chunk, int>>();
		height = h;
		//if (Time.frameCount % 120 == 0) Debug.Log ("set height");
		foreach (KeyValuePair<Chunk, int> chunkVert in chunkVertices) {
			if (chunkVert.Key.chunk == null) {
				deletes.Add (chunkVert);
				continue;
			}
			/*
			Vector3[] verts = new Vector3[chunkVert.Key.vertices.Length];
			for (int i=0; i<verts.Length; i++) {
				verts[i] = chunkVert.Key.vertices[i];
				if (i == chunkVert.Value) verts[i].y = height;
			}
			*/
			//if (Time.frameCount % 120 == 0) Debug.Log (chunkVert.Key);
			chunkVert.Key.UpdateVertex (chunkVert.Value, h);
			//chunkVert.Key.vertices [chunkVert.Value].y = height;
			//chunkVert.Key.vertices = verts;
		}
		foreach (KeyValuePair<Chunk, int> delete in deletes)
			chunkVertices.Remove (delete);
		updateNormal();
		WorldManager.instance.TERRAIN_MATERIAL.SetFloat("Blend", Mathf.Abs(h/WorldManager.instance.VERT_HEIGHT_SCALE/2f));

	}

	public void AddHeight (float h) {
		setHeight (height + h);
	}

	public void lerpHeight(float factor) {
		float diff = height - currHeight;
		currHeight += diff * factor;
	}

	// Returns the world position of a vertex
	public Vector3 WorldPos () {
		return new Vector3 (
			(float)x / (float)(WorldManager.instance.CHUNK_RESOLUTION) * WorldManager.instance.CHUNK_SIZE - WorldManager.instance.CHUNK_SIZE/2f,
			height,
			(float)y / (float)(WorldManager.instance.CHUNK_RESOLUTION) * WorldManager.instance.CHUNK_SIZE - WorldManager.instance.CHUNK_SIZE/2f
		);
	}

	public override string ToString ()
	{
		string result = "Vertex ("+x+","+y+") Height: "+height+" | nearRoad: "+nearRoad;
		foreach (KeyValuePair<Chunk, int> member in chunkVertices) {
			result += "\nChunk "+member.Key.getX() +","+member.Key.getY();
		}
		return result;
	}
}
