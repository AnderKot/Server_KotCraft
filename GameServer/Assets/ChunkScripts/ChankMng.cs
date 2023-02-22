using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseObjects;
using MyStruct;

public class ChankMng : MonoBehaviour
{
    private void OnDestroy()
    {
        Chunk.Alphabet[ChunkPos.VectorToPoss(transform.position)].Save();
    }

}
