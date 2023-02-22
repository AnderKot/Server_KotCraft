using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGizmos : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Gizmos.DrawWireMesh(gameObject.GetComponent<MeshCollider>().sharedMesh,-1,transform.position);
    }
}
