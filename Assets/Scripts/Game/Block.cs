using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockType
{
	None = -1,
	Air,
	Dirt,
	Grass,
	Count
}

public enum BlockFace
{
	None = -1,
	Up,
	Down,
	Left,
	Right,
	Front,
	Back,
	Count
}

public class Block
{
	public Vector3 position = Vector3.zero;
	public BlockType type = BlockType.None;

	public MeshData CreateMeshData()
	{
		MeshData meshData = new MeshData();

		//Face Up
		meshData.AddVertex(position + new Vector3(-0.5f, 0.5f,  0.5f)); // 0
		meshData.AddVertex(position + new Vector3( 0.5f, 0.5f,  0.5f)); // 1
		meshData.AddVertex(position + new Vector3(-0.5f, 0.5f, -0.5f)); // 2
		meshData.AddVertex(position + new Vector3( 0.5f, 0.5f, -0.5f)); // 3

		meshData.AddTriangle(0, 1, 2);
		meshData.AddTriangle(1, 3, 2);

		var faceUV = MapGenerator.TextureData.GetUVPosition(type, BlockFace.Up);
		meshData.AddUV((faceUV + new Vector2(0.0f, 1.0f)) * MapGenerator.TextureData.GetTextureTileSize() / MapGenerator.TextureData.GetTextureSize());
		meshData.AddUV((faceUV + new Vector2(1.0f, 1.0f)) * MapGenerator.TextureData.GetTextureTileSize() / MapGenerator.TextureData.GetTextureSize());
		meshData.AddUV(faceUV * MapGenerator.TextureData.GetTextureTileSize() / MapGenerator.TextureData.GetTextureSize());
		meshData.AddUV((faceUV + new Vector2(1.0f, 0.0f)) * MapGenerator.TextureData.GetTextureTileSize() / MapGenerator.TextureData.GetTextureSize());

		//Face Down
		meshData.AddVertex(position + new Vector3(-0.5f, -0.5f,  0.5f)); // 4
		meshData.AddVertex(position + new Vector3( 0.5f, -0.5f,  0.5f)); // 5
		meshData.AddVertex(position + new Vector3(-0.5f, -0.5f, -0.5f)); // 6
		meshData.AddVertex(position + new Vector3( 0.5f, -0.5f, -0.5f)); // 7

		meshData.AddTriangle(4, 6, 5);
		meshData.AddTriangle(5, 6, 7);

		faceUV = MapGenerator.TextureData.GetUVPosition(type, BlockFace.Down);
		meshData.AddUV((faceUV + new Vector2(0.0f, 1.0f)) * MapGenerator.TextureData.GetTextureTileSize() / MapGenerator.TextureData.GetTextureSize());
		meshData.AddUV((faceUV + new Vector2(1.0f, 1.0f)) * MapGenerator.TextureData.GetTextureTileSize() / MapGenerator.TextureData.GetTextureSize());
		meshData.AddUV(faceUV * MapGenerator.TextureData.GetTextureTileSize() / MapGenerator.TextureData.GetTextureSize());
		meshData.AddUV((faceUV + new Vector2(1.0f, 0.0f)) * MapGenerator.TextureData.GetTextureTileSize() / MapGenerator.TextureData.GetTextureSize());

		//Face Right
		meshData.AddVertex(position + new Vector3( 0.5f,  0.5f, -0.5f)); // 8
		meshData.AddVertex(position + new Vector3( 0.5f,  0.5f,  0.5f)); // 9
		meshData.AddVertex(position + new Vector3( 0.5f, -0.5f, -0.5f)); // 10
		meshData.AddVertex(position + new Vector3( 0.5f, -0.5f,  0.5f)); // 11

		meshData.AddTriangle(8, 9, 10);
		meshData.AddTriangle(9, 11, 10);

		faceUV = MapGenerator.TextureData.GetUVPosition(type, BlockFace.Right);
		meshData.AddUV((faceUV + new Vector2(0.0f, 1.0f)) * MapGenerator.TextureData.GetTextureTileSize() / MapGenerator.TextureData.GetTextureSize());
		meshData.AddUV((faceUV + new Vector2(1.0f, 1.0f)) * MapGenerator.TextureData.GetTextureTileSize() / MapGenerator.TextureData.GetTextureSize());
		meshData.AddUV(faceUV * MapGenerator.TextureData.GetTextureTileSize() / MapGenerator.TextureData.GetTextureSize());
		meshData.AddUV((faceUV + new Vector2(1.0f, 0.0f)) * MapGenerator.TextureData.GetTextureTileSize() / MapGenerator.TextureData.GetTextureSize());

		//Face Front
		meshData.AddVertex(position + new Vector3( 0.5f,  0.5f,  0.5f)); // 12
		meshData.AddVertex(position + new Vector3(-0.5f,  0.5f,  0.5f)); // 13
		meshData.AddVertex(position + new Vector3( 0.5f, -0.5f,  0.5f)); // 14
		meshData.AddVertex(position + new Vector3(-0.5f, -0.5f,  0.5f)); // 15

		meshData.AddTriangle(12, 13, 14);
		meshData.AddTriangle(13, 15, 14);

		faceUV = MapGenerator.TextureData.GetUVPosition(type, BlockFace.Front);
		meshData.AddUV((faceUV + new Vector2(0.0f, 1.0f)) * MapGenerator.TextureData.GetTextureTileSize() / MapGenerator.TextureData.GetTextureSize());
		meshData.AddUV((faceUV + new Vector2(1.0f, 1.0f)) * MapGenerator.TextureData.GetTextureTileSize() / MapGenerator.TextureData.GetTextureSize());
		meshData.AddUV(faceUV * MapGenerator.TextureData.GetTextureTileSize() / MapGenerator.TextureData.GetTextureSize());
		meshData.AddUV((faceUV + new Vector2(1.0f, 0.0f)) * MapGenerator.TextureData.GetTextureTileSize() / MapGenerator.TextureData.GetTextureSize());

		//Face Left
		meshData.AddVertex(position + new Vector3(-0.5f,  0.5f,  0.5f)); // 16
		meshData.AddVertex(position + new Vector3(-0.5f,  0.5f, -0.5f)); // 17
		meshData.AddVertex(position + new Vector3(-0.5f, -0.5f,  0.5f)); // 18
		meshData.AddVertex(position + new Vector3(-0.5f, -0.5f, -0.5f)); // 19

		meshData.AddTriangle(16, 17, 18);
		meshData.AddTriangle(17, 19, 18);

		faceUV = MapGenerator.TextureData.GetUVPosition(type, BlockFace.Left);
		meshData.AddUV((faceUV + new Vector2(0.0f, 1.0f)) * MapGenerator.TextureData.GetTextureTileSize() / MapGenerator.TextureData.GetTextureSize());
		meshData.AddUV((faceUV + new Vector2(1.0f, 1.0f)) * MapGenerator.TextureData.GetTextureTileSize() / MapGenerator.TextureData.GetTextureSize());
		meshData.AddUV(faceUV * MapGenerator.TextureData.GetTextureTileSize() / MapGenerator.TextureData.GetTextureSize());
		meshData.AddUV((faceUV + new Vector2(1.0f, 0.0f)) * MapGenerator.TextureData.GetTextureTileSize() / MapGenerator.TextureData.GetTextureSize());

		//Face Back
		meshData.AddVertex(position + new Vector3(-0.5f,  0.5f, -0.5f)); // 20
		meshData.AddVertex(position + new Vector3( 0.5f,  0.5f, -0.5f)); // 21
		meshData.AddVertex(position + new Vector3(-0.5f, -0.5f, -0.5f)); // 22
		meshData.AddVertex(position + new Vector3( 0.5f, -0.5f, -0.5f)); // 23

		meshData.AddTriangle(20, 21, 22);
		meshData.AddTriangle(21, 23, 22);

		faceUV = MapGenerator.TextureData.GetUVPosition(type, BlockFace.Back);
		meshData.AddUV((faceUV + new Vector2(0.0f, 1.0f)) * MapGenerator.TextureData.GetTextureTileSize() / MapGenerator.TextureData.GetTextureSize());
		meshData.AddUV((faceUV + new Vector2(1.0f, 1.0f)) * MapGenerator.TextureData.GetTextureTileSize() / MapGenerator.TextureData.GetTextureSize());
		meshData.AddUV(faceUV * MapGenerator.TextureData.GetTextureTileSize() / MapGenerator.TextureData.GetTextureSize());
		meshData.AddUV((faceUV + new Vector2(1.0f, 0.0f)) * MapGenerator.TextureData.GetTextureTileSize() / MapGenerator.TextureData.GetTextureSize());

		return meshData;
	}
}
