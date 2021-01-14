using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TMP : MonoBehaviour
{
	public int size = 20;

	private void Start()
	{
		for (int i = 0; i < size; i++)
			for (int j = 0; j < size; j++)
				for (int k = 0; k < size; k++)
				{
					var height = World.MapGenerator.caveMap.GetHeightUnscale(i, j, k);
					if (height > 0.5f)
					{
						var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
						cube.transform.position = new Vector3(i, j, k);
						cube.transform.parent = transform;
					}
				}
	}
}
