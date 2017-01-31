// VertexMap.cs
// ©2016 Team 95

using Route95.Core;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Route95.World {

	/// <summary>
	/// Class to hold vertex data.
	/// </summary>
	public class VertexMap {

		#region Vars

		/// <summary>
		/// Map of all vertices.
		/// </summary>
		Map<Vertex> _vertices;

		int _xMin = 0;
		int _xMax = 0;
		int _yMin = 0;
		int _yMax = 0;

		List<GameObject> _decorationDeletes;

		#endregion
		#region Constructors

		/// <summary>
		/// Default constructor.
		/// </summary>
		public VertexMap() {
			// Init vars
			_decorationDeletes = new List<GameObject>();

			int chunkRadius = WorldManager.Instance.ChunkLoadRadius;
			int chunkRes = WorldManager.Instance.ChunkResolution;
			int width = chunkRadius * (chunkRes - 1);
			if (width % 2 == 1) width++;
			_vertices = new Map<Vertex>(width);
		}

		#endregion
		#region Properties

		public int XMax { get { return _xMax; } }

		public int XMin { get { return _xMin; } }

		public int YMax { get { return _yMax; } }

		public int YMin { get { return _yMin; } }

		public int Width { get { return _vertices.Width; } }

		#endregion
		#region Methods

		public bool ContainsVertex(int x, int y) {
			return (VertexAt(x, y) != null);
		}

		public float GetHeight(int x, int y) {
			if (!ContainsVertex(x, y)) return float.NaN;
			return VertexAt(x, y).Height;
		}

		public void SetHeight(int x, int y, float h) {
			Vertex vert = ContainsVertex(x, y) ? VertexAt(x, y) : AddVertex(x, y);
			vert.SetHeight(h);
		}

		public void AddHeight(IntVector2 i, float h) {
			VertexAt(i.x, i.y).AddHeight(h);
		}

		//
		// Lock a vertex
		//
		public void Lock(IntVector2 i) {
			Lock(i.x, i.y);
		}

		public void Lock(int x, int y) {
			VertexAt(x, y).Lock();
		}

		public void Unlock(int x, int y) {
			VertexAt(x, y).Unlock();
		}

		public bool IsLocked(int x, int y) {
			return (ContainsVertex(x, y) ? VertexAt(x, y).Locked : true);
		}

		//
		// Functions to check if a vertex is constrained (too close to road)
		//
		public bool IsConstrained(IntVector2 i) {
			return IsConstrained(i.x, i.y);
		}

		public bool IsConstrained(int x, int y) {
			if (!ContainsVertex(x, y)) return false;
			return VertexAt(x, y).NearRoad;
		}
		// 

		public void DoCheckRoads(Vector3 point) {
			WorldManager.Instance.StartCoroutine(CheckRoads(point));
		}

		IEnumerator CheckRoads(Vector3 roadPoint) {
			float startTime = Time.realtimeSinceStartup;
			float xWPos, yWPos;
			float chunkSize = WorldManager.Instance.ChunkSize;
			int chunkRes = WorldManager.Instance.ChunkResolution;
			float clearDist = WorldManager.Instance.RoadClearDistance;

			for (int x = _xMin; x <= _xMax; x++) {

				// Skip if impossible for a point to be in range
				xWPos = x * chunkSize / (chunkRes - 1) - chunkSize / 2f;
				if (Mathf.Abs(xWPos - roadPoint.x) > clearDist)
					continue;

				for (int y = _yMin; y < _yMax; y++) {

					// Skip if impossible for a point to be in range
					yWPos = y * chunkSize / (chunkRes - 1) - chunkSize / 2f;
					if (Mathf.Abs(yWPos - roadPoint.z) > clearDist)
						continue;

					Vertex vert = _vertices.At(x, y);
					if (vert == null) continue;

					// Get vertex distance to road
					float dist = Vector2.Distance(
						new Vector2(xWPos, yWPos),
						new Vector2(roadPoint.x, roadPoint.z)
					);

					if (dist < vert.DistToRoad) {
						vert.DistToRoad = dist;
						var g = vert.DistToRoad / clearDist / 4f;
						vert.SetGreen(1f - Mathf.Clamp01(g));

						if (!vert.NoDecorations) {
							vert.NoDecorations = true;
							vert.RemoveDecorations();
						}
					}

					if (vert.NearRoad) {

						// Smooth vertex
						if (vert.OnRoad)
							vert.SmoothHeight (roadPoint.y, 1f, WorldManager.Instance.RoadWidth);

						else {
							if (!vert.Locked)
								vert.SmoothHeight(roadPoint.y, UnityEngine.Random.Range(0.98f, 0.99f), UnityEngine.Random.Range(2, 6));
							else continue;
						}

						// Gather decorations to remove
						foreach (GameObject decoration in vert.Decorations)
							_decorationDeletes.Add(decoration);

						// Remove decorations
						foreach (GameObject decoration in _decorationDeletes)
							WorldManager.Instance.RemoveDecoration(decoration);
						_decorationDeletes.Clear();

						// Lock vertex
						vert.Lock();
					}

					if (Time.realtimeSinceStartup - startTime > GameManager.TargetDeltaTime) {
						yield return null;
						startTime = Time.realtimeSinceStartup;
					}
				}

			}

			if (Time.realtimeSinceStartup - startTime > GameManager.TargetDeltaTime) {
				yield return null;
				startTime = Time.realtimeSinceStartup;
			}
		}

		public Vertex VertexAt(int x, int y, bool make = false) {
			Vertex vert = _vertices.At(x, y);
			if (vert == null && make) vert = AddVertex(x, y);
			return vert;
		}

		public Vertex VertexAt(IntVector2 i, bool make = false) {
			return VertexAt(i.x, i.y, make);
		}

		public void RegisterDecoration(IntVector2 i, GameObject deco) {
			if (!ContainsVertex(i.x, i.y)) AddVertex(i.x, i.y);

			VertexAt(i.x, i.y).Decorations.Add(deco);
		}

		public Vertex LeftNeighbor(Vertex v) {
			if (ContainsVertex(v.X - 1, v.Y)) return VertexAt(v.X - 1, v.Y);
			else return null;
		}

		public Vertex RightNeighbor(Vertex v) {
			if (ContainsVertex(v.X + 1, v.Y)) return VertexAt(v.X + 1, v.Y);
			else return null;
		}

		public Vertex DownNeighbor(Vertex v) {
			if (ContainsVertex(v.X, v.Y - 1)) return VertexAt(v.X, v.Y - 1);
			else return null;
		}
		public Vertex UpNeighbor(Vertex v) {
			if (ContainsVertex(v.X, v.Y + 1)) return VertexAt(v.X, v.Y + 1);
			else return null;
		}

		//

		public Vertex AddVertex(IntVector2 i) {
			return AddVertex(i.x, i.y);
		}

		Vertex AddVertex(int x, int y) {
			Vertex result = new Vertex(x, y);
			_vertices.Set(x, y, result);
			if (x < _xMin) _xMin = x;
			if (x > _xMax) _xMax = x;
			if (y < _yMin) _yMin = y;
			if (y > _yMax) _yMax = y;
			return result;
		}

		#endregion
	}
}
