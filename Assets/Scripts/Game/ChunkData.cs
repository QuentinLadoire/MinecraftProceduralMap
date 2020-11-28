using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkData
{
	public Chunk Parent { get; set; } = null;

	public Vector3 Position { get; set; } = Vector3.zero;
	public Block[,,] Blocks { get; set; } = new Block[Chunk.ChunkSize, Chunk.ChunkSize, Chunk.ChunkHeight];

	MeshData meshData = null;

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
