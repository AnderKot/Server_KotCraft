using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generator
{
    public class TerrainGenerator
    {
        public static int[,,] Run(Vector3 position)
        {
            int[,,] Terrain = new int[20, 20, 20];

            for (int x = 0; x < 20; x++)
            {
                for (int z = 0; z < 20; z++)
                {
                    float height = Mathf.PerlinNoise((x + position.x) * 0.05f, (z + position.z) * 0.05f) * 10;

                    for (int y = 0; ((y+ position.y) < height) & (y < 20); y++)
                    {
                       Terrain[x, y, z] = 1;
                    }
                }
            }

            return Terrain;
        }
    }
}

