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
	public VertexMap vertexmap;
	public Dictionary<int, Dictionary<int, bool>> chunkmap;

	public LinInt freqData;
	public int freqSampleSize = 128;
	public FFTWindow fftWindow = FFTWindow.Rectangular;

	private Dictionary<Chunk, int> chunkPriorities;
	private int chunkUpdatesPerCycle = 4;
	List<Chunk> chunksToUpdate;

	LinInt UpdateFreqData () {
		float[] data = new float[freqSampleSize];
		List<AudioSource> sources = new List<AudioSource>(); //InputManager.instance.audioSources;
		sources.AddRange(MusicManager.instance.instrumentAudioSources.Values);
		foreach (AudioSource source in sources) {
			if (!source.enabled) continue;
			float[] sample = new float[freqSampleSize];
			source.GetSpectrumData (sample, 0, fftWindow);
			for (int i = 0; i < freqSampleSize; i++) {
				if (sample [i] != float.NaN && sample [i] != 0f) {
					if (source == MusicManager.instance.instrumentAudioSources [Instrument.AllInstruments[0]]) {
						data [i] = sample [i];
					} else {
						data [i] += sample [i];
					}
				}
			}
		}
		return new LinInt (data);
	}

	public bool activeChunksContains(Chunk chunk) {
		return activeChunks.Contains (chunk);
	}

	public bool activeRoadChunksContains(Chunk chunk) {
		return activeRoadChunks.Contains (chunk);
	}

	public bool activeCloseToRoadChunksContains(Chunk chunk) {
		return activeCloseToRoadChunks.Contains (chunk);
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
		vertexmap = new VertexMap();
		SMOOTH_FACTOR = smoothFactor;
		chunkPriorities = new Dictionary<Chunk, int>();
		chunkmap = new Dictionary<int, Dictionary< int, bool>> ();
	}

	void updateChunks(float[] freqDataArray){
		//Debug.Log ("updatechunks");
		//AudioListener.GetSpectrumData (freqDataArray, 0, FFTWindow.Rectangular);
		//LinInt freqData = new LinInt (freqDataArray);
		freqData = UpdateFreqData();
		List<int> xChunks = new List<int>(); //x coords of chunks to be loaded
		List<int> yChunks = new List<int>(); //y coords of chunks to be loaded
		createChunkLists (xChunks, yChunks);
		createChunks (xChunks, yChunks);
		deleteChunks (xChunks, yChunks);
		if (chunksToUpdate == null)
			chunksToUpdate = new List<Chunk> ();
		else
			chunksToUpdate.Clear ();
		//int max = 0;
		//Debug.Log ("before scouting");
		foreach (Chunk chunk in activeChunks) {
			if (chunk.locked) continue;
			//Debug.Log ("before heuristic");
			chunkPriorities [chunk] += ChunkHeuristic (chunk) + 1;
			if (chunksToUpdate.Count == 0) {
				chunksToUpdate.Add (chunk);
				//max = chunkPriorities [chunk];
			} else {
				//Debug.Log ("before insertion");
				for (int i = 0; i < chunksToUpdate.Count; i++) {
					if (chunkPriorities [chunk] > chunkPriorities [chunksToUpdate [i]]) {
						chunksToUpdate.Insert (i, chunk);
						break;

					}
				}
			}
		}
		//Debug.Log ("before updating");
		for (int i = 0; i < chunkUpdatesPerCycle && i < activeChunks.Count; i++) {
			chunksToUpdate [i].update (player, VERT_UPDATE_DISTANCE, freqData);
			chunkPriorities [chunksToUpdate [i]] = 0;
		}
				
		/*for (int i=0; i<chunkUpdatesPerCycle; i++) {
			Chunk temp = HighestPriorityChunk();
			if (temp == null) return;
			temp.update(player, VERT_UPDATE_DISTANCE, freqData);
			chunkPriorities[temp] = 0;
		}*/
	}

	Chunk HighestPriorityChunk () {
		int max = 0;
		Chunk result = null;
		foreach (Chunk chunk in activeChunks) {
			chunkPriorities[chunk] += ChunkHeuristic(chunk) + 1;
			if (chunkPriorities[chunk] > max) {
				result = chunk;
				max = chunkPriorities[chunk];
			}
		}
		//Debug.Log(max);
		return result;
	}

	public int ChunkHeuristic (Chunk chunk) {
		//int dist = (int)DistanceToPlayer(chunk);
		//Debug.Log(dist);
		return 
			(int)DistanceToPlayer(chunk) + 
			//(chunk.nearbyRoad() ? 250 : 0) +
			(InView(chunk) ? 20000 : 0);
	}

	float DistanceToPlayer (Chunk chunk) {
		return Vector3.Distance (
			new Vector3 (chunk.getX()*CHUNK_SIZE, 0f, chunk.getY()*CHUNK_SIZE),
			new Vector3 (player.transform.position.x, 0f, player.transform.position.y)
		);
	}

	bool InView (Chunk chunk) {
		/*Vector3 camEulers = Camera.main.transform.rotation.eulerAngles;
		float CameraAngle = camEulers.y;
		float diff = AngularDistance(camEulers.y, Vector3.Angle(Camera.main.transform.position, chunk.chunk.transform.position));
		//Debug.Log(diff, chunk.chunk);
		bool result = diff < Camera.main.fieldOfView*Mathf.Deg2Rad/2f/2f/Mathf.PI;
		if (result && UnityEngine.Random.Range(0,60) == 0) Debug.Log("inview",chunk.chunk);*/
		//return result;
		//Vector3 screenPos = Camera.main.WorldToViewportPoint(chunk.chunk.transform.position);
		//bool result = screenPos.x >= 0f && screenPos.x <= 1f && ;
		bool result = Vector3.Angle((chunk.chunk.transform.position-Camera.main.gameObject.transform.position), Camera.main.transform.forward) < Camera.main.fieldOfView/2f;
		//if (result && UnityEngine.Random.Range(0,3840) == 0) Debug.Log("inview",chunk.chunk);
		return result;

	}

	Chunk createChunk(int x, int y){
		Chunk newChunk = new Chunk(x, y, CHUNK_SIZE, CHUNK_RESOLUTION, TERRAIN_MATERIAL, HEIGHT_SCALE);
		newChunk.chunk.transform.parent = terrain.transform;
		chunkPriorities.Add(newChunk, 0);
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
			if (!chunkmap.ContainsKey(x)) chunkmap.Add (x, new Dictionary<int, bool>());
			foreach (int y in yChunks) {
				if (!chunkmap[x].ContainsKey(y)) chunkmap[x].Add (y, false);
				if (!chunkmap[x][y]) {
					Chunk chunk = createChunk (x, y);
					activeChunks.Add (chunk);
					if (chunk.nearbyRoad()) {
						if (chunk.containsRoad()) activeRoadChunks.Add(activeChunks[activeChunks.Count-1]);
						activeCloseToRoadChunks.Add(activeChunks[activeChunks.Count-1]);
					}
					chunkmap[x][y] = true;
				}
			}
		}
	}

	void deleteChunk(Chunk chunk){
		//vertexmap.UnregisterChunkVertex(
		chunkPriorities.Remove(chunk);
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

		//Vertex.Vertices[x][y].SetHeight(v);
	}

	/*public void WriteHeightMap (int i, float v) {
		WriteHeightMap (v);
	}*/

	float AngularDistance (float angle, float pos) {
		float d = angle - pos;
		while (d < -Mathf.PI)
			d += 2f * Mathf.PI;
		while (d > Mathf.PI)
			d -= 2f * Mathf.PI;
		return 1.0f-Mathf.Abs(5f*d/Mathf.PI/2.0f);
	}

	//create a width by depth mountain centered at vertex (x,y) with a maximum altitude of height and a jaggedness of rough
	public void createMountain (int x, int y, int width, int depth, float height, float rough) {
		//ensure width and depth are odd
		if (width % 2 == 0)
			width++;
		if (depth % 2 == 0)
			depth++;
		VertexMap vmap = DynamicTerrain.instance.vertexmap;
		int size = Math.Max (width, depth);
		size--;
		size = makePowerTwo (size); //size of the Diamond Square Alg array
		if (size < 2) return;
		float[,] heightmap = new float[size, size];
		float[] corners = initializeCorners (vmap, x, y, width, depth);
		fillDiamondSquare (heightmap, rough);
	}

	//raises n to the nearest power of 2
	public int makePowerTwo (int n) {
		if (n < 2) return -1; // if n is less than 2, return error value -1
		if ((n != 0) && ((n & (n-1)) != 0)) return n; //if n is already a power of 2, return n
		int r = 0; //counter of highest power of 2 in n
		//bit shift n to get place of leading bit, r, which is the log base 2 of n
		while ((n >>= 1) != 0) {
			r++;
		}
		r++; //raise power of two to next highest
		return (int)Math.Pow (2, r);
	}

	//returns the initial corners for the Diamond Square Algorithm
	public float[] initializeCorners(VertexMap vmap, int x, int y, int width, int depth) {
		float[] corners = new float[4];
		//corner lower left
		int vx = x - (width - 1) / 2; //vertex x
		int vy = y - (width - 1) / 2; //vertex y
		if (!float.IsNaN(corners[1] = vmap.GetHeight(vx, vy))) corners[0] = 0f;
		//corner lower right
		vx = x + (width - 1) / 2; //vertex x
		vy = y - (width - 1) / 2; //vertex y
		if (!float.IsNaN(corners[1] = vmap.GetHeight(vx, vy))) corners[1] = 0f;
		//corner upper right
		vx = x + (width - 1) / 2; //vertex x
		vy = y + (width - 1) / 2; //vertex y
		if (!float.IsNaN(corners[1] = vmap.GetHeight(vx, vy))) corners[2] = 0f;
		//corner upper left
		vx = x - (width - 1) / 2; //vertex x
		vy = y + (width - 1) / 2; //vertex y
		if (!float.IsNaN(corners[1] = vmap.GetHeight(vx, vy))) corners[3] = 0f;
		return corners;
	}

	public void fillDiamondSquare (float[,] heightmap, float rough){

	}
}
