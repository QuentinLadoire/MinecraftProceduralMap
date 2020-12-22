using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
	public const int ChunkSize = 15;
	public const int ChunkHeight = 50;
	public const float ChunkRadius = ChunkSize / 2.0f;

	public ChunkData ChunkData { get; set; } = null;

	MeshFilter meshFilter = null;
	
	IEnumerator ChunkDataGeneration()
	{
		ChunkData.GenerateGround();

		yield return new WaitForFixedUpdate();

		ChunkData.GenerateTree();

		yield return null;

		ChunkData.CalculateMeshData();

		yield return null;

		meshFilter.mesh = ChunkData.CreateMesh();
	}

	private void Awake()
	{
		meshFilter = GetComponent<MeshFilter>();
	}
	private void Start()
	{
		ChunkData = new ChunkData();
		ChunkData.ChunkParent = this;
		ChunkData.WorldPosition = transform.position;

		StartCoroutine(ChunkDataGeneration());
	}
}
