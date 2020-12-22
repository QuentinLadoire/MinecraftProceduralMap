using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ChunkKey = UnityEngine.Vector2Int;

public class MapGenerator : MonoBehaviour
{
	static MapGenerator instance = null;
	public static TextureData TextureData { get => instance.textureData; }
	public static HeightMap GroundHeightMap { get => instance.heightMap; }
	public static HeightMap TreeHeightMap { get => instance.treeHeightMap; }
	public static int GroundHeightMax { get => instance.groundHeightMax; }
	public static float TreeProbability { get => instance.treeProbability; }

	public int ChunkViewRadius { get => nbChunk; }

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
	[SerializeField] TextureData textureData = null;

	HeightMap heightMap = null;
	HeightMap treeHeightMap = null;

	public ChunkData GenerateChunkData(ChunkKey chunkKey)
	{
		ChunkData chunkData = new ChunkData();
		chunkData.WorldPosition = new Vector3(chunkKey.x, 0.0f, chunkKey.y) * Chunk.ChunkSize;

		chunkData.GenerateGround();

		chunkData.GenerateTree();

		return chunkData;
	}
	public Block GenerateBlock(Vector3 worldPosition)
	{
		var height = heightMap.GetHeight(worldPosition.x, worldPosition.z);
		var groundHeight = height * groundHeightMax;
		var blockType = (worldPosition.y > groundHeight) ? BlockType.Air : BlockType.Grass;

		var block = Block.Default;
		var chunkPosition = World.GetKeyFromWorldPosition(worldPosition) * Chunk.ChunkSize;
		block.Position = new Vector3(worldPosition.x - chunkPosition.x, worldPosition.y, worldPosition.z - chunkPosition.y);
		block.Type = blockType;

		return block;
	}

	private void Awake()
	{
		instance = this;

		heightMap = new HeightMap(seed, octaves, lacunarity, persistance, scale);
		treeHeightMap = new HeightMap(seedTree, octavesTree, lacunarityTree, persistanceTree, scaleTree);
	}
}
