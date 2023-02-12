using BaseObjects;
using MyNET;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering.RendererUtils;
using UnityEngine.UIElements;

public class ChunkSpawner : MonoBehaviour
{
    private static List<Vector3Int> ReadySpawnList = new List<Vector3Int>();
    private static List<Vector3Int> NeedSpawnList = new List<Vector3Int>();

    void FixedUpdate()
    {
        if (NeedSpawnList.Count > 0)
        {
            if (ReadySpawnList.Contains(NeedSpawnList[0]))
            {
                Chunk.Spawn(NeedSpawnList[0]);
                NeedSpawnList.RemoveAt(0);
            }
        }
    }

    public static void AddToNeadSpawnList(Vector3Int point)
    {
        if (!NeedSpawnList.Contains(point))
        {
            NeedSpawnList.Add(point);
        }
    }

    public static void AddToReadySpawnList(Vector3Int point)
    {
        if (!ReadySpawnList.Contains(point))
        {
            ReadySpawnList.Add(point);
        }
    }
}
