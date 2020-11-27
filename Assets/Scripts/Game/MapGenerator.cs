using System.Linq;
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

	ChunkData CreateChunkData(ChunkKey chunkKey)
	{
		Vector3 chunkWorldPosition = new Vector3(chunkKey.x, 0.0f, chunkKey.y) * Chunk.ChunkSize;

		ChunkData chunkData = new ChunkData();
		chunkData.position = chunkWorldPosition;

		for (int k = 0; k < Chunk.ChunkHeight; k++)
			for (int j = 0; j < Chunk.ChunkSize; j++)
				for (int i = 0; i < Chunk.ChunkSize; i++)
				{
					chunkData.blocks[i, j, k] = new Block();
					var blockPosition = new Vector3(i - Chunk.ChunkRadius, k, j - Chunk.ChunkRadius);
					chunkData.blocks[i, j, k].position = blockPosition;
					chunkData.blocks[i, j, k].type = BlockType.Air;

					var height = heightMap.GetHeight(chunkWorldPosition.x + blockPosition.x , chunkWorldPosition.z + blockPosition.z);
					if (!(k > height * Chunk.ChunkHeight))
						chunkData.blocks[i, j, k].type = BlockType.Grass;
				}

		chunkData.CalculateMeshData();

		return chunkData;
	}
	void CreateChunk(ChunkData chunkData)
	{
		var chunk = Instantiate(chunkPrefab).GetComponent<Chunk>();
		chunk.transform.position = chunkData.position;
		chunk.ChunkData = chunkData;
		chunkData.parent = chunk;
	}

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
				Destroy(loadedChunkDic[keyToRemove].parent.gameObject);
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
