using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkData
{
	public Chunk parent = null;

	public Vector3 position = Vector3.zero;
	public Block[,,] blocks = new Block[Chunk.ChunkSize, Chunk.ChunkSize, Chunk.ChunkHeight];
	public MeshData meshData = null;

	public void CalculateMeshData()
	{
		meshData = new MeshData();

		for (int j = 0; j < Chunk.ChunkSize; j++)
			for (int i = 0; i < Chunk.ChunkSize; i++)
				for (int k = Chunk.ChunkHeight - 1; k >= 0; k--)
					if (blocks[i, j, k].type == BlockType.Grass)
					{
						meshData += blocks[i, j, k].CreateMeshData();
						break;
					}
	}
	public Mesh CreateMesh()
	{
		return meshData.CreateMesh();
	}
}
