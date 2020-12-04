using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ChunkKey = UnityEngine.Vector2Int;

public class World : MonoBehaviour
{
	static World instance = null;

	[SerializeField] ThreadGeneration threadGeneration = null;
	[SerializeField] MapGenerator mapGenerator = null;

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

	private void Awake()
	{
		instance = this;
	}
}
