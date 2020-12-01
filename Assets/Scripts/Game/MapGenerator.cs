using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using ChunkKey = UnityEngine.Vector2Int;
using ChunkKeyData = System.Collections.Generic.KeyValuePair<UnityEngine.Vector2Int, ChunkData>;

public class MapGenerator : MonoBehaviour
{
	static MapGenerator instance = null;
	public static TextureData TextureData { get => instance.textureData; }

	public int NbChunk { get => nbChunk; }

	// Ground HeightMap Parameters
	[SerializeField] int seed = 0;
	[SerializeField] int octaves = 4;
	[SerializeField] float lacunarity = 2.0f;
	[SerializeField] [Range(0.0f, 1.0f)] float persistance = 0.5f;
	[SerializeField] Vector2 scale = new Vector2(20, 40);

	[SerializeField] int groundHeightMax = 15; 

	// Tree HeightMap Parameters
	[SerializeField] int seedTree = 0;
	[SerializeField] int octavesTree = 4;
	[SerializeField] float lacunarityTree = 2.0f;
	[SerializeField] [Range(0.0f, 1.0f)] float persistanceTree = 0.5f;
	[SerializeField] Vector2 scaleTree = new Vector2(20, 40);

	[SerializeField] [Range(0.0f, 1.0f)] float treeProbability = 0.0f;

	// Games Parameters
	[SerializeField] int nbChunk = 10;
	[SerializeField] GameObject chunkPrefab = null;
	[SerializeField] TextureData textureData = null;

	//Debug Parameters
	[SerializeField] Text debugText = null;
	float globalTimeGeneration = 0.0f;
	float globalMeshDataTimeGeneration = 0.0f;
	float blockDataTimeGeneration = 0.0f;

	readonly object logLock = new object();
	Queue<string> logQueue = new Queue<string>();
	void DebugLog(string log)
	{
		lock (logLock)
		{
			logQueue.Enqueue(log);
		}
	}
	void ProcessLog()
	{
		if (Monitor.TryEnter(logLock))
		{
			for (int i = 0; i < logQueue.Count; i++)
			{
				Debug.Log(logQueue.Dequeue());
			}

			Monitor.Exit(logLock);
		}
	}

	public HeightMap HeightMap { get; private set; } = null;
	public HeightMap TreeHeightMap { get; private set; } = null;

	public ChunkData GenerateChunkData(ChunkKey chunkKey)
	{
		ChunkData chunkData = new ChunkData();
		chunkData.Position = new Vector3(chunkKey.x, 0.0f, chunkKey.y) * Chunk.ChunkSize;

		for (int j = 0; j < Chunk.ChunkSize; j++)
			for (int i = 0; i < Chunk.ChunkSize; i++)
			{
				bool checkTree = true;
				for (int k = 0; k < Chunk.ChunkHeight; k++)
				{
					var blockPosition = new Vector3(i - Chunk.ChunkRadius, k, j - Chunk.ChunkRadius);
					var height = HeightMap.GetHeight(chunkData.Position.x + blockPosition.x, chunkData.Position.z + blockPosition.z);
					var groundHeight = height * groundHeightMax;
					var blockType = (blockPosition.y > groundHeight) ? BlockType.Air : BlockType.Grass; // if block is below ground Height, create a block.

					if (chunkData.Blocks[i, j, k].Type == BlockType.None) chunkData.SetBlock(i, j, k, blockType, blockPosition);

					//Check block type if is the first Ground Layer
					if (checkTree && blockType == BlockType.Air)
					{
						checkTree = false;

						CreateTree(chunkData, i, j, k, blockPosition);
					}
				}
			}

		return chunkData;
	}

	void CreateTree(ChunkData chunkData, int i, int j, int k, Vector3 blockPosition)
	{
		var treeHeight = TreeHeightMap.GetHeight(chunkData.Position.x + blockPosition.x, chunkData.Position.z + blockPosition.z);
		if (treeHeight > treeProbability)
		{
			// Create Leaves
			for (int kk = 4; kk < 8; kk++)
				for (int jj = -2; jj < 3; jj++)
					for (int ii = -2; ii < 3; ii++)
						if ((Mathf.Abs(ii) + Mathf.Abs(kk - 5) + Mathf.Abs(jj) < 5))
							chunkData.SetBlock(i + ii, j + jj, k + kk, BlockType.Leaves, blockPosition + new Vector3(ii, kk, jj));

			// Create Trunk
			chunkData.SetBlock(i, j, k, BlockType.Wood, blockPosition);
			chunkData.SetBlock(i, j, k + 1, BlockType.Wood, blockPosition + new Vector3(0.0f, 1.0f, 0.0f));
			chunkData.SetBlock(i, j, k + 2, BlockType.Wood, blockPosition + new Vector3(0.0f, 2.0f, 0.0f));
			chunkData.SetBlock(i, j, k + 3, BlockType.Wood, blockPosition + new Vector3(0.0f, 3.0f, 0.0f));
			chunkData.SetBlock(i, j, k + 4, BlockType.Wood, blockPosition + new Vector3(0.0f, 4.0f, 0.0f));
			chunkData.SetBlock(i, j, k + 5, BlockType.Wood, blockPosition + new Vector3(0.0f, 5.0f, 0.0f));
		}
	}
	ChunkData CreateChunkData(ChunkKey chunkKey)
	{
		ChunkData chunkData = new ChunkData();
		
		Vector3 chunkWorldPosition = new Vector3(chunkKey.x, 0.0f, chunkKey.y) * Chunk.ChunkSize;
		chunkData.Position = chunkWorldPosition;

		//Create blocks
		for (int j = 0; j < Chunk.ChunkSize; j++)
			for (int i = 0; i < Chunk.ChunkSize; i++)
			{
				bool checkTree = true;
				for (int k = 0; k < Chunk.ChunkHeight; k++)
				{
					var blockPosition = new Vector3(i - Chunk.ChunkRadius, k, j - Chunk.ChunkRadius);
					var height = HeightMap.GetHeight(chunkWorldPosition.x + blockPosition.x, chunkWorldPosition.z + blockPosition.z);
					var groundHeight = height * groundHeightMax;
					var blockType = (blockPosition.y > groundHeight) ? BlockType.Air : BlockType.Grass; // if block is below ground Height, create a block.

					if (chunkData.Blocks[i, j, k].Type == BlockType.None) chunkData.SetBlock(i, j, k, blockType, blockPosition);

					//Check block type if is the first Ground Layer
					if (checkTree && blockType == BlockType.Air)
					{
						checkTree = false;

						CreateTree(chunkData, i, j, k, blockPosition);
					}
				}
			}

		var startMeshDataTimeGeneration = DateTime.Now;
		chunkData.CalculateMeshData();
		globalMeshDataTimeGeneration += (float)DateTime.Now.Subtract(startMeshDataTimeGeneration).TotalSeconds;

		return chunkData;
	}
	void CreateChunk(ChunkData chunkData)
	{
		var chunk = Instantiate(chunkPrefab).GetComponent<Chunk>();
		chunk.transform.position = chunkData.Position;
		chunk.ChunkData = chunkData;
		chunkData.ChunkParent = chunk;
	}

	volatile bool updateDebugText = false;
	void UpdateDebugText()
	{
		debugText.text = 
			"Chunk Generation\n" +
			"	Global Time : " + ((int)(globalTimeGeneration * 100)) / 100.0f + "s\n" +
			"	Global MeshData : " + ((int)(globalMeshDataTimeGeneration * 100)) / 100.0f + "s\n";
	}

	#region Threading Chunk Loading
	readonly object loadingLock = new object();
	Dictionary<ChunkKey, ChunkData> loadedChunkDic = new Dictionary<ChunkKey, ChunkData>();
	Queue<ChunkKeyData> loadingChunkQueue = new Queue<ChunkKeyData>();
	Queue<ChunkKey> unloadingChunkQueue = new Queue<ChunkKey>();
	volatile bool forceUpdate = false;

	void StartLoadingChunkThread()
	{
		var playerKeyPosition = GetKeyFromWorldPosition(PlayerController.Position);
		var keysToCheck = MathfPlus.GetAllPointInRadius(nbChunk);

		new Thread(() => LoadingChunkThread(keysToCheck, playerKeyPosition)).Start();
	}
	void LoadingChunkThread(ChunkKey[] keys, ChunkKey playerKey)
	{
		while (forceUpdate) ;

		lock (loadingLock)
		{
			globalMeshDataTimeGeneration = 0.0f;
			var startGlobalTimeGeneration = DateTime.Now;
			foreach (var key in keys)
			{
				var chunkKey = playerKey + key;
				if (!ChunkIsLoaded(chunkKey) && !ChunkIsLoading(chunkKey))
				{
					var chunkData = CreateChunkData(chunkKey);
					loadingChunkQueue.Enqueue(new ChunkKeyData(chunkKey, chunkData));
				}
			}
			globalTimeGeneration = (float)DateTime.Now.Subtract(startGlobalTimeGeneration).TotalSeconds;

			updateDebugText = true;

			foreach (var loadedChunk in loadedChunkDic)
			{
				if ((loadedChunk.Key - playerKey).magnitude > nbChunk && !ChunkIsUnloading(loadedChunk.Key))
				{
					unloadingChunkQueue.Enqueue(loadedChunk.Key);
				}
			}
		}

		forceUpdate = true;
	}
	void ProcessLoadChunk()
	{
		if (Monitor.TryEnter(loadingLock))
		{
			var count = unloadingChunkQueue.Count;
			for (int i = 0; i < count; i++)
			{
				var keyToRemove = unloadingChunkQueue.Dequeue();
				Destroy(loadedChunkDic[keyToRemove].ChunkParent.gameObject);
				loadedChunkDic.Remove(keyToRemove);
			}

			count = loadingChunkQueue.Count;
			for (int i = 0; i < count; i++)
			{
				var chunkKeyData = loadingChunkQueue.Dequeue();
				CreateChunk(chunkKeyData.Value);
				loadedChunkDic.Add(chunkKeyData.Key, chunkKeyData.Value);
			}

			forceUpdate = false;

			Monitor.Exit(loadingLock);
		}
	}

	ChunkKey GetKeyFromWorldPosition(Vector3 position)
	{
		return new Vector2Int(Mathf.RoundToInt(position.x / Chunk.ChunkSize), Mathf.RoundToInt(position.z / Chunk.ChunkSize));
	}
	bool ChunkIsLoaded(ChunkKey key)
	{
		return loadedChunkDic.ContainsKey(key);
	}
	bool ChunkIsLoading(ChunkKey key)
	{
		foreach (var chunkKeyData in loadingChunkQueue)
		{
			if (chunkKeyData.Key == key)
			{
				return true;
			}
		}

		return false;
	}
	bool ChunkIsUnloading(ChunkKey key)
	{
		return unloadingChunkQueue.Contains(key);
	}
	#endregion

	private void Awake()
	{
		instance = this;

		HeightMap = new HeightMap(seed, octaves, lacunarity, persistance, scale);
		TreeHeightMap = new HeightMap(seedTree, octavesTree, lacunarityTree, persistanceTree, scaleTree);
	}
	private void Start()
	{
		//StartLoadingChunkThread();
	}
	private void Update()
	{
		////OnChunkEnter
		//if (GetKeyFromWorldPosition(PlayerController.Position) != GetKeyFromWorldPosition(PlayerController.PreviousPosition))
		//{
		//	StartLoadingChunkThread();
		//}

		//if (updateDebugText)
		//{
		//	UpdateDebugText();
		//	updateDebugText = false;
		//}

		//ProcessLoadChunk();
		//
		//ProcessLog();
	}
}
