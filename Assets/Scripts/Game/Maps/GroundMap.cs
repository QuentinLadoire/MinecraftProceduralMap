using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GroundMap
{
    public int seed = 0;

    public int octaves = 4;
    public float lacunarity = 2;
    [Range(0.0f, 1.0f)]
    public float persistance = 0.5f;

    public Vector2 scale = Vector2.one;

    public int heightMax = 15;

    public float GetHeight(float x, float y)
	{
        return Noise.CoherentNoise2D(x, y, octaves, persistance, lacunarity, scale.x, scale.y, seed) * heightMax;
	}
    public float GetHeight(Vector2 position)
	{
        return GetHeight(position.x, position.y);
	}

    public float GetHeightUnscale(float x, float y)
	{
        return Noise.CoherentNoise2D(x, y, octaves, persistance, lacunarity, scale.x, scale.y, seed);
	}
    public float GetHeightUnscale(Vector2 position)
	{
        return GetHeightUnscale(position.x, position.y);
	}

    public GroundMap() { }
    public GroundMap(int seed, int octaves, float lacunarity, float persistance, Vector2 scale)
	{
        this.seed = seed;
        this.octaves = octaves;
        this.lacunarity = lacunarity;
        this.persistance = persistance;
        this.scale = scale;
	}
}
