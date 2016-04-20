using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Vertex {
	public static Dictionary<int, Dictionary<int, Vertex>> Vertices; 
	public List<KeyValuePair<Mesh, int>> chunkVertices;
	int x;
	int y;
	public float height;
	public float distToRoad;

	static Vertex() {
		Vertices = new Dictionary<int, Dictionary<int, Vertex>>();
	}

	public Vertex (int x, int y, float height) {
		this.x = x;
		this.y = y;
		this.height = height;

		if (!Vertices.ContainsKey (x)) {
			Vertex.Vertices.Add (x, new Dictionary<int, Vertex> ());
		}
		if (!Vertices [x].ContainsKey (y)) {
			Vertex.Vertices [x].Add (y, this);
		} 
	}

	public void Update () {
		height = DynamicTerrain.instance.heightmap[x][y];
	}

	public void SetHeight (float height) {
		foreach (KeyValuePair<Mesh, int> chunk in chunkVertices) {
			chunk.Key.vertices[chunk.Value].y = height;
		}
	}

	public static void UpdateVertex (int x, int y) {
		if (!Vertices.ContainsKey(x)) {
			Vertices.Add(x, new Dictionary<int, Vertex>());
		}
		if (!Vertices[x].ContainsKey(y)) {
			Vertices[x].Add(y, new Vertex (x, y, 0f));
		}
		Vertices[x][y].Update();
	}
}
