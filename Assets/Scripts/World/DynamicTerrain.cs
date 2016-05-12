using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DynamicTerrain {
	public static DynamicTerrain instance;

	#region DynamicTerrain Vars

	private GameObject terrain;
	public List<Chunk> activeChunks; //list of active chunks
	public List<Chunk> activeRoadChunks; // list of active chunks with road on them
	public List<Chunk> activeCloseToRoadChunks;

	public VertexMap vertexmap;
	public Map<Chunk> chunkmap;

	public LinInt freqData;
	public int freqSampleSize = 128;
	public FFTWindow fftWindow = FFTWindow.Rectangular;

	//private Dictionary<Chunk, int> chunkPriorities;
	List<Chunk> chunksToUpdate;

	ObjectPool chunkPool;
	float chunkSize;
	int chunkRes;
	int chunkLoadRadius;

	bool initialLoad = true;

	Vector3 playerPos;
	IntVector2 playerChunkPos;

	#endregion
	#region DynamicTerrain Methods

	public DynamicTerrain () {
		instance = this;
		terrain = new GameObject ("Terrain");
		terrain.transform.position = new Vector3 (0f, 0f, 0f);

		chunkSize = WorldManager.instance.chunkSize;
		chunkLoadRadius = WorldManager.instance.chunkLoadRadius;

		vertexmap = new VertexMap();
		vertexmap.terrain = this;
		chunkmap = new Map<Chunk>(chunkLoadRadius*2);

		activeChunks = new List<Chunk>();
		activeRoadChunks = new List<Chunk>();
		activeCloseToRoadChunks = new List<Chunk>();

		chunkPool = new ObjectPool();

		playerChunkPos = new IntVector2(0, 0);
	}

	GameObject CreateChunk(int x, int y){
		GameObject newChunk;
		Chunk chunk;
		if (chunkPool.Empty) {
			newChunk = new GameObject ("", 
				typeof(MeshFilter), 
				typeof(MeshRenderer),
				typeof(MeshCollider),
				typeof(Rigidbody),
				typeof(Chunk)
			);
			newChunk.transform.parent = terrain.transform;
			chunk = newChunk.GetComponent<Chunk>();
			chunk.Initialize(x, y);
		} else {
			newChunk = chunkPool.Get();
			newChunk.transform.parent = terrain.transform;
			chunk = newChunk.GetComponent<Chunk>();
			chunk.Reuse(x, y);
		}

		//chunkPriorities.Add(newChunk, 0);
		return newChunk;
	}

	public void DoLoadChunks () {
		// Init chunk pool

		WorldManager.instance.StartCoroutine(UpdateChunks());
	}

	IEnumerator UpdateChunks () {

		int chunksToLoad = 0;
		float startTime = Time.realtimeSinceStartup;
		int numLoaded = 0;
		chunksToUpdate = new List<Chunk>();

		if (initialLoad) GameManager.instance.ChangeLoadingMessage("Loading chunks...");

		while (true) {

			if (!initialLoad) UpdateFreqData ();

			//List<int> xChunks = new List<int> (); //x coords of chunks to be loaded
			//List<int> yChunks = new List<int> (); //y coords of chunks to be loaded
			//CreateChunkLists (xChunks, yChunks);
			//if (initialLoad) chunksToLoad = xChunks.Count * yChunks.Count;
			if (initialLoad) chunksToLoad = (chunkLoadRadius*2+1) * (chunkLoadRadius*2+1);

			if (!initialLoad) DeleteChunks();

			//foreach (int x in xChunks) {
			//	foreach (int y in yChunks) {
			playerPos = PlayerMovement.instance.transform.position;
			playerChunkPos.x = (int)Mathf.RoundToInt((playerPos.x - chunkSize/2f) / chunkSize);
			playerChunkPos.y = (int)Mathf.RoundToInt((playerPos.z -chunkSize/2f) / chunkSize);
			for (int x=playerChunkPos.x - chunkLoadRadius; x<=playerChunkPos.x + chunkLoadRadius; x++) {
				for (int y=playerChunkPos.y - chunkLoadRadius; y<=playerChunkPos.y + chunkLoadRadius; y++) {
					if (chunkmap.At(x,y) == null) {
						Chunk chunk = CreateChunk (x, y).GetComponent<Chunk>();
						activeChunks.Add (chunk);
						chunkmap.Set(x, y, chunk);

						if (initialLoad) numLoaded++;

						if (Time.realtimeSinceStartup - startTime > 1f/Application.targetFrameRate) {
							yield return null;
							startTime = Time.realtimeSinceStartup;
							if (initialLoad) {
								GameManager.instance.ReportLoaded(numLoaded);
								numLoaded = 0;
							}
						}
					}
				}
			}

			if (Time.realtimeSinceStartup - startTime > 1f/Application.targetFrameRate) {
				yield return null;
				startTime = Time.realtimeSinceStartup;
			}

			if (initialLoad && activeChunks.Count == chunksToLoad) {
				foreach (Chunk chunk in activeChunks) chunk.UpdateCollider();
				int res = vertexmap.vertices.Width;
				CreateMountain (0, 0, res, res, 10f, 20f, -0.03f, 0.03f);
				initialLoad = false;
				WorldManager.instance.DoLoadRoad();

			} else {
				//if (PlayerMovement.instance.moving) {
				chunksToUpdate.Clear ();
				foreach (Chunk chunk in activeChunks) {
					if (chunk.needsColliderUpdate) chunk.UpdateCollider();
					if (chunk.needsColorUpdate) chunk.UpdateColors();
					if (DistanceToPlayer (chunk) <= WorldManager.instance.vertexUpdateDistance) {
						chunk.priority += ChunkHeuristic (chunk) +1;
						if (chunksToUpdate.Count == 0)
							chunksToUpdate.Add (chunk);
						else {
							for (int i = 0; i < chunksToUpdate.Count; i++) {
								if (chunk.priority > chunksToUpdate[i].priority) {
									chunksToUpdate.Insert (i, chunk);
									break;
								}
							}
						}
					}
				}

				int listLength = chunksToUpdate.Count;
					
				if (listLength > 0) {
					for (int i=0; i < WorldManager.instance.chunkUpdatesPerCycle && i < activeChunks.Count && i < listLength; i++) {
						try {
							chunksToUpdate [i].ChunkUpdate ();
							chunksToUpdate[i].priority = 0;
						}catch (ArgumentOutOfRangeException a) {
							Debug.LogError ("Index: "+i+" Count: "+chunksToUpdate.Count+" "+a.Message);
							continue;
						}
					}
				}
					

				if (Time.realtimeSinceStartup - startTime > 1f/Application.targetFrameRate) {
					yield return null;
					startTime = Time.realtimeSinceStartup;
				}
			}
			yield return null;
		}
	}


	public int ChunkHeuristic (Chunk chunk) {
		return 
			(int)DistanceToPlayer(chunk) + 
			//(chunk.nearbyRoad() ? 250 : 0) +
			(InView(chunk) ? 20000 : 0);
	}

	float DistanceToPlayer (Chunk chunk) {
		return Vector3.Distance (
			new Vector3 (chunk.x*chunkSize +chunkSize/2, 0f, chunk.y*chunkSize+chunkSize/2),
			new Vector3 (playerPos.x, 0f, playerPos.z)
		);
	}

	bool InView (Chunk chunk) {
		bool result = Vector3.Angle(
			(chunk.gameObject.transform.position-Camera.main.gameObject.transform.position), 
			Camera.main.transform.forward
		) < Camera.main.fieldOfView/2f;
		return result;

	}
		
	void UpdateFreqData () {
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
		if (freqData == null) freqData = new LinInt();
		freqData.Update(data);
	}
		
	public void TranslateWorld (Vector3 offset){
		terrain.transform.position += offset;
	}

	public void TranslateLocal (Vector3 offset){
		terrain.transform.localPosition += offset;
	}

	// Gives a random chunk (for decoration testing)
	public Chunk RandomChunk () {
		if (activeChunks.Count == 0) {
			Debug.LogError("DynamicTerrain.RandomChunk(): no active chunks!");
			return null;
		}

		Chunk chunk = activeChunks[UnityEngine.Random.Range(0, activeChunks.Count)];

		if (chunk == null) {
			Debug.LogError("DynamicTerrain.RandomChunk(): tried to return null chunk!");
			return null;
		}

		return chunk;
	}

	// Returns a random chunk with road on it
	public Chunk RandomRoadChunk() {
		if (activeRoadChunks.Count == 0) {
			Debug.LogError("DynamicTerrain.RandomRoadChunk(): no active road chunks!");
			return null;
		}

		Chunk chunk = activeRoadChunks[UnityEngine.Random.Range(0, activeRoadChunks.Count)];

		if (chunk == null) {
			Debug.LogError("DynamicTerrain.RandomRoadChunk(): tried to return null chunk!");
			return null;
		}
			
		return chunk;
	}

	public Chunk RandomCloseToRoadChunk() {
		if (activeCloseToRoadChunks.Count == 0) {
			Debug.LogError("DynamicTerrain.RandomCloseToRoadChunk(): no active close to road chunks!");
			return null;
		}

		Chunk chunk = activeCloseToRoadChunks[UnityEngine.Random.Range(0, activeCloseToRoadChunks.Count)];

		if (chunk == null) {
			Debug.LogError("DynamicTerrain.RandomCloseToRoadChunk(): tried to return null chunk!");
			return null;
		}

		return chunk;
	}

	public void RegisterChunk (Chunk chunk) {
		if (chunk.nearRoad) {
			if (chunk.hasRoad) activeRoadChunks.Add(chunk);
			activeCloseToRoadChunks.Add(chunk);
		}
	}

	public void CheckAllChunksForRoad() {
		foreach (Chunk chunk in activeChunks)
			chunk.CheckForRoad(PlayerMovement.instance.moving ? PlayerMovement.instance.progress : 0f);
	}

	public void OnExtendRoad () {
		foreach (Chunk chunk in activeChunks) {
			if (!chunk.nearRoad) chunk.hasCheckedForRoad = false;
		}
	}

	//populates lists of chunk coords to be loaded
	void CreateChunkLists(List<int> xChunks, List<int> yChunks){
		playerPos = PlayerMovement.instance.transform.position;
		int playerChunkX = (int)Math.Floor((playerPos.x - this.terrain.transform.position.x) / chunkSize);
		int playerChunkY = (int)Math.Floor((playerPos.z - this.terrain.transform.position.z) / chunkSize);
		if (UnityEngine.Random.Range(0,100) == 0) Debug.Log("player at "+ playerChunkX + " "+playerChunkY);
		xChunks.Add (playerChunkX);
		yChunks.Add (playerChunkY);
		for (int i = 1; i <= chunkLoadRadius; i++){
			//get player coords in chunk coords
			xChunks.Add (playerChunkX + i);
			xChunks.Add (playerChunkX - i);
			yChunks.Add (playerChunkY + i);
			yChunks.Add (playerChunkY - i);
		}
	}

	void DeleteChunk(Chunk chunk){

		// Remove all decorations on chunk
		chunk.RemoveDecorations();

		// Deregister from lists/map
		DeregisterChunk(chunk);

		// Destroy chunk
		chunkPool.Add(chunk.gameObject);

	}

	void DeleteChunks () {
		List<Chunk> deletions = new List<Chunk>();

		foreach (Chunk chunk in activeChunks) {
			if (chunk.x < playerChunkPos.x - chunkLoadRadius || chunk.x > playerChunkPos.x + chunkLoadRadius ||
				chunk.y < playerChunkPos.y - chunkLoadRadius || chunk.y > playerChunkPos.y + chunkLoadRadius)
					deletions.Add(chunk);
		}
		foreach (Chunk chunk in deletions) DeleteChunk (chunk);
	}

	public void DeregisterChunk (Chunk chunk) {
		activeChunks.Remove(chunk);
		if (chunk.nearRoad) {
			activeCloseToRoadChunks.Remove(chunk);
			if (chunk.hasRoad) activeRoadChunks.Remove(chunk);
		}
		chunk.transform.parent = null;
		chunkmap.Set(chunk.x, chunk.y, null);
	}

	public void Update(float[] freqDataArray){
		//UpdateChunks (freqDataArray);
	}

	float AngularDistance (float angle, float pos) {
		float d = angle - pos;
		while (d < -Mathf.PI)
			d += 2f * Mathf.PI;
		while (d > Mathf.PI)
			d -= 2f * Mathf.PI;
		return 1.0f-Mathf.Abs(5f*d/Mathf.PI/2.0f);
	}

	//create a width by depth mountain centered at vertex (x,y) with a maximum altitude of height, a jaggedness of rough, and a min and max percentage of the random scale 
	public void CreateMountain (int x, int y, int width, int depth, float height, float rough, 
		float rangeMin = -0.1f, float rangeMax = 1f) {
		//ensure width and depth are odd
		if (width % 2 == 0)
			width++;
		if (depth % 2 == 0)
			depth++;
		VertexMap vmap = DynamicTerrain.instance.vertexmap;
		int size = Math.Max (width, depth);
		size--;
		size = MakePowerTwo (size); //size of the Diamond Square Alg array
		//Debug.Log("Size is: " + size);
		if (size < 2) return;
		float[,] heightmap = new float[size + 1, size + 1];
		float[] corners = InitializeCorners (vmap, x, y, width, depth);
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
				if (vmap.ContainsVertex(i, j)) {
					float normalizedY = (float)(j - minY) / (float)(maxY - minY);
					int yFloor = Mathf.FloorToInt (normalizedY * (float)mapMax);
					int yCeil = Mathf.FloorToInt (normalizedY * (float)mapMax);
					float yT = normalizedY % 1f;
					float p00 = GetFromHMap(heightmap, xFloor, yFloor);
					float p10 = GetFromHMap(heightmap, xCeil, yFloor);
					float p01 = GetFromHMap(heightmap, xFloor, yCeil);
					float p11 = GetFromHMap(heightmap, xCeil, yCeil);
					float interpH = ((1 - xT)*(1-yT))*p00 + ((xT)*(1-yT))*p10 + ((1-xT)*(yT))*p01 + ((xT)*(yT))*p11;
					if (!vmap.IsConstrained (i, j) && !vmap.IsLocked (i,j))
						vmap.SetHeight (i, j, interpH);
				} else continue;
			}
		}
		foreach (Chunk chunk in activeChunks) chunk.UpdateCollider();

		//Road.instance.DoBulldoze(0f);
	}

	//raises n to the nearest power of 2
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
