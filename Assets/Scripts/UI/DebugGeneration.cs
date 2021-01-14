using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugGeneration : MonoBehaviour
{
    [SerializeField] Text debugText = null;

	void Update()
	{
		debugText.text = "World Creation\n" +
						 "	- Chunk nb : " + World.ChunkCount + "\n" +
						 "	- Ground : " + Chunk.groundGenerationDelay + "s\n" +
						 "	- Trees : " + Chunk.treeGenerationDelay + "s\n" +
						 "	- Caves : " + Chunk.caveGenerationDelay + "s\n" +
						 "	- Mesh : " + Chunk.meshGenerationDelay + "s\n" +
						 "		- Face Up : " + ChunkData.meshUpDelay + "s\n" +
						 "		- Face Down : " + ChunkData.meshDownDelay + "s\n" +
						 "		- Face Left : " + ChunkData.meshLeftDelay + "s\n" +
						 "		- Face Right : " + ChunkData.meshRightDelay + "s\n" +
						 "		- Face Back : " + ChunkData.meshBackDelay + "s\n" +
						 "		- Face Front : " + ChunkData.meshFrontDelay + "s\n" +
						 "		- Face All : " + ChunkData.meshAllDelay + "s\n";
	}
}
