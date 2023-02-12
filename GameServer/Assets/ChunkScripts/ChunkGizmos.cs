using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGizmos : MonoBehaviour
{
    void Start()
    {
        Gizmos.DrawWireMesh(gameObject.GetComponent<MeshCollider>().sharedMesh);
    }
}
