using BaseObjects;
using MyNET;
using MyStruct;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering.RendererUtils;
using UnityEngine.UIElements;

public class ChunkSpawner : MonoBehaviour
{
    public int SpawnedCount = 0;

    private static List<ChunkPos> NeedSpawnList = new List<ChunkPos>();
    private Chunk CurrChunk;

    void FixedUpdate()
    {
        if (NeedSpawnList.Count > 0)
        {
            CurrChunk = null;
            if (Chunk.Alphabet.TryGetValue(NeedSpawnList[0],out CurrChunk))
            { 
                if(CurrChunk.IsSpawn)
                {
                    NeedSpawnList.RemoveAt(0);
                    return;
                }

                if (CurrChunk.IsBuilded)
                {
                    CurrChunk.IsSpawn = true;
                    CurrChunk.MyObject = GameObject.Instantiate(Chunk.GameObject) as GameObject;
                    CurrChunk.MyObject.transform.position = this.CurrChunk.ChankPoint.GepVector3();

                    Mesh MyMesh = new Mesh();
                    MyMesh.vertices = CurrChunk.Geometry.VerticleList.ToArray();
                    MyMesh.triangles = CurrChunk.Geometry.TriangleList.ToArray();

                    MyMesh.RecalculateBounds();
                    MyMesh.RecalculateNormals();

                    CurrChunk.MyObject.GetComponent<MeshCollider>().sharedMesh = MyMesh;
                    NeedSpawnList.RemoveAt(0);
                    SpawnedCount++;

                    Debug.Log("Отрендерен чанк:" + CurrChunk.ChankPoint);
                    return;
                }
            }
            NeedSpawnList.Add(NeedSpawnList[0]);
            NeedSpawnList.RemoveAt(0);
        }
    }

    public static void AddToNeadSpawnList(ChunkPos point)
    {
        if (!NeedSpawnList.Contains(point))
        {
            NeedSpawnList.Add(point);
        }
    }
}
