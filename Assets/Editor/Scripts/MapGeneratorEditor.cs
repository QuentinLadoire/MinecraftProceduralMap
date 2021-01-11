using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

public static class Utility
{
	private static object GetValue_Imp(object source, string name)
	{
		if (source == null)
			return null;
		var type = source.GetType();

		while (type != null)
		{
			var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			if (f != null)
				return f.GetValue(source);

			var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
			if (p != null)
				return p.GetValue(source, null);

			type = type.BaseType;
		}
		return null;
	}
	private static object GetValue_Imp(object source, string name, int index)
	{
		var enumerable = GetValue_Imp(source, name) as System.Collections.IEnumerable;
		if (enumerable == null) return null;
		var enm = enumerable.GetEnumerator();
		//while (index-- >= 0)
		//    enm.MoveNext();
		//return enm.Current;

		for (int i = 0; i <= index; i++)
		{
			if (!enm.MoveNext()) return null;
		}
		return enm.Current;
	}

	public static object GetTargetObjectOfProperty(SerializedProperty prop)
	{
		if (prop == null) return null;

		var path = prop.propertyPath.Replace(".Array.data[", "[");
		object obj = prop.serializedObject.targetObject;
		var elements = path.Split('.');
		foreach (var element in elements)
		{
			if (element.Contains("["))
			{
				var elementName = element.Substring(0, element.IndexOf("["));
				var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
				obj = GetValue_Imp(obj, elementName, index);
			}
			else
			{
				obj = GetValue_Imp(obj, element);
			}
		}
		return obj;
	}
}

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
	GUIStyle titleStyle = null;
	GUIStyle subTitleStyle = null;

	// MapGenerator property
	SerializedProperty groundMapProperty = null;
	SerializedProperty treeMapProperty = null;
	SerializedProperty caveMapProperty = null;

	// Noise2D preview
	Texture2D groundHeightMapPreview = null;
	Texture2D treeHeightMapPreview = null;

	// Noise3D preview
	PreviewRenderUtility renderUtils;
	RenderTexture renderTexture = null;
	GameObject noise3DPreview = null;
	GameObject[] faces = null;

	void GenerateGroundTexture()
	{
		groundHeightMapPreview = new Texture2D(300, 300);
		groundHeightMapPreview.filterMode = FilterMode.Point;
		groundHeightMapPreview.wrapMode = TextureWrapMode.Clamp;

		GroundMap groundMap = Utility.GetTargetObjectOfProperty(groundMapProperty) as GroundMap;

		var nbXPixel = groundHeightMapPreview.width / (Chunk.ChunkSize * 5);
		var nbYPixel = groundHeightMapPreview.height / (Chunk.ChunkSize * 5);

		for (int j = 0; j < Chunk.ChunkSize * 5; j++)
			for (int i = 0; i < Chunk.ChunkSize * 5; i++)
			{
				var height = groundMap.GetHeightUnscale(i - Chunk.ChunkRadius, j - Chunk.ChunkRadius);
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

		TreeMap treeMap = Utility.GetTargetObjectOfProperty(treeMapProperty) as TreeMap;

		var nbXPixel = treeHeightMapPreview.width / (Chunk.ChunkSize * 5);
		var nbYPixel = treeHeightMapPreview.height / (Chunk.ChunkSize * 5);

		for (int j = 0; j < Chunk.ChunkSize * 5; j++)
			for (int i = 0; i < Chunk.ChunkSize * 5; i++)
			{
				var height = treeMap.GetHeight(i - Chunk.ChunkRadius, j - Chunk.ChunkRadius);
				var color = (height > treeMap.probability) ? Color.Lerp(Color.black, Color.white, height) : Color.green;
				
				var colors = Enumerable.Repeat<Color>(color, nbXPixel * nbYPixel).ToArray();
				treeHeightMapPreview.SetPixels(i * nbXPixel, j * nbYPixel, nbXPixel, nbYPixel, colors);
			}

		treeHeightMapPreview.Apply();
	}

	GameObject CreateNoise3DPreview()
	{
		var noisePreview = new GameObject();

		faces = new GameObject[6];
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

		return noisePreview;
	}
	void UpdateNoise3DPreview()
	{
		CaveMap caveMap = Utility.GetTargetObjectOfProperty(caveMapProperty) as CaveMap;

		for (int i = 0; i < 6; i++)
		{
			var colors = new Color[20 * 20];
			for (int x = 0; x < 20; x++)
				for (int y = 0; y < 20; y++)
					colors[y + x * 20] = Color.Lerp(Color.black, Color.white, caveMap.GetHeight(faces[i].transform.position.x, faces[i].transform.position.y, faces[i].transform.position.z));

			Texture2D texture = new Texture2D(20, 20);
			texture.filterMode = FilterMode.Point;

			texture.SetPixels(colors);
			texture.Apply();

			var mat = new Material(Shader.Find("Standard"));
			mat.mainTexture = texture;
			faces[i].GetComponent<Renderer>().material = mat;
		}
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
		groundMapProperty = serializedObject.FindProperty("groundMap");
		treeMapProperty = serializedObject.FindProperty("treeMap");
		caveMapProperty = serializedObject.FindProperty("caveMap");

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

		// GroundMap
		EditorGUILayout.PropertyField(groundMapProperty);

		GUILayout.Box(groundHeightMapPreview);
		if (GUILayout.Button(new GUIContent("Generate Ground"))) GenerateGroundTexture();

		// TreeMap
		EditorGUILayout.PropertyField(treeMapProperty);

		GUILayout.Box(treeHeightMapPreview);
		if (GUILayout.Button(new GUIContent("Generate Tree"))) GenerateTreeTexture();

		// CaveMap
		EditorGUILayout.PropertyField(caveMapProperty);

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

		if (GUILayout.Button(new GUIContent("Generate Cave"))) UpdateNoise3DPreview();

		serializedObject.ApplyModifiedProperties();
	}
}
