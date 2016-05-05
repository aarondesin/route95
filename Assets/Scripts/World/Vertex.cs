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
	//public Dictionary<int, Dictionary<int, Vertex>> vertices; 
	public Vertex[,] vertices;
	public int width;

	//const float NEARBY_ROAD_DISTANCE = 8f; // max dist from a road for a vert to be considered nearby a road
	float NEARBY_ROAD_DISTANCE;
	int activeBulldozeRoutines = 0;

	float chunkSize;
	int chunkRes;
	int chunkRadius; 

	List<GameObject> deletes;

	public VertexMap () {
		//vertices = new Dictionary<int, Dictionary<int, Vertex>>();
		deletes = new List<GameObject>();

		chunkSize = WorldManager.instance.chunkSize;
		chunkRes = WorldManager.instance.chunkResolution;
		chunkRadius = WorldManager.instance.chunkLoadRadius;
		NEARBY_ROAD_DISTANCE = WorldManager.instance.roadWidth;
		width = chunkRadius*(chunkRes-1);
		if (width %2 == 1) width++;
		vertices = new Vertex[width,width];
	}

	//
	// Functions to check if the vertex map contains a vertex
	//
	public bool ContainsVertex (IntVector2 i) {
		return ContainsVertex (i.x, i.y);
	}

	public bool ContainsVertex (int x, int y) {
		if (vertices == null) return false;
		if (x >= width || y >= width) return false;
		if (VertexAt(x,y) == null) return false;
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
		return VertexAt(x, y).height;
	}

	//
	// Set the height of a vertex
	//
	public void SetHeight (IntVector2 i, float h) {
		SetHeight (i.x, i.y, h);
	}

	public void SetHeight (int x, int y, float h) {
		if (!ContainsVertex(x, y)) AddVertex (x,y);
		Vertex vert = VertexAt(x,y);
		if (vert.nearRoad) return;
		vert.SetHeight (h);
	}

	public void AddHeight (IntVector2 i, float h) {
		VertexAt(i.x, i.y).AddHeight(h);
	}

	//
	//
	//
	public Vector3 GetNormal (IntVector2 i) {
		return GetNormal (i.x, i.y);
	}

	public Vector3 GetNormal (int x, int y) {
		return VertexAt(x, y).normal;
	}

	//
	// Lock a vertex
	//
	public void Lock (IntVector2 i) {
		Lock (i.x, i.y);
	}

	public void Lock (int x, int y) {
		VertexAt(x, y).locked = true;
	}

	public void Unlock (int x, int y) {
		VertexAt(x, y).locked = false;
	}
		
	//
	// Functions to check if a vertex is locked
	//
	public bool IsLocked (IntVector2 i) {
		return IsLocked (i.x, i.y);
	}

	public bool IsLocked (int x, int y) {
		return (ContainsVertex(x,y) ? VertexAt(x,y).locked : true);
	}

	//
	// Functions to check if a vertex is constrained (too close to road)
	//
	public bool IsConstrained (IntVector2 i) {
		return IsConstrained (i.x, i.y);
	}

	public bool IsConstrained (int x, int y) {
		if (!ContainsVertex(x,y)) return false;
		return VertexAt(x,y).nearRoad;
	}
	// 

	public void DoCheckRoads (List<Vector3> roadPoints) {
		WorldManager.instance.StartCoroutine (CheckRoads(roadPoints));
	}

	IEnumerator CheckRoads (List<Vector3> roadPoints) {
		float startTime = Time.realtimeSinceStartup;

		foreach (Vector3 roadPoint in roadPoints) {
			for (int x = 0; x<width; x++) {

				// Skip if impossible for a point to be in range
				if (Mathf.Abs((x-width/2) * chunkSize/(chunkRes-1) - chunkSize/2f - roadPoint.x) > NEARBY_ROAD_DISTANCE) 
					continue;

				for (int y=0; y<width; y++) {

					// Skip if impossible for a point to be in range
					if (Mathf.Abs((y-width/2) * chunkSize/(chunkRes-1) - chunkSize/2f - roadPoint.z) > NEARBY_ROAD_DISTANCE) 
						continue;
					
					Vertex vert = vertices[x,y];
					if (vert == null) continue;
					if (vert.locked) continue;

					Vector3 worldPos = vert.WorldPos();
					float dist = Vector2.Distance (new Vector2 (worldPos.x, worldPos.z), new Vector2 (roadPoint.x, roadPoint.z));
					vert.nearRoad = dist <= NEARBY_ROAD_DISTANCE;

					if (vert.nearRoad) {
						vert.SmoothHeight(roadPoint.y);
						foreach (GameObject decoration in vert.decorations) deletes.Add(decoration);
						foreach (GameObject decoration in deletes) 
							WorldManager.instance.RemoveDecoration(decoration.GetComponent<Decoration>());
						vert.locked = true;
					}

					if (Time.realtimeSinceStartup - startTime > 1f / GameManager.instance.targetFrameRate) {
						yield return null;
						startTime = Time.realtimeSinceStartup;
					}
				}

			}

		}
		yield return null;
	}

	public void Randomize (float noise) {
		int x = width;
		//Debug.Log(x);
		//return;
		for (int i=0; i<x; i++) {
			for (int j=0; j<x; j++) {
				IntVector2 coords = new IntVector2 (i, j);
				if (!ContainsVertex(coords)) AddVertex(i, j);
			}
		}
		while (!Mathf.IsPowerOfTwo(x-1)) x--;

		vertices[0,0].SetHeight(Random.Range (-noise, noise));
		vertices[x,0].SetHeight(Random.Range (-noise, noise));
		vertices[0,x].SetHeight(Random.Range (-noise, noise));
		vertices[x,x].SetHeight(Random.Range (-noise, noise));
		int currRes = x-1;
		var currNoise = noise;
		while (currRes%1 == 0 && currRes > 1) {
			Debug.Log(currRes);
			for (int i=0; i<x-1; i+=currRes) {
				for (int j=0; j<x-1; j+=currRes) {
					int midptX = i+currRes/2;
					int midptY = j+currRes/2;
					float avg = (vertices[i,j].height + vertices[i+currRes,j].height +
						vertices[i,j+currRes].height + vertices[i+currRes,j+currRes].height)/4f;
					vertices[midptX,midptY].SetHeight (avg + Random.Range(-currNoise, currNoise));

					vertices[midptX,j].SetHeight ((vertices[i,j].height + vertices[i+currRes,j].height)/2f + Random.Range(0f, currNoise));
					vertices[midptX,j+currRes].SetHeight ((vertices[i,j+currRes].height + vertices[i+currRes,j+currRes].height)/2f+ Random.Range(0f, currNoise));
					vertices[i,midptY].SetHeight ((vertices[i,j].height + vertices[i,j+currRes].height)/2f+ Random.Range(0f, currNoise));
					vertices[i+currRes,midptY].SetHeight ((vertices[i+currRes,j].height + vertices[i+currRes,j+currRes].height)/2f+ Random.Range(0f, currNoise));
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
		VertexAt(x, y).chunkVertices.Add(new KeyValuePair<Chunk, int> (chunk, vertIndex));
		//vertices [x,y].updateNormal();
	}

	public Vertex VertexAt (int x, int y) {
		if (x+width/2 >= width || y+width/2 >= width) return null;
		//if (Random.Range(0,1000)==0) Debug.Log(x+","+y+" is actually "+(x+width/2)+","+(y+width/2)+" width:"+width);
		return vertices[x+width/2, y+width/2];
	}

	public void RegisterDecoration (IntVector2 i, GameObject deco) {
		if (!ContainsVertex(i.x, i.y)) AddVertex (i.x, i.y);

		VertexAt(i.x,i.y).decorations.Add (deco);
	}

	public Vertex LeftNeighbor (Vertex v) {
		if (ContainsVertex (v.x -1, v.y)) return VertexAt(v.x-1,v.y);
		else return null;
	}

	public Vertex RightNeighbor (Vertex v) {
		if (ContainsVertex (v.x +1, v.y)) return VertexAt(v.x+1,v.y);
		else return null;
	}

	public Vertex DownNeighbor (Vertex v) {
		if (ContainsVertex (v.x , v.y-1)) return VertexAt(v.x,v.y-1);
		else return null;
	}
	public Vertex UpNeighbor (Vertex v) {
		if (ContainsVertex (v.x, v.y+1)) return VertexAt(v.x,v.y+1);
		else return null;
	}

	//

	void AddVertex (int x, int y) {
		//AddVertex (new Vertex (x, y));
		while (x+width/2 >= width || y+width/2 >= width) {
			ResizeMap ();
		}
		vertices[x+width/2,y+width/2] = new Vertex(x,y);
		VertexAt(x,y).map = this;
		/*float avgH = 0f;
		avgH += (ContainsVertex(x-1, y) ? vertices[x-1,y].height/4f : 0f);
		avgH += (ContainsVertex(x+1, y) ? vertices[x+1,y].height/4f : 0f);
		avgH += (ContainsVertex(x, y +1) ? vertices[x,y+1].height/4f : 0f);
		avgH += (ContainsVertex(x, y-1) ? vertices[x,y-1].height/4f : 0f);
		avgH += Random.Range (-WorldManager.instance.heightScale/4f, WorldManager.instance.heightScale/4f);
		//if (Random.Range (0,100) == 0) Debug.Log(avgH);
		SetHeight (new IntVector2 (x,y), avgH);*/

	}

	public void ResizeMap () {
		
		Vertex[,] newMap = new Vertex[width*2,width*2];
		for (int x =0; x<width; x++) {
			for (int y=0; y<width; y++) {
				//vertices[x,y].x += width/2;
				//vertices[x,y].y += width/2;
				newMap[x+width/2,y+width/2] = vertices[x,y];
				//if (Random.Range(0,1000) == 0) Debug.Log("newWidth"+width*2+" "+x+","+y+" mapped to "+(x+width)+","+(y+width)+".");
			}
		}
		vertices = newMap;
		width *= 2;
		//PrintMap();
	}

	void PrintMap () {
		string result = "";
		for (int i=0; i<width; i++) {
			if (i==0) result += "|";
			for (int j=0; j<width; j++) {
				if (vertices[i,j] == null) result += "_";
				else result += "v";
			}
			if (i==width-1) result += "|\n";
		}
		Debug.Log(result);
	}
			
}

public class Vertex {
	static int chunkRes;
	static float chunkSize;
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
		chunkRes = WorldManager.instance.chunkResolution;
		chunkSize = WorldManager.instance.chunkSize;
	}

	public void SmoothHeight (float h) {
		SetHeight (h);
		Vertex l = map.LeftNeighbor (this);
		if (l != null && !l.locked) l.Smooth (h, 0.5f);

		Vertex r = map.RightNeighbor (this);
		if (r != null && !r.locked) r.Smooth (h, 0.5f);

		Vertex d = map.DownNeighbor (this);
		if (d != null && !d.locked) d.Smooth (h, 0.5f);

		Vertex u = map.UpNeighbor (this);
		if (u != null && !u.locked) u.Smooth (h, 0.5f);

		if (u != null) {
			Vertex ul = map.LeftNeighbor (u);
			if (ul != null && !ul.locked) ul.Smooth (h, 0.3f);

			Vertex ur = map.RightNeighbor (u);
			if (ur != null && !ur.locked) ur.Smooth (h, 0.3f);
		}

		if (d != null) {

			Vertex dl = map.LeftNeighbor (d);
			if (dl != null && !dl.locked) dl.Smooth (h, 0.3f);

			Vertex dr = map.RightNeighbor (d);
			if (dr != null && !dr.locked) dr.Smooth (h, 0.3f);
		}
	}

	public void Smooth (float h, float factor) {
		SetHeight (height + (h-height) * factor);
	}

		
	public void SetHeight (float h) {
		List<KeyValuePair<Chunk, int>> deletes = new List<KeyValuePair<Chunk, int>>();
		height = h;
		//if (Time.frameCount % 120 == 0) Debug.Log ("set height");
		normal = Vector3.zero;
		slope = 0f;
		int numPoints = 0;
		if (map.ContainsVertex(x-1, y)) {
			slope += Mathf.Abs(map.VertexAt(x-1,y).height-height);
			numPoints++;
		}
		if (map.ContainsVertex(x+1, y)) {
			slope += Mathf.Abs(map.VertexAt(x+1,y).height-height);
			numPoints++;
		}
		if (map.ContainsVertex(x, y-1)) {
			slope += Mathf.Abs(map.VertexAt(x,y-1).height-height);
			numPoints++;
		}
		if (map.ContainsVertex(x, y+1)) {
			slope += Mathf.Abs(map.VertexAt(x,y+1).height-height);
			numPoints++;
		}
		slope /= (float)numPoints;
		float blendValue = Mathf.Clamp01(slope/50f);///WorldManager.instance.heightScale;
		//if (Random.Range(0, 1000) == 0) Debug.Log(blendValue);
		foreach (KeyValuePair<Chunk, int> chunkVert in chunkVertices) {
			if (chunkVert.Key == null || chunkVert.Key.chunk == null) {
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
			normal += chunkVert.Key.mesh.normals[chunkVert.Value];
			normal.Normalize ();
			chunkVert.Key.UpdateVertex (chunkVert.Value, h, normal);
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
			(float)x / (float)(chunkRes-1) * chunkSize - chunkSize/2f,
			//x * WorldManager.instance.chunkSize - WorldManager.instance.chunkSize/2f,
			height,
			//y * WorldManager.instance.chunkSize - WorldManager.instance.chunkSize/2f
			(float)y / (float)(chunkRes-1) * chunkSize - chunkSize/2f
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
