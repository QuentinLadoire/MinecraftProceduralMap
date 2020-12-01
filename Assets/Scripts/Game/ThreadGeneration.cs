using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ChunkKey = UnityEngine.Vector2Int;
using ChunkDataDico = System.Collections.Generic.Dictionary<UnityEngine.Vector2Int, ChunkData>;

public class ThreadGeneration : MonoBehaviour
{
	[SerializeField] MapGenerator mapGenerator = null;
	[SerializeField] GameObject chunkPrefab = null;

	Thread generationThread = null;

	volatile bool shutdown = false;
	volatile bool updateChunk = true;
	volatile bool forceUpdate = false;

	ChunkKey[] keys = null;
	ChunkKey playerKeyPosition = ChunkKey.zero;

	ChunkDataDico chunkDataDico = new ChunkDataDico();
	List<ChunkData> chunkCreated = new List<ChunkData>();
	Queue<ChunkData> chunkToCreate = new Queue<ChunkData>();
	Queue<ChunkData> chunkToDestroy = new Queue<ChunkData>();

	void DestroyChunk(ChunkData chunkData)
	{
		chunkCreated.Remove(chunkData);
		Destroy(chunkData.ChunkParent.gameObject);
	}
	void CreateChunk(ChunkData chunkData)
	{
		var chunk = Instantiate(chunkPrefab).GetComponent<Chunk>();
		chunk.transform.position = chunkData.Position;
		chunk.ChunkData = chunkData;
		chunkData.ChunkParent = chunk;

		chunkCreated.Add(chunkData);
	}

	void GenerateChunkData()
	{
		foreach (var key in keys)
		{
			var chunkKey = playerKeyPosition + key;
			if (!chunkDataDico.ContainsKey(chunkKey))
			{
				var chunkData = mapGenerator.GenerateChunkData(key);
				chunkDataDico.Add(key, chunkData);
			}
		}
	}
	void GenerateMeshData()
	{
		foreach (var keyChunkDataValue in chunkDataDico)
			keyChunkDataValue.Value.CalculateMeshData();
	}
	void CheckChunkToCreateOrDestroy()
	{
		foreach (var keyChunkDataValue in chunkDataDico)
		{
			var chunkSqrDistance = (keyChunkDataValue.Key - playerKeyPosition).sqrMagnitude;
			if (chunkSqrDistance <= mapGenerator.NbChunk * mapGenerator.NbChunk)
			{
				if (!chunkCreated.Contains(keyChunkDataValue.Value))
				{
					//CreateChunk
					chunkToCreate.Enqueue(keyChunkDataValue.Value);
				}
			}
			else
			{
				if (chunkCreated.Contains(keyChunkDataValue.Value))
				{
					//DestroyCHunk
					chunkToDestroy.Enqueue(keyChunkDataValue.Value);
				}
			}
		}
	}
	void GenerationThread()
	{
		while (!shutdown)
		{
			while (!updateChunk || forceUpdate) ; // Wait
			updateChunk = false;

			GenerateChunkData();

			GenerateMeshData();

			CheckChunkToCreateOrDestroy();

			forceUpdate = true;
		}
	}
	void ResolveChunkToCreate()
	{
		var count = chunkToCreate.Count;
		for (int i = 0; i < count; i++)
		{
			CreateChunk(chunkToCreate.Dequeue());
		}
	}
	void ResolveChunkToDestroy()
	{
		var count = chunkToDestroy.Count;
		for (int i = 0; i < count; i++)
		{
			DestroyChunk(chunkToDestroy.Dequeue());
		}
	}
	void ResolveGenerationThread()
	{
		if (forceUpdate)
		{
			ResolveChunkToCreate();
			ResolveChunkToDestroy();

			forceUpdate = false;
		}
	}

	void StartThread()
	{
		generationThread = new Thread(GenerationThread);
		generationThread.Start();
	}

	ChunkKey GetKeyFromWorldPosition(Vector3 position)
	{
		return new ChunkKey(Mathf.RoundToInt(position.x / Chunk.ChunkSize), Mathf.RoundToInt(position.z / Chunk.ChunkSize));
	}

	private void Start()
	{
		keys = MathfPlus.GetAllPointInRadius(mapGenerator.NbChunk);

		if (mapGenerator != null)
		{
			StartThread();
		}
	}
	private void Update()
	{
		if (GetKeyFromWorldPosition(PlayerController.Position) != GetKeyFromWorldPosition(PlayerController.PreviousPosition))
		{
			playerKeyPosition = GetKeyFromWorldPosition(PlayerController.Position);
			updateChunk = true;
		}

		ResolveGenerationThread();
	}
}
