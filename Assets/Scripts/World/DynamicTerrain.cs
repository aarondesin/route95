using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif


/// <summary>
/// Class to manage chunks. 
/// </summary>
public class DynamicTerrain : MonoBehaviour {
	
	#region DynamicTerrain Vars

	bool loaded = false;                 // is the terrain loaded?

	ObjectPool chunkPool;                // pool of chunk GameObjects
	List<Chunk> chunksToUpdate;          // list of chunks to be updated
	int chunkUpdatesPerCycle;            // number of chunks to update each cycle (copied from WM)

	List<Chunk> activeChunks;            // list of active chunks
	List<Chunk> activeCloseToRoadChunks; // list of active chunks that are within one chunk width of the road
	List<Chunk> activeRoadChunks;        // list of active chunks with road on them
	List<Chunk> deletions;               // list of chunks that will be deleted

	public VertexMap vertexmap;          // map of vertices
	Map<Chunk> chunkmap;                 // map of chunks

	float chunkSize;                     // size of chunks in world units (copied from WM)
	int chunkRes;                        // resolution of chunks (copied from WM)
	int chunkLoadRadius;                 // radius around player to load chunks, in number of chunks (copied from WM)

	Vector3 playerPos;                   // world position of player avatar (copied from PlayerMovement)
	IntVector2 playerChunkPos;           // 2D position of player avatar in terms of chunk

	#endregion
	#region Frequency Sampling Vars

	public LinInt freqData;              // pointer to stored music frequency data
	int freqSampleSize;                  // sample size to use when reading frequency data (copied from WM)
	float[] data;                        // raw frequency data
	FFTWindow fftWindow;                 // FFT window to use when reading frequency data (copied from WM)
	List<AudioSource> sources;           // list of instrument audio sources to read from (copied from MM)

	#endregion
	#region Unity Callbacks

	void Awake () {

		// Copy vars from WM
		chunkUpdatesPerCycle = WorldManager.instance.chunkUpdatesPerCycle;
		chunkSize = WorldManager.instance.chunkSize;
		chunkLoadRadius = WorldManager.instance.chunkLoadRadius;

		// Init chunk pool
		chunkPool = new ObjectPool();

		// Init chunk lists
		chunksToUpdate = new List<Chunk>();
		activeChunks = new List<Chunk>();
		activeRoadChunks = new List<Chunk>();
		activeCloseToRoadChunks = new List<Chunk>();
		deletions = new List<Chunk>();

		// Init vertex map
		vertexmap = new VertexMap();
		vertexmap.terrain = this;

		// Init chunk map
		chunkmap = new Map<Chunk>(chunkLoadRadius*2);

		// Init player chunk position
		playerChunkPos = new IntVector2(0, 0);

		// Init frequency data vars
		freqSampleSize = WorldManager.instance.freqArraySize;
		data = new float[freqSampleSize];
		fftWindow = WorldManager.instance.freqFFTWindow;

	}

	#endregion
	#region DynamicTerrain Load Callbacks

	/// <summary>
	/// Starts the chunk update coroutine.
	/// </summary>
	public void DoLoadChunks () {
		WorldManager.instance.StartCoroutine(UpdateChunks());
	}


	#endregion
	#region DynamicTerrain Callbacks

	/// <summary>
	/// Adds a new chunk at the specified coordinates,
	/// drawing from the pool if possible.
	/// </summary>
	/// <returns>The chunk.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	GameObject CreateChunk (int x, int y) {

		GameObject chunk;

		// If no chunks available to reuse
		if (chunkPool.Empty) {

			// Create new chunk
			chunk = new GameObject ("", 
				typeof(MeshFilter), 
				typeof(MeshRenderer),
				typeof(MeshCollider),
				typeof(Rigidbody),
				typeof(Chunk)
			);

			// Initialize chunk
			chunk.GetComponent<Chunk>().Initialize(x, y);

		// If a chunk is available to reuse
		} else {

			// Take a chunk from the pool
			chunk = chunkPool.Get();

			// Reuse chunk
			chunk.GetComponent<Chunk>().Reuse(x, y);
		}

		// Parent chunk to terrain
		chunk.transform.parent = transform;

		// Register chunk as active
		activeChunks.Add(chunk.GetComponent<Chunk>());

		return chunk;
	}
		
	/// <summary>
	/// Updates the chunks.
	/// </summary>
	/// <returns>The chunks.</returns>
	IEnumerator UpdateChunks () {
		
		// Init loading vars
		int chunksToLoad = 0; // number of chunks to be loaded at start
		float startTime = Time.realtimeSinceStartup;

		// Change loading screen message
		if (!loaded) GameManager.instance.ChangeLoadingMessage("Loading chunks...");

		// Main loop
		while (true) {

			// If loading terrain for the first time
			if (!loaded) {
				chunksToLoad = (chunkLoadRadius*2+1) * (chunkLoadRadius*2+1);

			// If updating terrain
			} else {
				DeleteChunks();
				UpdateFreqData ();
			}

			// Update player world and chunk positions
			playerPos = PlayerMovement.instance.transform.position;
			playerChunkPos = new IntVector2 (
				(int)Mathf.RoundToInt((playerPos.x - chunkSize/2f) / chunkSize),
				(int)Mathf.RoundToInt((playerPos.z -chunkSize/2f) / chunkSize)
			);

			// For each space where there should be a chunk
			for (int x=playerChunkPos.x - chunkLoadRadius; x<=playerChunkPos.x + chunkLoadRadius; x++) {
				for (int y=playerChunkPos.y - chunkLoadRadius; y<=playerChunkPos.y + chunkLoadRadius; y++) {
					
					// Skip if chunk exists
					if (chunkmap.At(x,y) != null) continue;

					// Create chunk
					chunkmap.Set(x,y, CreateChunk (x, y).GetComponent<Chunk>());

					if (!loaded) {
						GameManager.instance.ReportLoaded(1);
						startTime = Time.realtimeSinceStartup;
					}
					yield return null;
						
				}
			}

			// Take a break if target frame rate is missed
			if (Time.realtimeSinceStartup - startTime > GameManager.instance.targetDeltaTime) {
				yield return null;
				startTime = Time.realtimeSinceStartup;
			}

			// If finished loading terrain
			if (!loaded && activeChunks.Count == chunksToLoad) {
				loaded = true;

				// Update all colliders
				foreach (Chunk chunk in activeChunks) chunk.UpdateCollider();

				// Deform initial terrain
				int res = vertexmap.vertices.Width;
				CreateMountain (0, 0, res, res, 10f, 20f, -0.03f, 0.03f);

				// Begin loading road
				WorldManager.instance.DoLoadRoad();

			// If updating terrain
			} else {

				// Reset list of chunks to update
				chunksToUpdate.Clear ();

				// For each active chunk
				foreach (Chunk chunk in activeChunks) {

					// Increase priority
					chunk.priority++;

					// Insert chunk into list based on priority
					if (chunksToUpdate.Count == 0) chunksToUpdate.Add (chunk);
					else for (int i=0; i<chunksToUpdate.Count; i++) 
						if (chunk.priority > chunksToUpdate[i].priority) {
							chunksToUpdate.Insert (i, chunk);
							break;
						}
				}
					
				// Update highest priority chunks
				for (int i=0; i < WorldManager.instance.chunkUpdatesPerCycle && i < activeChunks.Count && i < chunksToUpdate.Count; i++) {
					try {
						chunksToUpdate [i].ChunkUpdate ();
						chunksToUpdate[i].priority = 0;
					}catch (ArgumentOutOfRangeException a) {
						Debug.LogError ("Index: "+i+" Count: "+chunksToUpdate.Count+" "+a.Message);
						continue;
					}
				}

				// Take a break if target frame rate missed
				if (Time.realtimeSinceStartup - startTime > GameManager.instance.targetDeltaTime) {
					yield return null;
					startTime = Time.realtimeSinceStartup;
				}

			}
			yield return null;
		}
	}

	/// <summary>
	/// Determines which chunks to delete and deletes them.
	/// </summary>
	void DeleteChunks () {

		// Init list of chunks to delete
		deletions.Clear();

		// Check if each active chunk is within chunk load radius
		foreach (Chunk chunk in activeChunks) {
			if (chunk.x < playerChunkPos.x - chunkLoadRadius || chunk.x > playerChunkPos.x + chunkLoadRadius ||
				chunk.y < playerChunkPos.y - chunkLoadRadius || chunk.y > playerChunkPos.y + chunkLoadRadius)
				deletions.Add(chunk);
		}

		// Delete all marked chunks
		foreach (Chunk chunk in deletions) DeleteChunk (chunk);
	}

	/// <summary>
	/// Deletes (pools) a chunk.
	/// </summary>
	/// <param name="chunk">Chunk.</param>
	void DeleteChunk(Chunk chunk){

		// Stop chunk updating verts.
		chunk.StopUpdatingVerts();

		// Remove all decorations on chunk
		chunk.RemoveDecorations();

		// Deregister from lists/map
		DeregisterChunk(chunk);

		// Pool chunk
		chunkPool.Add(chunk.gameObject);
	}

	/// <summary>
	/// Chunks at x and y.
	/// </summary>
	/// <returns>The <see cref="Chunk"/>.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public Chunk ChunkAt (int x, int y) {
		return chunkmap.At (x, y);
	}

	/// <summary>
	/// Removes a chunk from the appropriate lists.
	/// </summary>
	/// <param name="chunk">Chunk.</param>
	public void DeregisterChunk (Chunk chunk) {

		// Remove chunk from active list
		activeChunks.Remove(chunk);

		// Remove chunk from road and close to road lists
		if (chunk.nearRoad) {
			activeCloseToRoadChunks.Remove(chunk);
			if (chunk.hasRoad) activeRoadChunks.Remove(chunk);
		}

		// Deparent chunk
		chunk.transform.parent = null;

		// Remove entry from chunkmap
		chunkmap.Set(chunk.x, chunk.y, null);
	}

	/// <summary>
	/// Checks all active chunks for road.
	/// </summary>
	public void CheckAllChunksForRoad() {
		foreach (Chunk chunk in activeChunks)
			chunk.CheckForRoad(PlayerMovement.instance.moving ? PlayerMovement.instance.progress : 0f);
	}

	/// <summary>
	/// Called when the road is extended.
	/// </summary>
	public void OnExtendRoad () {

		// Mark any chunk that is not near a road to be checked for road
		foreach (Chunk chunk in activeChunks) {
			if (!chunk.nearRoad) chunk.hasCheckedForRoad = false;
		}
	}
		
	/// <summary>
	/// Reads the frequency data put out from instruments.
	/// </summary>
	void UpdateFreqData () {

		// Get audio sources from MM
		if (sources == null) {
			sources = new List<AudioSource>();
			sources.AddRange(MusicManager.instance.instrumentAudioSources.Values);
		}

		// For each instrument audio source
		foreach (AudioSource source in sources) {

			// Skip if disabled
			if (!source.enabled) continue;

			// Sample audio source
			float[] sample = new float[freqSampleSize];
			source.GetSpectrumData (sample, 0, fftWindow);

			// Add audio source data into final array
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

		// Convert audio data into LinInt
		if (freqData == null) freqData = new LinInt();
		freqData.Update(data);
	}
		
	/// <summary>
	/// Returns a random active chunk.
	/// </summary>
	/// <returns>A random active chunk.</returns>
	public Chunk RandomChunk () {

		// Check if no active chunks
		if (activeChunks.Count == 0) {
			Debug.LogError("DynamicTerrain.RandomChunk(): no active chunks!");
			return null;
		}

		// Pick a random active chunk
		Chunk chunk = activeChunks[UnityEngine.Random.Range(0, activeChunks.Count)];

		// Check if chunk is null
		if (chunk == null) {
			Debug.LogError("DynamicTerrain.RandomChunk(): tried to return null chunk!");
			return null;
		}

		return chunk;
	}

	public int NumActiveChunks {
		get {
			return activeChunks.Count;
		}
	}

	/// <summary>
	/// Registers a chunk as having a road on it.
	/// </summary>
	/// <param name="chunk">Chunk.</param>
	public void AddRoadChunk (Chunk chunk) {
		activeRoadChunks.Add(chunk);
	}

	/// <summary>
	/// Returns a random chunk that has a road on it.
	/// </summary>
	/// <returns>The road chunk.</returns>
	public Chunk RandomRoadChunk() {

		// Check if no active chunks with road
		if (activeRoadChunks.Count == 0) {
			Debug.LogError("DynamicTerrain.RandomRoadChunk(): no active road chunks!");
			return null;
		}

		// Pick a random road chunk
		Chunk chunk = activeRoadChunks[UnityEngine.Random.Range(0, activeRoadChunks.Count)];

		// Check if chunk is null
		if (chunk == null) {
			Debug.LogError("DynamicTerrain.RandomRoadChunk(): tried to return null chunk!");
			return null;
		}
			
		return chunk;
	}

	/// <summary>
	/// Registers a chunk as being close to a road.
	/// </summary>
	/// <param name="chunk">Chunk.</param>
	public void AddCloseToRoadChunk (Chunk chunk) {
		activeCloseToRoadChunks.Add (chunk);
	}

	/// <summary>
	/// Returns a random chunk within one chunk width of a road.
	/// </summary>
	/// <returns>The close to road chunk.</returns>
	public Chunk RandomCloseToRoadChunk() {

		// Check if no active chunks near a road
		if (activeCloseToRoadChunks.Count == 0) {
			Debug.LogError("DynamicTerrain.RandomCloseToRoadChunk(): no active close to road chunks!");
			return null;
		}

		// Pick a random chunk near the road.
		Chunk chunk = activeCloseToRoadChunks[UnityEngine.Random.Range(0, activeCloseToRoadChunks.Count)];

		// Check if chunk is null
		if (chunk == null) {
			Debug.LogError("DynamicTerrain.RandomCloseToRoadChunk(): tried to return null chunk!");
			return null;
		}

		return chunk;
	}

	/// <summary>
	/// Creates a randomized mountain.
	/// </summary>
	/// <param name="x">x coordinate of the center vertex.</param>
	/// <param name="y">y coordinate of the center vertex.</param>
	/// <param name="width">Width.</param>
	/// <param name="depth">Depth.</param>
	/// <param name="height">Height.</param>
	/// <param name="rough">Rough.</param>
	/// <param name="rangeMin">Range minimum.</param>
	/// <param name="rangeMax">Range max.</param>
	public void CreateMountain (int x, int y, int width, int depth, float height, float rough, 
		float rangeMin = -0.1f, float rangeMax = 1f) {

		//ensure width and depth are odd
		if (width % 2 == 0)
			width++;
		if (depth % 2 == 0)
			depth++;
		int size = Math.Max (width, depth);
		size--;
		size = MakePowerTwo (size); //size of the Diamond Square Alg array
		//Debug.Log("Size is: " + size);
		if (size < 2) return;
		float[,] heightmap = new float[size + 1, size + 1];
		float[] corners = InitializeCorners (vertexmap, x, y, width, depth);
		FillDiamondSquare (ref heightmap, corners, height, rough, rangeMin, rangeMax);

		//set vertices
		int minX = x - width/2;
		int maxX = x + width / 2;
		int minY = y - depth / 2;
		int maxY = y + depth / 2;
		int mapMax = (int)heightmap.GetLongLength (0) - 1;
		for (int i = minX; i <= maxX; i++) {
			float normalizedX = (float)(i - minX) / (float)(maxX - minX);
			int xFloor = Mathf.FloorToInt(normalizedX * (float)mapMax);
			int xCeil = Mathf.FloorToInt (normalizedX * (float)mapMax);
			float xT = normalizedX % 1f;
			for (int j = minY; j <= maxY; j++) {
				if (vertexmap.ContainsVertex(i, j)) {
					float normalizedY = (float)(j - minY) / (float)(maxY - minY);
					int yFloor = Mathf.FloorToInt (normalizedY * (float)mapMax);
					int yCeil = Mathf.FloorToInt (normalizedY * (float)mapMax);
					float yT = normalizedY % 1f;
					float p00 = GetFromHMap(heightmap, xFloor, yFloor);
					float p10 = GetFromHMap(heightmap, xCeil, yFloor);
					float p01 = GetFromHMap(heightmap, xFloor, yCeil);
					float p11 = GetFromHMap(heightmap, xCeil, yCeil);
					float interpH = ((1 - xT)*(1-yT))*p00 + ((xT)*(1-yT))*p10 + ((1-xT)*(yT))*p01 + ((xT)*(yT))*p11;
					if (!vertexmap.IsConstrained (i, j) && !vertexmap.IsLocked (i,j)) {
						vertexmap.SetHeight (i, j, interpH);
					}
				} else continue;
			}
		}
		foreach (Chunk chunk in activeChunks) chunk.UpdateCollider();
	}

	/// <summary>
	/// Raises n to the next power of two.
	/// </summary>
	/// <returns>The power two.</returns>
	/// <param name="n">N.</param>
	public int MakePowerTwo (int n) {
		if (n < 2) return -1; // if n is less than 2, return error value -1
		if ((n != 0) && ((n & (n-1)) == 0)) return n; //if n is already a power of 2, return n
		int r = 0; //counter of highest power of 2 in n
		//bit shift n to get place of leading bit, r, which is the log base 2 of n
		while ((n >>= 1) != 0) {
			r++;
			//Debug.Log ("r is: " + r);
		}
		r++; //raise power of two to next highest
		//Debug.Log("Final r is: " + r);
		return (int)Math.Pow (2, r);
	}

	// Returns the initial corners for the Diamond Square Algorithm
	public float[] InitializeCorners(VertexMap vmap, int x, int y, int width, int depth) {
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

	// Fills heightmap with DiamondSquare generated heights, 
	// using corners as the seeds, height as the initial center 
	// value, and rough as the height offset value
	void FillDiamondSquare (ref float[,] heightmap, float[] corners, float height, float rough, float rangeMin, float rangeMax){
		//set middle of hmap
		int max = (int)heightmap.GetLongLength(0) - 1;
		heightmap [max/2 + 1, max/2 + 1] = height; //set middle height
		Divide(ref heightmap, max, rough, rangeMin, rangeMax);
	}

	void Square(ref float[,] heightmap, int x, int y, int size, float offset) {
		float ave = Average (new float[] {
			GetFromHMap(heightmap, x - size, y - size), //lower left
		 	GetFromHMap(heightmap, x + size, y - size), //lower right
			GetFromHMap(heightmap, x + size, y + size), //upper right
			GetFromHMap(heightmap, x - size, y + size) //upper left
		});
		heightmap [x, y] = ave + offset;
	}

	void Diamond(ref float[,] heightmap, int x, int y, int size, float offset) {
		float ave = Average (new float[] {
			GetFromHMap(heightmap, x, y - size), //bottom
			GetFromHMap(heightmap, x + size, y), //right
			GetFromHMap(heightmap, x, y + size), //top
			GetFromHMap(heightmap, x - size, y) //left
		});
		heightmap [x, y] = ave + offset;
	}

	void Divide(ref float[,] heightmap, int size, float rough, float rangeMin, float rangeMax) {
		int x, y, half = size / 2;
		float scale = size * rough;
		if (half < 1) //past the minimum size
			return;

		//do squares
		for (y = half; y < heightmap.GetLongLength(1) - 1; y += size) {
			for (x = half; x < heightmap.GetLongLength(0) - 1; x += size) {
				if (size == heightmap.GetLongLength(0) - 1) { //ignore setting the very center of the mountain
					continue;
				}else {
					Square (ref heightmap, x, y, half, UnityEngine.Random.Range (rangeMin, rangeMax) * scale);
				}
			}
		}

		//do diamonds
		for (y = 0; y <= heightmap.GetLongLength (1) - 1; y += half) {
			for (x = (y + half) % size; x <= heightmap.GetLength (0) - 1; x += size) {
				Diamond (ref heightmap, x, y, half, UnityEngine.Random.Range (rangeMin, rangeMax) * scale);
			}
		}
		//recursive call
		Divide (ref heightmap, half, rough, rangeMin, rangeMax);
	}

	//accesses heightmap and returns -INF for out of bounds vertices
	float GetFromHMap (float [,] heightmap, int x, int y) {
		if (x < 0 || x >= heightmap.GetLength (0) || y < 0 || y >= heightmap.GetLength (1)) {
			return float.NegativeInfinity;
		}
		return heightmap [x, y];
	}

	//returns the average of 4 corners, excluding non-legal values
	float Average(float[] corners) {
		float count = 4f;
		float sum = 0f;
		foreach (float corner in corners) {
			if (float.IsNegativeInfinity (corner)) {
				count -= 1f;
				continue;
			}
			sum += corner;
		}
		return (sum/count);
	}
}

#endregion
