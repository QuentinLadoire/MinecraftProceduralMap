using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TMP : MonoBehaviour
{
    // Width and height of the texture in pixels.
    public int pixWidth;
    public int pixHeight;

    // The origin of the sampled area in the plane.
    public float xOrg;
    public float yOrg;

    // The number of cycles of the basic noise pattern that are repeated
    // over the width and height of the texture.
    public Vector2 scale = Vector2.one;

    [Range(0.0f, 1.0f)] public float percent = 0.89f;

    private Texture2D noiseTex;
    private Color[] pix;
    private Renderer rend;

    void CalcNoise()
    {
        // For each pixel in the texture...
        float y = 0.0F;

        while (y < noiseTex.height)
        {
            float x = 0.0F;
            while (x < noiseTex.width)
            {
                float xCoord = xOrg + x / noiseTex.width * scale.x;
                float yCoord = yOrg + y / noiseTex.height * scale.y;
                //float sample = Mathf.PerlinNoise(xCoord, yCoord);
                float sample = Noise.Noise2D(xCoord, yCoord);
                Debug.Log(sample);
                if (sample < percent)
                    pix[(int)y * noiseTex.width + (int)x] = new Color(sample, sample, sample);
                else
                    pix[(int)y * noiseTex.width + (int)x] = Color.green;
                x++;
            }
            y++;
        }

        // Copy the pixel data to the texture and load it into the GPU.
        noiseTex.SetPixels(pix);
        noiseTex.Apply();
    }

    void Start()
    {
        rend = GetComponent<Renderer>();

        // Set up the texture and a Color array to hold pixels during processing.
        noiseTex = new Texture2D(pixWidth, pixHeight);
        noiseTex.filterMode = FilterMode.Point;
        pix = new Color[noiseTex.width * noiseTex.height];
        rend.material.mainTexture = noiseTex;

        Noise.Benchmark();
        Noise.Benchmark2();
    }
    void Update()
    {
        //CalcNoise();

        //Debug.Log(World.GetChunkPosition(transform.position));
    }
}
