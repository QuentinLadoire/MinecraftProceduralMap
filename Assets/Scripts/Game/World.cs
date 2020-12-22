using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ChunkKey = UnityEngine.Vector2Int;

public class World : MonoBehaviour
{
	static World instance = null;

	public static int ChunkView { get => instance.chunkView; }
	public static TextureData TextureData { get => instance.textureData; }

	[SerializeField] ThreadGeneration threadGeneration = null;
	[SerializeField] MapGenerator mapGenerator = null;

	public static Vector2 Get2DPosition(Vector3 position)
	{
		return new Vector2(position.x, position.z);
	}
	public static Vector2Int GetChunkPosition(Vector3 position)
	{
		return new Vector2Int(Mathf.RoundToInt(position.x / Chunk.ChunkSize), Mathf.RoundToInt(position.z / Chunk.ChunkSize));
	}
	public static ChunkKey GetKeyFromWorldPosition(Vector3 position)
	{
		return new ChunkKey(Mathf.RoundToInt(position.x / Chunk.ChunkSize), Mathf.RoundToInt(position.z / Chunk.ChunkSize));
	}

	public static ChunkData GetChunkDataAt(ChunkKey chunkKey)
	{
		return instance.threadGeneration.GetChunkData(chunkKey);
	}
	public static Block GetBlockAt(Vector3 worldPosition)
	{
		return instance.mapGenerator.GenerateBlock(worldPosition);
	}

	[SerializeField] GameObject chunkPrefab = null;
	[SerializeField] TextureData textureData = null;
	[SerializeField] int chunkView = 5;

	List<Chunk> chunkList = new List<Chunk>();

	void CreateChunk(Vector3 worldPosition)
	{
		var chunk = Instantiate(chunkPrefab).GetComponent<Chunk>();
		chunk.transform.position = worldPosition;
		chunk.transform.parent = transform;

		chunkList.Add(chunk);
	}
	void UpdateChunk()
	{
		var playerChunkPosition = GetChunkPosition(PlayerController.Position);
		var playerPreviousChunkPosition = GetChunkPosition(PlayerController.PreviousPosition);
		var direction = playerChunkPosition - playerPreviousChunkPosition;

		chunkList.RemoveAll((Chunk chunk) => 
		{
			var chunkPosition = GetChunkPosition(chunk.transform.position);
			var distance = playerChunkPosition - chunkPosition;

			if (Mathf.Abs(distance.x) >= chunkView + 1 || Mathf.Abs(distance.y) >= chunkView + 1)
			{
				Destroy(chunk.gameObject);
				return true;
			}

			return false;
		});

		if (direction.x != 0)
		{
			for (int i = 0; i < chunkView * 2 + 1; i++)
			{
				var chunkPosition = new Vector3();
				chunkPosition.x = playerChunkPosition.x + chunkView * direction.x;
				chunkPosition.y = 0.0f;
				chunkPosition.z = playerChunkPosition.y + i - chunkView;

				CreateChunk(chunkPosition * Chunk.ChunkSize);
			}
		}
		if (direction.y != 0)
		{
			for (int i = 0; i < chunkView * 2 + 1; i++)
			{
				var chunkPosition = new Vector3();
				chunkPosition.x = playerChunkPosition.x + i - chunkView;
				chunkPosition.y = 0.0f;
				chunkPosition.z = playerChunkPosition.y + chunkView * direction.y;

				CreateChunk(chunkPosition * Chunk.ChunkSize);
			}
		}
	}
	void WorldCreation()
	{
		for (int i = 0; i < chunkView * 2 + 1; i++)
			for (int j = 0; j < chunkView * 2 + 1; j++)
				CreateChunk(new Vector3((i - chunkView), 0.0f, (j - chunkView)) * Chunk.ChunkSize);
	}

	private void Awake()
	{
		instance = this;
	}
	private void Start()
	{
		WorldCreation();
	}
	private void Update()
	{
		//if (GetChunkPosition(PlayerController.Position) != GetChunkPosition(PlayerController.PreviousPosition))
		//	UpdateChunk();
	}
}
