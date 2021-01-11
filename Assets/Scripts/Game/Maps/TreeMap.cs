using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TreeMap
{
    public int seed = 0;

    public Vector2 scale = Vector2.one;
    public float probability = 0.15f;

    public bool GetTreeProbability(float x, float y)
	{
        return Noise.Noise2D(x / scale.x, y / scale.y) < probability;
	}
    public float GetHeight(float x, float y)
	{
        return Noise.Noise2D(x / scale.x, y / scale.y);
	}

    public TreeMap() { }
    public TreeMap(int seed, Vector2 scale, float probability)
	{
        this.seed = seed;
        this.scale = scale;
        this.probability = probability;
	}
}
