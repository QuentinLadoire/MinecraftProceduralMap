using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshData
{
	List<Vector3> vertices = new List<Vector3>();
	List<int> triangles = new List<int>();
	List<Vector2> uvs = new List<Vector2>();

	public void AddVertex(Vector3 vertex)
	{
		vertices.Add(vertex);
	}
	public void AddTriangle(int a, int b, int c)
	{
		triangles.Add(a);
		triangles.Add(b);
		triangles.Add(c);
	}
	public void AddUV(Vector2 uv)
	{
		uvs.Add(uv);
	}

	public Mesh CreateMesh()
	{
		Mesh mesh = new Mesh();
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.uv = uvs.ToArray();
		mesh.RecalculateNormals();

		return mesh;
	}

	static public MeshData operator +(MeshData a, MeshData b)
	{
		MeshData meshData = new MeshData();

		var count = meshData.vertices.Count;
		meshData.vertices.AddRange(a.vertices);
		for (int i = 0; i < a.triangles.Count; i++) meshData.triangles.Add(count + a.triangles[i]);
		meshData.uvs.AddRange(a.uvs);

		count = meshData.vertices.Count;
		meshData.vertices.AddRange(b.vertices);
		for (int i = 0; i < b.triangles.Count; i++)	meshData.triangles.Add(count + b.triangles[i]);
		meshData.uvs.AddRange(b.uvs);

		return meshData;
	}
}
