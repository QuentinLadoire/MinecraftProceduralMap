using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockType
{
	None = -1,
	Air,
	Dirt,
	Grass,
	Wood,
	Leaves,
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

public struct Block
{
	public static Block Default = new Block(Vector3.zero, BlockType.None, false);

	public Vector3 Position { get; set; }
	public BlockType Type { get => type; set => SetType(value); }
	public bool IsTransparent { get; private set; }

	BlockType type;
	void SetType(BlockType type)
	{
		switch(type)
		{
			case BlockType.Dirt:
			case BlockType.Grass:
			case BlockType.Wood:
				IsTransparent = false;
				break;

			case BlockType.Leaves:
				IsTransparent = true;
				break;
		}

		this.type = type;
	}

	public MeshData CreateMeshUp()
	{
		MeshData meshData = new MeshData();

		//Face Up
		meshData.AddVertex(Position + new Vector3(-0.5f, 0.5f, 0.5f));	// 0
		meshData.AddVertex(Position + new Vector3(0.5f, 0.5f, 0.5f));	// 1
		meshData.AddVertex(Position + new Vector3(-0.5f, 0.5f, -0.5f));	// 2
		meshData.AddVertex(Position + new Vector3(0.5f, 0.5f, -0.5f));	// 3

		meshData.AddTriangle(0, 1, 2);
		meshData.AddTriangle(1, 3, 2);

		var faceUV = World.TextureData.GetUVPosition(Type, BlockFace.Up);
		meshData.AddUV((faceUV + new Vector2(0.0f, 1.0f)) * World.TextureData.GetTextureTileSize() / World.TextureData.GetTextureSize());
		meshData.AddUV((faceUV + new Vector2(1.0f, 1.0f)) * World.TextureData.GetTextureTileSize() / World.TextureData.GetTextureSize());
		meshData.AddUV( faceUV							  * World.TextureData.GetTextureTileSize() / World.TextureData.GetTextureSize());
		meshData.AddUV((faceUV + new Vector2(1.0f, 0.0f)) * World.TextureData.GetTextureTileSize() / World.TextureData.GetTextureSize());

		return meshData;
	}
	public MeshData CreateMeshDown()
	{
		MeshData meshData = new MeshData();

		//Face Down
		meshData.AddVertex(Position + new Vector3(-0.5f, -0.5f, 0.5f));	// 0
		meshData.AddVertex(Position + new Vector3(0.5f, -0.5f, 0.5f));	// 1
		meshData.AddVertex(Position + new Vector3(-0.5f, -0.5f, -0.5f));// 2
		meshData.AddVertex(Position + new Vector3(0.5f, -0.5f, -0.5f));	// 3

		meshData.AddTriangle(0, 2, 1);
		meshData.AddTriangle(1, 2, 3);

		var faceUV = World.TextureData.GetUVPosition(Type, BlockFace.Down);
		meshData.AddUV((faceUV + new Vector2(0.0f, 1.0f)) * World.TextureData.GetTextureTileSize() / World.TextureData.GetTextureSize());
		meshData.AddUV((faceUV + new Vector2(1.0f, 1.0f)) * World.TextureData.GetTextureTileSize() / World.TextureData.GetTextureSize());
		meshData.AddUV( faceUV							  * World.TextureData.GetTextureTileSize() / World.TextureData.GetTextureSize());
		meshData.AddUV((faceUV + new Vector2(1.0f, 0.0f)) * World.TextureData.GetTextureTileSize() / World.TextureData.GetTextureSize());

		return meshData;
	}
	public MeshData CreateMeshRight()
	{
		MeshData meshData = new MeshData();

		//Face Right
		meshData.AddVertex(Position + new Vector3(0.5f, 0.5f, -0.5f));	// 0
		meshData.AddVertex(Position + new Vector3(0.5f, 0.5f, 0.5f));	// 1
		meshData.AddVertex(Position + new Vector3(0.5f, -0.5f, -0.5f));	// 2
		meshData.AddVertex(Position + new Vector3(0.5f, -0.5f, 0.5f));	// 3

		meshData.AddTriangle(0, 1, 2);
		meshData.AddTriangle(1, 3, 2);

		var faceUV = World.TextureData.GetUVPosition(Type, BlockFace.Right);
		meshData.AddUV((faceUV + new Vector2(0.0f, 1.0f)) * World.TextureData.GetTextureTileSize() / World.TextureData.GetTextureSize());
		meshData.AddUV((faceUV + new Vector2(1.0f, 1.0f)) * World.TextureData.GetTextureTileSize() / World.TextureData.GetTextureSize());
		meshData.AddUV( faceUV						      * World.TextureData.GetTextureTileSize() / World.TextureData.GetTextureSize());
		meshData.AddUV((faceUV + new Vector2(1.0f, 0.0f)) * World.TextureData.GetTextureTileSize() / World.TextureData.GetTextureSize());

		return meshData;
	}
	public MeshData CreateMeshLeft()
	{
		MeshData meshData = new MeshData();

		//Face Left
		meshData.AddVertex(Position + new Vector3(-0.5f, 0.5f, 0.5f));	// 0
		meshData.AddVertex(Position + new Vector3(-0.5f, 0.5f, -0.5f));	// 1
		meshData.AddVertex(Position + new Vector3(-0.5f, -0.5f, 0.5f));	// 2
		meshData.AddVertex(Position + new Vector3(-0.5f, -0.5f, -0.5f));// 3

		meshData.AddTriangle(0, 1, 2);
		meshData.AddTriangle(1, 3, 2);

		var faceUV = World.TextureData.GetUVPosition(Type, BlockFace.Left);
		meshData.AddUV((faceUV + new Vector2(0.0f, 1.0f)) * World.TextureData.GetTextureTileSize() / World.TextureData.GetTextureSize());
		meshData.AddUV((faceUV + new Vector2(1.0f, 1.0f)) * World.TextureData.GetTextureTileSize() / World.TextureData.GetTextureSize());
		meshData.AddUV( faceUV							  * World.TextureData.GetTextureTileSize() / World.TextureData.GetTextureSize());
		meshData.AddUV((faceUV + new Vector2(1.0f, 0.0f)) * World.TextureData.GetTextureTileSize() / World.TextureData.GetTextureSize());

		return meshData;
	}
	public MeshData CreateMeshFront()
	{
		MeshData meshData = new MeshData();

		//Face Front
		meshData.AddVertex(Position + new Vector3(0.5f, 0.5f, 0.5f));	// 0
		meshData.AddVertex(Position + new Vector3(-0.5f, 0.5f, 0.5f));	// 1
		meshData.AddVertex(Position + new Vector3(0.5f, -0.5f, 0.5f));	// 2
		meshData.AddVertex(Position + new Vector3(-0.5f, -0.5f, 0.5f)); // 3

		meshData.AddTriangle(0, 1, 2);
		meshData.AddTriangle(1, 3, 2);

		var faceUV = World.TextureData.GetUVPosition(Type, BlockFace.Front);
		meshData.AddUV((faceUV + new Vector2(0.0f, 1.0f)) * World.TextureData.GetTextureTileSize() / World.TextureData.GetTextureSize());
		meshData.AddUV((faceUV + new Vector2(1.0f, 1.0f)) * World.TextureData.GetTextureTileSize() / World.TextureData.GetTextureSize());
		meshData.AddUV( faceUV							  * World.TextureData.GetTextureTileSize() / World.TextureData.GetTextureSize());
		meshData.AddUV((faceUV + new Vector2(1.0f, 0.0f)) * World.TextureData.GetTextureTileSize() / World.TextureData.GetTextureSize());

		return meshData;
	}
	public MeshData CreateMeshBack()
	{
		MeshData meshData = new MeshData();

		//Face Back
		meshData.AddVertex(Position + new Vector3(-0.5f, 0.5f, -0.5f));	// 0
		meshData.AddVertex(Position + new Vector3(0.5f, 0.5f, -0.5f));	// 1
		meshData.AddVertex(Position + new Vector3(-0.5f, -0.5f, -0.5f));// 2
		meshData.AddVertex(Position + new Vector3(0.5f, -0.5f, -0.5f));	// 3

		meshData.AddTriangle(0, 1, 2);
		meshData.AddTriangle(1, 3, 2);

		var faceUV = World.TextureData.GetUVPosition(Type, BlockFace.Back);
		meshData.AddUV((faceUV + new Vector2(0.0f, 1.0f)) * World.TextureData.GetTextureTileSize() / World.TextureData.GetTextureSize());
		meshData.AddUV((faceUV + new Vector2(1.0f, 1.0f)) * World.TextureData.GetTextureTileSize() / World.TextureData.GetTextureSize());
		meshData.AddUV( faceUV						      * World.TextureData.GetTextureTileSize() / World.TextureData.GetTextureSize());
		meshData.AddUV((faceUV + new Vector2(1.0f, 0.0f)) * World.TextureData.GetTextureTileSize() / World.TextureData.GetTextureSize());

		return meshData;
	}

	public MeshData CreateMeshAll()
	{
		return CreateMeshUp() + CreateMeshDown() + CreateMeshRight() + CreateMeshLeft() + CreateMeshFront() + CreateMeshBack();
	}
	
	public Block(Vector3 position, BlockType type, bool isSolid)
	{
		Position = position;
		this.type = type;
		IsTransparent = isSolid;
	}
}
