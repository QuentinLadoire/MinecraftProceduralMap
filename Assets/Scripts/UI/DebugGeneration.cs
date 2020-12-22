using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugGeneration : MonoBehaviour
{
    [SerializeField] Text debugText = null;

    [SerializeField] ThreadGeneration threadGeneration = null;

	void UpdateText()
	{
		debugText.text = "Thread Generation\n" +
			"	Chunk Loaded Count : " + threadGeneration.LoadedChunkCount + "\n" +
			"	Chunk Instantiated Count : " + threadGeneration.InstantiatedChunkCount + "\n" +
			"\n" +
			"State : " + threadGeneration.ThreadState + "s\n" +
			"	Global Time : " + threadGeneration.GlobalGenerationTime + "s\n" +
			"	ChunkData Time : " + threadGeneration.ChunkDataGenerationTime + "s\n" +
			"	MeshData Time : " + threadGeneration.MeshDataGenerationTime + "s\n";
	}

	private void Update()
	{
		if (threadGeneration == null) return;

		if (threadGeneration.updateDebugInfo)
		{
			UpdateText();
			threadGeneration.updateDebugInfo = false;
		}
	}
}
