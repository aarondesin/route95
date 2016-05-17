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
	public Map<Vertex> vertices;
	public DynamicTerrain terrain;
	//const float NEARBY_ROAD_DISTANCE = 8f; // max dist from a road for a vert to be considered nearby a road
	float nearbyRoadDistance;
	float noDecorationsDistance;

	public int xMin = 0;
	public int xMax = 0;
	public int yMin = 0;
	public int yMax = 0;

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
		nearbyRoadDistance = WorldManager.instance.roadWidth * 0.75f;
		noDecorationsDistance = WorldManager.instance.roadWidth * 1.5f;
		roadHeight = WorldManager.instance.roadHeight;
		int width = chunkRadius*(chunkRes-1);
		if (width %2 == 1) width++;
		vertices = new Map<Vertex>(width);
	}

	//
	// Functions to check if the vertex map contains a vertex
	//
	public bool ContainsVertex (IntVector2 i) {
		return ContainsVertex (i.x, i.y);
	}

	public bool ContainsVertex (int x, int y) {
		return (VertexAt(x,y) != null);
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
		Vertex vert = ContainsVertex(x,y) ? VertexAt (x,y) : AddVertex(x,y);
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
			for (int x = xMin; x <= xMax; x++) {

				// Skip if impossible for a point to be in range
				xWPos = x * chunkSize/(chunkRes-1) - chunkSize/2f;
				if (Mathf.Abs(xWPos - roadPoint.x) > noDecorationsDistance) 
					continue;

				for (int y = yMin; y < yMax; y++) {

					// Skip if impossible for a point to be in range
					yWPos = y * chunkSize/(chunkRes-1) - chunkSize/2f ;
					if (Mathf.Abs(yWPos- roadPoint.z) > noDecorationsDistance) 
						continue;
					
					Vertex vert = vertices.At(x,y);
					if (vert == null) continue;
					if (vert.locked) continue;

					float dist = Vector2.Distance (new Vector2 (xWPos, yWPos), new Vector2 (roadPoint.x, roadPoint.z));
					vert.noDecorations = dist <= noDecorationsDistance;

					if (vert.noDecorations) {
						vert.nearRoad = dist <= nearbyRoadDistance;

						if (vert.nearRoad) {
							vert.SmoothHeight(roadPoint.y, UnityEngine.Random.Range(0.98f, 0.99f), UnityEngine.Random.Range(2, 8));
							foreach (GameObject decoration in vert.decorations) decorationDeletes.Add(decoration);
							foreach (GameObject decoration in decorationDeletes) 
								WorldManager.instance.RemoveDecoration(decoration);
							decorationDeletes.Clear();
							vert.locked = true;
						}

						if (Time.realtimeSinceStartup - startTime > GameManager.instance.targetDeltaTime) {
							yield return null;
							startTime = Time.realtimeSinceStartup;
						}
					}
				}

			}

			if (Time.realtimeSinceStartup - startTime > GameManager.instance.targetDeltaTime) {
				yield return null;
				startTime = Time.realtimeSinceStartup;
			}
		}
		yield return null;
	}

	public void Randomize (float noise) {
		int x = vertices.Width;
		//Debug.Log(x);
		//return;
		for (int i=0; i<x; i++) {
			for (int j=0; j<x; j++) {
				IntVector2 coords = new IntVector2 (i, j);
				if (!ContainsVertex(coords)) AddVertex(i, j);
			}
		}
		while (!Mathf.IsPowerOfTwo(x-1)) x--;

		vertices.At(0,0).SetHeight(UnityEngine.Random.Range (-noise, noise));
		vertices.At(x,0).SetHeight(UnityEngine.Random.Range (-noise, noise));
		vertices.At(0,x).SetHeight(UnityEngine.Random.Range (-noise, noise));
		vertices.At(x,x).SetHeight(UnityEngine.Random.Range (-noise, noise));
		int currRes = x-1;
		var currNoise = noise;
		while (currRes%1 == 0 && currRes > 1) {
			Debug.Log(currRes);
			for (int i=0; i<x-1; i+=currRes) {
				for (int j=0; j<x-1; j+=currRes) {
					int midptX = i+currRes/2;
					int midptY = j+currRes/2;
					float avg = (vertices.At(i,j).height + vertices.At(i+currRes,j).height +
						vertices.At(i,j+currRes).height + vertices.At(i+currRes,j+currRes).height)/4f;
					vertices.At(midptX,midptY).SetHeight (avg + UnityEngine.Random.Range(-currNoise, currNoise));

					vertices.At(midptX,j).SetHeight ((vertices.At(i,j).height + vertices.At(i+currRes,j).height)/2f + UnityEngine.Random.Range(0f, currNoise));
					vertices.At(midptX,j+currRes).SetHeight ((vertices.At(i,j+currRes).height + vertices.At(i+currRes,j+currRes).height)/2f+ UnityEngine.Random.Range(0f, currNoise));
					vertices.At(i,midptY).SetHeight ((vertices.At(i,j).height + vertices.At(i,j+currRes).height)/2f+ UnityEngine.Random.Range(0f, currNoise));
					vertices.At(i+currRes,midptY).SetHeight ((vertices.At(i+currRes,j).height + vertices.At(i+currRes,j+currRes).height)/2f+ UnityEngine.Random.Range(0f, currNoise));
				}
			}
			currRes /= 2;
			currNoise /= 2f;
		}
	}

	//
	// Add a chunk vertex <-> Vertex relationship
	//
	/*public void RegisterChunkVertex (IntVector2 i, Chunk chunk, int vertIndex) {
		RegisterChunkVertex (i.x, i.y, chunk, vertIndex);
	}

	public void RegisterChunkVertex (int x, int y, Chunk chunk, int vertIndex) {
		//Debug.Log("register");
		if (!ContainsVertex (x, y)) AddVertex (x, y);
		VertexAt(x, y).chunkVertices.Add(new KeyValuePair<Chunk, int> (chunk, vertIndex));

		//vertices [x,y].updateNormal();
	}*/

	public void RegisterChunkVertex (IntVector2 vertCoords, IntVector2 chunkCoords, int vertIndex) {
		Vertex vert = VertexAt(vertCoords, true);
		vert.chunkVertices.Add(new KeyValuePair<IntVector2, int> (chunkCoords, vertIndex));
	}

	public Vertex VertexAt (int x, int y, bool make = false) {
		Vertex vert = vertices.At (x,y);
		if (vert == null && make) vert = AddVertex (x, y);
		return vert;
	}

	public Vertex VertexAt (IntVector2 i, bool make = false) {
		return VertexAt(i.x, i.y, make);
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
		return AddVertex (i.x, i.y);
	}

	Vertex AddVertex (int x, int y) {
		//AddVertex (new Vertex (x, y));
		Vertex result = new Vertex(x,y);
		result.map = this;
		result.terrain = terrain;
		vertices.Set(x, y, result);
		if (x < xMin) xMin = x;
		if (x > xMax) xMax = x;
		if (y < yMin) yMin = y;
		if (y > yMax) yMax = y;
		return result;
		/*float avgH = 0f;
		avgH += (ContainsVertex(x-1, y) ? vertices[x-1,y].height/4f : 0f);
		avgH += (ContainsVertex(x+1, y) ? vertices[x+1,y].height/4f : 0f);
		avgH += (ContainsVertex(x, y +1) ? vertices[x,y+1].height/4f : 0f);
		avgH += (ContainsVertex(x, y-1) ? vertices[x,y-1].height/4f : 0f);
		avgH += Random.Range (-WorldManager.instance.heightScale/4f, WorldManager.instance.heightScale/4f);
		//if (Random.Range (0,100) == 0) Debug.Log(avgH);
		SetHeight (new IntVector2 (x,y), avgH);*/

	}
			
}

public class Vertex {
	public DynamicTerrain terrain;
	static int chunkRes;
	static float chunkSize;
	public VertexMap map;
	public List<KeyValuePair<IntVector2, int>> chunkVertices;
	public bool locked = false;
	public int x;
	public int y;
	public float height = 0f;
	public float currHeight = 0f;
	public bool nearRoad = false;
	public bool noDecorations = false;
	public Vector3 normal = Vector3.up;
	public float slope = 0f;
	public float blendValue = UnityEngine.Random.Range (0f, 1.0f);
	public List<GameObject> decorations;

	public Vertex (int x, int y) {
		this.x = x;
		this.y = y;
		chunkVertices = new List<KeyValuePair<IntVector2, int>>();
		decorations = new List<GameObject>();
		chunkRes = WorldManager.instance.chunkResolution;
		chunkSize = WorldManager.instance.chunkSize;
	}

	public void SmoothHeight (float h, float factor, float range) {
		SetHeight (h);
		Vector2 v = new Vector2 ((float)x, (float)y);
		for (int ix = map.xMin; ix <= map.xMax; ix++) {
			if ((float)Mathf.Abs(ix - x) > range) continue;
			for (int iy = map.yMin; iy <= map.yMax; iy++) {
				if ((float)Mathf.Abs(iy - y) > range) continue;

				Vector2 n = new Vector2 ((float)ix, (float)iy);
				float dist = Vector2.Distance (n,v);
				if (dist > range) continue;

				if (ix == x && iy == y) continue;

				Vertex vert = map.VertexAt(ix, iy);
				if (vert != null && !vert.locked) vert.Smooth(h, factor * (range-dist) / range);
			}
		}
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
		//if (UnityEngine.Random.Range(0,100) == 1) Debug.Log(factor);
		//Debug.Log(h);
		SetHeight (height + (h-height) * factor);
		//Debug.Log(h);
	}

	bool IsEdge (int coord) {
		return (coord % (chunkRes-1) == 0);
	}

	int ChunkMin (int coord) {
		return coord / (chunkRes-1) - 1;
	}

	int ChunkMax (int coord) {
		return coord < 0 ? coord / (chunkRes-1) -1 : coord / (chunkRes-1);
	}

	//int CoordToIndex (int x, int y, bool r, bool u, bool edge) {
	int CoordToIndex (int chunkX, int chunkY) {
		int localX = x - chunkX * (chunkRes-1);
		int localY = y - chunkY * (chunkRes-1);

		int i = localY * chunkRes + localX;

		return i;
	}

		
	public void SetHeight (float h) {
		// Skip locked vertices
		if (locked) return;

		// Set height
		height = h;


		if (IsEdge (x)) {

			// Corner
			if (IsEdge (y)) {

				Chunk ul = terrain.ChunkAt (ChunkMin(x), ChunkMax (y));
				if (ul != null) ul.UpdateVertex (CoordToIndex (ul.x, ul.y), height);

				Chunk ur = terrain.ChunkAt (ChunkMax(x), ChunkMax (y));
				if (ur != null) ur.UpdateVertex (CoordToIndex (ur.x, ur.y), height);

				Chunk dl = terrain.ChunkAt (ChunkMin(x), ChunkMin (y));
				if (dl != null) dl.UpdateVertex (CoordToIndex (dl.x, dl.y), height);

				Chunk dr = terrain.ChunkAt (ChunkMax(x), ChunkMin (y));
				if (dr != null) dr.UpdateVertex (CoordToIndex (dr.x, dr.y), height);

			// X edge
			} else {
				Chunk left = terrain.ChunkAt(ChunkMin (x), ChunkMax(y));
				if (left != null) left.UpdateVertex (CoordToIndex (left.x, left.y), height);

				Chunk right = terrain.ChunkAt(ChunkMax (x), ChunkMax(y));
				if (right != null) right.UpdateVertex(CoordToIndex(right.x, right.y), height);

			} 

		// Y edge
		} else if (IsEdge (y)) {
			Chunk bottom = terrain.ChunkAt(ChunkMax(x), ChunkMin(y));
			if (bottom != null) bottom.UpdateVertex (CoordToIndex (bottom.x, bottom.y), height);

			Chunk top = terrain.ChunkAt(ChunkMax(x), ChunkMax(y));
			if (top != null) top.UpdateVertex (CoordToIndex (top.x, top.y), height);
		
		// No edge
		} else {
			try {
				Chunk chunk = terrain.ChunkAt(ChunkMax(x), ChunkMax(y));
				if (chunk != null) chunk.UpdateVertex (CoordToIndex (chunk.x, chunk.y), height);
			} catch (NullReferenceException e) {
				Debug.LogError ("Vertex.SetHeight(): tried to access nonexistent chunk at "
					+ChunkMax(x)+","+ChunkMax(y)+" "
					+e.Message);
				return;
			}
		}


		/*slope = 0f;
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
		float blendValue = Mathf.Clamp01(slope/50f);///WorldManager.instance.heightScale;*/


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
		//foreach (KeyValuePair<Chunk, int> member in chunkVertices) {
		//	result += "\nChunk "+member.Key.x +","+member.Key.y;
		//}
		return result;
	}
}
