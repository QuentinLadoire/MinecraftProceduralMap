using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
	public GroundMap groundMap = new GroundMap();

	public TreeMap treeMap = new TreeMap();

	public CaveMap caveMap = new CaveMap();

	public ChunkData GenerateChunkData(Vector2Int chunkKey)
	{
		ChunkData chunkData = new ChunkData();
		chunkData.WorldPosition = new Vector3(chunkKey.x, 0.0f, chunkKey.y) * Chunk.ChunkSize;

		chunkData.GenerateGround();

		chunkData.GenerateTree();

		return chunkData;
	}
	public BlockType GenerateBlockType(Vector3 worldPosition)
	{
		var groundHeight = groundMap.GetHeight(worldPosition.x, worldPosition.z);
		return (worldPosition.y > groundHeight) ? BlockType.Air : BlockType.Grass;
	}
}
