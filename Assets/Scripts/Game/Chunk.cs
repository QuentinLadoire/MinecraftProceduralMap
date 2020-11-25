using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TextureGenerator
{
	static public Color[] GenerateHeightMapColor(float[] heightMap)
	{
		Color[] heightMapColor = new Color[heightMap.Length];

		for (int i = 0; i < heightMap.Length; i++)
		{
			heightMapColor[i] = Color.Lerp(Color.black, Color.white, heightMap[i]);
		}

		return heightMapColor;
	}
	static public Texture2D GenerateTexture(Color[] colors, int size)
	{
		var texture = new Texture2D(size, size);
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.SetPixels(colors);
		texture.Apply();

		return texture;
	}
}
public class MeshGenerator
{
	static public Mesh GenerateMesh(int size)
	{
		Vector3[] vertices = new Vector3[size * size];
		int[] triangles = new int[(size - 1) * (size - 1) * 2 * 3];
		Vector2[] uv = new Vector2[size * size];

		Vector3 startPosition = new Vector3(-size / 2, 0.0f, -size / 2);
		int trianglesIndex = 0;
		for (int i = 0; i < size * size; i++)
		{
			vertices[i].x = startPosition.x + (i % size);
			vertices[i].y = startPosition.y;
			vertices[i].z = startPosition.z + (i / size);

			uv[i] = new Vector2(i % size, i / size);

			if ((i % size) < size - 1 && (i / size) < size - 1)
			{
				triangles[trianglesIndex] = i;
				triangles[trianglesIndex + 1] = i + size + 1;
				triangles[trianglesIndex + 2] = i + 1;

				triangles[trianglesIndex + 3] = i + size + 1;
				triangles[trianglesIndex + 4] = i;
				triangles[trianglesIndex + 5] = i + size;

				trianglesIndex += 6;
			}
		}

		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uv;

		mesh.RecalculateNormals();

		return mesh;
	}
}

public enum CubeType
{
	None = -1,
	Solid,
	Air
}

public class ChunkData
{
	public Vector3 position = Vector3.zero;
	public CubeType[,,] cubeTypes = new CubeType[Chunk.ChunkSize, Chunk.ChunkSize, Chunk.ChunkHeight];
	public List<Matrix4x4> matrices = new List<Matrix4x4>();
	public Chunk parent = null;

	public void CalculateMatrices()
	{
		for (int j = 0; j < Chunk.ChunkSize; j++)
			for (int i = 0; i < Chunk.ChunkSize; i++)
				for (int k = Chunk.ChunkHeight - 1; k >= 0; k--)
					if (cubeTypes[i, j, k] == CubeType.Solid)
					{
						matrices.Add(Matrix4x4.TRS(position + new Vector3(i - Chunk.ChunkSize / 2, k, j - Chunk.ChunkSize / 2), Quaternion.identity, Vector3.one));
						break;
					}
	}
}

public class Chunk : MonoBehaviour
{
	public const int ChunkSize = 15;
	public const int ChunkHeight = 15;

	[SerializeField] Mesh cubeMesh = null;
	[SerializeField] Material cubeMaterial = null;

	public ChunkData ChunkData { get; set; } = null;

	void DisplayChunk()
	{
		for (int i = 0; i < ChunkData.matrices.Count / 1023; i++)
		{
			Graphics.DrawMeshInstanced(cubeMesh, 0, cubeMaterial, ChunkData.matrices.Skip(i * 1023).ToArray(), 1023);
		}
		Graphics.DrawMeshInstanced(cubeMesh, 0, cubeMaterial, ChunkData.matrices.Skip((ChunkData.matrices.Count / 1023) * 1023).ToArray());
	}

	private void Update()
	{
		if (ChunkData == null) return;

		DisplayChunk();
	}
}
