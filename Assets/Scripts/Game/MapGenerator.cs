using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ChunkKey = UnityEngine.Vector2Int;
using ChunkKeyData = System.Collections.Generic.KeyValuePair<UnityEngine.Vector2Int, ChunkData>;

public class MapGenerator : MonoBehaviour
{
	static MapGenerator instance = null;
	public static TextureData TextureData { get => instance.textureData; }

	[Header("Parameters")]
	[SerializeField] int seed = 0;
	[SerializeField] int octaves = 4;
	[SerializeField] float lacunarity = 2.0f;
	[SerializeField] [Range(0.0f, 1.0f)] float persistance = 0.5f;
	[SerializeField] Vector2 scale = new Vector2(20, 40);

	[Header("Game")]
	[SerializeField] int nbChunk = 10;
	[SerializeField] GameObject chunkPrefab = null;
	[SerializeField] TextureData textureData = null;

	HeightMap heightMap = null;

	Block CreateBlock(Vector3 blockPosition, BlockType blockType)
	{
		Block block = new Block();
		block.Position = blockPosition;
		block.Type = blockType;

		return block;
	}
	ChunkData CreateChunkData(ChunkKey chunkKey)
	{
		ChunkData chunkData = new ChunkData();

		Vector3 chunkWorldPosition = new Vector3(chunkKey.x, 0.0f, chunkKey.y) * Chunk.ChunkSize;
		chunkData.Position = chunkWorldPosition;

		//Create blocks
		for (int k = 0; k < Chunk.ChunkHeight; k++)
			for (int j = 0; j < Chunk.ChunkSize; j++)
				for (int i = 0; i < Chunk.ChunkSize; i++)
				{
					var blockPosition = new Vector3(i - Chunk.ChunkRadius, k, j - Chunk.ChunkRadius);
					var groundHeight = heightMap.GetHeight(chunkWorldPosition.x + blockPosition.x , chunkWorldPosition.z + blockPosition.z) * Chunk.ChunkHeight;
					var blockType = (blockPosition.y > groundHeight) ? BlockType.Air : BlockType.Grass; // if block is below ground Height, create a block.

					chunkData.Blocks[i, j, k] = CreateBlock(blockPosition, blockType);
				}

		chunkData.CalculateMeshData();

		return chunkData;
	}
	void CreateChunk(ChunkData chunkData)
	{
		var chunk = Instantiate(chunkPrefab).GetComponent<Chunk>();
		chunk.transform.position = chunkData.Position;
		chunk.ChunkData = chunkData;
		chunkData.Parent = chunk;
	}

	#region Threading Chunk Loading
	readonly object loadingLock = new object();
	Dictionary<ChunkKey, ChunkData> loadedChunkDic = new Dictionary<ChunkKey, ChunkData>();
	Queue<ChunkKeyData> loadingChunkQueue = new Queue<ChunkKeyData>();
	Queue<ChunkKey> unloadingChunkQueue = new Queue<ChunkKey>();

	void StartLoadingChunkThread()
	{
		var playerKeyPosition = GetKeyFromWorldPosition(PlayerController.Position);
		var keysToCheck = MathfPlus.GetAllPointInRadius(nbChunk);

		new Thread(() => LoadingChunkThread(keysToCheck, playerKeyPosition)).Start();
	}
	void LoadingChunkThread(ChunkKey[] keys, ChunkKey playerKey)
	{
		lock (loadingLock)
		{
			foreach (var key in keys)
			{
				var chunkKey = playerKey + key;
				if (!ChunkIsLoaded(chunkKey) && !ChunkIsLoading(chunkKey))
				{
					var chunkData = CreateChunkData(chunkKey);
					loadingChunkQueue.Enqueue(new ChunkKeyData(chunkKey, chunkData));
				}
			}

			foreach (var loadedChunk in loadedChunkDic)
			{
				if ((loadedChunk.Key - playerKey).magnitude > nbChunk && !ChunkIsUnloading(loadedChunk.Key))
				{
					unloadingChunkQueue.Enqueue(loadedChunk.Key);
				}
			}
		}
	}
	void ProcessLoadChunk()
	{
		if (Monitor.TryEnter(loadingLock))
		{
			for (int i = 0; i < unloadingChunkQueue.Count; i++)
			{
				var keyToRemove = unloadingChunkQueue.Dequeue();
				Destroy(loadedChunkDic[keyToRemove].Parent.gameObject);
				loadedChunkDic.Remove(keyToRemove);
			}

			for (int i = 0; i < loadingChunkQueue.Count; i++)
			{
				var chunkKeyData = loadingChunkQueue.Dequeue();
				CreateChunk(chunkKeyData.Value);
				loadedChunkDic.Add(chunkKeyData.Key, chunkKeyData.Value);
			}

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

		heightMap = new HeightMap(seed, octaves, lacunarity, persistance, scale);
	}
	private void Start()
	{
		StartLoadingChunkThread();
	}
	private void Update()
	{
		//OnChunkEnter
		if (GetKeyFromWorldPosition(PlayerController.Position) != GetKeyFromWorldPosition(PlayerController.PreviousPosition))
		{
			StartLoadingChunkThread();
		}

		ProcessLoadChunk();
	}
}
