using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkData
{
	public Chunk ChunkParent { get; set; } = null;

	public Vector3 WorldPosition { get; set; } = Vector3.zero;

	Block[,,] blocks = null;
	MeshData meshData = null;

	bool isModified = false;

	public ChunkData()
	{
		blocks = new Block[Chunk.ChunkSize, Chunk.ChunkHeight, Chunk.ChunkSize];
		for (int i = 0; i < Chunk.ChunkSize; i++)
			for (int j = 0; j < Chunk.ChunkHeight; j++)
				for (int k = 0; k < Chunk.ChunkSize; k++)
				{
					blocks[i, j, k].Position = new Vector3(i - Chunk.ChunkRadius, j, k - Chunk.ChunkRadius);
					blocks[i, j, k].Type = BlockType.Air;
				}
	}

	public void SetBlock(int i, int j, int k, BlockType blockType)
	{
		if (j < 0 || j >= Chunk.ChunkHeight) return;

		if (i < 0 || k < 0 || i >= Chunk.ChunkSize ||  k >= Chunk.ChunkSize)
		{
			World.SetBlock(WorldPosition + new Vector3(i - Chunk.ChunkRadius, j, k - Chunk.ChunkRadius), blockType);

			return;
		}

		if (blocks[i, j, k].Type == BlockType.Air || blocks[i, j, k].IsTransparent)
			blocks[i, j, k].Type = blockType;

		isModified = true;
	}
	public Block GetBlock(int i, int j, int k)
	{
		if (i < Chunk.ChunkSize && i >= 0 &&
			j < Chunk.ChunkHeight && j >= 0 &&
			k < Chunk.ChunkSize && k >= 0)
			return blocks[i, j, k];

		return Block.Default;
	}

	bool CheckBlock(int i, int j, int k)
	{
		if (!(j < Chunk.ChunkHeight)) return true;
		if (!(j >= 0)) return false;

		if (i >= Chunk.ChunkSize) // Check the block in the adjacent X + 1 chunk
		{
			var blockType = World.MapGenerator.GenerateBlockType(WorldPosition + new Vector3(i - Chunk.ChunkRadius, j, k - Chunk.ChunkRadius));
			return blockType == BlockType.Air;
		}
		if (k >= Chunk.ChunkSize) // Check the block in the adjacent Z + 1 chunk
		{
			var blockType = World.MapGenerator.GenerateBlockType(WorldPosition + new Vector3(i - Chunk.ChunkRadius, j, k - Chunk.ChunkRadius));
			return blockType == BlockType.Air;
		}

		if (i < 0) // Check the block in the adjacent X - 1 chunk
		{
			var blockType = World.MapGenerator.GenerateBlockType(WorldPosition + new Vector3(i - Chunk.ChunkRadius, j, k - Chunk.ChunkRadius));
			return blockType == BlockType.Air;
		}
		if (k < 0) // Check the block in the adjacent Z - 1 chunk
		{
			var blockType = World.MapGenerator.GenerateBlockType(WorldPosition + new Vector3(i - Chunk.ChunkRadius, j, k - Chunk.ChunkRadius));
			return blockType == BlockType.Air;
		}

		if (blocks[i, j, k].Type == BlockType.Air || blocks[i, j, k].IsTransparent)
			return true;

		return false;
	}
	bool CheckUp(int i, int j, int k)
	{
		return CheckBlock(i, j + 1, k);
	}
	bool CheckDown(int i, int j, int k)
	{
		return CheckBlock(i, j - 1, k);
	}
	bool CheckRight(int i, int j, int k)
	{
		return CheckBlock(i + 1, j, k);
	}
	bool CheckLeft(int i, int j, int k)
	{
		return CheckBlock(i - 1, j, k);
	}
	bool CheckFront(int i, int j, int k)
	{
		return CheckBlock(i, j, k + 1);
	}
	bool CheckBack(int i, int j, int k)
	{
		return CheckBlock(i, j, k - 1);
	}

	void CreateTree(int i, int j, int k)
	{
		// Create Leaves
		for (int kk = -2; kk < 3; kk++)
			for (int jj = 4; jj < 8; jj++)
				for (int ii = -2; ii < 3; ii++)
					if ((Mathf.Abs(ii) + Mathf.Abs(jj - 5) + Mathf.Abs(kk) < 5))
						SetBlock(i + ii, j + jj, k + kk, BlockType.Leaves);

		// Create Trunk
		SetBlock(i, j + 0, k, BlockType.Wood);
		SetBlock(i, j + 1, k, BlockType.Wood);
		SetBlock(i, j + 2, k, BlockType.Wood);
		SetBlock(i, j + 3, k, BlockType.Wood);
		SetBlock(i, j + 4, k, BlockType.Wood);
		SetBlock(i, j + 5, k, BlockType.Wood);
	}
	public void GenerateGround()
	{
		for (int i = 0; i < Chunk.ChunkSize; i++)
			for (int j = 0; j < Chunk.ChunkHeight; j++)
			{
				for (int k = 0; k < Chunk.ChunkSize; k++)
				{
					var blockPosition = new Vector3(i - Chunk.ChunkRadius, j, k - Chunk.ChunkRadius);
					var groundHeight = World.MapGenerator.groundMap.GetHeight(WorldPosition.x + blockPosition.x, WorldPosition.z + blockPosition.z);
					if (blockPosition.y < groundHeight)
						SetBlock(i, j, k, BlockType.Grass); // if block is below ground Height, create a block.
				}
			}
	}
	public void GenerateTree()
	{
		for (int i = 0; i < Chunk.ChunkSize; i++)
			for (int k = 0; k < Chunk.ChunkSize; k++)
				for (int j = Chunk.ChunkHeight - 1; j >= 0; j--)
				{
					//Check block type if is the first Ground Layer
					if (blocks[i, j, k].Type == BlockType.Grass)
					{
						var blockPosition = new Vector3(i - Chunk.ChunkRadius, j, k - Chunk.ChunkRadius);
						var treeProbability = World.MapGenerator.treeMap.GetTreeProbability(WorldPosition.x + blockPosition.x, WorldPosition.z + blockPosition.z);
						
						if (treeProbability)
							CreateTree(i, j + 1, k);

						break;
					}
				}
	}

	public void CalculateMeshData()
	{
		if (!isModified) return;

		meshData = new MeshData();

		for (int k = 0; k < Chunk.ChunkSize; k++)
			for (int i = 0; i < Chunk.ChunkSize; i++)
				for (int j = Chunk.ChunkHeight - 1; j >= 0; j--)
				{
					if (blocks[i, j, k].Type != BlockType.Air)
					{
						if (!blocks[i, j, k].IsTransparent)
						{
							if (CheckUp(i, j, k))
								meshData += blocks[i, j, k].CreateMeshUp();
							if (CheckDown(i, j, k))
								meshData += blocks[i, j, k].CreateMeshDown();
							if (CheckRight(i, j, k))
								meshData += blocks[i, j, k].CreateMeshRight();
							if (CheckLeft(i, j, k))
								meshData += blocks[i, j, k].CreateMeshLeft();
							if (CheckFront(i, j, k))
								meshData += blocks[i, j, k].CreateMeshFront();
							if (CheckBack(i, j, k))
								meshData += blocks[i, j, k].CreateMeshBack();
						}
						else
							meshData += blocks[i, j, k].CreateMeshAll();
					}
				}

		isModified = false;
	}
	public Mesh CreateMesh()
	{
		return meshData.CreateMesh();
	}
}
