// Vertex.cs
// ©2016 Team 95

using Route95.Core;

using System;
using System.Collections.Generic;

using UnityEngine;

namespace Route95.World {

	/// <summary>
	/// Class to store various data in a world vertex.
	/// </summary>
	public class Vertex {

		#region Vars

		/// <summary>
		/// List of coordinates and the mesh index they correspond to.
		/// </summary>
		List<KeyValuePair<IntVector2, int>> _chunkVertices;

		/// <summary>
		/// If locked, a vertex can't be affect by world changes.
		/// </summary>
		bool _locked = false;

		/// <summary>
		/// Vertex coordinates.
		/// </summary>
		int _x, _y;

		/// <summary>
		/// Ideal height of this vertex (world units).
		/// </summary>
		float _height = 0f;

		/// <summary>
		/// Current height of this vertex (world units).
		/// </summary>
		float _currHeight = 0f;

		float _distToRoad = Mathf.Infinity;

		/// <summary>
		/// Does the vertex block decorations.
		/// </summary>
		bool _noDecorations = false;

		/// <summary>
		/// Slope near this vertex.
		/// </summary>
		float _slope = 0f;

		/// <summary>
		/// Color of this vertex.
		/// </summary>
		Color _color;

		/// <summary>
		/// Decorations near this vertex.
		/// </summary>
		List<GameObject> _decorations;

		/// <summary>
		/// Is this vertex loaded?
		/// </summary>
		bool _loaded = false;

		#endregion
		#region Constructors

		public Vertex(int x, int y) {
			_x = x;
			_y = y;
			_chunkVertices = new List<KeyValuePair<IntVector2, int>>();
			_decorations = new List<GameObject>();
			float v = Mathf.PerlinNoise ((float)_x, (float)_y);
			_color = new Color(
				0f,
				(v >= 0.5f ? 1f : 0f),
				(v < 0.5f ? 0.75f : 0f),
				0.5f
			);
		}

		#endregion
		#region Properties

		public int X { get { return _x; } }

		public int Y { get { return _y; } }

		public float Height { get { return _height; } }

		public Color Color { get { return _color; } }

		public bool Locked { get { return _locked; } }

		public float DistToRoad {
			get { return _distToRoad; }
			set { _distToRoad = value; }
		}

		public bool NearRoad {
			get { return _distToRoad <= WorldManager.Instance.RoadClearDistance; }
		}

		public bool OnRoad {
			get { return _distToRoad <= WorldManager.Instance.RoadWidth; } }


		public List<GameObject> Decorations { get { return _decorations; } }

		public bool NoDecorations {
			get { return _noDecorations; }
			set { _noDecorations = value; }
		}

		#endregion
		#region Methods

		public void SmoothHeight(float h, float factor, float range) {
			SetHeight(h);
			Vector2 v = new Vector2((float)_x, (float)_y);
			VertexMap map = DynamicTerrain.Instance.VertexMap;
			for (int ix = map.XMin; ix <= map.XMax; ix++) {
				if ((float)Mathf.Abs(ix - _x) > range) continue;
				for (int iy = map.YMin; iy <= map.YMax; iy++) {
					if ((float)Mathf.Abs(iy - _y) > range) continue;

					Vector2 n = new Vector2((float)ix, (float)iy);
					float dist = Vector2.Distance(n, v);
					if (dist > range) continue;

					if (ix == _x && iy == _y) continue;

					Vertex vert = map.VertexAt(ix, iy);
					if (vert != null && !vert._locked) vert.Smooth(h, factor * (range - dist) / range);
				}
			}
		}

		public void SmoothHeight(float h, float factor) {
			SetHeight(h);
			VertexMap map = DynamicTerrain.Instance.VertexMap;
			Vertex l = map.LeftNeighbor(this);
			if (l != null && !l._locked && !l.NearRoad) l.Smooth(h, factor);

			Vertex r = map.RightNeighbor(this);
			if (r != null && !r._locked && r.NearRoad) r.Smooth(h, factor);

			Vertex d = map.DownNeighbor(this);
			if (d != null && !d._locked && d.NearRoad) d.Smooth(h, factor);

			Vertex u = map.UpNeighbor(this);
			if (u != null && !u._locked && u.NearRoad) u.Smooth(h, factor);

			float factor_squared = factor * factor;

			if (u != null) {
				Vertex ul = map.LeftNeighbor(u);
				if (ul != null && !ul._locked && ul.NearRoad) ul.Smooth(h, factor_squared);

				Vertex ur = map.RightNeighbor(u);
				if (ur != null && !ur._locked && ur.NearRoad) ur.Smooth(h, factor_squared);
			}

			if (d != null) {

				Vertex dl = map.LeftNeighbor(d);
				if (dl != null && !dl._locked && dl.NearRoad) dl.Smooth(h, factor_squared);

				Vertex dr = map.RightNeighbor(d);
				if (dr != null && !dr._locked && dr.NearRoad) dr.Smooth(h, factor_squared);
			}
		}

		public void Smooth(float h, float factor) {
			//if (UnityEngine.Random.Range(0,100) == 1) Debug.Log(factor);
			//Debug.Log(h);
			SetHeight(_height + (h - _height) * factor);
			//Debug.Log(h);
		}

		public void SetGreen (float g) {
			_color.g = g;
		}

		/// <summary>
		/// Returns true if the given x/y coordinate is an edge between chunks.
		/// </summary>
		bool IsEdge(int coord) {
			int chunkRes = WorldManager.Instance.ChunkResolution;
			return (coord % (chunkRes - 1) == 0);
		}

		/// <summary>
		/// Converts a coordinate into chunk space.
		/// </summary>
		int ChunkAt(int coord) {
			int chunkRes = WorldManager.Instance.ChunkResolution;
			return coord / (chunkRes - 1) - (coord < 0 ? 1 : 0);
		}

		/// <summary>
		/// Returns the left-/down-most chunk on an edge.
		/// </summary>
		int ChunkMin(int coord) {
			return ChunkMax(coord) - 1;
		}

		/// <summary>
		/// Returns the right-/up-most chunk on an edge.
		/// </summary>
		int ChunkMax(int coord) {
			int chunkRes = WorldManager.Instance.ChunkResolution;
			return coord / (chunkRes - 1);
		}

		/// <summary>
		/// Converts a vertex coordinate to a mesh vert index.
		/// </summary>
		int CoordToIndex(int chunkX, int chunkY) {
			int localX;
			int chunkRes = WorldManager.Instance.ChunkResolution;
			if (chunkX >= 0) localX = _x - chunkX * (chunkRes - 1);
			else localX = _x + Mathf.Abs(chunkX) * (chunkRes - 1);

			if (localX >= chunkRes || localX < 0)
				throw new ArgumentOutOfRangeException("Vertex.CoordToIndex(): x coord " + _x + " not on chunk " + chunkX + "!");

			int localY;
			if (chunkY >= 0) localY = _y - chunkY * (chunkRes - 1);
			else localY = _y + Mathf.Abs(chunkY) * (chunkRes - 1);

			if (localY >= chunkRes || localY < 0)
				throw new ArgumentOutOfRangeException("Vertex.CoordToIndex(): y coord " + _y + " not on chunk " + chunkY + "!");

			int i = localY * chunkRes + localX;

			return i;
		}

		void CalculateBlend() {
			bool debug = (UnityEngine.Random.Range(0, 10000) == 1);
			if (debug) Debug.Log(ToString());

			int numNeighbors = 0;
			float delta = 0f;

			VertexMap map = DynamicTerrain.Instance.VertexMap;
			Vertex l = map.VertexAt(_x - 1, _y);
			if (l != null && l._loaded) {
				numNeighbors++;
				float diff = Mathf.Abs(_height - l._height);
				if (debug) Debug.Log("Left: " + l.ToString() + " " + diff);
				delta += diff;
			}

			Vertex r = map.VertexAt(_x + 1, _y);
			if (r != null && r._loaded) {
				numNeighbors++;
				float diff = Mathf.Abs(_height - r._height);
				if (debug) Debug.Log("Right: " + r.ToString() + " " + diff);
				delta += diff;
			}

			Vertex u = map.VertexAt(_x, _y + 1);
			if (u != null && u._loaded) {
				numNeighbors++;
				float diff = Mathf.Abs(_height - u._height);
				if (debug) Debug.Log("Up: " + u.ToString() + " " + diff);
				delta += diff;
			}

			Vertex d = map.VertexAt(_x, _y - 1);
			if (d != null && d._loaded) {
				numNeighbors++;
				float diff = Mathf.Abs(_height - d._height);
				if (debug) Debug.Log("Down: " + d.ToString() + " " + diff);
				delta += diff;
			}

			_color.a = delta / (float)numNeighbors / 100f;
			if (debug) Debug.Log("final blend: " + _color.a);
		}

		void CalculateBlend2() {
			float max = 0f;

			VertexMap map = DynamicTerrain.Instance.VertexMap;
			Vertex l = map.VertexAt(_x - 1, _y);
			if (l != null && l._loaded) {
				float diff = Mathf.Abs(l._height - _height);
				if (diff > max) max = diff;
			}

			Vertex r = map.VertexAt(_x + 1, _y);
			if (r != null && r._loaded) {
				float diff = Mathf.Abs(r._height - _height);
				if (diff > max) max = diff;
			}

			Vertex u = map.VertexAt(_x, _y + 1);
			if (u != null && u._loaded) {
				float diff = Mathf.Abs(u._height - _height);
				if (diff > max) max = diff;
			}

			Vertex d = map.VertexAt(_x, _y - 1);
			if (d != null && d._loaded) {
				float diff = Mathf.Abs(d._height - _height);
				if (diff > max) max = diff;
			}

			_color.a = Mathf.Clamp01(max / 50f);
		}


		public void SetHeight(float h) {
			// Skip locked vertices
			if (_locked || h == _height) return;

			_loaded = true;

			// Set height
			_height = h;

			_color.a = 0f;
			/*Vertex l = map.VertexAt(x-1,y);
            color.a += (l != null ? Mathf.Abs (h - l.height) : 0f);

            Vertex r = map.VertexAt(x+1,y);
            color.a += (r != null ? Mathf.Abs (h - r.height) : 0f);

            Vertex u = map.VertexAt(x,y+1);
            color.a += (u != null ? Mathf.Abs (h - u.height) : 0f);

            Vertex d = map.VertexAt(x,y-1);
            color.a += (d != null ? Mathf.Abs (h - d.height) : 0f);

            color.a /= (WorldManager.Instance.heightScale/10f);
            color.a = Mathf.Clamp01(color.a);*/

			CalculateBlend2();

			int index;
			DynamicTerrain terrain = DynamicTerrain.Instance;

			if (IsEdge(_x)) {

				// Corner
				if (IsEdge(_y)) {

					Chunk ul = terrain.ChunkAt(ChunkMin(_x), ChunkMax(_y));
					if (ul != null) {
						index = CoordToIndex(ul.X, ul.Y);
						ul.UpdateVertex(index, _height, true);
						ul.UpdateColor(index, _color);
					}

					Chunk ur = terrain.ChunkAt(ChunkMax(_x), ChunkMax(_y));
					if (ur != null) {
						index = CoordToIndex(ur.X, ur.Y);
						ur.UpdateVertex(index, _height, true);
						ur.UpdateColor(index, _color);
					}

					Chunk dl = terrain.ChunkAt(ChunkMin(_x), ChunkMin(_y));
					if (dl != null) {
						index = CoordToIndex(dl.X, dl.Y);
						dl.UpdateVertex(index, _height, true);
						dl.UpdateColor(index, _color);
					}

					Chunk dr = terrain.ChunkAt(ChunkMax(_x), ChunkMin(_y));
					if (dr != null) {
						index = CoordToIndex(dr.X, dr.Y);
						dr.UpdateVertex(index, _height, true);
						dr.UpdateColor(index, _color);
					}

					// X edge
				} else {
					Chunk left = terrain.ChunkAt(ChunkMin(_x), ChunkAt(_y));
					if (left != null) {
						index = CoordToIndex(left.X, left.Y);
						left.UpdateVertex(index, _height, true);
						left.UpdateColor(index, _color);
					}

					Chunk right = terrain.ChunkAt(ChunkMax(_x), ChunkAt(_y));
					if (right != null) {
						index = CoordToIndex(right.X, right.Y);
						right.UpdateVertex(index, _height, true);
						right.UpdateColor(index, _color);
					}
				}

				// Y edge
			} else if (IsEdge(_y)) {
				Chunk bottom = terrain.ChunkAt(ChunkAt(_x), ChunkMin(_y));
				if (bottom != null) {
					index = CoordToIndex(bottom.X, bottom.Y);
					bottom.UpdateVertex(index, _height, true);
					bottom.UpdateColor(index, _color);
				}

				Chunk top = terrain.ChunkAt(ChunkAt(_x), ChunkMax(_y));
				if (top != null) {
					index = CoordToIndex(top.X, top.Y);
					top.UpdateVertex(index, _height, true);
					top.UpdateColor(index, _color);
				}

				// No edge
			} else {
				Chunk chunk = terrain.ChunkAt(ChunkAt(_x), ChunkAt(_y));
				if (chunk != null) {
					index = CoordToIndex(chunk.X, chunk.Y);
					chunk.UpdateVertex(index, _height, false);
					chunk.UpdateColor(index, _color);
				}
			}
		}

		public void AddHeight(float h) {
			SetHeight(_height + h);
		}

		public void LerpHeight(float factor) {
			float diff = _height - _currHeight;
			_currHeight += diff * factor;
		}

		// Returns the world position of a vertex
		public Vector3 WorldPos() {
			int chunkRes = WorldManager.Instance.ChunkResolution;
			float chunkSize = WorldManager.Instance.ChunkSize;
			Vector3 result = new Vector3(
				(float)_x / (float)(chunkRes - 1) * chunkSize - chunkSize / 2f,
				//x * WorldManager.Instance.chunkSize - WorldManager.Instance.chunkSize/2f,
				_height,
				//y * WorldManager.Instance.chunkSize - WorldManager.Instance.chunkSize/2f
				(float)_y / (float)(chunkRes - 1) * chunkSize - chunkSize / 2f
			);
			//Debug.Log(ToString() + result.ToString());
			return result;
		}

		public void RemoveDecorations() {
			while (_decorations.Count > 0)
				WorldManager.Instance.RemoveDecoration(_decorations.PopFront());
		}

		public void Lock () {
			_locked = true;
		}

		public void Unlock () {
			_locked = false;
		}

		public void AddChunkVertex (IntVector2 chunkCoords, int vertIndex) {
			_chunkVertices.Add(new KeyValuePair<IntVector2, int>(chunkCoords, vertIndex));
		}

		public override string ToString() {
			string result = "Vertex (" + _x + "," + _y + ") Height: " + _height + " | nearRoad: " + NearRoad;
			//foreach (KeyValuePair<Chunk, int> member in chunkVertices) {
			//	result += "\nChunk "+member.Key.x +","+member.Key.y;
			//}
			return result;
		}

		#endregion
	}
}
