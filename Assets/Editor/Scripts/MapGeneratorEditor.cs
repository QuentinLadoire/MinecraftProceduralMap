using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
	GUIStyle titleStyle = null;
	GUIStyle subTitleStyle = null;

	// Ground HeightMap Parameters
	SerializedProperty seedProperty = null;
	SerializedProperty octavesProperty = null;
	SerializedProperty lacunarityProperty = null;
	SerializedProperty persistanceProperty = null;
	SerializedProperty scaleProperty = null;

	SerializedProperty groundHeightMaxProperty = null;

	// Tree HeightMap Parameters
	SerializedProperty seedTreeProperty = null;
	SerializedProperty octavesTreeProperty = null;
	SerializedProperty lacunarityTreeProperty = null;
	SerializedProperty persistanceTreeProperty = null;
	SerializedProperty scaleTreeProperty = null;

	SerializedProperty treeProbabilityProperty = null;

	// Games Parameters
	SerializedProperty nbChunkProperty = null;
	SerializedProperty chunkPrefabProperty = null;
	SerializedProperty textureDataProperty = null;

	Texture2D groundHeightMapPreview = null;
	Texture2D treeHeightMapPreview = null;

	void GenerateGroundTexture()
	{
		groundHeightMapPreview = new Texture2D(300, 300);
		groundHeightMapPreview.filterMode = FilterMode.Bilinear;
		groundHeightMapPreview.wrapMode = TextureWrapMode.Clamp;

		HeightMap groundHeighMap = new HeightMap(seedProperty.intValue, octavesProperty.intValue, lacunarityProperty.floatValue, persistanceProperty.floatValue, scaleProperty.vector2Value);

		var nbXPixel = groundHeightMapPreview.width / (Chunk.ChunkSize * nbChunkProperty.intValue);
		var nbYPixel = groundHeightMapPreview.height / (Chunk.ChunkSize * nbChunkProperty.intValue);

		for (int j = 0; j < Chunk.ChunkSize * nbChunkProperty.intValue; j++)
			for (int i = 0; i < Chunk.ChunkSize * nbChunkProperty.intValue; i++)
			{
				var height = groundHeighMap.GetHeight(i - Chunk.ChunkRadius, j - Chunk.ChunkRadius);
				var color = Color.Lerp(Color.black, Color.white, height);

				var colors = Enumerable.Repeat<Color>(color, nbXPixel * nbYPixel).ToArray();
				groundHeightMapPreview.SetPixels(i * nbXPixel, j * nbYPixel, nbXPixel, nbYPixel, colors);
			}

		groundHeightMapPreview.Apply();
	}
	void GenerateTreeTexture()
	{
		treeHeightMapPreview = new Texture2D(300, 300);
		treeHeightMapPreview.filterMode = FilterMode.Bilinear;
		treeHeightMapPreview.wrapMode = TextureWrapMode.Clamp;

		HeightMap treeHeighMap = new HeightMap(seedTreeProperty.intValue, octavesTreeProperty.intValue, lacunarityTreeProperty.floatValue, persistanceTreeProperty.floatValue, scaleTreeProperty.vector2Value);

		var nbXPixel = treeHeightMapPreview.width / (Chunk.ChunkSize * nbChunkProperty.intValue);
		var nbYPixel = treeHeightMapPreview.height / (Chunk.ChunkSize * nbChunkProperty.intValue);

		for (int j = 0; j < Chunk.ChunkSize * nbChunkProperty.intValue; j++)
			for (int i = 0; i < Chunk.ChunkSize * nbChunkProperty.intValue; i++)
			{
				var height = treeHeighMap.GetHeight(i - Chunk.ChunkRadius, j - Chunk.ChunkRadius);
				var color = (height < treeProbabilityProperty.floatValue) ? Color.Lerp(Color.black, Color.white, height) : Color.green;

				var colors = Enumerable.Repeat<Color>(color, nbXPixel * nbYPixel).ToArray();
				treeHeightMapPreview.SetPixels(i * nbXPixel, j * nbYPixel, nbXPixel, nbYPixel, colors);
			}

		treeHeightMapPreview.Apply();
	}

	private void Awake()
	{
		titleStyle = new GUIStyle();
		titleStyle.fontStyle = FontStyle.Bold;
		titleStyle.normal.textColor = Color.grey * 1.5f;
		titleStyle.alignment = TextAnchor.MiddleCenter;

		subTitleStyle = new GUIStyle();
		subTitleStyle.fontStyle = FontStyle.Bold;
		subTitleStyle.normal.textColor = Color.grey * 1.5f;
		subTitleStyle.alignment = TextAnchor.MiddleLeft;
	}

	private void OnEnable()
	{
		// Ground HeightMap Parameters
		seedProperty = serializedObject.FindProperty("seed");
		octavesProperty = serializedObject.FindProperty("octaves");
		lacunarityProperty = serializedObject.FindProperty("lacunarity");
		persistanceProperty = serializedObject.FindProperty("persistance");
		scaleProperty = serializedObject.FindProperty("scale");

		groundHeightMaxProperty = serializedObject.FindProperty("groundHeightMax");

		// Tree HeightMap Parameters
		seedTreeProperty = serializedObject.FindProperty("seedTree");
		octavesTreeProperty = serializedObject.FindProperty("octavesTree");
		lacunarityTreeProperty = serializedObject.FindProperty("lacunarityTree");
		persistanceTreeProperty = serializedObject.FindProperty("persistanceTree");
		scaleTreeProperty = serializedObject.FindProperty("scaleTree");

		treeProbabilityProperty = serializedObject.FindProperty("treeProbability");

		// Games Parameters
		nbChunkProperty = serializedObject.FindProperty("nbChunk");
		chunkPrefabProperty = serializedObject.FindProperty("chunkPrefab");
		textureDataProperty = serializedObject.FindProperty("textureData");

		GenerateGroundTexture();
		GenerateTreeTexture();
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		// Ground HeightMap Parameters
		EditorGUILayout.LabelField(new GUIContent("Ground HeightMap"), titleStyle);
		EditorGUILayout.LabelField(new GUIContent("Parameters :"), subTitleStyle);

		EditorGUILayout.PropertyField(seedProperty);
		EditorGUILayout.PropertyField(octavesProperty);
		EditorGUILayout.PropertyField(lacunarityProperty);
		EditorGUILayout.PropertyField(persistanceProperty);
		EditorGUILayout.PropertyField(scaleProperty);

		EditorGUILayout.PropertyField(groundHeightMaxProperty);

		GUILayout.Box(groundHeightMapPreview);
		if (GUILayout.Button(new GUIContent("Generate Ground"))) GenerateGroundTexture();

		// Tree HeightMap Parameters
		EditorGUILayout.Space();
		EditorGUILayout.LabelField(new GUIContent("Tree HeightMap"), titleStyle);
		EditorGUILayout.LabelField(new GUIContent("Parameters :"), subTitleStyle);

		EditorGUILayout.PropertyField(seedTreeProperty);
		EditorGUILayout.PropertyField(octavesTreeProperty);
		EditorGUILayout.PropertyField(lacunarityTreeProperty);
		EditorGUILayout.PropertyField(persistanceTreeProperty);
		EditorGUILayout.PropertyField(scaleTreeProperty);

		EditorGUILayout.PropertyField(treeProbabilityProperty);

		GUILayout.Box(treeHeightMapPreview);
		if (GUILayout.Button(new GUIContent("Generate Tree"))) GenerateTreeTexture();

		// Games Parameters
		EditorGUILayout.Space();
		EditorGUILayout.LabelField(new GUIContent("Game"), titleStyle);
		EditorGUILayout.PropertyField(nbChunkProperty);
		EditorGUILayout.PropertyField(chunkPrefabProperty);
		EditorGUILayout.PropertyField(textureDataProperty);

		serializedObject.ApplyModifiedProperties();
	}
}
