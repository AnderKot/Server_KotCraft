using BaseObjects;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Workers;
using MyStruct;

public class ChunkLoaderFactory : MonoBehaviour
{
    public int LoadedCount = 0;
    private static List<ChunkPos> NeedLoadList = new List<ChunkPos>();
    Thread ChunkLoaderThread;

    private void Start()
    {
        ChunkLoaderThread = new Thread(new ThreadStart(Chop));
        
    }

    void FixedUpdate()
    {
        if (!ChunkLoaderThread.IsAlive)
        {
            if (NeedLoadList.Count > 0)
            {

                if (Chunk.Alphabet.ContainsKey(NeedLoadList[0]))
                {
                    NeedLoadList.RemoveAt(0);
                    return;
                }
                else
                {
                    ChunkLoader ChankMng = new ChunkLoader(NeedLoadList[0]);
                    ChunkLoaderThread = new Thread(ChankMng.LoadLoop);
                    ChunkLoaderThread.IsBackground = true;
                    ChunkLoaderThread.Start();
                    NeedLoadList.RemoveAt(0);
                    LoadedCount++;
                }
            }
        }

    }

    public static void AddToNeadLoadList(ChunkPos point)
    {
        if (!NeedLoadList.Contains(point))
        {
            NeedLoadList.Add(point);
        }
    }

    private void Chop() { }
}
