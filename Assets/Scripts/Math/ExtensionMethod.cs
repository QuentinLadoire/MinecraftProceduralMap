using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class ExtensionMethod
{
    static public Vector3Int ToVector3Int(this Vector3 vec)
	{
		return new Vector3Int((int)vec.x, (int)vec.y, (int)vec.z);
	}

	static public Vector3 Round(this Vector3 vec)
	{
		return new Vector3(Mathf.Round(vec.x), Mathf.Round(vec.y), Mathf.Round(vec.z));
	}
	static public Vector3Int RoundToInt(this Vector3 vec)
	{
		return new Vector3Int(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y), Mathf.RoundToInt(vec.z));
	}
}
