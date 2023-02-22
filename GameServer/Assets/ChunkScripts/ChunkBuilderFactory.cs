using BaseCreature;
using BaseObjects;
using MyStruct;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;
using Workers;

public class ChunkBuilderFactory : MonoBehaviour
{
    public int BuldedCount = 0;
    private static List<ChunkPos> NeedBuildList = new List<ChunkPos>();
    private Chunk CurrChunk;

    void FixedUpdate()
    {
        if (NeedBuildList.Count > 0)
        {
            CurrChunk = null;
            if (Chunk.Alphabet.TryGetValue(NeedBuildList[0], out CurrChunk))
            {
                if (CurrChunk.IsBuild)
                {
                    NeedBuildList.RemoveAt(0);
                    return;
                }

                if (CurrChunk.IsLoaded & CurrChunk.NearbyChanksIsLoad())
                {
                    ChunkBuilder ChankMng = new ChunkBuilder(CurrChunk.ChankPoint);
                    Thread ChunkBuilderThread = new Thread(new ThreadStart(ChankMng.BuildLoop));
                    ChunkBuilderThread.IsBackground = true;
                    ChunkBuilderThread.Start();
                    NeedBuildList.RemoveAt(0);
                    BuldedCount++;
                    return;
                }
            }
            NeedBuildList.Add(NeedBuildList[0]);
            NeedBuildList.RemoveAt(0);
        }
    }

    public static void AddToNeadBuildList(ChunkPos point)
    {
        if (!NeedBuildList.Contains(point))
        {
            NeedBuildList.Add(point);
        }
    }
}
