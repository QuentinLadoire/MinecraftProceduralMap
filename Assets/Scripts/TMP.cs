using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TMP : MonoBehaviour
{
	[SerializeField] Renderer down = null;
	[SerializeField] Renderer up = null;
	[SerializeField] Renderer left = null;
	[SerializeField] Renderer right = null;
	[SerializeField] Renderer back = null;
	[SerializeField] Renderer front = null;

	[SerializeField] int textureSize = 20;

	[SerializeField] int octaves = 4;
	[SerializeField] [Range(0.0f, 1.0f)] float persistance = 0.5f;
	[SerializeField] float lacunarity = 2;
	[SerializeField] Vector3 scale = Vector3.one;
	[SerializeField] int seed = 0;

	//private void Start()
	//{
	//	Noise.Benchmark();
	//}

	private void Update()
	{
		Color[] colors0 = new Color[textureSize * textureSize];
		Color[] colors1 = new Color[textureSize * textureSize];
		Color[] colors2 = new Color[textureSize * textureSize];
		Color[] colors3 = new Color[textureSize * textureSize];
		Color[] colors4 = new Color[textureSize * textureSize];
		Color[] colors5 = new Color[textureSize * textureSize];

		int index = 0;
		for (int i = 0; i < textureSize; i++)
			for (int j = 0; j < textureSize; j++)
			{
				colors0[index] = Color.Lerp(Color.black, Color.white, Noise.CoherentNoise3D(i, j, -0.5f, octaves, persistance, lacunarity, scale.x, scale.y, scale.z, seed));
				colors1[index] = Color.Lerp(Color.black, Color.white, Noise.CoherentNoise3D(i, j, 0.5f, octaves, persistance, lacunarity, scale.x, scale.y, scale.z, seed));
																			
				colors2[index] = Color.Lerp(Color.black, Color.white, Noise.CoherentNoise3D(-0.5f, j, i, octaves, persistance, lacunarity, scale.x, scale.y, scale.z, seed));
				colors3[index] = Color.Lerp(Color.black, Color.white, Noise.CoherentNoise3D(0.5f, j, i, octaves, persistance, lacunarity, scale.x, scale.y, scale.z, seed));
																			
				colors4[index] = Color.Lerp(Color.black, Color.white, Noise.CoherentNoise3D(i, -0.5f, j, octaves, persistance, lacunarity, scale.x, scale.y, scale.z, seed));
				colors5[index] = Color.Lerp(Color.black, Color.white, Noise.CoherentNoise3D(i, 0.5f, j, octaves, persistance, lacunarity, scale.x, scale.y, scale.z, seed));
				index++;
			}

		Texture2D texture0 = new Texture2D(textureSize, textureSize);
		Texture2D texture1 = new Texture2D(textureSize, textureSize);
		Texture2D texture2 = new Texture2D(textureSize, textureSize);
		Texture2D texture3 = new Texture2D(textureSize, textureSize);
		Texture2D texture4 = new Texture2D(textureSize, textureSize);
		Texture2D texture5 = new Texture2D(textureSize, textureSize);
		texture0.filterMode = FilterMode.Point;
		texture1.filterMode = FilterMode.Point;
		texture2.filterMode = FilterMode.Point;
		texture3.filterMode = FilterMode.Point;
		texture4.filterMode = FilterMode.Point;
		texture5.filterMode = FilterMode.Point;

		texture0.SetPixels(colors0);
		texture1.SetPixels(colors1);
		texture2.SetPixels(colors2);
		texture3.SetPixels(colors3);
		texture4.SetPixels(colors4);
		texture5.SetPixels(colors5);

		texture0.Apply();
		texture1.Apply();
		texture2.Apply();
		texture3.Apply();
		texture4.Apply();
		texture5.Apply();

		back.material.mainTexture = texture0;
		front.material.mainTexture = texture1;

		left.material.mainTexture = texture2;
		right.material.mainTexture = texture3;

		down.material.mainTexture = texture4;
		up.material.mainTexture = texture5;
	}
}
