using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DynamicTerrain {
	public static DynamicTerrain instance;

	private float HEIGHT_SCALE; //scales the heights of the vertices from the LinInt data
	private float CHUNK_SIZE; //the width of the terrain square in Unity distance units
	private int CHUNK_RESOLUTION; //the number of vertices per row/column, minimum 2
	private Material TERRAIN_MATERIAL; //the material to apply to the terrain
	private int LOADED_CHUNK_RADIUS; //number of chunks from the player's chunk to load
	private float VERT_UPDATE_DISTANCE;
	public float SMOOTH_FACTOR;

	private int MAX_DECORATIONS; // maximum number of decorations

	private GameObject terrain;
	private List<Chunk> activeChunks; //list of active chunks
	private List<Chunk> activeRoadChunks; // list of active chunks with road on them
	private List<Chunk> activeCloseToRoadChunks;
	private GameObject player;

	public Dictionary<int ,Dictionary<int, float>> heightmap;

	public LinInt freqData;
	public int freqSampleSize = 128;
	public FFTWindow fftWindow = FFTWindow.Rectangular;

	LinInt UpdateFreqData () {
		float[] data = new float[freqSampleSize];
		foreach (AudioSource source in MusicManager.instance.instrumentAudioSources.Values) {
			float[] sample = new float[freqSampleSize];
			source.GetSpectrumData (sample, 0, fftWindow);
			for (int i = 0; i < freqSampleSize; i++) {
				if (sample [i] != float.NaN && sample [i] != 0f) {
					if (source == MusicManager.instance.instrumentAudioSources [(Instrument)0]) {
						data [i] = sample [i];
					} else {
						data [i] += sample [i];
					}
				}
			}
		}
		return new LinInt (data);
	}

	//if no value given for parameters in Unity Editor, set to these defaults

	public void translateWorld (Vector3 offset){
		terrain.transform.position += offset;
	}

	public void translateLocal (Vector3 offset){
		terrain.transform.localPosition += offset;
	}

	public DynamicTerrain (GameObject player, float chunkSize, int chunkResolution, Material material, int chunkRadius, float vertUpdateDist, float heightScale, float smoothFactor){
		instance = this;
		activeChunks = new List<Chunk>();
		activeRoadChunks = new List<Chunk>();
		activeCloseToRoadChunks = new List<Chunk>();
		this.player = player;
		CHUNK_SIZE = chunkSize;
		CHUNK_RESOLUTION = chunkResolution;
		TERRAIN_MATERIAL = material;
		LOADED_CHUNK_RADIUS = chunkRadius;
		initializeParams ();
		terrain = new GameObject ("terrain");
		terrain.transform.position = player.transform.position;
		VERT_UPDATE_DISTANCE = vertUpdateDist;
		HEIGHT_SCALE = heightScale;
		heightmap = new Dictionary<int, Dictionary<int, float>>();
		SMOOTH_FACTOR = smoothFactor;
	}

	void updateChunks(float[] freqDataArray){
		//AudioListener.GetSpectrumData (freqDataArray, 0, FFTWindow.Rectangular);
		//LinInt freqData = new LinInt (freqDataArray);
		freqData = UpdateFreqData();
		List<int> xChunks = new List<int>(); //x coords of chunks to be loaded
		List<int> yChunks = new List<int>(); //y coords of chunks to be loaded
		createChunkLists (xChunks, yChunks);
		createChunks (xChunks, yChunks);
		deleteChunks (xChunks, yChunks);
		foreach (Chunk chunk in activeChunks) {
			chunk.update(player, VERT_UPDATE_DISTANCE, freqData);
		}
	}

	Chunk createChunk(int x, int y){
		Chunk newChunk = new Chunk(x, y, CHUNK_SIZE, CHUNK_RESOLUTION, TERRAIN_MATERIAL, HEIGHT_SCALE);
		newChunk.chunk.transform.parent = terrain.transform;
		return newChunk;
	}

	// Gives a random chunk (for decoration testing)
	public Chunk RandomChunk () {
		return activeChunks[UnityEngine.Random.Range(0, activeChunks.Count)];
	}

	// Returns a random chunk with road on it
	public Chunk RandomRoadChunk() {
		return activeRoadChunks[UnityEngine.Random.Range(0, activeRoadChunks.Count)];
	}

	public Chunk RandomCloseToRoadChunk() {
		return activeCloseToRoadChunks[UnityEngine.Random.Range(0, activeCloseToRoadChunks.Count)];
	}

	//populates lists of chunk coords to be loaded
	void createChunkLists(List<int> xChunks, List<int> yChunks){
		int playerChunkX = (int)Math.Floor((player.transform.position.x - this.terrain.transform.position.x) / CHUNK_SIZE);
		int playerChunkY = (int)Math.Floor((player.transform.position.z - this.terrain.transform.position.z) / CHUNK_SIZE);
		xChunks.Add (playerChunkX);
		yChunks.Add (playerChunkY);
		for (int i = 1; i <= LOADED_CHUNK_RADIUS; i++){
			//get player coords in chunk coords
			xChunks.Add (playerChunkX + i);
			xChunks.Add (playerChunkX - i);
			yChunks.Add (playerChunkY + i);
			yChunks.Add (playerChunkY - i);
		}
	}

	void createChunks(List<int> xChunks, List<int> yChunks){
		foreach (int x in xChunks) {
			foreach (int y in yChunks) {
				bool loaded = false;
				foreach (Chunk chunk in activeChunks) {
					if (x == chunk.getX() && y == chunk.getY()) {
						loaded = true;
						break;
					}
				}
				if (!loaded) {
					activeChunks.Add (createChunk(x, y));
					if (x >= -1 && x <= 1) {
						if (x == 0) activeRoadChunks.Add(activeChunks[activeChunks.Count-1]);
						activeCloseToRoadChunks.Add(activeChunks[activeChunks.Count-1]);
					}
				}
			}
		}
	}

	void deleteChunk(Chunk chunk){
		UnityEngine.Object.Destroy(chunk.chunk);
		int numChildren = chunk.chunk.transform.childCount;
		WorldManager.instance.DecNumDeco (numChildren);
	}

	void deleteChunks(List<int> xChunks, List<int> yChunks) {
		List<Chunk> deletions = new List<Chunk> ();
		foreach (Chunk chunk in activeChunks) {
			//if chunk is not in chunks to be loaded
			if (!(xChunks.Contains(chunk.getX()) && yChunks.Contains(chunk.getY()))) {
				deleteChunk (chunk);
				deletions.Add (chunk);
			}
		}
		foreach (Chunk chunk in deletions) {
			activeChunks.Remove (chunk);
			if (activeRoadChunks.Contains(chunk))
				activeRoadChunks.Remove (chunk);
			if (activeCloseToRoadChunks.Contains(chunk))
				activeCloseToRoadChunks.Remove(chunk);
		}
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

	public void update(float[] freqDataArray){
		updateChunks (freqDataArray);
	}

	public float ReadHeightMap (int x, int y) {
		if (!heightmap.ContainsKey(x)) return float.NaN;
		if (!heightmap[x].ContainsKey(y)) return float.NaN;
		return heightmap[x][y];
	}

	/*public float ReadHeightMap (int i) {
		return ReadHeightMap (i/instance.CHUNK_RESOLUTION, i%CHUNK_RESOLUTION);
	}*/

	public void WriteHeightMap (int x, int y, float v) {
		if (!heightmap.ContainsKey(x))
			heightmap.Add(x, new Dictionary<int, float>());
		if (!heightmap[x].ContainsKey(y))
			heightmap[x].Add(y, v);
		else heightmap[x][y] = v;
	}

	/*public void WriteHeightMap (int i, float v) {
		WriteHeightMap (v);
	}*/
		
}
