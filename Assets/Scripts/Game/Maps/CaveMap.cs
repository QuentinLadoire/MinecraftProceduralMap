using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveMap
{
    public int seed = 0;

    public int octaves = 4;
    public float lacunarity = 2;
    [Range(0.0f, 1.0f)]
    public float persistance = 0.5f;
    public Vector2 scale = Vector2.one;

    public int heighMax = 10;
    public float probability = 0.5f;
}
