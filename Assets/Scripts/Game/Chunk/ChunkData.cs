using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkData
{
	public static double meshUpDelay = 0.0d;
	public static double meshDownDelay = 0.0d;
	public static double meshLeftDelay = 0.0d;
	public static double meshRightDelay = 0.0d;
	public static double meshBackDelay = 0.0d;
	public static double meshFrontDelay = 0.0d;
	public static double meshAllDelay = 0.0d;

	public Chunk ChunkParent { get; set; } = null;

	public Vector3 WorldPosition { get; set; } = Vector3.zero;

	Block[,,] blocks = null;
	MeshData meshData = null;

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

	public void SetBlock(int i, int j, int k, BlockType blockType, bool usePrirority = true)
	{
		if (j < 0 || j >= Chunk.ChunkHeight) return;

		if (i < 0 || k < 0 || i >= Chunk.ChunkSize ||  k >= Chunk.ChunkSize)
		{
			World.SetBlock(WorldPosition + new Vector3(i - Chunk.ChunkRadius, j, k - Chunk.ChunkRadius), blockType);

			return;
		}

		if (usePrirority)
		{
			if (blocks[i, j, k].Type == BlockType.Air || blocks[i, j, k].IsTransparent)
				blocks[i, j, k].Type = blockType;
		}
		else
			blocks[i, j, k].Type = blockType;
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
			for (int k = 0; k < Chunk.ChunkSize; k++)
			{
				var groundHeight = World.MapGenerator.groundMap.GetHeight(WorldPosition.x + i - Chunk.ChunkRadius, WorldPosition.z + k - Chunk.ChunkRadius);
				for (int j = 0; j < Chunk.ChunkHeight; j++)
					if (j < groundHeight)
						SetBlock(i, j, k, BlockType.Grass); // if block is below ground Height, create a grass block.
			}
	}
	public void GenerateTrees()
	{
		for (int i = 0; i < Chunk.ChunkSize; i++)
			for (int k = 0; k < Chunk.ChunkSize; k++)
			{
				var treeProbability = World.MapGenerator.treeMap.GetTreeProbability(WorldPosition.x + i - Chunk.ChunkRadius, WorldPosition.z + k - Chunk.ChunkRadius);
				if (treeProbability)
					for (int j = 0; j < Chunk.ChunkHeight; j++)
						if (blocks[i, j, k].Type == BlockType.Air)
						{
							CreateTree(i, j, k);
							break;
						}
			}
	}
	public void GenerateCaves()
	{
		for (int i = 0; i < Chunk.ChunkSize; i++)
			for (int j = 0; j < World.MapGenerator.caveMap.heighMax; j++)
				for (int k = 0; k < Chunk.ChunkSize; k++)
				{
					var caveProba = World.MapGenerator.caveMap.GetProbability(WorldPosition.x + i - Chunk.ChunkRadius, j, WorldPosition.z + k - Chunk.ChunkRadius);
					if (caveProba)
						SetBlock(i, j, k, BlockType.Air, false);
				}
	}

	public void CalculateMeshData()
	{
		meshData = new MeshData();

		for (int i = 0; i < Chunk.ChunkSize; i++)
			for (int k = 0; k < Chunk.ChunkSize; k++)
				for (int j = Chunk.ChunkHeight - 1; j >= 0; j--)
				{
					if (blocks[i, j, k].Type != BlockType.Air)
					{
						if (!blocks[i, j, k].IsTransparent)
						{
							if (CheckUp(i, j, k))
							{
								var start = System.DateTime.Now;
								meshData += blocks[i, j, k].CreateMeshUp();
								meshUpDelay += System.DateTime.Now.Subtract(start).TotalSeconds;
							}
							if (CheckDown(i, j, k))
							{
								var start = System.DateTime.Now;
								meshData += blocks[i, j, k].CreateMeshDown();
								meshDownDelay += System.DateTime.Now.Subtract(start).TotalSeconds;
							}
							if (CheckRight(i, j, k))
							{
								var start = System.DateTime.Now;
								meshData += blocks[i, j, k].CreateMeshRight();
								meshRightDelay += System.DateTime.Now.Subtract(start).TotalSeconds;
							}
							if (CheckLeft(i, j, k))
							{
								var start = System.DateTime.Now;
								meshData += blocks[i, j, k].CreateMeshLeft();
								meshLeftDelay += System.DateTime.Now.Subtract(start).TotalSeconds;
							}
							if (CheckFront(i, j, k))
							{
								var start = System.DateTime.Now;
								meshData += blocks[i, j, k].CreateMeshFront();
								meshFrontDelay += System.DateTime.Now.Subtract(start).TotalSeconds;
							}
							if (CheckBack(i, j, k))
							{
								var start = System.DateTime.Now;
								meshData += blocks[i, j, k].CreateMeshBack();
								meshBackDelay += System.DateTime.Now.Subtract(start).TotalSeconds;
							}
						}
						else
						{
							var start = System.DateTime.Now;
							meshData += blocks[i, j, k].CreateMeshAll();
							meshAllDelay += System.DateTime.Now.Subtract(start).TotalSeconds;
						}
					}
				}
	}
	public Mesh CreateMesh()
	{
		return meshData.CreateMesh();
	}
}
