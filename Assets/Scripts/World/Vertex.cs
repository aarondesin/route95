using UnityEngine;
using System;
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

	float roadHeight;

	float chunkSize;
	int chunkRes;
	int chunkRadius; 

	List<GameObject> decorationDeletes;

	public VertexMap () {
		decorationDeletes = new List<GameObject>();

		chunkSize = WorldManager.instance.chunkSize;
		chunkRes = WorldManager.instance.chunkResolution;
		chunkRadius = WorldManager.instance.chunkLoadRadius;
		NEARBY_ROAD_DISTANCE = WorldManager.instance.roadWidth;
		roadHeight = WorldManager.instance.roadHeight;
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
		float xWPos;
		float yWPos;

		foreach (Vector3 roadPoint in roadPoints) {
			for (int x = 0; x<width; x++) {

				// Skip if impossible for a point to be in range
				xWPos = (x-width/2) * chunkSize/(chunkRes-1) - chunkSize/2f;
				if (Mathf.Abs(xWPos - roadPoint.x) > NEARBY_ROAD_DISTANCE) 
					continue;

				for (int y=0; y<width; y++) {

					// Skip if impossible for a point to be in range
					yWPos = (y-width/2) * chunkSize/(chunkRes-1) - chunkSize/2f ;
					if (Mathf.Abs(yWPos- roadPoint.z) > NEARBY_ROAD_DISTANCE) 
						continue;
					
					Vertex vert = vertices[x,y];
					if (vert == null) continue;
					if (vert.locked) continue;

					float dist = Vector2.Distance (new Vector2 (xWPos, yWPos), new Vector2 (roadPoint.x, roadPoint.z));
					vert.nearRoad = dist <= NEARBY_ROAD_DISTANCE;

					if (vert.nearRoad) {
						vert.SmoothHeight(roadPoint.y-roadHeight, 0.95f);
						foreach (GameObject decoration in vert.decorations) decorationDeletes.Add(decoration);
						foreach (GameObject decoration in decorationDeletes) 
							WorldManager.instance.RemoveDecoration(decoration);
						decorationDeletes.Clear();
						vert.locked = true;
					}

					if (Time.realtimeSinceStartup - startTime > 1f / Application.targetFrameRate) {
						yield return null;
						startTime = Time.realtimeSinceStartup;
					}
				}

			}

			if (Time.realtimeSinceStartup - startTime > 1f / Application.targetFrameRate) {
				yield return null;
				startTime = Time.realtimeSinceStartup;
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

		vertices[0,0].SetHeight(UnityEngine.Random.Range (-noise, noise));
		vertices[x,0].SetHeight(UnityEngine.Random.Range (-noise, noise));
		vertices[0,x].SetHeight(UnityEngine.Random.Range (-noise, noise));
		vertices[x,x].SetHeight(UnityEngine.Random.Range (-noise, noise));
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
					vertices[midptX,midptY].SetHeight (avg + UnityEngine.Random.Range(-currNoise, currNoise));

					vertices[midptX,j].SetHeight ((vertices[i,j].height + vertices[i+currRes,j].height)/2f + UnityEngine.Random.Range(0f, currNoise));
					vertices[midptX,j+currRes].SetHeight ((vertices[i,j+currRes].height + vertices[i+currRes,j+currRes].height)/2f+ UnityEngine.Random.Range(0f, currNoise));
					vertices[i,midptY].SetHeight ((vertices[i,j].height + vertices[i,j+currRes].height)/2f+ UnityEngine.Random.Range(0f, currNoise));
					vertices[i+currRes,midptY].SetHeight ((vertices[i+currRes,j].height + vertices[i+currRes,j+currRes].height)/2f+ UnityEngine.Random.Range(0f, currNoise));
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
		while (x+width/2 >= width || y+width/2 >= width || x+width/2 < 0 || y+width/2 < 0) ResizeMap();
		//if (Random.Range(0,1000)==0) Debug.Log(x+","+y+" is actually "+(x+width/2)+","+(y+width/2)+" width:"+width);
		try {
			return vertices[x+width/2, y+width/2];
		} catch (IndexOutOfRangeException i) {
			Debug.LogError (x + "," + y + " width: "+width+i.Message);
			return null;
		}
	}

	public Vertex VertexAt (IntVector2 i) {
		return VertexAt(i.x, i.y);
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

	public Vertex AddVertex (IntVector2 i) {
		AddVertex (i.x, i.y);
		return VertexAt(i);
	}

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
	public float blendValue = UnityEngine.Random.Range (0f, 1.0f);
	public List<GameObject> decorations;

	public Vertex (int x, int y) {
		this.x = x;
		this.y = y;
		chunkVertices = new List<KeyValuePair<Chunk, int>>();
		decorations = new List<GameObject>();
		chunkRes = WorldManager.instance.chunkResolution;
		chunkSize = WorldManager.instance.chunkSize;
	}

	public void SmoothHeight (float h, float factor) {
		SetHeight (h);
		Vertex l = map.LeftNeighbor (this);
		if (l != null && !l.locked) l.Smooth (h, factor);

		Vertex r = map.RightNeighbor (this);
		if (r != null && !r.locked) r.Smooth (h, factor);

		Vertex d = map.DownNeighbor (this);
		if (d != null && !d.locked) d.Smooth (h, factor);

		Vertex u = map.UpNeighbor (this);
		if (u != null && !u.locked) u.Smooth (h, factor);

		float factor_squared = factor * factor;

		if (u != null) {
			Vertex ul = map.LeftNeighbor (u);
			if (ul != null && !ul.locked) ul.Smooth (h, factor_squared);

			Vertex ur = map.RightNeighbor (u);
			if (ur != null && !ur.locked) ur.Smooth (h, factor_squared);
		}

		if (d != null) {

			Vertex dl = map.LeftNeighbor (d);
			if (dl != null && !dl.locked) dl.Smooth (h, factor_squared);

			Vertex dr = map.RightNeighbor (d);
			if (dr != null && !dr.locked) dr.Smooth (h, factor_squared);
		}
	}

	public void Smooth (float h, float factor) {
		SetHeight (height + (h-height) * factor);
	}

		
	public void SetHeight (float h) {
		if (locked) return;
		List<KeyValuePair<Chunk, int>> deletes = new List<KeyValuePair<Chunk, int>>();
		height = h;
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
		foreach (KeyValuePair<Chunk, int> chunkVert in chunkVertices) {
			if (chunkVert.Key == null || chunkVert.Key.chunk == null) {
				deletes.Add (chunkVert);
				continue;
			}

			chunkVert.Key.UpdateVertex (chunkVert.Value, h);
			chunkVert.Key.UpdateColor (chunkVert.Value, blendValue);
		}

		foreach (KeyValuePair<Chunk, int> delete in deletes)
			chunkVertices.Remove (delete);

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
