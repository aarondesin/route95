using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Vertex {
	public static Dictionary<int, Dictionary<int, Vertex>> Vertices; 
	int x;
	int y;
	float height;
	float distToRoad;

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
}
