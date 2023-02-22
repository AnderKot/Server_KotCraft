using System.Collections.Generic;
using UnityEngine;
using MyStruct;

namespace Generator
{
    public class TerrainGenerator
    {
        public static Dictionary<BlockPos, int> Run(ChunkPos chankPosition)
        {
            //int[,,] Terrain = new int[20, 20, 20];
            Dictionary<BlockPos, int> Terrain = new Dictionary<BlockPos, int>();
            for (int x = 0; x < 20; x++)
            {
                for (int z = 0; z < 20; z++)
                {
                    float height = Mathf.PerlinNoise((x + chankPosition.x) * 0.02f, (z + chankPosition.z) * 0.02f) * 30 + 10;

                    for (int y = 0; (y <= height) & (y < 100); y++)
                    {
                        if (y + 1 >= height)
                        {
                            Terrain.Add(new BlockPos(x, y, z), 3);
                            continue;
                        } 
                        
                        
                        if (y + 3 > height)
                        {
                            Terrain.Add(new BlockPos(x, y, z), 1);
                            continue;
                        }

                        Terrain.Add(new BlockPos(x, y, z), 2);

                        //Terrain[x, y, z] = Random.Range(1,2);
                    }
                }
            }

            return Terrain;
        }
    }
}

