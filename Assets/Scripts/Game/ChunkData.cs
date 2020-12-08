using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkData
{
	public Chunk ChunkParent { get; set; } = null;

	public Vector3 WorldPosition { get; set; } = Vector3.zero;
	public Block[,,] Blocks { get; set; } = null;

	MeshData meshData = null;

	bool isModified = false;

	public ChunkData()
	{
		Blocks = new Block[Chunk.ChunkSize, Chunk.ChunkSize, Chunk.ChunkHeight];
		for (int k = 0; k < Chunk.ChunkHeight; k++)
			for (int j = 0; j < Chunk.ChunkSize; j++)
				for (int i = 0; i < Chunk.ChunkSize; i++)
					Blocks[i, j, k] = Block.Default;
	}

	public void SetBlock(int i, int j, int k, BlockType blockType)
	{
		if (!(i >= 0 && j >= 0 && k >= 0)) return;
		if (!(i < Chunk.ChunkSize && j < Chunk.ChunkSize && k < Chunk.ChunkHeight)) return;

		Blocks[i, j, k].Type = blockType;

		isModified = true;
	}
	public void SetBlock(int i, int j, int k, Vector3 position)
	{
		if (!(i >= 0 && j >= 0 && k >= 0)) return;
		if (!(i < Chunk.ChunkSize && j < Chunk.ChunkSize && k < Chunk.ChunkHeight)) return;

		Blocks[i, j, k].Position = position;

		isModified = true;
	}
	public void SetBlock(int i, int j, int k, BlockType blockType, Vector3 position)
	{
		if (!(i >= 0 && j >= 0 && k >= 0)) return;
		if (!(i < Chunk.ChunkSize && j < Chunk.ChunkSize && k < Chunk.ChunkHeight)) return;

		Blocks[i, j, k].Type = blockType;
		Blocks[i, j, k].Position = position;

		isModified = true;
	}

	bool CheckBlock(int i, int j, int k)
	{
		if (!(k < Chunk.ChunkHeight)) return true;
		if (!(k >= 0)) return false;

		if (i >= Chunk.ChunkSize) // Check the block in the adjacent X + 1 chunk
		{
			var chunkData = World.GetChunkDataAt(World.GetKeyFromWorldPosition(WorldPosition + new Vector3(Chunk.ChunkSize, 0.0f, 0.0f)));
			if (chunkData != null)
				return chunkData.CheckBlock(0, j, k);

			var block = World.GetBlockAt(WorldPosition + new Vector3(i - Chunk.ChunkRadius, k, j - Chunk.ChunkRadius));
			return block.Type == BlockType.Air || block.IsTransparent; 
		}
		if (j >= Chunk.ChunkSize) // Check the block in the adjacent Z + 1 chunk
		{
			var chunkData = World.GetChunkDataAt(World.GetKeyFromWorldPosition(WorldPosition + new Vector3(0.0f, 0.0f, Chunk.ChunkSize)));
			if (chunkData != null)
				return chunkData.CheckBlock(i, 0, k);

			var block = World.GetBlockAt(WorldPosition + new Vector3(i - Chunk.ChunkRadius, k, j - Chunk.ChunkRadius));
			return block.Type == BlockType.Air || block.IsTransparent;
		}

		if (i < 0) // Check the block in the adjacent X - 1 chunk
		{
			var chunkData = World.GetChunkDataAt(World.GetKeyFromWorldPosition(WorldPosition + new Vector3(-Chunk.ChunkSize, 0.0f, 0.0f)));
			if (chunkData != null)
				return chunkData.CheckBlock(Chunk.ChunkSize - 1, j, k);

			var block = World.GetBlockAt(WorldPosition + new Vector3(i - Chunk.ChunkRadius, k, j - Chunk.ChunkRadius));
			return block.Type == BlockType.Air || block.IsTransparent;
		}
		if (j < 0) // Check the block in the adjacent Z - 1 chunk
		{
			var chunkData = World.GetChunkDataAt(World.GetKeyFromWorldPosition(WorldPosition + new Vector3(0.0f, 0.0f, -Chunk.ChunkSize)));
			if (chunkData != null)
				return chunkData.CheckBlock(i, Chunk.ChunkSize - 1, k);

			var block = World.GetBlockAt(WorldPosition + new Vector3(i - Chunk.ChunkRadius, k, j - Chunk.ChunkRadius));
			return block.Type == BlockType.Air || block.IsTransparent;
		}

		if (Blocks[i, j, k].Type == BlockType.Air || Blocks[i, j, k].IsTransparent)
			return true;

		return false;
	}
	bool CheckUp(int i, int j, int k)
	{
		return CheckBlock(i, j, k + 1);
	}
	bool CheckDown(int i, int j, int k)
	{
		return CheckBlock(i, j, k - 1);
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
		return CheckBlock(i, j + 1, k);
	}
	bool CheckBack(int i, int j, int k)
	{
		return CheckBlock(i, j - 1, k);
	}

	public void CalculateMeshData()
	{
		if (!isModified) return;

		meshData = new MeshData();

		for (int j = 0; j < Chunk.ChunkSize; j++)
			for (int i = 0; i < Chunk.ChunkSize; i++)
				for (int k = Chunk.ChunkHeight - 1; k >= 0; k--)
				{
					if (Blocks[i, j, k].Type != BlockType.Air)
					{
						if (!Blocks[i, j, k].IsTransparent)
						{
							if (CheckUp(i, j, k))
								meshData += Blocks[i, j, k].CreateMeshUp();
							if (CheckDown(i, j, k))
								meshData += Blocks[i, j, k].CreateMeshDown();
							if (CheckRight(i, j, k))
								meshData += Blocks[i, j, k].CreateMeshRight();
							if (CheckLeft(i, j, k))
								meshData += Blocks[i, j, k].CreateMeshLeft();
							if (CheckFront(i, j, k))
								meshData += Blocks[i, j, k].CreateMeshFront();
							if (CheckBack(i, j, k))
								meshData += Blocks[i, j, k].CreateMeshBack();
						}
						else
							meshData += Blocks[i, j, k].CreateMeshAll();
					}
				}

		isModified = false;
	}
	public Mesh CreateMesh()
	{
		return meshData.CreateMesh();
	}
}
