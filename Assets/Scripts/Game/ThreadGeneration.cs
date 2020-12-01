using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ChunkKey = UnityEngine.Vector2Int;
using ChunkDataDico = System.Collections.Generic.Dictionary<UnityEngine.Vector2Int, ChunkData>;

public enum ThreadState
{
	None = -1,
	Waiting,
	Generate,
	Count
}

public class ThreadGeneration : MonoBehaviour
{
	[SerializeField] MapGenerator mapGenerator = null;
	[SerializeField] GameObject chunkPrefab = null;

	Thread generationThread = null;

	volatile bool shutdownRequest = false;
	volatile bool generationRequest = true;
	volatile bool resolveRequest = false;

	public ThreadState ThreadState { get; private set; } = ThreadState.None;

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
		// for all chunk around the player
		foreach (var key in keys)
		{
			var chunkKey = playerKeyPosition + key;
			if (!chunkDataDico.ContainsKey(chunkKey)) // check if the chunkdata already exist 
			{
				var chunkData = mapGenerator.GenerateChunkData(chunkKey);	// Generate and 
				chunkDataDico.Add(chunkKey, chunkData);						// Add to dico
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
		// For all chunkData in the dico
		foreach (var keyChunkDataValue in chunkDataDico)
		{
			var chunkSqrDistance = (keyChunkDataValue.Key - playerKeyPosition).sqrMagnitude;
			if (chunkSqrDistance <= mapGenerator.ChunkViewRadius * mapGenerator.ChunkViewRadius) // Check if the distance with the player is less or equal as the chunkViewRadius
			{
				// if the condition is true, check if the chunk as need to be created 
				if (!chunkCreated.Contains(keyChunkDataValue.Value))	// check if is not already created
					chunkToCreate.Enqueue(keyChunkDataValue.Value);		// and if true add the chunkdata to the queue for creating the chunk
			}
			else
			{
				// if the condition is false, check if the chunk as need to be destroy
				if (chunkCreated.Contains(keyChunkDataValue.Value))		// Check if is created
					chunkToDestroy.Enqueue(keyChunkDataValue.Value);	// and if true add the chundata to the queue for destroying the chunk
			}
		}
	}
	void GenerationThread()
	{
		while (!shutdownRequest)
		{
			ThreadState = ThreadState.Waiting;

			while (!generationRequest || resolveRequest) ; // Wait if no generation request or if resolving generation is in process
			generationRequest = false; // reset the generation request

			ThreadState = ThreadState.Generate;

			GenerateChunkData(); // Generate ChunkData if doesnt exist

			GenerateMeshData(); // Recalculate the MeshData if a modification as applied

			CheckChunkToCreateOrDestroy(); // Check what chunk has need to be create or destroy

			resolveRequest = true; // Set true for resolving the generation in main thread
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
		if (resolveRequest)
		{
			ResolveChunkToCreate();
			ResolveChunkToDestroy();

			resolveRequest = false;
		}
	}

	void StartThread()
	{
		generationThread = new Thread(GenerationThread);
		generationThread.Start();
	}

	void ChunkGenerationIsTrigger()
	{
		if (GetKeyFromWorldPosition(PlayerController.Position) != GetKeyFromWorldPosition(PlayerController.PreviousPosition))
		{
			playerKeyPosition = GetKeyFromWorldPosition(PlayerController.Position);
			generationRequest = true;
		}
	}

	ChunkKey GetKeyFromWorldPosition(Vector3 position)
	{
		return new ChunkKey(Mathf.RoundToInt(position.x / Chunk.ChunkSize), Mathf.RoundToInt(position.z / Chunk.ChunkSize));
	}

	private void Start()
	{
		keys = MathfPlus.GetAllPointInRadius(mapGenerator.ChunkViewRadius);

		if (mapGenerator != null) StartThread();
	}
	private void Update()
	{
		ChunkGenerationIsTrigger();

		ResolveGenerationThread();
	}
}
