using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generator
{
    public class TerrainGenerator
    {
        public static Dictionary<Vector3Int, int> Run(Vector3 chankPosition)
        {
            //int[,,] Terrain = new int[20, 20, 20];
            Dictionary<Vector3Int, int> Terrain = new Dictionary<Vector3Int, int>();
            for (int x = 0; x < 20; x++)
            {
                for (int z = 0; z < 20; z++)
                {
                    float height = Mathf.PerlinNoise((x + chankPosition.x) * 0.02f, (z + chankPosition.z) * 0.02f) * 30 + 10;

                    for (int y = 0; ((y+ chankPosition.y) <= height) & (y < 100); y++)
                    {
                        if ((y + chankPosition.y) + 1 >= height)
                        {
                            Terrain.Add(new Vector3Int(x, y, z), 3);
                            continue;
                        } 
                        
                        
                        if ((y + chankPosition.y) + 3 > height)
                        {
                            Terrain.Add(new Vector3Int(x, y, z), 1);
                            continue;
                        }

                        Terrain.Add(new Vector3Int(x, y, z), 2);

                        //Terrain[x, y, z] = Random.Range(1,2);
                    }
                }
            }

            return Terrain;
        }
    }
}

