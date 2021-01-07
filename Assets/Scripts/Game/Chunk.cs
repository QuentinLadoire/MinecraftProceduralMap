﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
	public const int ChunkSize = 15;
	public const int ChunkHeight = 50;
	public static int ChunkRadius = Mathf.FloorToInt(ChunkSize / 2.0f);

	public ChunkData ChunkData { get; set; } = null;

	public Block GetBlockAt(Vector3 worldPosition)
	{
		var blockPosition = worldPosition - ChunkData.WorldPosition;

		return ChunkData.GetBlock((int)blockPosition.x, (int)blockPosition.y, (int)blockPosition.z);
	}
	public void SetBlockAt(Vector3 worldPosition, BlockType type)
	{
		var blockPosition = worldPosition - ChunkData.WorldPosition;
		blockPosition.x += ChunkRadius;
		blockPosition.z += ChunkRadius;

		ChunkData.SetBlock((int)blockPosition.x, (int)blockPosition.y, (int)blockPosition.z, type);
	}

 	MeshFilter meshFilter = null;
	
	IEnumerator ChunkDataGeneration()
	{
		ChunkData.GenerateGround();

		yield return new WaitForFixedUpdate();

		ChunkData.GenerateTree();

		yield return new WaitForFixedUpdate();

		ChunkData.CalculateMeshData();

		yield return new WaitForFixedUpdate();

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

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(transform.position, new Vector3(ChunkSize, ChunkHeight, ChunkSize));
	}
}
