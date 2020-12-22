using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
	public HeightMap GroundHeightMap { get => heightMap; }
	public int		 GroundHeightMax { get => groundHeightMax; }

	public HeightMap TreeHeightMap	 { get => treeHeightMap; }
	public float	 TreeProbability { get => treeProbability; }

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

	HeightMap heightMap = null;
	HeightMap treeHeightMap = null;

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
		var height = heightMap.GetHeight(worldPosition.x, worldPosition.z);
		var groundHeight = height * groundHeightMax;
		return (worldPosition.y > groundHeight) ? BlockType.Air : BlockType.Grass;
	}

	private void Awake()
	{
		heightMap = new HeightMap(seed, octaves, lacunarity, persistance, scale);
		treeHeightMap = new HeightMap(seedTree, octavesTree, lacunarityTree, persistanceTree, scaleTree);
	}
}
