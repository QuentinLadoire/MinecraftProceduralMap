using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
	[Header("Parameters")]
	[SerializeField] int seed = 0;
	[SerializeField] int octaves = 4;
	[SerializeField] float lacunarity = 2.0f;
	[SerializeField] [Range(0.0f, 1.0f)] float persistance = 0.5f;
	[SerializeField] Vector2 scale = new Vector2(20, 40);

	[Header("Game")]
	[SerializeField] GameObject chunkPrefab = null;

	HeightMap heightMap = null;

	void CreateChunk(Vector2 chunkPosition)
	{
		var chunk = Instantiate(chunkPrefab).GetComponent<Chunk>();
		chunk.transform.position = new Vector3(chunkPosition.x, 0.0f, chunkPosition.y);

		for (int k = 0; k < Chunk.ChunkHeight; k++)
		{
			for (int j = 0; j < Chunk.ChunkSize; j++)
			{
				for (int i = 0; i < Chunk.ChunkSize; i++)
				{
					if (!(k > heightMap.GetHeight(chunkPosition + new Vector2(i - Chunk.ChunkSize / 2, j - Chunk.ChunkSize / 2)) * Chunk.ChunkHeight))
					{
						var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
						cube.transform.parent = chunk.transform;
						cube.transform.localPosition = new Vector3(i - Chunk.ChunkSize / 2, k, j - Chunk.ChunkSize / 2);
					}
				}
			}
		}
	}

	private void Awake()
	{
		heightMap = new HeightMap(seed, octaves, lacunarity, persistance, scale);
	}
	private void Start()
	{
		int nbChunk = 10;
		
		for (int j = 0; j < nbChunk; j++)
		{
			for (int i = 0; i < nbChunk; i++)
			{
				CreateChunk(new Vector2(i * Chunk.ChunkSize, j * Chunk.ChunkSize));
			}
		}
	}
}
