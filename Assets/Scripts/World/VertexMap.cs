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

		/// <summary>
		/// Distance from a road for a vertex to be considered close to a road.
		/// </summary>
        float _nearbyRoadDistance;

		/// <summary>
		/// Distance from a road for a vertex to block decorations.
		/// </summary>
        float _noDecorationsDistance;

        int _xMin = 0;
        int _xMax = 0;
        int _yMin = 0;
        int _yMax = 0;

        List<GameObject> _decorationDeletes;

		#endregion
		#region Constructors

		public VertexMap() {
            _decorationDeletes = new List<GameObject>();

            _nearbyRoadDistance = WorldManager.Instance.RoadWidth * 0.75f;
            _noDecorationsDistance = WorldManager.Instance.RoadWidth * 1.5f;

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

		//
		// Functions to check if the vertex map contains a vertex
		//
		public bool ContainsVertex(IntVector2 i) {
            return ContainsVertex(i.x, i.y);
        }

        public bool ContainsVertex(int x, int y) {
            return (VertexAt(x, y) != null);
        }

        //
        // Check the height of a vertex
        //
        public float GetHeight(IntVector2 i) {
            return GetHeight(i.x, i.y);
        }

        public float GetHeight(int x, int y) {
            if (!ContainsVertex(x, y)) return float.NaN;
            return VertexAt(x, y).Height;
        }

        //
        // Set the height of a vertex
        //
        public void SetHeight(IntVector2 i, float h) {
            SetHeight(i.x, i.y, h);
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

        //
        // Functions to check if a vertex is locked
        //
        public bool IsLocked(IntVector2 i) {
            return IsLocked(i.x, i.y);
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
            float xWPos;
            float yWPos;
			float chunkSize = WorldManager.Instance.ChunkSize;
			int chunkRes = WorldManager.Instance.ChunkResolution;

            for (int x = _xMin; x <= _xMax; x++) {

                // Skip if impossible for a point to be in range
                xWPos = x * chunkSize / (chunkRes - 1) - chunkSize / 2f;
                if (Mathf.Abs(xWPos - roadPoint.x) > _noDecorationsDistance)
                    continue;

                for (int y = _yMin; y < _yMax; y++) {

                    // Skip if impossible for a point to be in range
                    yWPos = y * chunkSize / (chunkRes - 1) - chunkSize / 2f;
                    if (Mathf.Abs(yWPos - roadPoint.z) > _noDecorationsDistance)
                        continue;

                    Vertex vert = _vertices.At(x, y);

                    float dist = Vector2.Distance(new Vector2(xWPos, yWPos), new Vector2(roadPoint.x, roadPoint.z));
                    vert.SetGreen(Mathf.Clamp01(_noDecorationsDistance / (dist + .01f)));

                    if (!vert.NoDecorations) {
                        vert.NoDecorations = dist <= _noDecorationsDistance;
                        vert.RemoveDecorations();
                    }

                    if (vert == null) continue;
                    if (vert.Locked) continue;

                    if (vert.NoDecorations) {
                        vert.NearRoad = dist <= _nearbyRoadDistance;

                        if (vert.NearRoad) {
                            vert.SmoothHeight(roadPoint.y, UnityEngine.Random.Range(0.98f, 0.99f), UnityEngine.Random.Range(2, 8));
                            foreach (GameObject decoration in vert.Decorations) _decorationDeletes.Add(decoration);
                            foreach (GameObject decoration in _decorationDeletes)
                                WorldManager.Instance.RemoveDecoration(decoration);
                            _decorationDeletes.Clear();
                            vert.Lock();
                        }

                        if (Time.realtimeSinceStartup - startTime > GameManager.TargetDeltaTime) {
                            yield return null;
                            startTime = Time.realtimeSinceStartup;
                        }
                    }
                }

            }

            if (Time.realtimeSinceStartup - startTime > GameManager.TargetDeltaTime) {
                yield return null;
                startTime = Time.realtimeSinceStartup;
            }
        }

        public void Randomize(float noise) {
            int x = _vertices.Width;
            //Debug.Log(x);
            //return;
            for (int i = 0; i < x; i++) {
                for (int j = 0; j < x; j++) {
                    IntVector2 coords = new IntVector2(i, j);
                    if (!ContainsVertex(coords)) AddVertex(i, j);
                }
            }
            while (!Mathf.IsPowerOfTwo(x - 1)) x--;

            _vertices.At(0, 0).SetHeight(UnityEngine.Random.Range(-noise, noise));
            _vertices.At(x, 0).SetHeight(UnityEngine.Random.Range(-noise, noise));
            _vertices.At(0, x).SetHeight(UnityEngine.Random.Range(-noise, noise));
            _vertices.At(x, x).SetHeight(UnityEngine.Random.Range(-noise, noise));
            int currRes = x - 1;
            var currNoise = noise;
            while (currRes % 1 == 0 && currRes > 1) {
                Debug.Log(currRes);
                for (int i = 0; i < x - 1; i += currRes) {
                    for (int j = 0; j < x - 1; j += currRes) {
                        int midptX = i + currRes / 2;
                        int midptY = j + currRes / 2;
                        float avg = (_vertices.At(i, j).Height + _vertices.At(i + currRes, j).Height +
                            _vertices.At(i, j + currRes).Height + _vertices.At(i + currRes, j + currRes).Height) / 4f;
                        _vertices.At(midptX, midptY).SetHeight(avg + UnityEngine.Random.Range(-currNoise, currNoise));

                        _vertices.At(midptX, j).SetHeight((_vertices.At(i, j).Height + _vertices.At(i + currRes, j).Height) / 2f + UnityEngine.Random.Range(0f, currNoise));
                        _vertices.At(midptX, j + currRes).SetHeight((_vertices.At(i, j + currRes).Height + _vertices.At(i + currRes, j + currRes).Height) / 2f + UnityEngine.Random.Range(0f, currNoise));
                        _vertices.At(i, midptY).SetHeight((_vertices.At(i, j).Height + _vertices.At(i, j + currRes).Height) / 2f + UnityEngine.Random.Range(0f, currNoise));
                        _vertices.At(i + currRes, midptY).SetHeight((_vertices.At(i + currRes, j).Height + _vertices.At(i + currRes, j + currRes).Height) / 2f + UnityEngine.Random.Range(0f, currNoise));
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

        public void RegisterChunkVertex(IntVector2 vertCoords, IntVector2 chunkCoords, int vertIndex) {
            Vertex vert = VertexAt(vertCoords, true);
			vert.AddChunkVertex (chunkCoords, vertIndex);
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
            //AddVertex (new Vertex (x, y));
            Vertex result = new Vertex(x, y);
            _vertices.Set(x, y, result);
            if (x < _xMin) _xMin = x;
            if (x > _xMax) _xMax = x;
            if (y < _yMin) _yMin = y;
            if (y > _yMax) _yMax = y;
            return result;
            /*float avgH = 0f;
            avgH += (ContainsVertex(x-1, y) ? vertices[x-1,y].height/4f : 0f);
            avgH += (ContainsVertex(x+1, y) ? vertices[x+1,y].height/4f : 0f);
            avgH += (ContainsVertex(x, y +1) ? vertices[x,y+1].height/4f : 0f);
            avgH += (ContainsVertex(x, y-1) ? vertices[x,y-1].height/4f : 0f);
            avgH += Random.Range (-WorldManager.Instance.heightScale/4f, WorldManager.Instance.heightScale/4f);
            //if (Random.Range (0,100) == 0) Debug.Log(avgH);
            SetHeight (new IntVector2 (x,y), avgH);*/

        }

		#endregion
	}
}
