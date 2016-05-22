using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

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

	int ChunkAt (int coord) {
		return coord / (chunkRes-1) - (coord < 0 ? 1 : 0);
	}

	int ChunkMin (int coord) {
		int result = coord / (chunkRes - 1) - (coord < 0 ? 1 : coord % (chunkRes-1) == 0 ? 1 : 0);
		return result;
	}

	int ChunkMax (int coord) {
		int result = coord / (chunkRes - 1) - (coord < 0 ? 1 : 0 );
		return result;
	}

	int CoordToIndex (int chunkX, int chunkY) {
		int localX = x - chunkX * (chunkRes-1);
		if (localX >= chunkRes || localX < 0)
			throw new ArgumentOutOfRangeException ("Vertex.CoordToIndex(): x coord "+x+" not on chunk "+chunkX+"!");

		int localY = y - chunkY * (chunkRes-1);
		if (localY >= chunkRes || localY < 0) 
			throw new ArgumentOutOfRangeException ("Vertex.CoordToIndex(): y coord "+y+" not on chunk "+chunkY+"!");

		int i = localY * chunkRes + localX;

		return i;
	}

		
	public void SetHeight (float h) {
		// Skip locked vertices
		if (locked || h == height) return;

		// Set height
		height = h;

		float blend = 0f;
		Vertex l = map.VertexAt(x-1,y);
		blend += (l != null ? Mathf.Abs (h - l.height) : 0f);

		Vertex r = map.VertexAt(x+1,y);
		blend += (r != null ? Mathf.Abs (h - r.height) : 0f);

		Vertex u = map.VertexAt(x,y+1);
		blend += (u != null ? Mathf.Abs (h - u.height) : 0f);

		Vertex d = map.VertexAt(x,y-1);
		blend += (d != null ? Mathf.Abs (h - d.height) : 0f);

		blend /= (WorldManager.instance.heightScale/10f);
		blend = Mathf.Clamp01(blend);

		if (IsEdge (x)) {

			// Corner
			if (IsEdge (y)) {

				Chunk ul = terrain.ChunkAt (ChunkMin(x), ChunkMax (y));
				if (ul != null) {
					ul.UpdateVertex (CoordToIndex (ul.x, ul.y), height);
					ul.UpdateColor (CoordToIndex (ul.x, ul.y), blend);
				}

				Chunk ur = terrain.ChunkAt (ChunkMax(x), ChunkMax (y));
				if (ur != null) {
					ur.UpdateVertex (CoordToIndex (ur.x, ur.y), height);
					ur.UpdateColor (CoordToIndex (ur.x, ur.y), blend);
				}

				Chunk dl = terrain.ChunkAt (ChunkMin(x), ChunkMin (y));
				if (dl != null) {
					dl.UpdateVertex (CoordToIndex (dl.x, dl.y), height);
					dl.UpdateColor (CoordToIndex (dl.x, dl.y), blend);
				}

				Chunk dr = terrain.ChunkAt (ChunkMax(x), ChunkMin (y));
				if (dr != null) {
					dr.UpdateVertex (CoordToIndex (dr.x, dr.y), height);
					dr.UpdateColor (CoordToIndex (dr.x, dr.y), blend);
				}

			// X edge
			} else {
				Chunk left = terrain.ChunkAt(ChunkMin (x), ChunkAt(y));
				if (left != null) {
					left.UpdateVertex (CoordToIndex (left.x, left.y), height);
					left.UpdateColor (CoordToIndex (left.x, left.y), blend);
				} 

				Chunk right = terrain.ChunkAt(ChunkMax (x), ChunkAt(y));
				if (right != null) {
					right.UpdateVertex(CoordToIndex(right.x, right.y), height);
					right.UpdateColor (CoordToIndex (right.x, right.y), blend);
				}
			} 

		// Y edge
		} else if (IsEdge (y)) {
			Chunk bottom = terrain.ChunkAt(ChunkAt(x), ChunkMin(y));
			if (bottom != null) {
				bottom.UpdateVertex (CoordToIndex (bottom.x, bottom.y), height);
				bottom.UpdateColor (CoordToIndex (bottom.x, bottom.y), blend);
			}

			Chunk top = terrain.ChunkAt(ChunkAt(x), ChunkMax(y));
			if (top != null) {
				top.UpdateVertex (CoordToIndex (top.x, top.y), height);
				top.UpdateColor (CoordToIndex (top.x, top.y), blend);
			}
		
		// No edge
		} else {
			Chunk chunk = terrain.ChunkAt(ChunkAt(x), ChunkAt(y));
			if (chunk != null) {
				chunk.UpdateVertex (CoordToIndex (chunk.x, chunk.y), height);
				chunk.UpdateColor (CoordToIndex (chunk.x, chunk.y), blend);
			}
		}
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
