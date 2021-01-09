using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

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

	Texture2D groundHeightMapPreview = null;
	Texture2D treeHeightMapPreview = null;

	PreviewRenderUtility renderUtils;
	RenderTexture renderTexture = null;
	GameObject noise3DPreview = null;

	void GenerateGroundTexture()
	{
		groundHeightMapPreview = new Texture2D(300, 300);
		groundHeightMapPreview.filterMode = FilterMode.Point;
		groundHeightMapPreview.wrapMode = TextureWrapMode.Clamp;

		HeightMap groundHeighMap = new HeightMap(seedProperty.intValue, octavesProperty.intValue, lacunarityProperty.floatValue, persistanceProperty.floatValue, scaleProperty.vector2Value);

		var nbXPixel = groundHeightMapPreview.width / (Chunk.ChunkSize * 5);
		var nbYPixel = groundHeightMapPreview.height / (Chunk.ChunkSize * 5);

		for (int j = 0; j < Chunk.ChunkSize * 5; j++)
			for (int i = 0; i < Chunk.ChunkSize * 5; i++)
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
		treeHeightMapPreview.filterMode = FilterMode.Point;
		treeHeightMapPreview.wrapMode = TextureWrapMode.Clamp;

		HeightMap treeHeighMap = new HeightMap(seedTreeProperty.intValue, octavesTreeProperty.intValue, lacunarityTreeProperty.floatValue, persistanceTreeProperty.floatValue, scaleTreeProperty.vector2Value);

		var nbXPixel = treeHeightMapPreview.width / (Chunk.ChunkSize * 5);
		var nbYPixel = treeHeightMapPreview.height / (Chunk.ChunkSize * 5);

		for (int j = 0; j < Chunk.ChunkSize * 5; j++)
			for (int i = 0; i < Chunk.ChunkSize * 5; i++)
			{
				var height = treeHeighMap.GetHeight(i - Chunk.ChunkRadius, j - Chunk.ChunkRadius);
				var color = (height < treeProbabilityProperty.floatValue) ? Color.Lerp(Color.black, Color.white, height) : Color.green;

				var colors = Enumerable.Repeat<Color>(color, nbXPixel * nbYPixel).ToArray();
				treeHeightMapPreview.SetPixels(i * nbXPixel, j * nbYPixel, nbXPixel, nbYPixel, colors);
			}

		treeHeightMapPreview.Apply();
	}

	GameObject CreateNoise3DPreview()
	{
		var noisePreview = new GameObject();

		var faces = new GameObject[6];
		faces[0] = GameObject.CreatePrimitive(PrimitiveType.Quad);
		faces[0].transform.position = new Vector3(0.5f, 0.0f, 0.0f);
		faces[0].transform.eulerAngles = new Vector3(0.0f, -90.0f, 0.0f);
		faces[0].transform.parent = noisePreview.transform;

		faces[1] = GameObject.CreatePrimitive(PrimitiveType.Quad);
		faces[1].transform.position = new Vector3(-0.5f, 0.0f, 0.0f);
		faces[1].transform.eulerAngles = new Vector3(0.0f, 90.0f, 0.0f);
		faces[1].transform.parent = noisePreview.transform;

		faces[2] = GameObject.CreatePrimitive(PrimitiveType.Quad);
		faces[2].transform.position = new Vector3(0.0f, 0.5f, 0.0f);
		faces[2].transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);
		faces[2].transform.parent = noisePreview.transform;

		faces[3] = GameObject.CreatePrimitive(PrimitiveType.Quad);
		faces[3].transform.position = new Vector3(0.0f, -0.5f, 0.0f);
		faces[3].transform.eulerAngles = new Vector3(-90.0f, 0.0f, 0.0f);
		faces[3].transform.parent = noisePreview.transform;

		faces[4] = GameObject.CreatePrimitive(PrimitiveType.Quad);
		faces[4].transform.position = new Vector3(0.0f, 0.0f, 0.5f);
		faces[4].transform.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
		faces[4].transform.parent = noisePreview.transform;

		faces[5] = GameObject.CreatePrimitive(PrimitiveType.Quad);
		faces[5].transform.position = new Vector3(0.0f, 0.0f, -0.5f);
		faces[5].transform.parent = noisePreview.transform;

		for (int i = 0; i < 6; i++)
		{
			var colors = new Color[20 * 20];
			for (int x = 0; x < 20; x++)
				for (int y = 0; y < 20; y++)
					colors[y + x * 20] = Color.Lerp(Color.black, Color.white, Noise.Noise3D(faces[i].transform.position.x, faces[i].transform.position.y, faces[i].transform.position.z));

			Texture2D texture = new Texture2D(20, 20);
			texture.filterMode = FilterMode.Point;

			texture.SetPixels(colors);
			texture.Apply();

			var mat = new Material(Shader.Find("Standard"));
			mat.mainTexture = texture;
			faces[i].GetComponent<Renderer>().sharedMaterial = mat;
		}

		return noisePreview;
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

		GenerateGroundTexture();
		GenerateTreeTexture();

		renderTexture = new RenderTexture(300, 300, 16);
		renderTexture.filterMode = FilterMode.Point;

		renderUtils = new PreviewRenderUtility();
		noise3DPreview = CreateNoise3DPreview();
		renderUtils.AddSingleGO(noise3DPreview);

		renderUtils.camera.transform.position = new Vector3(0, 0, -10);
		renderUtils.camera.farClipPlane = 20;
		renderUtils.camera.targetTexture = renderTexture;
	}
	private void OnDisable()
	{
		renderUtils.Cleanup();
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

		renderTexture.Release();
		renderUtils.camera.Render();
		GUILayout.Box(renderTexture);
		var rect = GUILayoutUtility.GetLastRect();

		if (Event.current.type == EventType.MouseDrag)
		{
			if (rect.Contains(Event.current.mousePosition))
			{
				var delta = Event.current.delta;
				noise3DPreview.transform.RotateAround(Vector3.zero, Vector3.up, -delta.x);
				noise3DPreview.transform.RotateAround(Vector3.zero, Vector3.right, -delta.y);

				Repaint();
			}
		}

		serializedObject.ApplyModifiedProperties();
	}
}
