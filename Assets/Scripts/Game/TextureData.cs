using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
class BlockTexture
{
    [Header("Block Type")]
    public BlockType BlockType = BlockType.None;

    [Header("Block Face Texture Position")]
    [SerializeField] Vector2 faceUp = new Vector2();
    [SerializeField] Vector2 faceDown = new Vector2();
    [SerializeField] Vector2 faceLeft = new Vector2();
    [SerializeField] Vector2 faceRight = new Vector2();
    [SerializeField] Vector2 faceFront = new Vector2();
    [SerializeField] Vector2 faceBack = new Vector2();

    public Vector2 GetUVPosition(BlockFace blockFace)
	{
        switch (blockFace)
		{
            case BlockFace.Up:      return faceUp;
            case BlockFace.Down:    return faceDown;
            case BlockFace.Left:    return faceLeft;
            case BlockFace.Right:   return faceRight;
            case BlockFace.Front:   return faceFront;
            case BlockFace.Back:    return faceBack;

            default: return Vector2.zero;
        }
	}
}

[CreateAssetMenu(fileName = "NewTextureData", menuName = "Data/TextureData")]
public class TextureData : ScriptableObject
{
    [SerializeField] int textureTileSize = 16;
    [SerializeField] Texture2D texture = null;

    Vector2 textureSize = Vector2.zero;

    [SerializeField] List<BlockTexture> blocks = new List<BlockTexture>();

    public Vector2 GetUVPosition(BlockType blockType, BlockFace blockFace)
	{
        foreach (var block in blocks)
            if (block.BlockType == blockType)
                return block.GetUVPosition(blockFace);

        return Vector2.zero;
	}
    public Vector2 GetTextureSize()
	{
        return textureSize;
	}
    public int GetTextureTileSize()
	{
        return textureTileSize;
	}

	private void OnValidate()
	{
        textureSize = new Vector2(texture.width, texture.height);
	}
}
