using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkData
{
	public Chunk ChunkParent { get; set; } = null;

	public Vector3 Position { get; set; } = Vector3.zero;
	public Block[,,] Blocks { get; set; } = null;

	MeshData meshData = null;

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
	}
	public void SetBlock(int i, int j, int k, Vector3 position)
	{
		if (!(i >= 0 && j >= 0 && k >= 0)) return;
		if (!(i < Chunk.ChunkSize && j < Chunk.ChunkSize && k < Chunk.ChunkHeight)) return;

		Blocks[i, j, k].Position = position;
	}
	public void SetBlock(int i, int j, int k, BlockType blockType, Vector3 position)
	{
		if (!(i >= 0 && j >= 0 && k >= 0)) return;
		if (!(i < Chunk.ChunkSize && j < Chunk.ChunkSize && k < Chunk.ChunkHeight)) return;

		Blocks[i, j, k].Type = blockType;
		Blocks[i, j, k].Position = position;
	}

	public void CalculateMeshData()
	{
		meshData = new MeshData();

		for (int j = 0; j < Chunk.ChunkSize; j++)
			for (int i = 0; i < Chunk.ChunkSize; i++)
				for (int k = Chunk.ChunkHeight - 1; k >= 0; k--)
					if (Blocks[i, j, k].Type != BlockType.Air) // Add the first block of the ground at column(i, j)
					{
						meshData += Blocks[i, j, k].CreateMeshData();

						if (Blocks[i, j, k].Type == BlockType.Grass) break;
					}
	}
	public Mesh CreateMesh()
	{
		return meshData.CreateMesh();
	}
}
