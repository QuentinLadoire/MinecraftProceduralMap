using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ChunkKey = UnityEngine.Vector2Int;

public class MapGenerator : MonoBehaviour
{
	static MapGenerator instance = null;
	public static TextureData TextureData { get => instance.textureData; }

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

	public HeightMap HeightMap { get; private set; } = null;
	public HeightMap TreeHeightMap { get; private set; } = null;

	void CreateTree(ChunkData chunkData, int i, int j, int k, Vector3 blockPosition)
	{
		var treeHeight = TreeHeightMap.GetHeight(chunkData.WorldPosition.x + blockPosition.x, chunkData.WorldPosition.z + blockPosition.z);
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
	public ChunkData GenerateChunkData(ChunkKey chunkKey)
	{
		ChunkData chunkData = new ChunkData();
		chunkData.WorldPosition = new Vector3(chunkKey.x, 0.0f, chunkKey.y) * Chunk.ChunkSize;

		for (int j = 0; j < Chunk.ChunkSize; j++)
			for (int i = 0; i < Chunk.ChunkSize; i++)
			{
				bool checkTree = true;
				for (int k = 0; k < Chunk.ChunkHeight; k++)
				{
					var blockPosition = new Vector3(i - Chunk.ChunkRadius, k, j - Chunk.ChunkRadius);
					var height = HeightMap.GetHeight(chunkData.WorldPosition.x + blockPosition.x, chunkData.WorldPosition.z + blockPosition.z);
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

	private void Awake()
	{
		instance = this;

		HeightMap = new HeightMap(seed, octaves, lacunarity, persistance, scale);
		TreeHeightMap = new HeightMap(seedTree, octavesTree, lacunarityTree, persistanceTree, scaleTree);
	}
}
