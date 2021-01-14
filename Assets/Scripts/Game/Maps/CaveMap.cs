using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CaveMap
{
    public int seed = 0;

    public int octaves = 4;
    public float lacunarity = 2;
    [Range(0.0f, 1.0f)]
    public float persistance = 0.5f;
    public Vector3 scale = Vector3.one;

    public int heighMax = 10;
    public float probability = 0.5f;

    public bool GetProbability(float x, float y, float z)
	{
        return Noise.CoherentNoise3D(x, y, z, octaves, persistance, lacunarity, scale.x, scale.y, scale.z, seed) > probability;
	}
    public float GetHeight(float x, float y, float z)
	{
        return Noise.CoherentNoise3D(x, y, z, octaves, persistance, lacunarity, scale.x, scale.y, scale.z, seed) * heighMax;
	}
    public float GetHeightUnscale(float x, float y, float z)
    {
        return Noise.CoherentNoise3D(x, y, z, octaves, persistance, lacunarity, scale.x, scale.y, scale.z, seed);
    }
}
