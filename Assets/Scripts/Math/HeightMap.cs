using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightMap
{
	int Seed { get; set; } = 0;

	int Octaves { get; set; } = 4;
	float Lacunarity { get; set; } = 2.0f;
	float Persistance { get; set; } = 0.5f;

	Vector2 Scale { get; set; } = new Vector2(20.0f, 20.0f);

	public float GetHeight(float x, float y)
	{
		Random.InitState(Seed);

		float frequency = 1.0f;
		float amplitude = 1.0f;
		float height = 0.0f;
		float maxHeight = 0.0f;

		for (int i = 0; i < Octaves; i++)
		{
			var X = (x + Random.Range(-100000.0f, 100000.0f)) / Scale.x * frequency;
			var Y = (y + Random.Range(-100000.0f, 100000.0f)) / Scale.y * frequency;

			var perlinValue = Mathf.PerlinNoise(X, Y);
			height += perlinValue * amplitude;

			maxHeight += 1.0f * amplitude;

			frequency *= Lacunarity;
			amplitude *= Persistance;
		}

		height = Mathf.InverseLerp(0.0f, maxHeight, height);

		return height;
	}
	public float GetHeight(Vector2 position)
	{
		return GetHeight(position.x, position.y);
	}

	public HeightMap() { }
	public HeightMap(int seed, int octaves, float lacunarity, float persistance, Vector2 scale)
	{
		Seed = seed;

		Octaves = octaves;
		Lacunarity = lacunarity;
		Persistance = persistance;

		Scale = scale;
	}
}
