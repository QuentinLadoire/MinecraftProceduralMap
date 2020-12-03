using System;
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
	volatile bool resolveGenerationRequest = false;

	public volatile bool updateDebugInfo = false;
	public ThreadState ThreadState { get; private set; } = ThreadState.None;
	public int LoadedChunkCount { get => chunkDataDico.Count; }
	public int InstantiatedChunkCount { get => chunkCreated.Count; }
	public float GlobalGenerationTime { get; private set; } = 0.0f;
	public float ChunkDataGenerationTime { get; private set; } = 0.0f;
	public float MeshDataGenerationTime { get; private set; } = 0.0f;
	public float CreateOrDestroyChunkTime { get; private set; } = 0.0f;

	ChunkKey[] keys = null;
	ChunkKey playerKeyPosition = ChunkKey.zero;

	ChunkDataDico chunkDataDico = new ChunkDataDico();
	List<ChunkData> chunkCreated = new List<ChunkData>();
	Queue<ChunkData> chunkToCreate = new Queue<ChunkData>();
	Queue<ChunkData> chunkToDestroy = new Queue<ChunkData>();

	public ChunkData GetChunkData(ChunkKey chunkKey)
	{
		if (chunkDataDico.ContainsKey(chunkKey))
			return chunkDataDico[chunkKey];

		return null;
	}

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

	void WaitDebugInfo()
	{
		updateDebugInfo = true;
		while (updateDebugInfo) ;
	}
	void WaitResolveGeneration()
	{
		resolveGenerationRequest = true;
		while (resolveGenerationRequest) ;
	}
	void WaitGenerationRequest()
	{
		generationRequest = false;
		while (!generationRequest) ;
	}

	void GenerateChunkData(ChunkKey playerKeyPosition)
	{
		// for all chunk around the player
		foreach (var key in keys)
		{
			var chunkKey = playerKeyPosition + key;
			if (!chunkDataDico.ContainsKey(chunkKey)) // check if the chunkdata already exist 
			{
				var chunkData = mapGenerator.GenerateChunkData(chunkKey);   // Generate and 
				chunkDataDico.Add(chunkKey, chunkData);                     // Add to dico
			}
		}
	}
	void CheckChunkToCreateOrDestroy(ChunkKey playerKeyPosition)
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
	void GenerateMeshData()
	{
		foreach (var chunkData in chunkDataDico)
			chunkData.Value.CalculateMeshData();
	}
	void GenerationThread()
	{
		bool firstFrame = true;
		while (!shutdownRequest)
		{
			ThreadState = ThreadState.Waiting;
			WaitDebugInfo();

			if (!firstFrame)
				WaitGenerationRequest(); // Wait if no generation request or if resolving generation is in process
			else
				firstFrame = false;

			var playerKeyPositionTmp = playerKeyPosition;

			ThreadState = ThreadState.Generate;
			WaitDebugInfo();

			var startGlobalTimeGeneration = DateTime.Now;

			var startChunkDataGenerationTime = DateTime.Now;
			GenerateChunkData(playerKeyPositionTmp); // Generate ChunkData if doesnt exist
			ChunkDataGenerationTime = (int)(DateTime.Now.Subtract(startChunkDataGenerationTime).TotalSeconds * 100) / 100.0f;

			var startMeshDataGenerationTime = DateTime.Now;
			GenerateMeshData();
			MeshDataGenerationTime = (int)(DateTime.Now.Subtract(startMeshDataGenerationTime).TotalSeconds * 100) / 100.0f;

			var startCreateOrDestroyChunkTime = DateTime.Now;
			CheckChunkToCreateOrDestroy(playerKeyPositionTmp); // Check what chunk has need to be create or destroy
			CreateOrDestroyChunkTime = (int)(DateTime.Now.Subtract(startCreateOrDestroyChunkTime).TotalSeconds * 100) / 100.0f;

			GlobalGenerationTime = (int)(DateTime.Now.Subtract(startGlobalTimeGeneration).TotalSeconds * 100) / 100.0f;

			WaitDebugInfo();

			WaitResolveGeneration(); // Wait the main thread Resolving
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
		if (resolveGenerationRequest)
		{
			ResolveChunkToCreate();
			ResolveChunkToDestroy();

			resolveGenerationRequest = false;
		}
	}

	void StartThread()
	{
		generationThread = new Thread(GenerationThread);
		generationThread.Start();
	}

	void ChunkGenerationIsTrigger()
	{
		if (World.GetKeyFromWorldPosition(PlayerController.Position) != World.GetKeyFromWorldPosition(PlayerController.PreviousPosition))
		{
			playerKeyPosition = World.GetKeyFromWorldPosition(PlayerController.Position);

			generationRequest = true;
		}
	}

	private void Start()
	{
		keys = MathfPlus.GetAllPointInRadius(mapGenerator.ChunkViewRadius);

		StartThread();
	}
	private void Update()
	{
		ChunkGenerationIsTrigger();

		ResolveGenerationThread();
	}
	private void OnApplicationQuit()
	{
		shutdownRequest = true;
		generationThread.Abort();
	}
}
