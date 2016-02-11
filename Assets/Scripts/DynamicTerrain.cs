using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DynamicTerrain {

	private float CHUNK_SIZE; //the width of the terrain square in Unity distance units
	private int CHUNK_RESOLUTION; //the number of vertices per row/column, minimum 2
	private Material TERRAIN_MATERIAL; //the material to apply to the terrain
	private int LOADED_CHUNK_RADIUS; //number of chunks from the player's chunk to load

	private int MAX_DECORATIONS; // maximum number of decorations

	private GameObject terrain;
	private List<Chunk> activeChunks; //list of active chunks
	private GameObject player;

	//if no value given for parameters in Unity Editor, set to these defaults

	public void translateWorld (Vector3 offset){
		terrain.transform.position += offset;
	}

	public void translateLocal (Vector3 offset){
		terrain.transform.localPosition += offset;
	}

	public DynamicTerrain (GameObject player, float size, int resolution, Material material, int chunkRadius){
		activeChunks = new List<Chunk>();
		this.player = player;
		CHUNK_SIZE = size;
		CHUNK_RESOLUTION = resolution;
		TERRAIN_MATERIAL = material;
		LOADED_CHUNK_RADIUS = chunkRadius;
		initializeParams ();
		terrain = new GameObject ("terrain");
		terrain.transform.position = player.transform.position;
		activeChunks.Add (createChunk(0, 0));
		activeChunks.Add (createChunk (0, 1));
	}

	void updateChunks(){

	}

	Chunk createChunk(int x, int y){
		Chunk newChunk = new Chunk(x, y, CHUNK_SIZE, CHUNK_RESOLUTION, TERRAIN_MATERIAL);
		newChunk.chunk.transform.parent = terrain.transform;
		return newChunk;
	}


	void deleteChunk(){

	}

	// Gives a random chunk (for decoration testing)
	public Chunk RandomChunk () {
		return activeChunks[UnityEngine.Random.Range(0, activeChunks.Count)];
	}

	void initializeParams () { //if given defaults of 0 for SIZE and LINEAR RESOLUTION, set to working values
		if (CHUNK_SIZE == 0)
			CHUNK_SIZE = 10;
		if (CHUNK_RESOLUTION == 0)
			CHUNK_RESOLUTION = 100;
		if (TERRAIN_MATERIAL == null) {
			Debug.LogError ("No material given for terrain.");
		}
		if (LOADED_CHUNK_RADIUS == 0) {
			LOADED_CHUNK_RADIUS = 6;
		}
	}

	public void update(){
		updateChunks ();
	}
}
