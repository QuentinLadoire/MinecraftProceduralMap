using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapGenerator : MonoBehaviour
{
	[Header("Parameters")]
	[SerializeField] int seed = 0;
	[SerializeField] int octaves = 4;
	[SerializeField] float lacunarity = 2.0f;
	[SerializeField] [Range(0.0f, 1.0f)] float persistance = 0.5f;
	[SerializeField] Vector2 scale = new Vector2(20, 40);

	[Header("Game")]
	[SerializeField] int nbChunk = 10;
	[SerializeField] GameObject chunkPrefab = null;

	HeightMap heightMap = null;

	Dictionary<Vector2Int, ChunkData> loadedChunkDatas = new Dictionary<Vector2Int, ChunkData>();

	ChunkData CreateChunkData(Vector2 chunkPosition)
	{
		ChunkData chunkData = new ChunkData();
		chunkData.position = new Vector3(chunkPosition.x, 0.0f, chunkPosition.y);

		for (int k = 0; k < Chunk.ChunkHeight; k++)
		{
			for (int j = 0; j < Chunk.ChunkSize; j++)
			{
				for (int i = 0; i < Chunk.ChunkSize; i++)
				{
					chunkData.cubeTypes[i, j, k] = CubeType.Air;

					if (!(k > heightMap.GetHeight(chunkPosition + new Vector2(i - Chunk.ChunkSize / 2, j - Chunk.ChunkSize / 2)) * Chunk.ChunkHeight))
					{
						chunkData.cubeTypes[i, j, k] = CubeType.Solid;
					}
				}
			}
		}
		chunkData.CalculateMatrices();

		return chunkData;
	}
	void CreateChunk(ChunkData chunkData)
	{
		var chunk = Instantiate(chunkPrefab).GetComponent<Chunk>();
		chunk.transform.position = chunkData.position;
		chunk.ChunkData = chunkData;
		chunkData.parent = chunk;
	}

	void CheckChunk(Vector2Int playerPosition)
	{
		var keys = MathfPlus.GetAllPointInRadius(nbChunk);
		foreach (var key in keys)
		{
			var tmp = playerPosition + key;
			if (!loadedChunkDatas.ContainsKey(tmp))
			{
				var chunkData = CreateChunkData(tmp * Chunk.ChunkSize);
				CreateChunk(chunkData);
				loadedChunkDatas.Add(tmp, chunkData);
			}
		}

		List<Vector2Int> keysToRemove = new List<Vector2Int>();
		foreach (var loadedChunk in loadedChunkDatas)
		{
			if ((loadedChunk.Key - playerPosition).magnitude > nbChunk)
			{
				keysToRemove.Add(loadedChunk.Key);
			}
		}

		keysToRemove.ForEach(item => {
			Destroy(loadedChunkDatas[item].parent);
			loadedChunkDatas.Remove(item);
		});
	}

	private void Awake()
	{
		heightMap = new HeightMap(seed, octaves, lacunarity, persistance, scale);
	}
	private void Start()
	{
		var playerPosition = (PlayerController.Position / Chunk.ChunkSize).ToVector3Int();
		CheckChunk(new Vector2Int(playerPosition.x, playerPosition.z));
	}
	private void Update()
	{
		if ((PlayerController.Position / Chunk.ChunkSize).Round() != (PlayerController.PreviousPosition / Chunk.ChunkSize).Round())
		{
			var playerPosition = (PlayerController.Position / Chunk.ChunkSize).RoundToInt();
			CheckChunk(new Vector2Int(playerPosition.x, playerPosition.z));
		}
	}
}
