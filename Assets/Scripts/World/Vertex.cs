using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IntVector2 {
	public int x;
	public int y;

	public IntVector2 (int i, int j) {
		x = i;
		y = j;
	}

	public bool IsCorner () {
		return 
			(x == 0 || x == WorldManager.instance.chunkResolution-1) &&
			(y == 0 || y == WorldManager.instance.chunkResolution-1);
	}

	public override string ToString () {
		return "("+x+","+y+")";
	}
}

public class VertexMap {
	public Dictionary<int, Dictionary<int, Vertex>> vertices; 

	//const float NEARBY_ROAD_DISTANCE = 8f; // max dist from a road for a vert to be considered nearby a road
	float NEARBY_ROAD_DISTANCE;
	int activeBulldozeRoutines = 0;

	float chunkSize;
	int chunkRes;

	public VertexMap () {
		vertices = new Dictionary<int, Dictionary<int, Vertex>>();
		chunkSize = WorldManager.instance.chunkSize;
		chunkRes = WorldManager.instance.chunkResolution;
		NEARBY_ROAD_DISTANCE = WorldManager.instance.roadWidth;
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
		vertices[x][y].SetHeight (h);
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
	}

	public void Unlock (int x, int y) {
		vertices[x][y].locked = false;
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
		if (!ContainsVertex(x,y)) return false;
		return vertices[x][y].nearRoad;
	}
	// 

	public void DoCheckRoads (List<Vector3> roadPoints) {
		WorldManager.instance.StartCoroutine (CheckRoads(roadPoints));
	}
	//public void CheckRoads (Vector3 roadPoint) {
	IEnumerator CheckRoads (List<Vector3> roadPoints) {
		activeBulldozeRoutines ++;
		float startTime = Time.realtimeSinceStartup;
		foreach (Vector3 roadPoint in roadPoints) {
			//Debug.Log("check "+roadPoint);
			foreach (int x in vertices.Keys) {
				if (Mathf.Abs(x * chunkSize/(chunkRes-1) - chunkSize/2f - roadPoint.x) > NEARBY_ROAD_DISTANCE) continue;
				foreach (int y in vertices[x].Keys) {
					if (Mathf.Abs(y * chunkSize/(chunkRes-1) - chunkSize/2f - roadPoint.z) > NEARBY_ROAD_DISTANCE) continue;
					if (vertices[x][y].locked) continue;
					if (vertices[x][y].nearRoad) {
						//Debug.Log("nearRoad: "+vertices[x][y].ToString());
						//continue;
					}
					Vector3 worldPos = vertices[x][y].WorldPos();
					float dist = Vector2.Distance (new Vector2 (worldPos.x, worldPos.z), new Vector2 (roadPoint.x, roadPoint.z));
					//Debug.Log(dist);
					vertices[x][y].nearRoad = dist <= NEARBY_ROAD_DISTANCE;
						//Vector3.Distance (, roadPoint) <= NEARBY_ROAD_DISTANCE;
					if (vertices[x][y].nearRoad) {
						//Debug.Log("constrained "+vertices[x][y].ToString());
						//Unlock (x, y);
						//vertices[x][y].setHeight(roadPoint.y + (dist/Mathf.Pow(NEARBY_ROAD_DISTANCE, 3f))*(vertices[x][y].height - roadPoint.y));
						vertices[x][y].SetHeight(roadPoint.y);
						foreach (GameObject decoration in vertices[x][y].decorations) {
							Debug.Log("destroyed", decoration);
							//MonoBehaviour.Destroy (decoration);

						}
						Lock (x, y);

						//Debug.Log(PlayerMovement.instance.progress);
						//Debug.Log ("Bulldozed " +vertices[x][y].ToString());
					}

					if (Time.realtimeSinceStartup - startTime > 1f / GameManager.instance.targetFrameRate) {
						yield return null;
						startTime = Time.realtimeSinceStartup;
					}
				}

			}

		}
		activeBulldozeRoutines --;
		yield return null;
	}

	public void Randomize (float noise) {
		int x = vertices.Keys.Count-1;
		//Debug.Log(x);
		//return;
		for (int i=0; i<x; i++) {
			for (int j=0; j<x; j++) {
				IntVector2 coords = new IntVector2 (i, j);
				if (!ContainsVertex(coords)) AddVertex(i, j);
			}
		}
		while (!Mathf.IsPowerOfTwo(x-1)) x--;

		vertices[0][0].SetHeight(Random.Range (-noise, noise));
		vertices[x][0].SetHeight(Random.Range (-noise, noise));
		vertices[0][x].SetHeight(Random.Range (-noise, noise));
		vertices[x][x].SetHeight(Random.Range (-noise, noise));
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
					vertices[midptX][midptY].SetHeight (avg + Random.Range(-currNoise, currNoise));

					vertices[midptX][j].SetHeight ((vertices[i][j].height + vertices[i+currRes][j].height)/2f + Random.Range(0f, currNoise));
					vertices[midptX][j+currRes].SetHeight ((vertices[i][j+currRes].height + vertices[i+currRes][j+currRes].height)/2f+ Random.Range(0f, currNoise));
					vertices[i][midptY].SetHeight ((vertices[i][j].height + vertices[i][j+currRes].height)/2f+ Random.Range(0f, currNoise));
					vertices[i+currRes][midptY].SetHeight ((vertices[i+currRes][j].height + vertices[i+currRes][j+currRes].height)/2f+ Random.Range(0f, currNoise));
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
		//Debug.Log("register");
		if (!ContainsVertex (x, y)) AddVertex (x, y);

		vertices[x][y].chunkVertices.Add(new KeyValuePair<Chunk, int> (chunk, vertIndex));
		//vertices [x][y].updateNormal();
	}

	public void RegisterDecoration (IntVector2 i, GameObject deco) {
		if (!ContainsVertex(i.x, i.y)) AddVertex (i.x, i.y);

		vertices[i.x][i.y].decorations.Add (deco);
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
		vertices[x][y].map = this;
		float avgH = 0f;
		avgH += (ContainsVertex(x-1, y) ? vertices[x-1][y].height/4f : 0f);
		avgH += (ContainsVertex(x+1, y) ? vertices[x+1][y].height/4f : 0f);
		avgH += (ContainsVertex(x, y +1) ? vertices[x][y+1].height/4f : 0f);
		avgH += (ContainsVertex(x, y-1) ? vertices[x][y-1].height/4f : 0f);
		avgH += Random.Range (-WorldManager.instance.heightScale/4f, WorldManager.instance.heightScale/4f);
		//if (Random.Range (0,100) == 0) Debug.Log(avgH);
		SetHeight (new IntVector2 (x,y), avgH);

	}

	//void AddVertex (Vertex vert) {
		

	//}
			
}

public class Vertex {
	public VertexMap map;
	public List<KeyValuePair<Chunk, int>> chunkVertices;
	public bool locked = false;
	public int x;
	public int y;
	public float height = 0f;
	public float currHeight = 0f;
	public bool nearRoad = false;
	public Vector3 normal = Vector3.up;
	public float slope = 0f;
	public float blendValue = Random.Range (0f, 1.0f);
	public List<GameObject> decorations;

	public Vertex (int x, int y) {
		this.x = x;
		this.y = y;
		chunkVertices = new List<KeyValuePair<Chunk, int>>();
		decorations = new List<GameObject>();
	}
		
	public void SetHeight (float h) {
		List<KeyValuePair<Chunk, int>> deletes = new List<KeyValuePair<Chunk, int>>();
		height = h;
		//if (Time.frameCount % 120 == 0) Debug.Log ("set height");
		normal = Vector3.zero;
		slope = 0f;
		int numPoints = 0;
		if (map.ContainsVertex(x-1, y)) {
			slope += Mathf.Abs(map.vertices[x-1][y].height-height);
			numPoints++;
		}
		if (map.ContainsVertex(x+1, y)) {
			slope += Mathf.Abs(map.vertices[x+1][y].height-height);
			numPoints++;
		}
		if (map.ContainsVertex(x, y-1)) {
			slope += Mathf.Abs(map.vertices[x][y-1].height-height);
			numPoints++;
		}
		if (map.ContainsVertex(x, y+1)) {
			slope += Mathf.Abs(map.vertices[x][y+1].height-height);
			numPoints++;
		}
		slope /= (float)numPoints;
		float blendValue = Mathf.Clamp01(slope/50f);///WorldManager.instance.heightScale;
		//if (Random.Range(0, 1000) == 0) Debug.Log(blendValue);
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
			normal += chunkVert.Key.chunk.GetComponent<MeshFilter>().mesh.normals[chunkVert.Value];
			chunkVert.Key.UpdateColor (chunkVert.Value, blendValue);
			//chunkVert.Key.vertices [chunkVert.Value].y = height;
			//chunkVert.Key.vertices = verts;
		}
		normal.Normalize();
		foreach (KeyValuePair<Chunk, int> delete in deletes)
			chunkVertices.Remove (delete);
		//updateNormal();



	}

	public void AddHeight (float h) {
		SetHeight (height + h);
	}

	public void lerpHeight(float factor) {
		float diff = height - currHeight;
		currHeight += diff * factor;
	}

	// Returns the world position of a vertex
	public Vector3 WorldPos () {
		Vector3 result = new Vector3 (
			(float)x / (float)(WorldManager.instance.chunkResolution-1) * WorldManager.instance.chunkSize - WorldManager.instance.chunkSize/2f,
			//x * WorldManager.instance.chunkSize - WorldManager.instance.chunkSize/2f,
			height,
			//y * WorldManager.instance.chunkSize - WorldManager.instance.chunkSize/2f
			(float)y / (float)(WorldManager.instance.chunkResolution-1) * WorldManager.instance.chunkSize - WorldManager.instance.chunkSize/2f
		);
		//Debug.Log(ToString() + result.ToString());
		return result;
	}

	public override string ToString ()
	{
		string result = "Vertex ("+x+","+y+") Height: "+height+" | nearRoad: "+nearRoad;
		foreach (KeyValuePair<Chunk, int> member in chunkVertices) {
			result += "\nChunk "+member.Key.x +","+member.Key.y;
		}
		return result;
	}
}
