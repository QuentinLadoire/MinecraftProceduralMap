using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MathfPlus
{
	static public Vector2Int[] GetAllPointInRadius(int radius)
	{
		List<Vector2Int> points = new List<Vector2Int>();
		for (int j = -radius; j < radius + 1; j++)
		{
			for (int i = -radius; i < radius + 1; i++)
			{
				var tmp = new Vector2Int(i, j);
				if (tmp.sqrMagnitude <= radius * radius)
				{
					points.Add(tmp);
				}
			}
		}

		return points.ToArray();
	}
}
